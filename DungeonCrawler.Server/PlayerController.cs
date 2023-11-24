using System.Numerics;
using DungeonCrawler.Core;
using DungeonCrawler.Core.Items;
using DungeonCrawler.Core.Packets;
using DungeonCrawler.Server.Managers;
using DungeonCrawler.Server.PlayerComponents;
using LiteNetLib;
using LiteNetLib.Utils;

namespace DungeonCrawler.Server;

public class PlayerController {
	private readonly GameManager _gameManager;
	public readonly List<Item> items;
	private List<IPlayerComponent> _components;
	public Single baseMovementSpeed = 100f;
	public Single health;
	public PlayerInputs inputs;
	public Vector2 position;

	public PlayerController(NetPeer peer, GameManager gameManager) {
		this._gameManager = gameManager;
		this.Peer = peer;
		this.items = new List<Item>();
		this._components = new List<IPlayerComponent>();
	}

	public Single MovmentSpeed {
		get {
			Single result = this.baseMovementSpeed;
			MovementSpeedBuffComponent movementSpeedBuffComponent = this.GetComponent<MovementSpeedBuffComponent>();
			if (movementSpeedBuffComponent is { IsActive: true }) {
				result *= movementSpeedBuffComponent.Multiplier;
			}

			return result;
		}
	}

	public NetPeer Peer { get; set; }

	public T GetComponent<T>() where T : class, IPlayerComponent {
		return this._components.FirstOrDefault(component => component is T) as T;
	}

	public void Update(Single deltaTime) {
		this.UpdateComponents(deltaTime);
		this.HandleMovement(deltaTime);
	}

	public void SendHealthUpdate() {
		NetDataWriter writer = new NetDataWriter();
		this._gameManager.Server.PacketProcessor.Write(writer,
			new UpdateHealthPacket { Id = this.Peer.Id, Health = this.health });
		this.Peer.Send(writer, DeliveryMethod.ReliableOrdered);
	}

	private void UpdateComponents(Single deltaTime) {
		for (Int32 index = 0; index < this._components.Count;) {
			IPlayerComponent component = this._components[index];
			if (component.IsActive) {
				component.Update(deltaTime);
				index++;
			}
			else {
				this._components.RemoveAt(index);
				Console.WriteLine($"{this.Peer.Id} has lost buff {component}");
			}
		}
	}

	private void HandleMovement(Single deltaTime) {
		Single h = 0f;
		if (this.inputs.MoveLeft) {
			h -= 1f;
		}

		if (this.inputs.MoveRight) {
			h += 1f;
		}

		Single v = 0f;
		if (this.inputs.MoveUp) {
			v -= 1f;
		}

		if (this.inputs.MoveDown) {
			v += 1f;
		}

		if (h != 0f || v != 0f) {
			this.position += new Vector2(h, v) * (this.MovmentSpeed * deltaTime);
			NetDataWriter writer = new NetDataWriter();
			this._gameManager.Server.PacketProcessor.Write(writer, new PlayerMovedPacket {
				Id = this.Peer.Id,
				X = this.position.X,
				Y = this.position.Y
			});
			this._gameManager.Server.NetManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
		}
	}

	public void OnDestroy() {
	}

	public PlayerData GetPlayerData() {
		return new PlayerData {
			id = this.Peer.Id,
			health = this.health,
			position = this.position,
			items = this.items
		};
	}

	public T AddComponent<T>(T component) where T : IPlayerComponent {
		component.Owner = this;
		this._components.Add(component);

		return component;
	}

	public void UseItem(Guid id) {
		Int32 usedItemIndex = this.items.FindIndex(i => i.Id == id);
		if (usedItemIndex == -1) {
			throw new Exception("Failed to find item by that ID");
		}

		Item usedItem = this.items[usedItemIndex];
		this.items.RemoveAt(usedItemIndex);
		switch (usedItem) {
			case HealthPotion healthPotion: {
				this.AddComponent(new GainHealthComponent(healthPotion.Amount, healthPotion.Duration));

				break;
			}
			case InstantHealthPotion healthPotion: {
				this.health += healthPotion.Amount;
				this.SendHealthUpdate();

				break;
			}
			case SpeedPotion speedPotion: {
				this.AddComponent(
					new MovementSpeedBuffComponent(speedPotion.Multiplier, speedPotion.Duration));
				break;
			}
		}
	}
}