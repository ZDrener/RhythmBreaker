using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BeatDisplay : MonoBehaviour
{
	public static UnityEvent<List<Note>> ON_InitBeatmap = new UnityEvent<List<Note>>();
	public static UnityEvent<float> ON_SongStart = new UnityEvent<float>();

	[SerializeField] private GameObject _notePrefab;
	[SerializeField] private RectTransform _container;
	[Space]
	[SerializeField] private LineRenderer _lineRenderer;
	[SerializeField] private int _segments = 32; // Number of segments to define the circle shape
	[SerializeField] private float _endRadius = 200;
	[Space]
	[SerializeField] private List<NoteColors> _noteColors;

	private List<Note> _notesToPreview = new List<Note>();
	private float _offset;
	private bool _songStarted;

	private void Awake() {
		ON_InitBeatmap.AddListener(InitNotes);
		ON_SongStart.AddListener(InitPreview);
	}

	private void Start() {
		// Initialize the LineRenderer properties
		_lineRenderer.positionCount = _segments + 1; // +1 to close the circle
		_lineRenderer.useWorldSpace = false;

		float angle = 0f;
		for (int i = 0; i <= _segments; i++) {
			float x = Mathf.Cos(Mathf.Deg2Rad * angle) * _endRadius;
			float y = Mathf.Sin(Mathf.Deg2Rad * angle) * _endRadius;
			_lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
			angle += 360f / _segments;
		}
	}

	private void InitNotes(List<Note> pNotes) {
		foreach (Note lNote in pNotes) {
			_notesToPreview.Add(lNote);
		}
	}

	private void Update() {
		if (_songStarted) CheckForPreview();
	}

	private void InitPreview(float pOffset) {
		_offset = pOffset;
		_songStarted = true;
	}

	private void DisplayNotePreview(float pOffset, NoteType pType) {

		Color lColor = GetNoteColor(pType);

		// Note right
		NotePreview lNote = Instantiate(_notePrefab).GetComponent<NotePreview>();
		lNote.Init(Vector3.one, _offset, lColor);
		lNote.transform.SetParent(_container, false);
	}

	private Color GetNoteColor(NoteType pType) {
		Color lColor = Color.white;
		for (int i = 0; i < _noteColors.Count; i++) {
			if (pType == _noteColors[i].Type) lColor = _noteColors[i].Color;
		}
		return lColor;
	}

	private void CheckForPreview() {
		if (_notesToPreview.Count == 0) return;
		else if (_notesToPreview[0].GlobalOffset <= BeatmapManager.SampledTime + _offset) {
			DisplayNotePreview(_notesToPreview[0].GlobalOffset, _notesToPreview[0]._noteType);
			_notesToPreview.RemoveAt(0);
		}
	}
}

[Serializable]
public class NoteColors
{
	public NoteType Type;
	public Color Color;
}
