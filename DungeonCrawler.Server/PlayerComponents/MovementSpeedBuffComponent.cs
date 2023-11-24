namespace DungeonCrawler.Server.PlayerComponents;

public class MovementSpeedBuffComponent : IPlayerComponent {
	private readonly Single _duration;
	private Single _time;

	public MovementSpeedBuffComponent(Single multiplier, Single duration) {
		this.Multiplier = multiplier;
		this._duration = duration;
	}

	public Single Multiplier { get; }

	public PlayerController Owner { get; set; }

	public Boolean IsActive {
		get {
			return this._time < this._duration;
		}
	}

	public void Update(Single deltaTime) {
		this._time += deltaTime;
	}
}