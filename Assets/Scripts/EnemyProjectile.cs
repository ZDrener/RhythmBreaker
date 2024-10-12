using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : Projectile
{
	protected float m_Speed;
	protected float m_Lifetime;
	protected int m_Damage;
	protected GameObject m_Target;
	protected Vector2 m_MovementDirection;


    public void InitAndStart(GameObject pTarget, float pSpeed = 5f, int pDamage = 1, float pLifetime = 2.5f)
	{
		m_Target = pTarget;
		m_Speed = pSpeed ;
		m_Damage = pDamage ;
		m_Lifetime = pLifetime ;

		StartProjectile();
	}

	public void StartProjectile()
	{
		m_MovementDirection = (m_Target.transform.position - transform.position).normalized;
		transform.rotation *= Quaternion.AngleAxis(Vector2.SignedAngle(transform.right, m_MovementDirection), Vector3.forward);


		StartCoroutine(ManageMovement()); 
	}


    private void OnTriggerEnter(Collider pOther)
    {
        if (pOther.gameObject.GetComponentInParent<Player>())
        {
            Player.Instance.TakeDamage(m_Damage);
            // Could add hit SFXs and such here or through a call to a function, instead of simply destroying
            Destroy(gameObject);
        }
    }

    protected IEnumerator ManageMovement()
	{
		float lTime = 0f;

		while (lTime < m_Lifetime)
		{
			lTime += Time.deltaTime;

			transform.position += m_Speed * Time.deltaTime * (Vector3)m_MovementDirection;

			yield return new WaitForFixedUpdate();
		}

		Destroy(gameObject);
		yield break;
	}
}
