using DungeonCrawler.Core;
using DungeonCrawler.Core.Attributes;
using DungeonCrawler.Core.Entities;
using DungeonCrawler.Core.Packets;

namespace DungeonCrawler.Client.Entities;

[HashAs(typeof(DungeonCrawler.Core.Entities.PlayerEntity))]
public class ClientPlayerEntity : PlayerEntity {
	public override void Update(float deltaTime) {
		base.Update(deltaTime);
		var currentInputs = new PlayerInputs {
			MoveUp = IsKeyDown(KeyboardKey.KEY_W),
			MoveDown = IsKeyDown(KeyboardKey.KEY_S),
			MoveLeft = IsKeyDown(KeyboardKey.KEY_A),
			MoveRight = IsKeyDown(KeyboardKey.KEY_D)
		};

		if (this.CurrentInputs != currentInputs) {
			this.CurrentInputs = currentInputs;
			Networking.PacketProcessor.Write(Networking.Writer, new SetInputsPacket {
				Inputs = currentInputs
			});
		}
	}
}
