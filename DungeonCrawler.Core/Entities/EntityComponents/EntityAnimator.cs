using System.Collections;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Entities.EntityComponents;
public abstract class EntityAnimator : BaseEntityComponent{
    public abstract void Animate();
    public override void Deserialize(NetDataReader reader)
    {
        base.Deserialize(reader);
    }

    public override void Initialize(IDictionary properties)
    {
        base.Initialize(properties);
    }

    public override void Serialize(NetDataWriter writer)
    {
        base.Serialize(writer);
    }
}