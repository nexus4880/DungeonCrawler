using System.Collections;
using DungeonCrawler.Core.Extensions;
using LiteNetLib;

namespace DungeonCrawler.Core.Entities;

public class PlayerEntity : Entity, IInventoryOwner {
	public PlayerEntity() {
		this.Inventory = new Inventory(this, 4);
	}

	public virtual NetPeer NetPeer { get; set; }
	public virtual PlayerInputs CurrentInputs { get; set; }

	public override void Initialize(IDictionary properties) {
		base.Initialize(properties);
		this.NetPeer = properties.GetValueAsOrThrow<NetPeer>("NetPeer");
	}

	public virtual Inventory Inventory { get; }
}