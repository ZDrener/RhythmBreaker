using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShadowProperties", menuName = "RhythmBreaker/Shadow")]
public class ShadowSO : ScriptableObject
{
	[SerializeField] protected GameObject _shadowPrefab;
	[SerializeField] protected Color _color = Color.black;
	[SerializeField] protected Vector3 _offset = new Vector3(0, -(1 / 16), 0);
	[SerializeField] protected int _offsetMultiplier;
	[SerializeField] protected int _zOrder = -1;

	public GameObject shadowPrefab => _shadowPrefab;
	public Color color => _color;
	public Vector3 offset => _offset * _offsetMultiplier;
	public int zOrder => _zOrder;
}
