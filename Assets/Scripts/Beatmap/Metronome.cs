using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Metronome
{
	public AudioClip MetronomeClip;
	public bool TriggerFire;
	public UnityEvent Trigger = new UnityEvent();

	private int _lastInterval;
	public float GetIntervalLength(float pBpm) { return 60f / (pBpm); }
	public bool CheckForNewInterval(float interval, bool lBeatPlayed)
	{
		if (Mathf.FloorToInt(interval) != _lastInterval)
		{
			_lastInterval = Mathf.FloorToInt(interval);
			if (!lBeatPlayed) Trigger.Invoke();
			return true;
		}
		return false;
	}

	public void ResetMetronome() {
		_lastInterval = default;
	}
}
