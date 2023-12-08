using DungeonCrawler.Core;
using DungeonCrawler.Core.Entities;
using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Core.Packets;
using LiteNetLib;
using LiteNetLib.Utils;

namespace DungeonCrawler.Client;

public static class Networking
{
	public static NetPacketProcessor PacketProcessor { get; private set; }
	public static NetManager NetManager { get; private set; }
	public static EventBasedNetListener EventBasedNetListener { get; private set; }
	public static NetPeer LocalPeer { get; set; }
	public static NetDataWriter Writer { get; } = new NetDataWriter(true, UInt16.MaxValue);

	public static void Initialize()
	{
		Networking.PacketProcessor = new NetPacketProcessor();
		Networking.PacketProcessor.Initialize();
		Networking.Subscribe<InitializeWorldPacket>(Networking.OnInitializeWorld);
		Networking.EventBasedNetListener = new EventBasedNetListener();
		Networking.EventBasedNetListener.NetworkReceiveEvent += Networking.OnNetworkReceive;
		Networking.NetManager = new NetManager(Networking.EventBasedNetListener);
	}

	private static void OnInitializeWorld(InitializeWorldPacket packet, UserPacketEventArgs args)
	{
		Console.WriteLine($"[OnInitializeWorld] {packet.EntitiesCount} entities | {packet.LootItemsCount} loot items");
		for (Int32 i = 0; i < packet.EntitiesCount; i++)
		{
			Entity entity = args.PacketReader.GetDeserializable<Entity>();
			GameManager.AddEntity(entity);
		}

		for (Int32 i = 0; i < packet.LootItemsCount; i++)
		{
			DroppedLootItem lootItem = args.PacketReader.GetDeserializable<DroppedLootItem>();
			GameManager.AddLootItem(lootItem);
		}
	}

	public static void Update()
	{
		Networking.NetManager.PollEvents();
		if (Networking.Writer.Length > 0)
		{
			Networking.LocalPeer.Send(Networking.Writer, DeliveryMethod.Unreliable);
			Networking.Writer.Reset();
		}
	}

	public static void Subscribe<T>(Action<T, UserPacketEventArgs> onReceive) where T : class, new()
	{
		Networking.PacketProcessor.SubscribeReusable(onReceive);
	}

	private static void OnNetworkReceive(NetPeer peer, NetPacketReader reader, Byte channel,
		DeliveryMethod deliverymethod)
	{
		Console.WriteLine("[OnNetworkReceive]");
		try
		{
			Networking.PacketProcessor.ReadAllPackets(reader,
				new UserPacketEventArgs(peer, reader, channel, deliverymethod));
		}
		catch (ParseException)
		{
		}
	}
}