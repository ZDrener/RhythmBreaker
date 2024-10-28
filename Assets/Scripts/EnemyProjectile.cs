using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : Projectile
{
	[Header("STATS")]
	[Space]
	[SerializeField] protected float m_Speed = 5f;
	[SerializeField] protected float m_Lifetime = 2.5f;
	[SerializeField] protected int m_Damage = 1;
	[Header("VFXS")]
	[Space]
	[SerializeField] protected GameObject m_HitVFX;
	protected GameObject m_Target;
	protected Vector2 m_MovementDirection;

	protected float m_SlowdownTransitionDuration = 0.3f;
	protected float m_CurrentSlowdownFactor = 1f;
	protected float m_SlowdownForce = 0.1f;
	protected Coroutine m_SlowdownCoroutine;
	protected bool m_PlayerInFinisher = false;

	private void Awake()
	{
		Player.PlayerFinisherStart.AddListener(OnPlayerFinisherStart);
		Player.PlayerFinisherEnd.AddListener(OnPlayerFinisherEnd);
	}

	protected virtual void OnPlayerFinisherStart()
	{
		m_SlowdownCoroutine = StartCoroutine(ManageSlowdown(true));
	}

	protected virtual void OnPlayerFinisherEnd()
	{
		if (m_SlowdownCoroutine != null)
		{
			StopCoroutine(m_SlowdownCoroutine);
			m_SlowdownCoroutine = null;
		}

		m_SlowdownCoroutine = StartCoroutine(ManageSlowdown(false));
	}

	public void InitAndStart(GameObject pTarget/*, float pSpeed = 5f, int pDamage = 1, float pLifetime = 2.5f*/)
	{
		m_Target = pTarget;
		/*m_Speed = pSpeed ;
		m_Damage = pDamage ;
		m_Lifetime = pLifetime ;*/

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
			Instantiate(m_HitVFX, pOther.ClosestPoint(transform.position), transform.rotation);
			Destroy(gameObject);
		}
	}

	protected IEnumerator ManageMovement()
	{
		float lTime = 0f;

		while (lTime < m_Lifetime)
		{
			lTime += Time.fixedDeltaTime * m_CurrentSlowdownFactor;
			transform.position += m_Speed * Time.fixedDeltaTime * m_CurrentSlowdownFactor * (Vector3)m_MovementDirection;
			yield return new WaitForFixedUpdate();
		}

		Destroy(gameObject);
	}

	protected IEnumerator ManageSlowdown(bool pIsSlowInit)
	{
		float lTime = 0;
		float lStartFactor = pIsSlowInit ? 1f : m_SlowdownForce;
		float lTargetFactor = pIsSlowInit ? m_SlowdownForce : 1f;

		while (lTime < m_SlowdownTransitionDuration)
		{
			lTime += Time.deltaTime;
			m_CurrentSlowdownFactor = Mathf.Lerp(lStartFactor, lTargetFactor, lTime/m_SlowdownTransitionDuration);
			yield return null;
		}

		m_CurrentSlowdownFactor = lTargetFactor;
		m_SlowdownCoroutine = null;
	}
}
