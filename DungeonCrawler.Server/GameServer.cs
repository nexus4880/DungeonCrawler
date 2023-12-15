﻿using System.Collections;
using System.Collections.Specialized;
using System.IO.Compression;
using System.Net;
using System.Numerics;
using DungeonCrawler.Core;
using DungeonCrawler.Core.Entities;
using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Core.Items;
using DungeonCrawler.Core.Packets;
using DungeonCrawler.Server.Entities;
using DungeonCrawler.Server.Entities.EntityComponents.Renderers;
using DungeonCrawler.Server.Extensions;
using DungeonCrawler.Server.Managers;
using LiteNetLib;
using LiteNetLib.Utils;
using TiledCS;

namespace DungeonCrawler.Server;

public static class GameServer
{
	public static EventBasedNetListener EventBasedNetListener { get; } = new EventBasedNetListener();
	public static NetManager NetManager { get; private set; }
	public static NetPacketProcessor PacketProcessor { get; } = new NetPacketProcessor();
	private static Byte[] _assetsBuffer;
	private static TiledMap _map;

	public static void Initialize(IPAddress ipv4, IPAddress ipv6, Int32 port)
	{
		Console.WriteLine("Loading map...");
		GameServer._map = new TiledMap("/home/nicholas/Tiled/untitled.tmx");

		if (!Directory.Exists("assets"))
		{
			throw new Exception("Missing assets directory");
		}

		Console.WriteLine("Zipping assets...");
		using MemoryStream memoryStream = new MemoryStream();
		ZipFile.CreateFromDirectory("./assets/", memoryStream, CompressionLevel.SmallestSize, true);
		_assetsBuffer = memoryStream.ToArray();

		Console.WriteLine("Initializing server...");
		GameServer.PacketProcessor.Initialize();
		GameServer.EventBasedNetListener.NetworkReceiveEvent += GameServer.OnNetworkReceive;
		GameServer.EventBasedNetListener.ConnectionRequestEvent += GameServer.OnConnectionRequest;
		GameServer.EventBasedNetListener.PeerDisconnectedEvent += GameServer.OnPeerDisconnected;
		SubscribePacket<SetInputsPacket>(GameServer.OnSetInputsPacket);
		SubscribePacket<AssetsLoadedPacket>(GameServer.OnAssetsLoadedPacket);
		SubscribePacket<WorldLoadedPacket>(GameServer.OnWorldLoadedPacket);
		GameServer.NetManager = new NetManager(GameServer.EventBasedNetListener);
		String ip = $"{ipv4}:{port}";
		if (!GameServer.NetManager.Start(ipv4, ipv6, port))
		{
			throw new Exception($"Failed to start server on {ip}");
		}

		Console.WriteLine($"Started server on {ip}");

		foreach (TiledObject lootPoint in GameServer._map.GetLayerByName("Loot").objects)
		{
			Type itemType = LNHashCache.GetTypeByName(lootPoint.type);
			if (itemType is null)
			{
				throw new Exception($"Failed to deserialize type: '{lootPoint.type}'");
			}

			IDictionary itemProperties = lootPoint.properties.Parse();
			DroppedLootItem droppedItem = GameManager.CreateEntity<DroppedLootItem>(
				new ListDictionary {
					{"Guid", Guid.NewGuid()},
					{"Item", ItemManager.CreateItem(itemType, itemProperties)}
				}
			);

			droppedItem.AddComponent<ServerTextureRenderer>(itemProperties);
			droppedItem.Position = new Vector2(lootPoint.x, lootPoint.y);
		}
	}

	public static void SubscribePacket<T>(Action<T, UserPacketEventArgs> callback) where T : class, new()
	{
		GameServer.PacketProcessor.SubscribeReusable<T, UserPacketEventArgs>(callback);
	}

