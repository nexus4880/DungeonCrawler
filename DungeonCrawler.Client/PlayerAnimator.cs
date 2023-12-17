
using System.Collections;
using System.Security.Cryptography;
using DungeonCrawler.Core.Attributes;
using DungeonCrawler.Core.Entities.EntityComponents;
using LiteNetLib.Utils;
namespace DungeonCrawler.Client;

[HashAs("DungeonCrawler.Core.Entities.EntityComponents.EntityAnimator")]
public class PlayerAnimator : EntityAnimator
{
    public override void Animate()
    {
    }

    public override void Initialize(IDictionary properties)
    {
        //TODO: Make the fucking animations like idk fucking one picture in memory so that then i can just draw them lol.
    }

    public override void OnStateChange(NetDataReader reader)
    {
        System.Console.WriteLine((EAnimationType)reader.GetInt());
    }
}