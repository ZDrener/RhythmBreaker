using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStats", menuName = "RhythmBreaker/Weapon/WeaponStats")]
public class WeaponStatSO : ScriptableObject
{
	[SerializeField] protected GameObject _projectilePrefab;
	[SerializeField] protected GameObject _muzzlePrefab;
	[SerializeField] protected float _damagePerfect;
	[SerializeField] protected float _damageGreat;
	[SerializeField] protected float _damageOk;
	[SerializeField] protected float _range;
	[SerializeField] protected int _piercing;
	[SerializeField] protected float _lifetime;
	[SerializeField] protected float _speed;

	public GameObject projectilePrefab => _projectilePrefab;
	public GameObject muzzlePrefab => _muzzlePrefab;
	public float damagePerfect => _damagePerfect;
	public float damageGreat => _damageGreat;
	public float damageOk => _damageOk;
	public float range => _range;
	public int piercing => _piercing;
	public float lifetime => _lifetime;
	public float speed => _speed;
}
