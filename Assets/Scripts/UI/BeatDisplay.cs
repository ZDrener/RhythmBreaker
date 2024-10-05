using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.ParticleSystem;

public class BeatDisplay : MonoBehaviour
{
	public static UnityEvent<List<float>> ON_InitBeatmap = new UnityEvent<List<float>>();
	public static UnityEvent<float> ON_SongStart = new UnityEvent<float>();
	public static UnityEvent<float> ON_NoteHit = new UnityEvent<float>();

	[SerializeField] private GameObject _notePrefab;
	[SerializeField] private RectTransform _container;
	[Space]
	[SerializeField] private LineRenderer _lineRenderer;
	[SerializeField] private int _segments = 32; // Number of segments to define the circle shape
	[SerializeField] private float _endRadius = 200;
	[Space]
	[SerializeField] private List<NoteColors> _noteColors;

	private List<float> _notesToPreview = new List<float>();
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

	private void InitNotes(List<float> pNotes) {
		foreach (float lNote in pNotes) {
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

	private void DisplayNotePreview(float pOffset) {

		Color lColor = GetNoteColor(pOffset);

		// Note right
		NotePreview lNote = Instantiate(_notePrefab).GetComponent<NotePreview>();
		lNote.Init(Vector3.one, _offset, lColor);
		lNote.transform.SetParent(_container, false);
	}

	private Color GetNoteColor(float pOffset) {
		Color lColor = Color.white;
		for (int i = 0; i < _noteColors.Count; i++) {
			if (pOffset % _noteColors[i].Offset == 0) lColor = _noteColors[i].Color;
		}
		return lColor;
	}

	private void CheckForPreview() {
		if (_notesToPreview.Count == 0) return;
		else if (_notesToPreview[0] <= BeatmapManager.SampledTime + _offset) {
			DisplayNotePreview(_notesToPreview[0]);
			_notesToPreview.RemoveAt(0);
		}
	}
}

[Serializable]
public class NoteColors
{
	public float Offset;
	public Color Color;
}
