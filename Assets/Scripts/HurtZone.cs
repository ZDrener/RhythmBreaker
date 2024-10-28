using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtZone : MonoBehaviour
{
	[SerializeField] protected GameObject m_HitVFX;
	[SerializeField] protected int m_Damage = 1;
	private void OnTriggerEnter(Collider pOther)
	{
		Debug.Log(pOther.gameObject.name);
		if (pOther.gameObject.GetComponentInParent<Player>())
		{
			Player.Instance.TakeDamage(m_Damage);
			Instantiate(m_HitVFX, pOther.ClosestPoint(transform.position), transform.rotation);
		}
	}
}
