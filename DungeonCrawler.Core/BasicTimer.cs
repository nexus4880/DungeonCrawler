namespace DungeonCrawler.Core;

public class BasicTimer {
	private readonly Action _callback;
	private readonly float _interval;
	private float _timeElapsed;

	public BasicTimer(float interval, Action callback = null) {
		this._interval = interval;
		this._callback = callback;
	}

	public bool Update(float deltaTime) {
		this._timeElapsed += deltaTime;

		if (this._timeElapsed >= this._interval) {
			this._timeElapsed -= this._interval;
			this._callback?.Invoke();
			return true; // Timer has fulfilled the interval
		}

		return false; // Timer hasn't fulfilled the interval
	}
}