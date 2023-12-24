using System.Collections;
using DungeonCrawler.Core.Extensions;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Entities.EntityComponents.Renderers;

public abstract class CircleRenderer : BaseRenderer {
	public float Radius { get; set; }
	public uint Color { get; set; }
	public bool Filled { get; set; }

	public override void Deserialize(NetDataReader reader) {
		base.Deserialize(reader);
		this.Radius = reader.GetFloat();
		this.Color = reader.GetUInt();
		this.Filled = reader.GetBool();
	}

	public override void Initialize(IDictionary properties) {
		base.Initialize(properties);
		this.Radius = properties.GetValueAsOrThrow<float>("Radius");
		this.Color = properties.GetValueAs<UInt32>("Color", 0xFFFFFFFF);
		this.Filled = properties.GetValueAs<Boolean>("Filled", false);
	}

	public override void Serialize(NetDataWriter writer) {
		base.Serialize(writer);
		writer.Put(this.Radius);
		writer.Put(this.Color);
		writer.Put(this.Filled);
	}
}