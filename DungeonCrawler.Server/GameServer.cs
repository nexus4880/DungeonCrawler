using System.Collections;
using System.Drawing;
using System.IO.Compression;
using System.Net;
using System.Numerics;
using DungeonCrawler.Core;
using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Core.Map;
using DungeonCrawler.Core.Packets;
using DungeonCrawler.Server.Entities;
using DungeonCrawler.Server.Extensions;
using DungeonCrawler.Server.Managers;
using LiteNetLib;
using LiteNetLib.Utils;
using TiledCS;

namespace DungeonCrawler.Server;

public static class GameServer {
	public static EventBasedNetListener EventBasedNetListener { get; } = new EventBasedNetListener();
	public static NetManager NetManager { get; private set; }
	public static NetPacketProcessor PacketProcessor { get; } = new NetPacketProcessor();
	private static byte[] _assetsBuffer;
	private static TiledMap _map;
	private static List<BaseTile> _tiles;
	public static Rectangle MapBounds { get; private set; }
	public static Dictionary<int, object> MapReferences = [];

	public static void Initialize(IPAddress ipv4, IPAddress ipv6, int port) {
		Console.WriteLine("Loading map...");
		GameServer._map = new TiledMap("./map/untitled.tmx");
		GameServer.HandleMapLoad();
		if (!Directory.Exists("assets")) {
			throw new Exception("Missing assets directory");
		}

		Console.WriteLine("Zipping assets...");
		using var memoryStream = new MemoryStream();
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
		var ip = $"{ipv4}:{port}";
		if (!GameServer.NetManager.Start(ipv4, ipv6, port)) {
			throw new Exception($"Failed to start server on {ip}");
		}

		Console.WriteLine($"Started server on {ip}");
	}

	private static void HandleMapLoad() {
		GameServer.MapBounds = new Rectangle(0, 0, GameServer._map.Width * GameServer._map.TileWidth, GameServer._map.Height * GameServer._map.TileHeight);
		GameServer._tiles = new List<BaseTile>(GameServer._map.Width * GameServer._map.Height);
		var tilesets = GameServer._map.GetTiledTilesets("./map/");
		var sortedTileLayers = _map.Layers.Where(layer => layer.data is not null && layer.properties.Parse().ContainsKey("LayerIndex")).OrderBy(layer => layer.properties.Parse().GetValueAs<int>("LayerIndex")).ToList();
		foreach (var tileLayer in sortedTileLayers) {
			for (var i = 0; i < tileLayer.data.Length; i++) {
				var gid = tileLayer.data[i];
				if (gid is 0) {
					continue;
				}

				var tiledMapTileset = GameServer._map.GetTiledMapTileset(gid);
				var tileset = tilesets[tiledMapTileset.firstgid];
				var rect = GameServer._map.GetSourceRect(tiledMapTileset, tileset, gid);
				if (rect is null) {
					continue;
				}

				var x = i % tileLayer.width;
				var y = i / tileLayer.width;
				var tile = new BaseTile {
					WorldTilePosition = new Point { X = x, Y = y },
					SourceRectPosition = new Rectangle { X = rect.x, Y = rect.y, Width = rect.width, Height = rect.height },
					Layer = tileLayer.properties.Parse().GetValueAsOrThrow<int>("LayerIndex"),
					TilesetSource = tiledMapTileset.source.Replace("tsx", "png")
				};
				GameServer._tiles.Add(tile);
			}
		}

		foreach (var layer in GameServer._map.Layers) {
			if (layer.objects is not null) {
				foreach (var tiledObject in layer.objects) {
					IDictionary objectProperties = tiledObject.properties.Parse();
					if (objectProperties.GetValueAs<Boolean>("Ignored", false)) {
						continue;
					}

					if (tiledObject.type is null) {
						throw new Exception("What the fuck is this object? Set a type in the 'Class' property");
					}

					var objectType = LNHashCache.GetTypeByName(tiledObject.type) ??
						throw new Exception($"Failed to deserialize type: '{tiledObject.type}'");
				}
			}
		}
	}

