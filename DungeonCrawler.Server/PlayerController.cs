using System.Numerics;
using DungeonCrawler.Core;
using DungeonCrawler.Core.Packets;
using DungeonCrawler.Server.Managers;
using LiteNetLib;
using LiteNetLib.Utils;

namespace DungeonCrawler.Server;

public class PlayerController {
	private readonly GameManager _gameManager;
	public PlayerInputs inputs;
	public Vector2 position;

	public PlayerController(NetPeer peer, GameManager gameManager) {
		this._gameManager = gameManager;
		this.Peer = peer;
	}

	public NetPeer Peer { get; set; }

	public void Update(Single deltaTime) {
		this.HandleMovement(deltaTime);
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
			this.position += new Vector2(h, v);
			NetDataWriter writer = new NetDataWriter();
			this._gameManager.Server.PacketProcessor.Write(writer, new PlayerMovedPacket {
				Id = this.Peer.Id,
				Position = this.position
			});
			this._gameManager.Server.NetManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
		}
	}

	public void OnDestroy() {
	}

	public PlayerData GetPlayerData() {
		return new PlayerData {
			position = this.position
		};
	}
}