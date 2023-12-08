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
    public Vector2 Position { get; set; }

    //public Action Draw { get; set; } I don't know if I want to do it like this, maybe an event?

    public void Deserialize(NetDataReader reader)
    {
        this.Radius = reader.GetFloat();
        this.Position = reader.GetVector2();
    }

    public void Draw()
    {
        DrawCircleLines((int)this.Position.X, (int)this.Position.Y, this.Radius, RED);
    }

    public void Initialize(Queue properties)
    {
        this.Radius = properties.PopValue<Single>(8f);
        this.Position = properties.PopValue<Vector2>(Vector2.Zero);
    }

    public void Serialize(NetDataWriter writer)
    {
        writer.Put(this.Radius);
        writer.Put(this.Position);
    }
}