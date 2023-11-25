using System.Net;
using System.Numerics;
using DungeonCrawler.Client;
using DungeonCrawler.Core;
using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Core.Handlers;
using DungeonCrawler.Core.Items;
using DungeonCrawler.Core.Packets;
using LiteNetLib;
using LiteNetLib.Utils;
using Raylib_CsLo;

internal class Program {
	private static Dictionary<Int32, PlayerController> _controllers = new Dictionary<Int32, PlayerController>();
	public static NetPacketProcessor PacketProcessor { get; private set; } = new NetPacketProcessor();
	public static NetManager NetManager { get; private set; }
	public static EventBasedNetListener NetListener { get; private set; } = new EventBasedNetListener();
	public static NetPeer LocalPeer { get; private set; }

	public static void Main(String[] args) {
		Program.PacketProcessor = new NetPacketProcessor();
		Program.PacketProcessor.Initialize();
		Program.PacketProcessor.SubscribeReusable<InitializeWorldPacket>(Program.OnInitializeWorld);
		Program.PacketProcessor.SubscribeReusable<PlayerDisconnectedPacket>(Program.OnPlayerDisconnected);
		Program.PacketProcessor.SubscribeReusable<PlayerJoinedPacket>(Program.OnPlayerJoined);
		Program.PacketProcessor.SubscribeReusable<PlayerMovedPacket>(Program.OnPlayerMoved);
		Program.PacketProcessor.SubscribeReusable<UpdateHealthPacket>(Program.OnUpdateHealth);
		Program.PacketProcessor.SubscribeReusable<RemoveItemPacket>(Program.OnRemoveItem);

		ItemSerializationHandler.Initialize();

		Program.NetListener = new EventBasedNetListener();
		Program.NetListener.NetworkReceiveEvent += Program.OnNetworkReceive;

		Program.NetManager = new NetManager(Program.NetListener);
		if (!Program.NetManager.Start()) {
			throw new Exception("Failed to start NetManager");
		}

		Program.LocalPeer = Program.NetManager.Connect(new IPEndPoint(IPAddress.Loopback, 8278), "DungeonCrawler");
		while (Program.LocalPeer.ConnectionState == ConnectionState.Outgoing) {
			Thread.Sleep(1);
		}

		if (Program.LocalPeer.ConnectionState != ConnectionState.Connected) {
			throw new Exception("Failed to connect to server");
		}

		InitWindow(1280, 720, "DungeonCrawler");
		while (!WindowShouldClose()) {
			Program.NetManager.PollEvents();
			BeginDrawing();
			ClearBackground(BLACK);

			foreach (PlayerController controller in Program._controllers.Values) {
				controller.Update();
				controller.Draw();
			}

			Program.DrawUI();
			EndDrawing();
		}

		Program.LocalPeer.Disconnect();
		CloseWindow();
	}

	private static void OnRemoveItem(RemoveItemPacket packet) {
		PlayerController ownerController = Program._controllers.Values.FirstOrDefault(controller =>
			controller.items.Any(item => item.Id == packet.ItemId));
		if (ownerController is not null) {
			Int32 index = ownerController.items.FindIndex(item => item.Id == packet.ItemId);
			if (index != -1) {
				ownerController.items.RemoveAt(index);
			}
		}
	}

	private static void OnUpdateHealth(UpdateHealthPacket packet) {
		if (Program._controllers.TryGetValue(packet.Id, out PlayerController controller)) {
			controller.health = packet.Health;
		}
	}

	private static void DrawUI() {
		String text = $"{Program.LocalPeer.ConnectionState} | {Program.LocalPeer.RemoteId}";
		if (Program._controllers.TryGetValue(Program.LocalPeer.RemoteId, out PlayerController controller) &&
			controller.items is not null) {
			Int32 index = 0;
			foreach (Item item in controller.items) {
				if (RayGui.GuiLabelButton(new Rectangle(8f, index * 8f + 64f, 100f, 8f), item.GetType().Name)) {
					NetDataWriter writer = new NetDataWriter();
					Program.PacketProcessor.Write(writer, new UseItemPacket { ItemId = item.Id });
					Program.LocalPeer.Send(writer, DeliveryMethod.ReliableOrdered);
				}

				index++;
			}

			text += $" {controller.health}";
		}

		DrawText(text, 4, 4, 16, YELLOW);
	}

	private static void OnPlayerMoved(PlayerMovedPacket packet) {
		if (Program._controllers.TryGetValue(packet.Id, out PlayerController controller)) {
			controller.position = new Vector2(packet.X, packet.Y);
		}
	}

	private static void OnPlayerJoined(PlayerJoinedPacket packet) {
		Console.WriteLine("Player joined");
		Program._controllers[packet.PlayerData.id] = new PlayerController {
			position = packet.PlayerData.position, radius = packet.PlayerData.radius, items = packet.PlayerData.items, health = packet.PlayerData.health
		};
	}

	private static void OnPlayerDisconnected(PlayerDisconnectedPacket packet) {
		Program._controllers.Remove(packet.Id);
	}

	private static void OnInitializeWorld(InitializeWorldPacket packet) {
		foreach (PlayerData playerData in packet.Players) {
			Console.WriteLine(playerData);
			PlayerController controller = playerData.id == Program.LocalPeer.Id
				? new LocalPlayerController()
				: new PlayerController();
			controller.health = playerData.health;
			controller.position = playerData.position;
			controller.items = playerData.items;
			controller.radius = playerData.radius;
			Program._controllers[playerData.id] = controller;
		}
	}

	private static void OnNetworkReceive(NetPeer netPeer, NetPacketReader reader, Byte channel,
		DeliveryMethod deliveryMethod) {
		try {
			Program.PacketProcessor.ReadAllPackets(reader);
		}
		catch (ParseException ex) {
			Console.WriteLine($"{ex}: did you forget to register this packet?");
		}
	}
}