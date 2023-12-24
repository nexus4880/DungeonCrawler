
using System.Collections;
using DungeonCrawler.Core;
using DungeonCrawler.Core.Attributes;
using DungeonCrawler.Core.Entities.EntityComponents.Animators;

namespace DungeonCrawler.Client.Entities.Components.Animators;

// The text has to be done because if we do typeof(BaseAnimatorComponent<TEnum>) it will not hash the same
[HashAs("DungeonCrawler.Core.Entities.EntityComponents.Animators.EntityAnimatorComponent`1[DungeonCrawler.Core.EPlayerMovementAnimations]")]
public class ClientEntityAnimator : EntityAnimatorComponent<EPlayerMovementAnimations> {
	public override void Animate() {
	}

	public override void Initialize(IDictionary properties) {
		//TODO: Make the fucking animations like idk fucking one picture in memory so that then i can just draw them lol.
	}

	public override void OnStateChange(IDictionary properties) {
		if (properties.Contains(nameof(this.CurrentAnimation))) {
			var previousAnimation = this.CurrentAnimation;
			this.CurrentAnimation = (EPlayerMovementAnimations)properties[nameof(this.CurrentAnimation)];
			Console.WriteLine($"[EntityAnimator.OnStateChange] {previousAnimation} -> {this.CurrentAnimation}");
		}
	}
}