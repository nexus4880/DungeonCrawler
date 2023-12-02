using System.Numerics;
using DungeonCrawler.Core;
using DungeonCrawler.Core.Packets;
using DungeonCrawler.Server.EntityComponents;
using LiteNetLib;
using LiteNetLib.Utils;

namespace DungeonCrawler.Server.Entities;

public class PlayerEntity : Entity {
	private Inventory _inventory;

	public PlayerEntity() {
		this._inventory = new Inventory(this, 4);
		this.AddComponent<HealthComponent>(100f);
	}

	public NetPeer NetPeer { get; set; }
	public PlayerInputs CurrentInputs { get; set; }

	public override Inventory GetInventory() {
		return this._inventory;
	}

	public override void Update(Single deltaTime) {
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
			this.position += new Vector2(h, v);
			NetDataWriter writer = new NetDataWriter();
			GameServer.PacketProcessor.Write(writer, new EntityMovedPacket {
				EntityId = this.EntityId,
				Position = this.position
			});
			this.NetPeer.NetManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
		}
	}

	public PlayerData GetPlayerData() {
		return new PlayerData();
	}
}