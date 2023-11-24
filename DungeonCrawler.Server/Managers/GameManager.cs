using System.Numerics;
using DungeonCrawler.Core.Extensions;
using DungeonCrawler.Core.Items;
using LiteNetLib;

namespace DungeonCrawler.Server.Managers;

public class GameManager {
	private Dictionary<Int32, PlayerController> _playerControllers = new Dictionary<Int32, PlayerController>();

	public GameManager(GameServer server) {
		this.Server = server;
	}

	public ItemManager ItemManager { get; } = new ItemManager();

	public GameServer Server { get; }

	public IReadOnlyDictionary<Int32, PlayerController> AllPlayers {
		get {
			return this._playerControllers;
		}
	}

	public PlayerController GetControllerById(Int32 id) {
		return this._playerControllers.GetValueOrDefault(id);
	}

	public void Update(Single deltaTime) {
		foreach (PlayerController controller in this._playerControllers.Values) {
			controller.Update(deltaTime);
		}
	}

	public Boolean RemovePlayer(Int32 id) {
		Boolean removed = this._playerControllers.Remove(id, out PlayerController controller);
		if (removed) {
			controller.OnDestroy();
		}

		return removed;
	}

	public PlayerController CreatePlayer(NetPeer peer) {
		PlayerController controller = new PlayerController(peer, this);
		if (!this._playerControllers.TryAdd(peer.Id, controller)) {
			return null;
		}

		controller.position = Random.Shared.NextVector2(Vector2.Zero, new Vector2(1280f, 720f));
		controller.health = 100f;
		controller.items.Add(this.ItemManager.CreateItem<HealthPotion>(1000f, 15f));
		controller.items.Add(this.ItemManager.CreateItem<SpeedPotion>());
		controller.items.Add(this.ItemManager.CreateItem<InstantHealthPotion>());

		return controller;
	}
}