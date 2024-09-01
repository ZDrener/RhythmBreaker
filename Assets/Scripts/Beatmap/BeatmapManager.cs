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
	[SerializeField] private Metronome _metronome;

	void Start()
	{
		if (Instance != null) throw new Exception("Two instances of BeatmapManager exist at the same time");
		Instance = this;

		_musicSource.clip = Beatmap.Audio;
		StartCoroutine(WaitBeforeStart());

		Beatmap.InitNotes();
		foreach(Note lNote in Beatmap.AllNotes)
		{
			Debug.Log(lNote.GlobalOffset);
		}
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
		PlayHitSound(_metronome.MetronomeClip);

		// Play beats
		bool lBeatPlayed;
		while (_musicSource.isPlaying)
		{
			lBeatPlayed = false;

			float sampledTime = (_musicSource.timeSamples / (_musicSource.clip.frequency * _metronome.GetIntervalLength(Beatmap.Bpm)));

			if (_metronome.CheckForNewInterval(sampledTime, lBeatPlayed))
			{
				if (_metronome.MetronomeClip && !lBeatPlayed)
				{
					if (_metronome.TriggerFire) ON_TriggerNote.Invoke();

					PlayHitSound(_metronome.MetronomeClip);
					lBeatPlayed = true;
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