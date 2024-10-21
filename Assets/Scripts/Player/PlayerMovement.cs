using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
	public static bool IsDashing;
	public static UnityEvent ON_Dash = new UnityEvent();
	public static UnityEvent ON_Dash_Stop = new UnityEvent();

	[Header("MOVEMENT")]
	[SerializeField] protected float _moveSpeed = 3;
	[Header("DASH")]
	[SerializeField] protected float _dashDistance = 3;
	[SerializeField] protected float _dashDuration = 0.15f;
	[SerializeField] protected AnimationCurve _dashCurve;
	[Space]
	[Header("MISC")]
	[SerializeField] protected ParticleSystem _dashTrail;
	[SerializeField] protected ParticleSystem _dashRings;


	[HideInInspector] public Vector2 Direction { get; protected set; }

	protected virtual void Start() {
		PlayerInputManager.ON_DashInput.AddListener(DashBegin);
	}

	protected virtual void Update() {
		Move();
	}

	private void Move() {
		transform.position += PlayerInputManager.DirectionInput * _moveSpeed * Time.deltaTime;
	}

	protected virtual void DashBegin(Vector2 pDirection) {
		// You can't start another dash when you're already dashing, dumbass
		if (!IsDashing) StartCoroutine(DashCoroutine(pDirection));
	}

	protected virtual IEnumerator DashCoroutine(Vector3 pDirection) {
		float lT = 0;

		Debug.Log($"DASH SWIPE at {Time.time}");

		Vector3 lStartPos = transform.position;
		Vector3 lEndPos = lStartPos + pDirection.normalized * _dashDistance;
		Vector3 lDashDirection = (lEndPos - lStartPos).normalized;

		// Dash Start
		ON_Dash.Invoke();
		IsDashing = true;
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
		IsDashing = false;
		ON_Dash_Stop?.Invoke();
        _dashTrail.Stop();
		_dashRings.Stop();
	}
}
