using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Note
{
	public float PositionInMeasure;
	public NoteType _noteType;
	[HideInInspector] public float GlobalOffset;

	public Note CopyNote() {
		Note lNote = new Note();
		lNote.PositionInMeasure = PositionInMeasure;
		lNote._noteType = _noteType;
		lNote.GlobalOffset = GlobalOffset;
		return lNote;
	}	
}

[Serializable]
public enum NoteType {
	Blue,
	Red,
	Yellow
}


