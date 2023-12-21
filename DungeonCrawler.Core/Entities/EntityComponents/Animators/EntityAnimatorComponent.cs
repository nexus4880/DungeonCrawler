using System.Collections;
using LiteNetLib.Utils;

namespace DungeonCrawler.Core.Entities.EntityComponents.Animators;

public class EntityAnimatorComponent<TAnimationType> : BaseAnimatorComponent
    where TAnimationType : Enum
{
    public TAnimationType CurrentAnimation { get; set; }

    public override void Animate()
    {
        throw new NotImplementedException();
    }

    public override void Deserialize(NetDataReader reader)
    {
        base.Deserialize(reader);
        byte animationByte = reader.GetByte();
        CurrentAnimation = (TAnimationType)Enum.ToObject(typeof(TAnimationType), animationByte);
    }

    public override void Initialize(IDictionary properties)
    {
        base.Initialize(properties);
    }

    public override void Serialize(NetDataWriter writer)
    {
        base.Serialize(writer);
        writer.Put(Convert.ToByte(CurrentAnimation));
    }
}