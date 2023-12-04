using System.Collections;
using System.Numerics;
using DungeonCrawler.Core;
using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Core.Packets;
using DungeonCrawler.Server.EntityComponents;
using LiteNetLib;
using LiteNetLib.Utils;

namespace DungeonCrawler.Server.Entities;

public class PlayerEntity : Entity, IInventoryOwner {
	public PlayerEntity() {
		this.Inventory = new Inventory(this, 4);
		this.AddComponent<HealthComponent>(100f);
	}

	public NetPeer NetPeer { get; set; }
	public PlayerInputs CurrentInputs { get; set; }

	public void Update(Single deltaTime) {
		this.HandleMovement(deltaTime);
	}

	private void HandleMovement(Single deltaTime) {
		Single h = 0f;
		Single v = 0f;
		if (this.CurrentInputs.MoveLeft) {
			h -= 1f;
		}

		if (this.CurrentInputs.MoveRight) {
			h += 1f;
		}

		if (this.CurrentInputs.MoveDown) {
			v += 1f;
		}

		if (this.CurrentInputs.MoveUp) {
			v -= 1f;
		}

		h *= deltaTime;
		v *= deltaTime;
		if (h != 0f || v != 0f) {
			Vector2 newPosition = this.Position + new Vector2(h, v);
			this.Position = newPosition;
			NetDataWriter writer = new NetDataWriter();
			GameServer.PacketProcessor.Write(writer, new EntityMovedPacket {
				EntityId = this.EntityId,
				Position = newPosition
			});
			this.NetPeer.NetManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
		}
	}

    public void Initialize(Stack properties) {
	    base.Initialize(properties);
		this.NetPeer = properties.PopValueOrThrow<NetPeer>();
    }

    public Inventory Inventory { get; }
}