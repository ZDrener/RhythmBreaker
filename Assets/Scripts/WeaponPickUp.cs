using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
	[SerializeField] private GameObject _weaponPrefab;
	private void OnTriggerEnter(Collider other) {
		if (other.transform.GetComponentInParent<Player>()) {
			PlayerHands.ON_WeaponPickUp.Invoke(_weaponPrefab);
			Destroy(gameObject);
		}
	}
}
