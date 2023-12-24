using LiteNetLib.Utils;

namespace DungeonCrawler.Core;

public struct PlayerInputs : INetSerializable, IEquatable<PlayerInputs> {
	public bool MoveUp { get; set; }
	public bool MoveDown { get; set; }
	public bool MoveLeft { get; set; }
	public bool MoveRight { get; set; }

	public void Serialize(NetDataWriter writer) {
		writer.Put(this.MoveUp);
		writer.Put(this.MoveDown);
		writer.Put(this.MoveLeft);
		writer.Put(this.MoveRight);
	}

	public void Deserialize(NetDataReader reader) {
		this.MoveUp = reader.GetBool();
		this.MoveDown = reader.GetBool();
		this.MoveLeft = reader.GetBool();
		this.MoveRight = reader.GetBool();
	}

	public static bool operator ==(PlayerInputs left, PlayerInputs right) {
		return left.MoveUp == right.MoveUp && left.MoveDown == right.MoveDown &&
			left.MoveLeft == right.MoveLeft && left.MoveRight == right.MoveRight;
	}

	public static bool operator !=(PlayerInputs left, PlayerInputs right) {
		return !(left == right);
	}

	public bool Equals(PlayerInputs other) {
		return this.MoveUp == other.MoveUp && this.MoveDown == other.MoveDown &&
			this.MoveLeft == other.MoveLeft && this.MoveRight == other.MoveRight;
	}

	public override bool Equals(object obj) {
		return obj is PlayerInputs other && this.Equals(other);
	}

	public override int GetHashCode() {
		return HashCode.Combine(this.MoveUp, this.MoveDown, this.MoveLeft, this.MoveRight);
	}
}