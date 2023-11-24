namespace DungeonCrawler.Server.PlayerComponents;

public interface IPlayerComponent {
	PlayerController Owner { get; set; }
	Boolean IsActive { get; }
	void Update(Single deltaTime);
}