using System.Collections;
using DungeonCrawler.Core.Extensions;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Entities.EntityComponents;

public class MovementSpeedBuffComponent : BaseEntityComponent {
	public float Value { get; set; }
	public float Duration { get; set; }

	public override void Initialize(IDictionary properties) {
		this.Value = properties.GetValueAsOrThrow<float>("Value");
		this.Duration = properties.GetValueAsOrThrow<float>("Duration");
	}

	public override void Serialize(NetDataWriter writer) {
		base.Serialize(writer);
		writer.Put(this.Value);
		writer.Put(this.Duration);
	}

	public override void Deserialize(NetDataReader reader) {
		base.Deserialize(reader);
		this.Value = reader.GetFloat();
		this.Duration = reader.GetFloat();
	}
}