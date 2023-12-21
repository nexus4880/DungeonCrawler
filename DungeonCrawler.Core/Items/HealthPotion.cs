using System.Collections;
using DungeonCrawler.Core.Extensions;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Items;

public class HealthPotion : Item {
	public Single Amount { get; set; } = 100f;
	public Single Duration { get; set; } = 3f;

	public override String Name { get; set; } = "Potion of Healing";

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

	public override void Initialize(IDictionary properties) {
		this.Amount = properties.GetValueAs<Single>("Amount", 100f);
		this.Duration = properties.GetValueAs<Single>("Duration", 3f);
	}
}