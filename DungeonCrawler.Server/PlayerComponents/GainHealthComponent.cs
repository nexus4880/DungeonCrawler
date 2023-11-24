namespace DungeonCrawler.Server.PlayerComponents;

public class GainHealthComponent : IPlayerComponent {
	private readonly Single _amount;
	private readonly Single _amountPerTick;
	private readonly Single _duration;
	private Single _time;

	public GainHealthComponent(Single amount, Single duration) {
		this._amountPerTick = amount / duration;
		this._amount = amount;
		this._duration = duration;
	}

	public PlayerController Owner { get; set; }

	public Boolean IsActive {
		get {
			return this._amount > 0f && this._time < this._duration;
		}
	}

	public void Update(Single deltaTime) {
		this._time += deltaTime;
		this.Owner.health += this._amountPerTick * deltaTime;
		this.Owner.SendHealthUpdate();
	}
}