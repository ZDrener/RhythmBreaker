using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatmapManager : MonoBehaviour
{
	public static BeatmapManager Instance;

	public static UnityEvent ON_TriggerNote = new UnityEvent();

	public BeatmapSO Beatmap;

	[Header("SOURCES")]
	[SerializeField] private AudioSource _hitSoundSource;
	[SerializeField] private AudioSource _musicSource;
	private Interval[] _intervals;

	void Start()
	{
		if (Instance != null) throw new Exception("Two instances of BeatmapManager exist at the same time");
		Instance = this;

		_intervals = Beatmap.Intervals;

		_musicSource.clip = Beatmap.Audio;
		StartCoroutine(WaitBeforeStart());
	}
	private IEnumerator WaitBeforeStart()
	{
		yield return new WaitForSecondsRealtime(5);
		StartCoroutine(MusicCoroutine());
	}
	private IEnumerator MusicCoroutine()
	{
		_musicSource.Play();
		// Wait for the beatmap offset
		if (Beatmap.Offset > 0) yield return new WaitForSecondsRealtime(Beatmap.Offset / 1000);

		// Play the first Hit Sound
		PlayHitSound(_intervals[0].hitClip);

		bool lBeatPlayed;
		Interval lI;

		// Play beats
		while (_musicSource.isPlaying)
		{
			lBeatPlayed = false;

			for (int i = 0; i < _intervals.Length; i++)
			{
				lI = _intervals[i];
				float sampledTime = (_musicSource.timeSamples / (_musicSource.clip.frequency * lI.GetIntervalLength(Beatmap.Bpm)));

				if(lI.CheckForNewInterval(sampledTime, lBeatPlayed))
				{
					if (lI.hitClip && !lBeatPlayed)
					{
						if(lI.triggerFire) ON_TriggerNote.Invoke();

						PlayHitSound(lI.hitClip);
						lBeatPlayed = true;
					}
				}
			}

			yield return new WaitForEndOfFrame();
		}
	}
	public void PlayHitSound(AudioClip hitClip)
	{
		_hitSoundSource.PlayOneShot(hitClip);
		Debug.Log(hitClip.name);
	}
	private void OnDestroy()
	{
		if (Instance == this) Instance = null;
	}
}

[System.Serializable]
public class Interval
{
	[SerializeField] private float _stepsPerBeat;
	public AudioClip hitClip;
	public bool triggerFire;
	public UnityEvent trigger = new UnityEvent();

	private int _lastInterval;

	public void Init(float pStepsPerBeat)
	{
		_stepsPerBeat = pStepsPerBeat;
	}
	public float GetIntervalLength(float pBpm) { return 60f / (pBpm * _stepsPerBeat); }
	public bool CheckForNewInterval(float interval, bool lBeatPlayed)
	{
		if (Mathf.FloorToInt(interval) != _lastInterval)
		{
			_lastInterval = Mathf.FloorToInt(interval);
			if (!lBeatPlayed) trigger.Invoke();
			return true;
		}
		return false;
	}	
}