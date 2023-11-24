using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Items;

public class HealthPotion : Item {
	public Single Amount { get; set; } = 100f;
	public Single Duration { get; set; } = 3f;

	public override void Serialize(NetDataWriter writer) {
		base.Serialize(writer);
		writer.Put(this.Amount);
		writer.Put(this.Duration);
	}

	public override void Deserialize(NetDataReader reader) {
		base.Deserialize(reader);
		this.Amount = reader.GetFloat();
		this.Duration = reader.GetFloat();
	}

	public override void Initialize(params Object[] properties) {
		this.Amount = (Single)properties[0];
		this.Duration = (Single)properties[1];
	}
}