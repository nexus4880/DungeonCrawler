using System.Collections;
using System.Numerics;
using DungeonCrawler.Core;
using DungeonCrawler.Core.Attributes;
using DungeonCrawler.Core.Entities;
using DungeonCrawler.Core.Entities.EntityComponents;
using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Core.Packets;
using LiteNetLib;
using LiteNetLib.Utils;

namespace DungeonCrawler.Server.Entities;

[HashAs("DungeonCrawler.Core.Entities.PlayerEntity")]
public class ServerPlayerEntity : ServerEntity
{
	public NetPeer NetPeer { get; set; }
	public PlayerInputs CurrentInputs { get; set; }

	public override void Initialize(Queue properties)
	{
		base.Initialize(properties);
		this.NetPeer = properties.PopValueOrThrow<NetPeer>();
	}

	public override void Update(float deltaTime)
	{
		base.Update(deltaTime);
		this.HandleMovement(deltaTime);
	}

	private void HandleMovement(float deltaTime)
	{
		Vector2 movement = Vector2.Zero;
		if (this.CurrentInputs.MoveLeft)
		{
			movement.X -= 1f;
		}

		if (this.CurrentInputs.MoveRight)
		{
			movement.X += 1f;
		}

		if (this.CurrentInputs.MoveDown)
		{
			movement.Y += 1f;
		}

		if (this.CurrentInputs.MoveUp)
		{
			movement.Y -= 1f;
		}

		if (movement.Length() != 0f)
		{
			MovementSpeedBuffComponent movementSpeedBuffComponent = this.GetComponent<MovementSpeedBuffComponent>();
			if (movementSpeedBuffComponent is not null)
			{
				movement *= movementSpeedBuffComponent.Value;
			}

			this.Position += movement;
			NetDataWriter writer = new NetDataWriter();
			GameServer.PacketProcessor.Write(writer, new EntityMovedPacket { EntityId = this.EntityId, Position = this.Position });
			GameServer.NetManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
		}
	}
}