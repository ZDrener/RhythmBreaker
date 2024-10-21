using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
	public static bool IsDashing { get; protected set; }
	public static UnityEvent ON_Dash_Stop = new UnityEvent();
	public static UnityEvent<DashDirection> ON_Dash = new UnityEvent<DashDirection>();

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

	protected bool IsPlayerFinishing => Player.Instance.IsFinishing;

	[HideInInspector] public Vector2 Direction { get; protected set; }

	protected virtual void Awake()
	{
        PlayerInputManager.ON_DashInput.AddListener(DashBegin);
    }

	protected virtual void Update() {
		Move();
	}

	private void Move() {
		transform.position += (IsPlayerFinishing ? _moveSpeed * 0.25f : _moveSpeed) * Time.deltaTime * PlayerInputManager.DirectionInput;
	}

	protected virtual void DashBegin(Vector2 pDirection) {
		// You can't start another dash when you're already dashing, dumbass <- meanie
		if (!IsDashing) StartCoroutine(DashCoroutine(pDirection));
	}

	protected virtual IEnumerator DashCoroutine(Vector3 pDirection) {
		float lT = 0;

		Vector3 lStartPos = transform.position;
		Vector3 lEndPos = lStartPos + pDirection.normalized * (IsPlayerFinishing ? _dashDistance * 0.25f : _dashDistance);
		Vector3 lDashDirection = (lEndPos - lStartPos).normalized;

		float lMaxDot = .7f;

		// Determine closest direction
		if (Vector3.Dot(lDashDirection, Vector3.up) > lMaxDot) {
			ON_Dash.Invoke(DashDirection.Up);
		}
		else if (Vector3.Dot(lDashDirection, Vector3.down) > lMaxDot) {
			ON_Dash.Invoke(DashDirection.Down);
		}
		else if (Vector3.Dot(lDashDirection, Vector3.left) > lMaxDot) {
			ON_Dash.Invoke(DashDirection.Left);
		}
		else if (Vector3.Dot(lDashDirection, Vector3.right) > lMaxDot) {
			ON_Dash.Invoke(DashDirection.Right);
		}

		// Dash Start
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

public enum DashDirection { Up, Down, Left, Right }