	public static void SubscribePacket<T>(Action<T, UserPacketEventArgs> callback) where T : class, new() {
		GameServer.PacketProcessor.SubscribeReusable<T, UserPacketEventArgs>(callback);
	}

	private static void OnSetInputsPacket(SetInputsPacket packet, UserPacketEventArgs args) {
		ServerPlayerEntity player = GameManager.GetEntities<ServerPlayerEntity>().FirstOrDefault(player => player.NetPeer.Id == args.Peer.Id);
		if (player is null) {
			Console.WriteLine($"[OnSetInputsPacket] no peer {args.Peer.Id}");

			return;
		}

		player.CurrentInputs = packet.Inputs;
	}

	private static void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectinfo) {
		var playerEntity = GameManager.GetEntities<ServerPlayerEntity>().FirstOrDefault(player => player.NetPeer.Id == peer.Id);
		if (playerEntity is null) {
			return;
		}

		if (GameManager.DestroyEntity(playerEntity.EntityId)) {
			Console.WriteLine($"[OnPeerDisconnected] {playerEntity.EntityId} ({peer.Id}) disconnected");
		}
		else {
			Console.WriteLine($"[OnPeerDisconnected] failed to remove {playerEntity.EntityId} ({peer.Id}) from GameManager");
		}
	}

	private static void OnAssetsLoadedPacket(AssetsLoadedPacket packet, UserPacketEventArgs args) {
		Console.WriteLine($"[OnAssetsLoadedPacket] {args.Peer.Id} has finished loading assets, sending game state");
		var entities = GameManager.EntityList.Values;
		var writer = new NetDataWriter();
		GameServer.PacketProcessor.Write(writer, new InitializeWorldPacket {
			EntitiesCount = entities.Count,
			WorldWidth = _map.Width,
			WorldHeight = _map.Height,
			TileWidth = _map.TileWidth,
			TileHeight = _map.TileHeight,
			TileCount = _tiles.Count
		});

		foreach (var entity in entities) {
			writer.PutDeserializable(entity);
		}

		foreach (var tile in GameServer._tiles) {
			writer.Put(tile);
		}

		args.Peer.Send(writer, DeliveryMethod.ReliableOrdered);
	}

	private static void OnWorldLoadedPacket(WorldLoadedPacket packet, UserPacketEventArgs args) {
		ServerEntity thisPlayer = GameManager.CreateEntity<ServerPlayerEntity>(
			new Hashtable
			{
				{"NetPeer", args.Peer}
			}
		);

		var mapSize = new Vector2(GameServer.MapBounds.Width, GameServer.MapBounds.Height);
		var spawnPoint = Random.Shared.NextVector2(Vector2.Zero, mapSize);
		thisPlayer.Position = spawnPoint;
		thisPlayer.SendCreateEntity();
		thisPlayer.GiveControl(args.Peer);
	}

	private static void OnConnectionRequest(ConnectionRequest request) {
		var peer = request.AcceptIfKey("DungeonCrawler");
		if (peer is null) {
			Console.WriteLine($"[OnConnectionRequest] invalid key");

			return;
		}

		Console.WriteLine($"[OnConnectionRequest] connection from {peer.EndPoint} accepted as ID {peer.Id}, sending {_assetsBuffer.Length} bytes worth of assets");
		var writer = new NetDataWriter();
		GameServer.PacketProcessor.Write(writer, new InitializeAssetsPacket { });
		writer.Put(_assetsBuffer.Length);
		writer.Put(_assetsBuffer);
		peer.Send(writer, DeliveryMethod.ReliableOrdered);
	}

	private static void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channel,
		DeliveryMethod deliveryMethod) {
		try {
			GameServer.PacketProcessor.ReadAllPackets(reader, new UserPacketEventArgs(peer, reader, channel, deliveryMethod));
		}
		catch (ParseException) {
			Console.WriteLine($"[OnNetworkReceive] {peer.EndPoint} ({peer.Id}) sent a packet we don't understand");
		}
	}

	public static void Update(float deltaTime) {
		GameServer.NetManager.PollEvents();
		GameManager.Update(deltaTime);
	}

	public static void Shutdown() {
		GameServer.NetManager.Stop(true);
	}
}