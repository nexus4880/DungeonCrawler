using System.Net;
using DungeonCrawler.Server;

Boolean isRunning = true;
Console.CancelKeyPress += (_, __) => isRunning = false;
GameServer server = new GameServer(IPAddress.Any, IPAddress.IPv6Any, 8278);
while (isRunning) {
	server.Update();
}

server.Shutdown();