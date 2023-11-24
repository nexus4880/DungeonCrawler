using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Items;

public class SpeedPotion : Item {
	public Single Duration { get; set; } = 15f;
	public Single Multiplier { get; set; } = 2f;

	public override void Serialize(NetDataWriter writer) {
		base.Serialize(writer);
		writer.Put(this.Duration);
		writer.Put(this.Multiplier);
	}

	public override void Deserialize(NetDataReader reader) {
		base.Deserialize(reader);
		this.Duration = reader.GetFloat();
		this.Multiplier = reader.GetFloat();
	}
}