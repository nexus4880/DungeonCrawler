using System.Collections;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Entities.EntityComponents;

public class EntityAnimatorComponent<TAnimationType> : BaseEntityComponent
    where TAnimationType : Enum
{
    public TAnimationType CurrentAnimation { get; set; }

    public virtual void Animate()
    {
        throw new NotImplementedException();
    }

    public override void Deserialize(NetDataReader reader)
    {
        base.Deserialize(reader);
        Byte animationByte = reader.GetByte();
        this.CurrentAnimation = (TAnimationType)Enum.ToObject(typeof(TAnimationType), animationByte);
    }

    public override void Initialize(IDictionary properties)
    {
        base.Initialize(properties);
    }

    public override void Serialize(NetDataWriter writer)
    {
        base.Serialize(writer);
        writer.Put(Convert.ToByte(this.CurrentAnimation));
    }
}