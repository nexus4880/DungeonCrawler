using System.Collections;
using System.Numerics;
using DungeonCrawler.Core;
using DungeonCrawler.Core.Attributes;
using DungeonCrawler.Core.Entities;
using DungeonCrawler.Core.Entities.EntityComponents;
using DungeonCrawler.Core.Entities.EntityComponents.Animators;
using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Server.Entities.EntityComponents.Colliders;
using DungeonCrawler.Server.Entities.EntityComponents.Renderers;
using DungeonCrawler.Server.Managers;
using LiteNetLib;

namespace DungeonCrawler.Server.Entities;

[HashAs(typeof(PlayerEntity))]
public class ServerPlayerEntity : ServerEntity {
	public NetPeer NetPeer { get; set; }
	public PlayerInputs CurrentInputs { get; set; }

	public override void Initialize(IDictionary properties) {
		base.Initialize(properties);
		this.NetPeer = properties.GetValueAsOrThrow<NetPeer>("NetPeer");
		_ = this.AddComponent<HealthComponent>(new Hashtable { { "Value", 100f } });
		_ = this.AddComponent<ServerCircleRenderer>(new Hashtable {
			{ "Radius", 16f },
			{ "Color", 0xFFFFFFFF },
			{ "Filled", false }
		});

		_ = this.AddComponent<EntityAnimatorComponent<EPlayerMovementAnimations>>();
		_ = this.AddComponent<CircleColliderComponent>(new Hashtable {
			{ "Radius", 16f }
		});
	}

	public override void Update(float deltaTime) {
		base.Update(deltaTime);
		this.HandleMovement(deltaTime);
		var animatorComponent = this.GetComponent<EntityAnimatorComponent<EPlayerMovementAnimations>>();
		if (animatorComponent is not null) {
			this.HandleMovementAnimation(animatorComponent);
		}
	}

	public void HandleMovementAnimation(EntityAnimatorComponent<EPlayerMovementAnimations> animatorComponent) {
		var currentAnimation = animatorComponent.CurrentAnimation;
		var targetAnimation = EPlayerMovementAnimations.IDLE;
		var horizontalMovement = 0;
		if (this.CurrentInputs.MoveLeft) {
			horizontalMovement--;
		}

		if (this.CurrentInputs.MoveRight) {
			horizontalMovement++;
		}

		var verticalMovement = 0;
		if (this.CurrentInputs.MoveUp) {
			verticalMovement--;
		}

		if (this.CurrentInputs.MoveDown) {
			verticalMovement++;
		}

		// If this is true then we are not moving and are therefore idle
		if (verticalMovement != 0 || horizontalMovement != 0) {
			// This is idle right now but we are moving diagonally
			if (verticalMovement != 0 && horizontalMovement != 0) {
			}
			else {
				_ = verticalMovement == -1 ? EPlayerMovementAnimations.MOVE_UP : EPlayerMovementAnimations.MOVE_DOWN;

				targetAnimation = horizontalMovement == -1 ? EPlayerMovementAnimations.MOVE_LEFT : EPlayerMovementAnimations.MOVE_RIGHT;
			}
		}

		if (targetAnimation != currentAnimation) {
			animatorComponent.CurrentAnimation = targetAnimation;
			this.SendUpdateComponent(animatorComponent, new Hashtable
			{
				{nameof(animatorComponent.CurrentAnimation), (byte)animatorComponent.CurrentAnimation}
			});
		}
	}

	private void HandleMovement(float deltaTime) {
		var movement = Vector2.Zero;
		var currentPosition = this.Position;
		if (this.CurrentInputs.MoveLeft) {
			movement.X -= 1f;
		}

		if (this.CurrentInputs.MoveRight) {
			movement.X += 1f;
		}

		if (this.CurrentInputs.MoveDown) {
			movement.Y += 1f;
		}

		if (this.CurrentInputs.MoveUp) {
			movement.Y -= 1f;
		}

		if (movement != Vector2.Zero) {
			movement *= 100f;
			var movementSpeedBuffComponent = this.GetComponent<MovementSpeedBuffComponent>();
			if (movementSpeedBuffComponent is not null) {
				movement *= movementSpeedBuffComponent.Value;
			}

			// Calculate target position
			var targetPosition = currentPosition + (movement * deltaTime);

			float minX = GameServer.MapBounds.X;
			float minY = GameServer.MapBounds.Y;
			float maxX = GameServer.MapBounds.Width;
			float maxY = GameServer.MapBounds.Height;

			// Ensure the target position stays within boundaries
			var clampedX = Math.Max(minX, Math.Min(targetPosition.X, maxX));
			var clampedY = Math.Max(minY, Math.Min(targetPosition.Y, maxY));

			if (clampedX != currentPosition.X || clampedY != currentPosition.Y) {
				targetPosition = new Vector2(clampedX, clampedY);
				// TODO: implement properly, this is wrong
				var collider = this.GetComponent<BaseColliderComponent>();
				if (collider is not null) {
					BaseColliderComponent detectedCollision = null;
					foreach (var entity in GameManager.EntityList.Values) {
						if (entity == this) {
							continue;
						}

						var otherCollider = entity.GetComponent<BaseColliderComponent>();
						if (otherCollider is null) {
							continue;
						}

						if (collider.CollidesWith(otherCollider)) {
							detectedCollision = otherCollider;

							break;
						}
					}

					if (detectedCollision is not null) {
						return;
					}
				}

				this.Position = targetPosition;
				this.SendUpdatePosition();
			}
		}
	}
}
