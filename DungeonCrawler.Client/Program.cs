using System.Net;
using DungeonCrawler.Client;
using DungeonCrawler.Core;
using DungeonCrawler.Core.Entities;
using DungeonCrawler.Core.Items;
using LiteNetLib;

Networking.Initialize();

// Everything that we are expecting to be able to deserialize should be registered here
LNHashCache.RegisterAllOfType<Item>();
LNHashCache.RegisterAllOfType<Entity>();
LNHashCache.RegisterType<DroppedLootItem>();

if (!Networking.NetManager.Start())
{
	throw new Exception("Failed to start NetManager");
}

Networking.LocalPeer =
	Networking.NetManager.Connect(new IPEndPoint(IPAddress.Loopback, 8278), "DungeonCrawler");
while (Networking.LocalPeer.ConnectionState == ConnectionState.Outgoing)
{
	Thread.Sleep(1);
}

if (Networking.LocalPeer.ConnectionState != ConnectionState.Connected)
{
	throw new Exception("Failed to connect to server");
}

InitWindow(1280, 720, "DungeonCrawler");
while (!WindowShouldClose())
{
	Networking.Update();
	GameManager.Update();
	BeginDrawing();
	ClearBackground(BLACK);
	GameManager.Draw();
	EndDrawing();
}

Networking.LocalPeer.Disconnect();
CloseWindow();