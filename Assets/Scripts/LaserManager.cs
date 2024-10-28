using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserManager : MonoBehaviour
{
	[SerializeField] private GameObject _laserPrefab;
	[SerializeField] private NoteType _laserNoteType;
	[SerializeField] private float _laserTimeOffset;	// Must be positive

	private List<Note> _laserNotes = new List<Note>();
	private bool _searching;

	void Start()
    {
		BeatDisplay.ON_InitBeatmap.AddListener(InitLaserNotes);
		BeatDisplay.ON_SongStart.AddListener(StartSearchingForLaser);
	}
	void Update()
    {
		if (_searching) CheckForPreview();
    }
	private void StartSearchingForLaser(float arg0)	// Don't care about the arg (._.'
	{
		_searching = true;
		BeatDisplay.ON_SongStart.RemoveListener(StartSearchingForLaser);
	}
	private void InitLaserNotes(List<Note> pNotes)
	{
		foreach (Note lNote in pNotes)		
			if(lNote._noteType == _laserNoteType) _laserNotes.Add(lNote);
	}
	private void CheckForPreview()
	{
		if (_laserNotes.Count == 0) return;
		else if (_laserNotes[0].GlobalOffset <= BeatmapManager.SampledTime + _laserTimeOffset)
		{
			CreateLaser(_laserNotes[0].GlobalOffset);
			_laserNotes.RemoveAt(0);
		}
	}
	private void CreateLaser(float pLaserActivationTime)
	{
		GlobalLaser lLaser = Instantiate(_laserPrefab, Vector3.zero, Quaternion.identity).GetComponent<GlobalLaser>();
		lLaser.Init(pLaserActivationTime - _laserTimeOffset, pLaserActivationTime);
	}
}
