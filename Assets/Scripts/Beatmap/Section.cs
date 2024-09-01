using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Section
{
	public string Name;			// A nickname for the section, for easier mapping.
	public bool IsKiai;			// Define if we should trigger additionnal visual effects during this section.
	[Space]
	public List<Section> SubSections = new List<Section>();
	public List<Measure> Measures = new List<Measure>();
}
