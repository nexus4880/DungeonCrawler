using DungeonCrawler.Core;
using DungeonCrawler.Core.Packets;
using LiteNetLib;
using LiteNetLib.Utils;
using Raylib_CsLo;

namespace DungeonCrawler.Client;

public class LocalPlayerController : PlayerController {
	private PlayerInputs _inputs;

	public override void Update() {
		this.HandleInputs();
	}

	private void HandleInputs() {
		PlayerInputs currentInputs = new PlayerInputs {
			MoveUp = IsKeyDown(KeyboardKey.KEY_W),
			MoveDown = IsKeyDown(KeyboardKey.KEY_S),
			MoveLeft = IsKeyDown(KeyboardKey.KEY_A),
			MoveRight = IsKeyDown(KeyboardKey.KEY_D)
		};

		if (currentInputs != this._inputs) {
			NetDataWriter writer = new NetDataWriter();
			Program.PacketProcessor.Write(writer, new SetInputsPacket { Inputs = currentInputs });
			Program.LocalPeer.Send(writer, DeliveryMethod.ReliableOrdered);
			this._inputs = currentInputs;
		}
	}
}