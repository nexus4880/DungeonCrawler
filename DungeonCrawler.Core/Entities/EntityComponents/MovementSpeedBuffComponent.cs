using System.Collections;
using DungeonCrawler.Core.Extensions;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Entities.EntityComponents;

public class MovementSpeedBuffComponent : BaseEntityComponent
{
    public Single Value { get; set; }
    public Single Duration { get; set; }

    public override void Initialize(IDictionary properties)
    {
        this.Value = properties.GetValueAsOrThrow<Single>("Value");
        this.Duration = properties.GetValueAsOrThrow<Single>("Duration");
    }

    public override void Serialize(NetDataWriter writer)
    {
        base.Serialize(writer);
        writer.Put(this.Value);
        writer.Put(this.Duration);
    }

    public override void Deserialize(NetDataReader reader)
    {
        base.Deserialize(reader);
        this.Value = reader.GetFloat();
        this.Duration = reader.GetFloat();
    }
}