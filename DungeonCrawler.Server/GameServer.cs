using System.Collections;
using System.Collections.Specialized;
using System.IO.Compression;
using System.Net;
using System.Numerics;
using DungeonCrawler.Core;
using DungeonCrawler.Core.Entities;
using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Core.Items;
using DungeonCrawler.Core.Map;
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
	private static List<TiledTileset> _tiledSets = [];
	private static BaseTile[,] _tiles;

	public static void Initialize(IPAddress ipv4, IPAddress ipv6, Int32 port)
	{
		Console.WriteLine("Loading map...");
		GameServer._map = new TiledMap("/home/nicholas/Tiled/untitled.tmx");
		foreach (String file in Directory.GetFiles("/home/nicholas/Tiled/tilesets"))
		{
			_tiledSets.Add(new TiledTileset(file));
		}

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

		GameServer._tiles = new BaseTile[GameServer._map.Width, GameServer._map.Height];
		foreach (TiledLayer layer in _map.Layers)
		{
			if (layer.data is not null)
			{
				for (int i = 0; i < layer.data.Length; i++)
				{
					var tileSet = _map.GetTiledMapTileset(layer.data[i]);
					if (tileSet is null)
					{
						throw new Exception("Why was it not found?");
					}

					// TODO: This is definitely not right, but since they're both 16 right now it's okay
					Int32 x = i / _map.Width;
					Int32 y = i % _map.Width;
					BaseTile baseTile = new BaseTile { X = x, Y = y, TilesetSource = $"assets/{tileSet.source.Replace("tsx", "png")}" };
					GameServer._tiles[x, y] = baseTile;
				}
			}
		}

		for (Int32 y = 0; y < GameServer._map.Height; y++)
		{
			for (Int32 x = 0; x < GameServer._map.Width; x++)
			{
				if (GameServer._tiles is null)
				{
					GameServer._tiles[x, y] = new BaseTile { X = x, Y = y, TilesetSource = "" };
				}
			}
		}

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
			LocalPlayerEntityId = Guid.Empty,
			WorldWidth = _map.Width,
			WorldHeight = _map.Height,
			TileWidth = _map.TileWidth,
			TileHeight = _map.TileHeight
		});

		foreach (Entity entity in entities)
		{
			writer.PutDeserializable(entity);
		}

		for (Int32 y = 0; y < GameServer._map.Height; y++)
		{
			for (Int32 x = 0; x < GameServer._map.Width; x++)
			{
				writer.Put(GameServer._tiles[x, y]);
			}
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