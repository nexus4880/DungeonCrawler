using System.Collections;
using System.Numerics;
using DungeonCrawler.Core.Entities;
using DungeonCrawler.Core.Entities.EntityComponents;
using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Core.Packets;
using LiteNetLib;
using LiteNetLib.Utils;

namespace DungeonCrawler.Server.Entities;

public class ServerEntity : Entity
{
	public void SendUpdatePosition()
	{
		EntityMovedPacket packet = new EntityMovedPacket { EntityId = this.EntityId, Position = this.Position };
		NetDataWriter writer = new NetDataWriter();
		GameServer.PacketProcessor.Write(writer, packet);
		GameServer.NetManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
	}

	public void SendUpdateAnimator()
	{
		NetDataWriter writer = new NetDataWriter();
		UpdateComponentPacket packet = new UpdateComponentPacket
		{
			Entity = this.EntityId,
			Component = this.GetComponent<EntityAnimator>().ComponentId
		};

		GameServer.PacketProcessor.Write(writer,packet);
		writer.Put((int)this.currentAnimation);
		GameServer.NetManager.SendToAll(writer,DeliveryMethod.ReliableOrdered);
	}

	public void SendCreateEntity()
	{
		NetDataWriter writer = new NetDataWriter();
		GameServer.PacketProcessor.Write(writer, new EntityCreatePacket());
		writer.PutDeserializable(this);
		GameServer.NetManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
	}

	public void GiveControl(NetPeer peer)
	{
		NetDataWriter writer = new NetDataWriter();
		GameServer.PacketProcessor.Write(writer, new SetEntityContextPacket { EntityId = this.EntityId });
		peer.Send(writer, DeliveryMethod.ReliableOrdered);
	}

	public override T AddComponent<T>(IDictionary properties)
	{
		return base.AddComponent<T>(properties);
	}
}