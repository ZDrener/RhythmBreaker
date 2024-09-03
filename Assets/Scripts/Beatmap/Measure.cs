using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Measure
{
	public string Name;             // A nickname for the measure, for easier mapping.
	public int LengthInBeats = 4;   // The length of the measure, in beats. Usually 4, but can vary.
	public List<Note> Notes;
}
