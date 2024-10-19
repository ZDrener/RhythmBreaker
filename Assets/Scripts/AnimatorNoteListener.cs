using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorNoteListener : AnimatorSpeedScaler
{
	[SerializeField] protected NoteType _listenedNoteType;

	protected override void Start() {
		BeatmapManager.ON_TriggerNote.AddListener(PlayAnim);
	}

	protected virtual void PlayAnim(NoteType pNoteType) {
		if (_listenedNoteType == pNoteType) _animator.SetBool("0", true);
	}
}
