using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
	[Space]
	[SerializeField] protected Transform _rotater;
	[SerializeField] protected TrailRenderer _trail;
	protected bool alternate;

	public override void Fire() {
		alternate = !alternate;
		_trail?.Clear();
		Vector3 lScale = alternate ? new Vector3(1, -1, 1) : Vector3.one;
		_rotater.localScale = lScale;
		base.Fire();
	}
}
