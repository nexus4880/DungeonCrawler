
using System.Collections;
using DungeonCrawler.Core;
using DungeonCrawler.Core.Attributes;
using DungeonCrawler.Core.Entities.EntityComponents;

namespace DungeonCrawler.Client.Entities.Components;

[HashAs("DungeonCrawler.Core.Entities.EntityComponents.EntityAnimator")]
public class ClientEntityAnimator : EntityAnimatorComponent<EPlayerMovementAnimations>
{
    public override void Animate()
    {
    }

    public override void Initialize(IDictionary properties)
    {
        //TODO: Make the fucking animations like idk fucking one picture in memory so that then i can just draw them lol.
    }

    public override void OnStateChange(IDictionary properties)
    {
        if (properties.Contains(nameof(this.CurrentAnimation)))
        {
            EPlayerMovementAnimations previousAnimation = this.CurrentAnimation;
            this.CurrentAnimation = (EPlayerMovementAnimations)properties[nameof(this.CurrentAnimation)];
            Console.WriteLine($"[EntityAnimator.OnStateChange] {previousAnimation} -> {this.CurrentAnimation}");
        }
        else
        {
            Console.WriteLine($"[PlayerAnimator.OnStateChange] OnStateChanged but properties has no '${this.CurrentAnimation}'");
        }
    }
}