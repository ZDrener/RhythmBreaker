using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[Header("MOVEMENT")]
	[SerializeField] protected float _movementSpeed = 3;
	[SerializeField] protected float _dashDistance = 3;
	[SerializeField] protected float _dashDuration = 0.25f;
	[SerializeField] protected AnimationCurve _dashCurve;
	[Space]
	[Header("MISC")]
	[SerializeField] protected ParticleSystem _dashTrail;
	[SerializeField] protected ParticleSystem _dashRings;

	protected bool _isDashing;

	[HideInInspector] public Vector2 Direction { get; protected set; }

	protected virtual void Start()
	{
		PlayerInputManager.ON_Movement.AddListener(Move);
		PlayerInputManager.ON_DashKeyPressed.AddListener(DashBegin);
	}
	protected virtual void Move(Vector2 pDirection)
	{
		// Cancel movement if the player is dashing
		if (_isDashing) return;

		// Movement
		Direction = pDirection;

		Vector2 lVelocity = Direction.normalized * _movementSpeed * Time.deltaTime;

		transform.position = transform.position + (Vector3)lVelocity;
	}
	protected virtual void DashBegin()
	{
		// You can't start another dash when you're already dashing, dumbass
		if (!_isDashing) StartCoroutine(DashCoroutine(Direction));
	}

	protected virtual IEnumerator DashCoroutine(Vector3 pDirection)
	{
		float lT = 0;
		Vector3 lStartPos = transform.position;
		Vector3 lEndPos = lStartPos + pDirection.normalized * _dashDistance;
		Vector3 lDashDirection = (lEndPos - lStartPos).normalized;

		// Dash Start
		_isDashing = true;
		_dashTrail.Play();
		_dashRings.Play();
		_dashRings.transform.rotation = Quaternion.LookRotation(lDashDirection);

		// Dash Loop
		while (lT < _dashDuration)
		{
			transform.position = Vector3.Lerp(lStartPos, lEndPos, _dashCurve.Evaluate(lT / _dashDuration));
			lT += Time.deltaTime;

			yield return new WaitForEndOfFrame();
		}

		// Dash End
		_isDashing = false;
		_dashTrail.Stop();
		_dashRings.Stop();
	}
}
