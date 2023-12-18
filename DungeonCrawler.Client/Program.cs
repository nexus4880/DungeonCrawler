using System.Net;
using DungeonCrawler.Client;
using DungeonCrawler.Core;
using DungeonCrawler.Core.Entities;
using DungeonCrawler.Core.Entities.EntityComponents;
using DungeonCrawler.Core.Items;
using LiteNetLib;

Networking.Initialize();

// Everything that we are expecting to be able to deserialize should be registered here
Int32 itemCount = LNHashCache.RegisterAllOfType<Item>();
Console.WriteLine($"Registered {itemCount} item types");
Int32 entityCount = LNHashCache.RegisterAllOfType<Entity>();
Console.WriteLine($"Registered {entityCount} entity types");
Int32 entityComponentCount = LNHashCache.RegisterAllOfType<BaseEntityComponent>();
Console.WriteLine($"Registered {entityComponentCount} entity component types");
LNHashCache.RegisterType<EntityAnimatorComponent<EPlayerMovementAnimations>>();

if (!Networking.NetManager.Start())
{
	throw new Exception("Failed to start NetManager");
}

Networking.LocalPeer =
	Networking.NetManager.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8278), "DungeonCrawler");
while (Networking.LocalPeer.ConnectionState == ConnectionState.Outgoing)
{
	Thread.Sleep(1);
}

if (Networking.LocalPeer.ConnectionState != ConnectionState.Connected)
{
	throw new Exception("Failed to connect to server");
}

SetConfigFlags(ConfigFlags.FLAG_VSYNC_HINT | ConfigFlags.FLAG_MSAA_4X_HINT | ConfigFlags.FLAG_WINDOW_RESIZABLE);
InitWindow(1280, 720, "DungeonCrawler");
while (!WindowShouldClose())
{
	Networking.Update();
	if (Networking.receievedGameState)
	{
		GameManager.Update();
	}

	BeginDrawing();
	ClearBackground(BLACK);
	if (Networking.receievedGameState)
	{
		GameManager.Draw();
		UserInterface.Draw();
	}

	EndDrawing();
}

Networking.LocalPeer.Disconnect();
CloseWindow();