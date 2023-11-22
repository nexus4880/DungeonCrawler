using System.Diagnostics;
using LiteNetLib;

namespace DungeonCrawler.Server.Managers;

public class GameManager {
	private Stopwatch _gameTimer = Stopwatch.StartNew();
	private Dictionary<Int32, PlayerController> _playerControllers = new Dictionary<Int32, PlayerController>();

	public GameManager(GameServer server) {
		this.Server = server;
	}

	public GameServer Server { get; }

	public IReadOnlyDictionary<Int32, PlayerController> AllPlayers {
		get {
			return this._playerControllers;
		}
	}

	public PlayerController GetControllerById(Int32 id) {
		return this._playerControllers.GetValueOrDefault(id);
	}

	public void Update() {
		Single deltaTime = (Single)this._gameTimer.Elapsed.TotalMilliseconds;
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

		return controller;
	}
}