	private static void OnSetInputsPacket(SetInputsPacket packet, UserPacketEventArgs args)
	{
		ServerPlayerEntity player = GameManager.GetEntities<ServerPlayerEntity>().FirstOrDefault(player => player.NetPeer.Id == args.Peer.Id);
		if (player is null)
		{
			Console.WriteLine($"[OnSetInputsPacket] no peer {args.Peer.Id}");

			return;
		}

		player.CurrentInputs = packet.Inputs;
	}

	private static void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectinfo)
	{
		ServerPlayerEntity playerEntity = GameManager.GetEntities<ServerPlayerEntity>().FirstOrDefault(player => player.NetPeer.Id == peer.Id);
		if (playerEntity is not null)
		{
			if (GameManager.DestroyEntity(playerEntity.EntityId))
			{
				Console.WriteLine($"[OnPeerDisconnected] {playerEntity.EntityId} ({peer.Id}) disconnected");
			}
			else
			{
				Console.WriteLine($"[OnPeerDisconnected] failed to remove {playerEntity.EntityId} ({peer.Id}) from GameManager");
			}
		}
		else
		{
			Console.WriteLine($"[OnPeerDisconnected] failed to get PlayerEntity with PeerId {peer.Id}");
		}
	}

	private static void OnAssetsLoadedPacket(AssetsLoadedPacket packet, UserPacketEventArgs args)
	{
		Console.WriteLine($"[OnAssetsLoadedPacket] {args.Peer.Id} has finished loading assets, sending game state");
		Dictionary<Guid, Entity>.ValueCollection entities = GameManager.EntityList.Values;
		NetDataWriter writer = new NetDataWriter();
		GameServer.PacketProcessor.Write(writer, new InitializeWorldPacket
		{
			EntitiesCount = entities.Count,
			LocalPlayerEntityId = Guid.Empty
		});

		foreach (Entity entity in entities)
		{
			writer.PutDeserializable(entity);
		}

		args.Peer.Send(writer, DeliveryMethod.ReliableOrdered);
	}

	private static void OnWorldLoadedPacket(WorldLoadedPacket packet, UserPacketEventArgs args)
	{
		TiledObject[] spawnPoints = GameServer._map.GetLayerByName("SpawnPoints").objects;
		TiledObject spawnPoint = spawnPoints[Random.Shared.Next(0, spawnPoints.Length)];
		ServerEntity thisPlayer = GameManager.CreateEntity<ServerPlayerEntity>(
			new ListDictionary{
				{"NetPeer", args.Peer}
			}
		);
		thisPlayer.Position = new Vector2(spawnPoint.y, spawnPoint.y);
		thisPlayer.SendCreateEntity();
		thisPlayer.GiveControl(args.Peer);
	}

	private static void OnConnectionRequest(ConnectionRequest request)
	{
		NetPeer peer = request.AcceptIfKey("DungeonCrawler");
		if (peer is null)
		{
			Console.WriteLine($"[OnConnectionRequest] invalid key");

			return;
		}

		Console.WriteLine($"[OnConnectionRequest] connection from {peer.EndPoint} accepted as ID {peer.Id}, sending {_assetsBuffer.Length} bytes worth of assets");
		NetDataWriter writer = new NetDataWriter();
		GameServer.PacketProcessor.Write(writer, new InitializeAssetsPacket { });
		writer.Put((Int32)_assetsBuffer.Length);
		writer.Put(_assetsBuffer);
		peer.Send(writer, DeliveryMethod.ReliableOrdered);
	}

	private static void OnNetworkReceive(NetPeer peer, NetPacketReader reader, Byte channel,
		DeliveryMethod deliveryMethod)
	{
		try
		{
			GameServer.PacketProcessor.ReadAllPackets(reader, new UserPacketEventArgs(peer, reader, channel, deliveryMethod));
		}
		catch (ParseException)
		{
			Console.WriteLine($"[OnNetworkReceive] {peer.EndPoint} ({peer.Id}) sent a packet we don't understand");
		}
	}

	public static void Update(Single deltaTime)
	{
		GameServer.NetManager.PollEvents();
		GameManager.Update(deltaTime);
	}

	public static void Shutdown()
	{
		GameServer.NetManager.Stop(true);
	}
}