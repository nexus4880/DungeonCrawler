namespace DungeonCrawler.Core;

public class BasicTimer {
	private readonly Action _callback;
	private readonly Single _interval;
	private Single _timeElapsed;

	public BasicTimer(Single interval, Action callback = null) {
		this._interval = interval;
		this._callback = callback;
	}

	public Boolean Update(Single deltaTime) {
		this._timeElapsed += deltaTime;

		if (this._timeElapsed >= this._interval) {
			this._timeElapsed -= this._interval;
			this._callback?.Invoke();
			return true; // Timer has fulfilled the interval
		}

		return false; // Timer hasn't fulfilled the interval
	}
}