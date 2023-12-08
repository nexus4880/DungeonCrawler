using System.Collections;
using System.Numerics;
using DungeonCrawler.Core.Entities;
using DungeonCrawler.Core.Extensions;
using LiteNetLib.Utils;

namespace DungeonCrawler.Client.Renderers;

public class CircleRenderer : IRenderer
{
    public Entity Owner { get; init; }

    public Single Radius { get; set; }

    public void Deserialize(NetDataReader reader)
    {
        this.Radius = reader.GetFloat();
    }

    public void Draw()
    {
        DrawCircleLines((int)this.Owner.Position.X, (int)this.Owner.Position.Y, this.Radius, RED);
    }

    public void Initialize(Queue properties)
    {
        this.Radius = properties.PopValue<Single>(8f);
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(this.Radius);
    }
}