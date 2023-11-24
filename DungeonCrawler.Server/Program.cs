using System.Diagnostics;
using System.Net;
using DungeonCrawler.Core.Handlers;
using DungeonCrawler.Server;

const Int32 TARGET_FRAME_RATE = 240;
const Single TARGET_FRAME_TIME = 1.0f / TARGET_FRAME_RATE;

Stopwatch frameTimer = Stopwatch.StartNew();

Boolean isRunning = true;
Console.CancelKeyPress += (_, __) => isRunning = false;
GameServer server = new GameServer(IPAddress.Any, IPAddress.IPv6Any, 8278);
ItemSerializationHandler.Initialize();
while (isRunning) {
	Single deltaTime = (Single)frameTimer.Elapsed.TotalSeconds;
	frameTimer.Restart();
	server.Update(deltaTime);
	while (frameTimer.Elapsed.TotalSeconds < TARGET_FRAME_TIME) {
	}
}

server.Shutdown();