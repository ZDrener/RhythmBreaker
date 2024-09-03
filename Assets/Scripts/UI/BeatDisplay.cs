using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatDisplay : MonoBehaviour
{
	public static UnityEvent<List<Note>, float> ON_SongStart = new UnityEvent<List<Note>, float>();

	[SerializeField] private GameObject _notePrefab;
	[SerializeField] private RectTransform _track;

	private List<Note> _notesToPreview = new List<Note>();
	private float _offset;
	private bool _songStarted;

	private void Start() {
		ON_SongStart.AddListener(InitPreview);
	}

	private void Update() {
		if (_songStarted) CheckForPreview();
	}

	private void InitPreview(List<Note> pNotes, float pOffset) {
		_offset = pOffset;
		_songStarted = true;

		foreach(Note lNote in pNotes) {
			_notesToPreview.Add(lNote.CopyNote());
		}
		Debug.Log($"potato {_notesToPreview.Count}");
	}

	private void DisplayNotePreview() {
		NotePreview lNote = Instantiate(_notePrefab).GetComponent<NotePreview>();
		((RectTransform)lNote.transform).position = new Vector2(_track.rect.xMax, _track.rect.y + _track.rect.height / 2);
		lNote.transform.SetParent(_track, false);

		Debug.LogWarning($"DISPLAYED NOTE AT TIME {BeatmapManager.SampledTime} WOOOOOOOOOOOOOOOOOOOOOOO");
	}

	private void CheckForPreview() {
		if (_notesToPreview.Count == 0)	return;		
		else if (_notesToPreview[0].GlobalOffset <= BeatmapManager.SampledTime + _offset) {
			DisplayNotePreview();
			_notesToPreview.RemoveAt(0);
		}
	}
}
