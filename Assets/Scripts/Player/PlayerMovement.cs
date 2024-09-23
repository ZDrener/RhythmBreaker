using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[Header("MOVEMENT")]
	[SerializeField] protected float _movementSpeed = 10;
	[SerializeField] protected float _acceleration = 25;
	[SerializeField] protected float _friction = 1500;
	[SerializeField] protected float _dashDistance = 3;
	[SerializeField] protected float _dashDuration = 0.25f;
	[SerializeField] protected AnimationCurve _dashCurve;
	[Space]
	[Header("MISC")]
	[SerializeField] protected ParticleSystem _dashTrail;
	[SerializeField] protected ParticleSystem _dashRings;

	protected bool _isDashing;
	private Vector2 _currentVelocity;

	[HideInInspector] public Vector2 Direction { get; protected set; }

	protected virtual void Start() {
		PlayerInputManager.ON_DashKeyPressed.AddListener(DashBegin);
	}

	protected virtual void Update() {
		Move();
	}

	protected virtual void Move() {
		// Cancel movement if the player is dashing
		if (_isDashing) return;

		// Input direction
		Direction = PlayerInputManager.MovementInput;

		// If there's input, accelerate towards the desired direction
		if (Direction.magnitude > 0) {
			_currentVelocity = Vector2.MoveTowards(_currentVelocity, Direction.normalized * _movementSpeed, _acceleration * Time.deltaTime);
		}
		// If there's no input, apply friction
		else {
			_currentVelocity = Vector2.MoveTowards(_currentVelocity, Vector2.zero, _friction * Time.deltaTime);
		}

		// Apply velocity to the position
		transform.position += (Vector3)(_currentVelocity * Time.deltaTime);
	}

	protected virtual void DashBegin() {
		// You can't start another dash when you're already dashing, dumbass
		if (!_isDashing) StartCoroutine(DashCoroutine(Direction));
	}

	protected virtual IEnumerator DashCoroutine(Vector3 pDirection) {
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
		while (lT < _dashDuration) {
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
