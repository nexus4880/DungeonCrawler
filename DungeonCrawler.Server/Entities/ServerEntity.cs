using System.Collections;
using DungeonCrawler.Core.Entities;
using DungeonCrawler.Core.Entities.EntityComponents;
using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Core.Packets;
using LiteNetLib;
using LiteNetLib.Utils;

namespace DungeonCrawler.Server.Entities;

public class ServerEntity : Entity {
	public void SendUpdatePosition() {
		EntityMovedPacket packet = new EntityMovedPacket { EntityId = this.EntityId, Position = this.Position };
		NetDataWriter writer = new NetDataWriter();
		GameServer.PacketProcessor.Write(writer, packet);
		GameServer.NetManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
	}

	public void SendUpdateComponent(BaseEntityComponent component, IDictionary data) {
		NetDataWriter writer = new NetDataWriter();
		GameServer.PacketProcessor.Write(writer, new UpdateComponentPacket {
			Entity = this.EntityId,
			Component = component.ComponentId
		});
		writer.Put(data);
		GameServer.NetManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
	}

	public void SendCreateEntity() {
		NetDataWriter writer = new NetDataWriter();
		GameServer.PacketProcessor.Write(writer, new EntityCreatePacket());
		writer.PutDeserializable(this);
		GameServer.NetManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
	}

	public void GiveControl(NetPeer peer) {
		NetDataWriter writer = new NetDataWriter();
		GameServer.PacketProcessor.Write(writer, new SetEntityContextPacket { EntityId = this.EntityId });
		peer.Send(writer, DeliveryMethod.ReliableOrdered);
	}

	public override T AddComponent<T>(IDictionary properties = null) {
		return base.AddComponent<T>(properties);
	}
}