using DungeonCrawler.Core;
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
	}
}