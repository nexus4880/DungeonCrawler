using LiteNetLib.Utils;

namespace DungeonCrawler.Core;

public struct PlayerInputs : INetSerializable, IEquatable<PlayerInputs> {
	public Boolean MoveUp { get; set; }
	public Boolean MoveDown { get; set; }
	public Boolean MoveLeft { get; set; }
	public Boolean MoveRight { get; set; }


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

	public static Boolean operator ==(PlayerInputs left, PlayerInputs right) {
		return left.MoveUp == right.MoveUp && left.MoveDown == right.MoveDown &&
			left.MoveLeft == right.MoveLeft && left.MoveRight == right.MoveRight;
	}

	public static Boolean operator !=(PlayerInputs left, PlayerInputs right) {
		return !(left == right);
	}

	public Boolean Equals(PlayerInputs other) {
		return this.MoveUp == other.MoveUp && this.MoveDown == other.MoveDown &&
			this.MoveLeft == other.MoveLeft && this.MoveRight == other.MoveRight;
	}

	public override Boolean Equals(Object obj) {
		return obj is PlayerInputs other && this.Equals(other);
	}

	public override Int32 GetHashCode() {
		return HashCode.Combine(this.MoveUp, this.MoveDown, this.MoveLeft, this.MoveRight);
	}
}