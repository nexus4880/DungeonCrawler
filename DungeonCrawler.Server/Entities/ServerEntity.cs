using System.Numerics;
using DungeonCrawler.Core.Entities;
using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Core.Packets;
using LiteNetLib;
using LiteNetLib.Utils;

namespace DungeonCrawler.Server.Entities;

public class ServerEntity : Entity
{
	public override Vector2 Position
	{
		get
		{
			return base.Position;
		}
		set
		{
			base.Position = value;
			EntityMovedPacket packet = new EntityMovedPacket { EntityId = this.EntityId, Position = value };
			NetDataWriter writer = new NetDataWriter();
			GameServer.PacketProcessor.Write(writer, packet);
			if (this is ServerPlayerEntity serverPlayerEntity)
			{
				GameServer.NetManager.SendToAll(writer, DeliveryMethod.ReliableOrdered, serverPlayerEntity.NetPeer);
			}
			else
			{
				GameServer.NetManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
			}
		}
	}

	public override T AddComponent<T>(params object[] properties)
	{
		return base.AddComponent<T>(properties);

	}
}