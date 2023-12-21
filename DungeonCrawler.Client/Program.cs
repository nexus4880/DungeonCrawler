using System.Net;
using DungeonCrawler.Client;
using DungeonCrawler.Core;
using DungeonCrawler.Core.Entities;
using DungeonCrawler.Core.Entities.EntityComponents;
using DungeonCrawler.Core.Items;
using LiteNetLib;

IPAddress ip = args.Length > 0 ? IPAddress.Parse(args[0]) : IPAddress.Loopback;
Int32 port = args.Length > 1 ? Int32.Parse(args[1]) : 8278;

Networking.Initialize();

// Everything that we are expecting to be able to deserialize should be registered here
Int32 itemCount = LNHashCache.RegisterAllOfType<Item>();
Console.WriteLine($"Registered {itemCount} item types");
Int32 entityCount = LNHashCache.RegisterAllOfType<Entity>();
Console.WriteLine($"Registered {entityCount} entity types");
Int32 entityComponentCount = LNHashCache.RegisterAllOfType<BaseEntityComponent>();
Console.WriteLine($"Registered {entityComponentCount} entity component types");

Networking.LocalPeer =
	Networking.NetManager.Connect(new IPEndPoint(ip, port), "DungeonCrawler");
while (Networking.LocalPeer.ConnectionState == ConnectionState.Outgoing) {
	Thread.Sleep(1);
}

if (Networking.LocalPeer.ConnectionState != ConnectionState.Connected) {
	throw new Exception("Failed to connect to server");
}

SetConfigFlags(ConfigFlags.FLAG_VSYNC_HINT | ConfigFlags.FLAG_MSAA_4X_HINT | ConfigFlags.FLAG_WINDOW_RESIZABLE);
InitWindow(1280, 720, "DungeonCrawler");
while (!WindowShouldClose()) {
	Single deltaTime = GetFrameTime();
	Networking.Update();
	if (Networking.receievedGameState) {
		GameManager.Update(deltaTime);
	}

	BeginDrawing();
	ClearBackground(BLACK);
	if (Networking.receievedGameState) {
		GameManager.Draw();
		UserInterface.Draw();
	}

	EndDrawing();
}

Networking.LocalPeer.Disconnect();
CloseWindow();
