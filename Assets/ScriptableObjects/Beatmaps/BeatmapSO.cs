using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Beatmap", menuName = "Beatmap")]
public class BeatmapSO : ScriptableObject
{
	[Header("INFORMATION")]
	[SerializeField] protected string _name;
	[SerializeField] protected string _artist;
	[SerializeField] protected AudioClip _audio;
	[SerializeField] protected AudioClip _defaultHitSound;
	[SerializeField] protected AudioClip _missHitSound;

	[Space]
	[Header("SETTINGS")]
	[SerializeField] protected float _bpm;
	[SerializeField] protected int _offset;
	[SerializeField] [Range(0, 20)] protected float _difficulty;

	[Space]
	[Header("CHARTING")]
	[SerializeField] protected List<Section> Sections;
	protected List<float> _allNotes;

	public string Name => _name;
	public string Artist => _artist;
	public AudioClip Audio => _audio;
	public AudioClip DefaultHitSound => _defaultHitSound;
	public AudioClip MissHitSound => _missHitSound;
	public float Bpm => _bpm;
	public int Offset => _offset;
	public float Difficulty => _difficulty;
	public List<float> AllNotes => _allNotes;

	public void InitNotes()
	{
		List<float> allNotes = new List<float>();
		float globalOffset = 0f;

		// Function to process a section
		void ProcessSection(Section section, ref float currentGlobalOffset)
		{
			foreach (var measure in section.Measures)
			{
				// For each note in the measure, calculate its GlobalOffset and add it to the list
				foreach (var note in measure.Notes)	allNotes.Add(note + currentGlobalOffset);				

				// Update the currentGlobalOffset by adding the length of the current measure
				currentGlobalOffset += measure.LengthInBeats;
			}

			// Recursively process any subsections
			foreach (var subSection in section.SubSections)
			{
				ProcessSection(subSection, ref currentGlobalOffset);
			}
		}

		// Process each top-level section
		foreach (var section in Sections)
		{
			ProcessSection(section, ref globalOffset);
		}

		// Sort the notes
		allNotes.Sort(delegate (float n1, float n2) { return n1.CompareTo(n2); });

		_allNotes = allNotes;
	}
}