using DungeonCrawler.Client;
using DungeonCrawler.Core.Entities.EntityComponents;

public class UserInterface {
	public static void Draw() {
		if (GameManager.localPlayer is not null) {
			DrawText(GameManager.localPlayer.GetComponent<HealthComponent>().Value.ToString(), 0, 0, 24, RED);
		}
	}
}