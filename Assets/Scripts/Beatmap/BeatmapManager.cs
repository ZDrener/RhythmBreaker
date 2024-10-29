using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class BeatmapManager : MonoBehaviour
{
	public static BeatmapManager Instance;

	public static UnityEvent<NoteType> ON_TriggerNote = new UnityEvent<NoteType>();

	public static float SampledTime;

	public BeatmapSO Beatmap;

	[Header("SOURCES")]
	[SerializeField] private AudioSource _hitSoundSource;
	[SerializeField] private AudioSource _musicSource;
	[SerializeField] private AudioSource _slowMotionSource;
	public float AudioVolume
	{
		get { return _audioVolume; }
		protected set
		{
			_audioVolume = value;
			_hitSoundSource.volume = _audioVolume;
			_musicSource.volume = _audioVolume;
			AudioVolumeChangeEvent?.Invoke(_audioVolume);
        }
	}
	protected float _audioVolume = 1.0f;
	[SerializeField] private Metronome _metronome;
	[Space]
	[SerializeField] private int _preBeats = 3;
	[Space]
	[Header("INPUT BUFFER")]
	public float InputBufferWindow = 0.2f; // Adjust this value as needed
	[Space]
	[Header("AUDIO CLIPS")]
	[SerializeField] protected AudioClip _slowMotionStart;
	[SerializeField] protected AudioClip _slowMotionEnd;
	[Space]
	[Header("STUFF")]
	[SerializeField] protected GameObject _restartPrefab;
	protected bool _songEnded = false;

	public delegate void SimpleEvent();
	public static event SimpleEvent SongStartEvent;
	public static event SimpleEvent SongStopEvent;
	public static event SimpleEvent TriggerNoteEndEvent;

	public static UnityEvent<float, float> SlowdownEffectEvent = new UnityEvent<float, float>();
	public static UnityEvent<float> AudioVolumeChangeEvent = new UnityEvent<float>();
	protected float _slowdownTransitionDuration = 0.3f;
	protected float CurrentSlowdownFactor
	{
		get { return _currentSlowdownFactor; }
		set 
		{
			_currentSlowdownFactor = value;
			SlowdownEffectEvent?.Invoke(_currentSlowdownFactor, 
				1 - ((_currentSlowdownFactor - _slowdownForce) / (1f - _slowdownForce)));
		}
	}
	protected float _currentSlowdownFactor = 1f;
	protected float _slowdownForce = 0.1f;
	protected Coroutine _slowdownCoroutine;
	protected bool _playerInFinisher = false;
	protected float _slowedVolume = 0.3f;

	private void Awake()
	{
		Player.PlayerDeathEvent += StopSong;
		Player.PlayerFinisherStart.AddListener(OnPlayerFinisherStart);
		Player.PlayerFinisherEnd.AddListener(OnPlayerFinisherEnd);
	}

	void Start() {
		if (Instance != null) throw new Exception("Two instances of BeatmapManager exist at the same time");
		Instance = this;

	}
	
	public void StartSong()
	{
		_musicSource.clip = Beatmap.Audio;
		StartCoroutine(WaitForLoad());

		Beatmap.InitNotes();
		BeatDisplay.ON_InitBeatmap.Invoke(Beatmap.AllNotes);
	}

	private void StopSong()
	{
		_songEnded = true;
		_musicSource.Stop();
		SongStopEvent?.Invoke();
		Instantiate(_restartPrefab);
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
		SongStartEvent?.Invoke();

		// Play the first Hit Sound
		PlayHitSound(_metronome.MetronomeClip);
		SampledTime = 0;

		// Play beats
		bool lBeatPlayed;
		while (_musicSource.isPlaying) {
			lBeatPlayed = false;

			// Note Sampling
			SampledTime = _musicSource.timeSamples / (_musicSource.clip.frequency * _metronome.GetIntervalLength(Beatmap.Bpm));

			// Check for the metronome
			if (_metronome.CheckForNewInterval(SampledTime, lBeatPlayed)) {
				if (_metronome.MetronomeClip && !lBeatPlayed) {

					Metronome.Trigger.Invoke();
					PlayHitSound(_metronome.MetronomeClip);
					lBeatPlayed = true;
				}
			}

			// Check for the notes
			CheckForNote(SampledTime);
			if (_songEnded)
				yield break;

			yield return new WaitForEndOfFrame();
		}
	}

	protected IEnumerator ManageSlowdownEffect(bool pIsSlowInit)
	{
		float lTime = 0;
		float lStartFactor = pIsSlowInit ? _currentSlowdownFactor : _slowdownForce;
		float lTargetFactor = pIsSlowInit ? _slowdownForce : 1f;
		float lStartVolume = pIsSlowInit ? _musicSource.volume : _slowedVolume;
		float lTargetVolume = pIsSlowInit ? _slowedVolume : 1f;

		if (_slowMotionSource.isPlaying) _slowMotionSource.Stop();

		_slowMotionSource.clip = pIsSlowInit ? _slowMotionStart : _slowMotionEnd;
		_slowMotionSource.Play();

		while (lTime < _slowdownTransitionDuration)
		{
			lTime += Time.deltaTime;
			CurrentSlowdownFactor = Mathf.Lerp(lStartFactor, lTargetFactor, lTime / _slowdownTransitionDuration);
			AudioVolume = Mathf.Lerp(lStartVolume, lTargetVolume, lTime / _slowdownTransitionDuration);
            yield return null;
		}

		CurrentSlowdownFactor = lTargetFactor;
		_musicSource.volume = lTargetVolume;
		_slowdownCoroutine = null;


		yield break;
	}

	protected virtual void OnPlayerFinisherStart()
	{
		_playerInFinisher = true;
		_slowdownCoroutine = StartCoroutine(ManageSlowdownEffect(true));
	}

	protected virtual void OnPlayerFinisherEnd()
	{
		if (_slowdownCoroutine != null)
		{
			StopCoroutine(_slowdownCoroutine);
			_slowdownCoroutine = null;
		}

		_slowdownCoroutine = StartCoroutine(ManageSlowdownEffect(false));
		_playerInFinisher = false;
	}

	private void CheckForNote(float pSampledTime) {
		// Stop if there are no more notes
		if (Beatmap.AllNotes.Count == 0) {
			Debug.LogWarning("SONG ENDED");
			SongEnd();
			return;
		}

		Note lNote = Beatmap.AllNotes[0];
		float noteTime = lNote.GlobalOffset;
		float timeDifference = noteTime - pSampledTime;

		bool pCondition = DebugGameMode.Instance.AllowHold ?
			noteTime <= pSampledTime :
			(noteTime <= pSampledTime) && (noteTime + InputBufferWindow >= pSampledTime);        

		// Check if the note is within the buffer window and if the player pressed it
		if (pCondition) {

			ON_TriggerNote.Invoke(lNote._noteType);
			Beatmap.AllNotes.RemoveAt(0);
			TriggerNoteEndEvent?.Invoke();
		}
		#region OLD WAY / FAIL
		//// check if the note is within the buffer window and if the player pressed it
		//if (pcondition/* && playerinputmanager.attackinput && lnote._notetype == playerinputmanager.attacktype*/) {

		//	on_triggernote.invoke(lnote._notetype);
		//	beatmap.allnotes.removeat(0);
		//	triggernoteendevent?.invoke();

		//	// play hit sound
  //          if (beatmap.defaulthitsound) playhitsound(beatmap.defaulthitsound);

		//	// add a bar to the accuracy display
		//	accuracydisplay.displayaccuracyevent.invoke((notetime - psampledtime) / inputbufferwindow);

		//	// remove the note from the list once it's hit
		//	beatmap.allnotes.removeat(0);
		//}
		//// destroy note on fail
		//else if (notetime - psampledtime < -inputbufferwindow) {
		//	beatmap.allnotes.removeat(0);
		//	if (beatmap.misshitsound) playhitsound(beatmap.misshitsound);
		//}
		#endregion
	}

	public float GetNotePrediction(float pPpredictionTimeReach, NoteType pDesiredNoteType)
	{
		return Beatmap.GetNotePrediction(SampledTime, pPpredictionTimeReach, pDesiredNoteType);
	}

	protected void SongEnd()
	{
		StopSong();
	}

	public void PlayHitSound(AudioClip hitClip) {
		if (hitClip) _hitSoundSource.PlayOneShot(hitClip);
	}

	private void OnDestroy() {
		if (Instance == this) Instance = null;

		Player.PlayerDeathEvent -= StopSong;
	}
}