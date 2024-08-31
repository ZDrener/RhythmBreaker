using UnityEngine;

[CreateAssetMenu(fileName = "New Beatmap", menuName = "Beatmap")]
public class BeatmapSO : ScriptableObject
{
	[Header("INFORMATION")]
	[SerializeField] protected string _name;
	[SerializeField] protected string _artist;
	[SerializeField] protected AudioClip _audio;

	[Space]
	[Header("SETTINGS")]
	[SerializeField] protected float _bpm;
	[SerializeField] protected int _offset;
	[SerializeField] [Range(0, 20)] protected float _difficulty;
	[SerializeField] protected Interval[] _intervals;

	public string Name => _name;
	public string Artist => _artist;
	public AudioClip Audio => _audio;
	public float Bpm => _bpm;
	public int Offset => _offset;
	public float Difficulty => _difficulty;
	public Interval[] Intervals => _intervals;
}