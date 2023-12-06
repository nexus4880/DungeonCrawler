using System.Collections;
using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Core.Entities.EntityComponents;
using LiteNetLib;

namespace DungeonCrawler.Core.Entities;

public class PlayerEntity : Entity, IInventoryOwner
{
	public PlayerEntity()
	{
		this.Inventory = new Inventory(this, 4);
		this.AddComponent<HealthComponent>(100f);
	}

	public NetPeer NetPeer { get; set; }
	public PlayerInputs CurrentInputs { get; set; }

	public override void Initialize(Stack properties)
	{
		base.Initialize(properties);
		this.NetPeer = properties.PopValueOrThrow<NetPeer>();
	}

	public Inventory Inventory { get; }
}