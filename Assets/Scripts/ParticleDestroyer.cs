using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDestroyer : MonoBehaviour
{
	[SerializeField] protected ParticleSystem[] _ps;
	void Start()
	{
		Array.Sort(_ps, (x, y) => (x).main.duration.CompareTo(y.main.duration));

		for (int i = _ps.Length - 1; i >= 0; i--)
		{
			if (!_ps[i].main.loop)
			{
				Destroy(gameObject, _ps[i].main.duration);
				break;
			}
		}
	}
}
