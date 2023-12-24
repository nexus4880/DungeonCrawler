using System.Diagnostics;
using System.Net;
using DungeonCrawler.Core;
using DungeonCrawler.Core.Entities;
using DungeonCrawler.Core.Entities.EntityComponents;
using DungeonCrawler.Core.Items;
using DungeonCrawler.Server;

const int TARGET_FRAME_RATE = 60;
const float TARGET_FRAME_TIME = 1.0f / TARGET_FRAME_RATE;

var frameTimer = Stopwatch.StartNew();

var isRunning = true;
Console.CancelKeyPress += (_, __) => isRunning = false;

var itemsRegistered = LNHashCache.RegisterAllOfType<Item>();
Console.WriteLine($"Registered {itemsRegistered} item types");

var entitiesRegistered = LNHashCache.RegisterAllOfType<Entity>();
Console.WriteLine($"Registered {entitiesRegistered} entity types");

var entityComponentsRegistered = LNHashCache.RegisterAllOfType<BaseEntityComponent>();
Console.WriteLine($"Registered {entityComponentsRegistered} entity component types");

GameServer.Initialize(IPAddress.Any, IPAddress.IPv6Any, 8278);
while (isRunning) {
	var deltaTime = (float)frameTimer.Elapsed.TotalSeconds;
	frameTimer.Restart();
	GameServer.Update(deltaTime);
	while (frameTimer.Elapsed.TotalSeconds < TARGET_FRAME_TIME) {
	}
}

GameServer.Shutdown();
