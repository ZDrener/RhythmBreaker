using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantAOE : Projectile
{
	void Start() {
		Destroy(gameObject, .15f);
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		DemoDummy lEnemy;
		if (collision.gameObject.TryGetComponent(out lEnemy)) {
			lEnemy.Damage(2);
		}
	}
}
