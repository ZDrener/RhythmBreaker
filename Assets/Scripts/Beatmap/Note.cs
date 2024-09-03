using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Note
{
	public float PositionInMeasure;
	[HideInInspector] public AudioClip CustomClip;
	[HideInInspector] public float GlobalOffset;

	public Note CopyNote() {
		Note lNote = new Note();
		lNote.PositionInMeasure = PositionInMeasure;
		lNote.CustomClip = CustomClip;
		lNote.GlobalOffset = GlobalOffset;
		return lNote;
	}
}
