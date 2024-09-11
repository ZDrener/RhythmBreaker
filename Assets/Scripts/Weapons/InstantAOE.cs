using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantAOE : Projectile
{
	void Start() {
		Destroy(gameObject, .1f);
	}
}
