using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-10)]
public class PlayerInputManager : MonoBehaviour
{
	public static UnityEvent<Vector2> ON_DashInput = new UnityEvent<Vector2>();
	public static UnityEvent<NoteType> ON_ArcheroAttackInput = new UnityEvent<NoteType>();
	public static Vector3 DirectionInput;
	public static bool AttackInput;
	public static NoteType AttackType;
	public static Vector2 DashDirection;
	public static string LastWeaponInput;
	[SerializeField] private float _minSwipeDistance = 10f;
	[SerializeField] private float _maxSwipeTime = .1f;
	[SerializeField] private VariableJoystick _joystick;
	[Space]
	[Header("ARCHERO GAMEPLAY")]
	public bool ArcheroGameplay;
	[SerializeField] private float _fireDelay;
	private float _timeSinceLastShot = 0;

	private Camera _mainCamera;
	private bool _canDash = true;
	private Vector2 _touchStartPos;
	private float _touchStartTime;

	protected float _minSwipeDistanceFinisherFactor = 0.05f;
	protected bool _playerIsInFinisher = false;

	// Singleton
	public static PlayerInputManager Instance;

	protected virtual void Awake() {
		// Singleton Setup
		if (Instance != null) throw new Exception("Two instances of a singleton exist at the same time. Go fix it dumbass.");
		Instance = this;

		Player.PlayerFinisherStart.AddListener(OnPlayerFinisherStart);
		Player.PlayerFinisherEnd.AddListener(OnPlayerFinisherEnd);
	}

	protected void OnPlayerFinisherStart()
	{
		_playerIsInFinisher = true;
    }

	protected void OnPlayerFinisherEnd()
	{
		_playerIsInFinisher = false;
    }


    protected virtual void Start() {
		_mainCamera = Camera.main;
	}

	protected virtual void Update() {
		if (ArcheroGameplay) HandleArcheroInput();
		else HandleMobileTouch();
		DirectionInput = _joystick.Direction;
	}

	private void HandleArcheroInput() {
		//_timeSinceLastShot += Time.deltaTime;

		// joystick direction input
		if (_joystick.Direction == Vector2.zero) {
			_timeSinceLastShot = 0;
			AttackInput = true;  // We don't care about the type lol
		}
		else AttackInput = false;

		// swipe dash input
		Touch lTouch;
		for (int i = Input.touchCount - 1; i >= 0; i--) {
			lTouch = Input.GetTouch(i);
			if (lTouch.phase == TouchPhase.Began) {
				_touchStartPos = Utils.ScreenToWorld(_mainCamera, Input.GetTouch(i).position); // Record the starting position of the touch
				_touchStartTime = Time.time; // Record the starting time of the touch
				AllowDash(lTouch);
			}
		}
	}

	private void HandleMobileTouch() {
		// Reset Input
		AttackInput = false;

		// For every finger on screen
		Touch lTouch;
		for (int i = Input.touchCount - 1; i >= 0; i--) {
			lTouch = Input.GetTouch(i);

			if (lTouch.phase == TouchPhase.Began || lTouch.phase == TouchPhase.Moved || lTouch.phase == TouchPhase.Stationary) {
				// Screen right side => Attack input
				if (lTouch.position.x >= _mainCamera.scaledPixelWidth / 2) {
					AttackInput = true;

					// Determine if the cursor is in red or blue
					if ((lTouch.position.y <= _mainCamera.scaledPixelHeight / 2) || (Vector2.Dot(Vector2.down, lTouch.deltaPosition.normalized) > 0.8f) && lTouch.deltaPosition.magnitude >= 5) {
						AttackType = NoteType.Red;
					}
					if ((lTouch.position.y >= _mainCamera.scaledPixelHeight / 2) || ((Vector2.Dot(Vector2.up, lTouch.deltaPosition.normalized) > 0.8f) && lTouch.deltaPosition.magnitude >= 5)) {
						AttackType = NoteType.Blue;
					}
				}
			}
		}
	}

	protected virtual void AllowDash(Touch lTouch) {
		_canDash = true;
		StartCoroutine(DashCoroutine(lTouch));
	}

	protected virtual IEnumerator DashCoroutine(Touch lTouch) {
		float lElapsedTime = 0;

		while (lElapsedTime <= _maxSwipeTime && _canDash) {
			if (Input.touchCount > 0) {
				Touch touch = Input.GetTouch(lTouch.fingerId);

				if (touch.phase == TouchPhase.Ended) break;  // Exit the loop because of a new input

				if (touch.phase == TouchPhase.Moved) {
					Vector3 currentPos = Utils.ScreenToWorld(_mainCamera, touch.position);
					Vector3 startPos = _touchStartPos;

					if (Vector3.Distance(startPos, currentPos) >= (!_playerIsInFinisher ? _minSwipeDistance : _minSwipeDistance * _minSwipeDistanceFinisherFactor) && Time.time - _touchStartTime <= _maxSwipeTime) {
						ON_DashInput.Invoke((currentPos - startPos).normalized);
						break; // Exit the loop since dash is triggered
					}
				}
			}

			lElapsedTime += Time.deltaTime;
			yield return null; // Using null for a cleaner frame wait
		}
	}

	protected virtual void OnDestroy() {
		if (Instance == this) Instance = null;
	}
}
