using System.Collections;
using DungeonCrawler.Core.Extensions;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Entities.EntityComponents.Renderers;

public abstract class TextureRenderer : BaseRenderer
{
    public String TexturePath { get; set; }

    public override void Deserialize(NetDataReader reader)
    {
        base.Deserialize(reader);
        this.TexturePath = reader.GetString();
    }

    public override void Initialize(IDictionary properties)
    {
        base.Initialize(properties);
        this.TexturePath = properties.GetValueAsOrThrow<String>("TexturePath");
    }

    public override void Serialize(NetDataWriter writer)
    {
        base.Serialize(writer);
        writer.Put(this.TexturePath);
    }
}