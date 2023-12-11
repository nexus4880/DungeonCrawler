using System.Collections;
using DungeonCrawler.Core.Extensions;
using LiteNetLib.Utils;

namespace DungeonCrawler.Client.Renderers;

public class CircleRenderer : BaseRenderer
{
    public Single Radius { get; set; }

    public override void Deserialize(NetDataReader reader)
    {
        base.Deserialize(reader);
        this.Radius = reader.GetFloat();
    }

    public override void Draw()
    {
        DrawCircleLines((int)this.Owner.Position.X, (int)this.Owner.Position.Y, this.Radius, RED);
    }

    public override void Initialize(Queue properties)
    {
        base.Initialize(properties);
        this.Radius = properties.PopValue<Single>(8f);
    }

    public override void Serialize(NetDataWriter writer)
    {
        base.Serialize(writer);
        writer.Put(this.Radius);
    }
}