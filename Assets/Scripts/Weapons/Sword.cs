using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
	[Space]
	[SerializeField] protected Transform _rotater;
	private bool alternate;

	public override void Fire() {
		alternate = !alternate;
		Vector3 lScale = alternate ? new Vector3(1, -1, 1) : Vector3.one;
		_rotater.localScale = lScale;
		Debug.Log($"rotater.rotation = {_rotater.localScale} {Time.time}");
		base.Fire();
	}
}
