using System.Collections;
using DungeonCrawler.Core.Extensions;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Items;

public class InstantHealthPotion : Item {
	public Single Amount { get; set; } = 100f;

	public override String Name { get; set; } = "Potion of Health";

	public override void Serialize(NetDataWriter writer) {
		base.Serialize(writer);
		writer.Put(this.Amount);
	}

	public override void Deserialize(NetDataReader reader) {
		base.Deserialize(reader);
		this.Amount = reader.GetFloat();
	}

	public override void Initialize(IDictionary properties) {
		this.Amount = properties.GetValueAs<Single>("Amount", 100f);
	}
}