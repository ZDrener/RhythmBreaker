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
	protected Coroutine _dashCoroutine;
	[Space]
	[Header("MISC")]
	[SerializeField] protected ParticleSystem _dashTrail;
	[SerializeField] protected ParticleSystem _dashRings;

	protected bool IsPlayerFinishing => Player.Instance.IsFinishing;
	protected bool _playerFinished = false;

	[HideInInspector] public Vector2 Direction { get; protected set; }

	protected virtual void Awake()
	{
        PlayerInputManager.ON_DashInput.AddListener(DashBegin);
		Player.PlayerFinisherStart.AddListener(PlayerFinisherStart);
		Player.PlayerFinisherEnd.AddListener(PlayerFinisherEnd);
    }

	protected virtual void Update() {
		Move();
	}

	private void Move() {
		if (IsPlayerFinishing) return;
		transform.position += _moveSpeed * Time.deltaTime * PlayerInputManager.DirectionInput;
	}

	protected virtual void DashBegin(Vector2 pDirection) {
		// You can't start another dash when you're already dashing, dumbass <- meanie
		if (!IsDashing) _dashCoroutine = StartCoroutine(DashCoroutine(pDirection));
	}

	protected virtual IEnumerator DashCoroutine(Vector3 pDirection) {
		float lT = 0;

		Vector3 lStartPos = transform.position;
		Vector3 lEndPos = lStartPos + pDirection.normalized * _dashDistance;
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

		if (IsPlayerFinishing) yield break;
		if (_playerFinished) { _playerFinished = false; yield break; }

        // Dash Start
        SetDash(true);
		_dashRings.transform.rotation = Quaternion.LookRotation(lDashDirection);

		// Dash Loop
		while (lT < _dashDuration) {
			transform.position = Vector3.Lerp(lStartPos, lEndPos, _dashCurve.Evaluate(lT / _dashDuration));
			lT += Time.deltaTime;

			yield return new WaitForEndOfFrame();
		}

		// Dash End
		ON_Dash_Stop?.Invoke();
        SetDash(false);
		_dashCoroutine = null;
    }

	protected void SetDash(bool pIsActive)
	{
		IsDashing = pIsActive;

		if (pIsActive)
		{
            _dashTrail.Play();
            _dashRings.Play();
        }
		else
		{
            _dashTrail.Stop();
            _dashRings.Stop();
        }
	}

	protected void PlayerFinisherStart()
	{
		if (_dashCoroutine != null)
		{
			StopCoroutine(_dashCoroutine);
            _dashCoroutine = null;
        }

        SetDash(false);
	}
	
    protected void PlayerFinisherEnd()
	{
		_playerFinished = true;
    }
}

public enum DashDirection { Up, Down, Left, Right }
