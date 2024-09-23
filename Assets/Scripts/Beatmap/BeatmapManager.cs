using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatmapManager : MonoBehaviour
{
	public static BeatmapManager Instance;

	public static UnityEvent ON_TriggerNote = new UnityEvent();

	public static float SampledTime;

	public BeatmapSO Beatmap;

	[Header("SOURCES")]
	[SerializeField] private AudioSource _hitSoundSource;
	[SerializeField] private AudioSource _musicSource;
	[SerializeField] private Metronome _metronome;
	[Space]
	[SerializeField] private int _preBeats = 3;

	void Start() {
		if (Instance != null) throw new Exception("Two instances of BeatmapManager exist at the same time");
		Instance = this;

		_musicSource.clip = Beatmap.Audio;
		StartCoroutine(WaitForLoad());

		Beatmap.InitNotes();
		BeatDisplay.ON_InitBeatmap.Invoke(Beatmap.AllNotes);
	}

	private IEnumerator WaitForLoad() {
		yield return new WaitForSeconds(3);
		StartCoroutine(Countdown());
	}

	private IEnumerator Countdown() {

		SampledTime = -_preBeats - 1;
		int lLastPrebeat = 0;

		AnimatorSpeedScaler.ON_SetAnimatorSpeedScales.Invoke(Beatmap.Bpm);
		BeatDisplay.ON_SongStart.Invoke(_preBeats - 1);

		while (Mathf.FloorToInt(SampledTime) < 0) {
			// Countdown
			if (Mathf.FloorToInt(SampledTime) >= lLastPrebeat) {
				lLastPrebeat = Mathf.FloorToInt(SampledTime);
			}
			// Play the metronome sound
			if (_metronome.CheckForNewInterval(SampledTime, false) && _metronome.MetronomeClip) {
				PlayHitSound(_metronome.MetronomeClip);
			}

			SampledTime += Time.deltaTime / (_metronome.GetIntervalLength(Beatmap.Bpm));

			yield return new WaitForEndOfFrame();
		}

		StartCoroutine(MusicCoroutine());
	}

	private IEnumerator MusicCoroutine() {
		_musicSource.Play();

		// Play the first Hit Sound
		PlayHitSound(_metronome.MetronomeClip);
		SampledTime = 0;

		// Play beats
		bool lBeatPlayed;
		while (_musicSource.isPlaying) {
			lBeatPlayed = false;

			SampledTime = _musicSource.timeSamples / (_musicSource.clip.frequency * _metronome.GetIntervalLength(Beatmap.Bpm));

			// Check for the metronome
			if (_metronome.CheckForNewInterval(SampledTime, lBeatPlayed)) {
				if (_metronome.MetronomeClip && !lBeatPlayed) {
					if (_metronome.TriggerFire) ON_TriggerNote.Invoke();

					Metronome.Trigger.Invoke();
					PlayHitSound(_metronome.MetronomeClip);
					lBeatPlayed = true;
				}
			}

			// Check for the notes
			CheckForNote(SampledTime);

			yield return new WaitForEndOfFrame();
		}
	}

	private void CheckForNote(float pSampledTime) {
		// Stop if there is no more notes
		if (Beatmap.AllNotes.Count == 0) {
			Debug.LogWarning("SONG ENDED");
			return;
		}
		// Check for a note
		else if (Beatmap.AllNotes[0] <= pSampledTime) {
			ON_TriggerNote.Invoke();

			// Play hit sound
			if (Beatmap.DefaultHitSound) PlayHitSound(Beatmap.DefaultHitSound);

			// Play VFX
			if (PlayerInputManager.MainFireKeyHold || PlayerInputManager.SecondaryFireKeyHold) {
				BeatDisplay.ON_NoteHit.Invoke(Beatmap.AllNotes[0]);
			}

			Beatmap.AllNotes.RemoveAt(0);
		}
	}

	public void PlayHitSound(AudioClip hitClip) {
		if (hitClip) _hitSoundSource.PlayOneShot(hitClip);
	}

	private void OnDestroy() {
		if (Instance == this) Instance = null;
	}
}