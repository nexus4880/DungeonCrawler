using System.Net;
using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Server.Managers;
using LiteNetLib;
using LiteNetLib.Utils;

namespace DungeonCrawler.Server;

public static class GameServer {
	public static EventBasedNetListener EventBasedNetListener { get; } = new EventBasedNetListener();
	public static NetManager NetManager { get; private set; }
	public static NetPacketProcessor PacketProcessor { get; } = new NetPacketProcessor();

	public static void Initialize(IPAddress ipv4, IPAddress ipv6, Int32 port) {
		GameServer.PacketProcessor.Initialize();
		GameServer.NetManager = new NetManager(GameServer.EventBasedNetListener);
		String ip = $"{ipv4}:{port}";
		if (!GameServer.NetManager.Start(ipv4, ipv6, port)) {
			throw new Exception($"Failed to start server on {ip}");
		}

		Console.WriteLine($"Started server on {ip}");
	}

	public static void Update(Single deltaTime) {
		GameServer.NetManager.PollEvents();
		GameManager.Update(deltaTime);
	}

	public static void Shutdown() {
	}
}