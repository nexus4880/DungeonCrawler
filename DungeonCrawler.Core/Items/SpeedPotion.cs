using System.Collections;
using DungeonCrawler.Core.Extensions;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Items;

public class SpeedPotion : Item
{
	public Single Duration { get; set; } = 15f;
	public Single Multiplier { get; set; } = 2f;

	public override String Name { get; set; } = "Potion of Speed";

	public override void Serialize(NetDataWriter writer)
	{
		base.Serialize(writer);
		writer.Put(this.Duration);
		writer.Put(this.Multiplier);
	}

	public override void Deserialize(NetDataReader reader)
	{
		base.Deserialize(reader);
		this.Duration = reader.GetFloat();
		this.Multiplier = reader.GetFloat();
	}

	public override void Initialize(IDictionary properties)
	{
		this.Duration = properties.GetValueAs<Single>("Duration", 15f);
		this.Multiplier = properties.GetValueAs<Single>("Multiplier", 1.5f);
	}
}