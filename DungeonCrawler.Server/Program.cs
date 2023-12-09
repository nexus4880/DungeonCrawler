using System.Diagnostics;
using System.Net;
using DungeonCrawler.Core;
using DungeonCrawler.Core.Items;
using DungeonCrawler.Server;

const Int32 TARGET_FRAME_RATE = 60;
const Single TARGET_FRAME_TIME = 1.0f / TARGET_FRAME_RATE;

Stopwatch frameTimer = Stopwatch.StartNew();

Boolean isRunning = true;
Console.CancelKeyPress += (_, __) => isRunning = false;
Int32 itemsRegistered = LNHashCache.RegisterAllOfType<Item>();
Console.WriteLine($"Registered {itemsRegistered} item types");
GameServer.Initialize(IPAddress.Loopback, IPAddress.IPv6Any, 8278);
while (isRunning)
{
	Single deltaTime = (Single)frameTimer.Elapsed.TotalSeconds;
	frameTimer.Restart();
	GameServer.Update(deltaTime);
	while (frameTimer.Elapsed.TotalSeconds < TARGET_FRAME_TIME)
	{
	}
}

GameServer.Shutdown();