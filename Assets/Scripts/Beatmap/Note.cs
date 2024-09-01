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
}
