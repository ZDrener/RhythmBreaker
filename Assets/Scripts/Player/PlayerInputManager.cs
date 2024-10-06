using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-10)]
public class PlayerInputManager : MonoBehaviour
{
    public static UnityEvent<Vector2> ON_DashInput = new UnityEvent<Vector2>();
    public static bool AttackInput;
    public static Vector2 DashDirection;
    public static string LastWeaponInput;
    [SerializeField] private float _minSwipeDistance = .2f;
    [SerializeField] private float _maxSwipeTime = .1f;

    private Camera _mainCamera;
    private bool _canDash = true;
    private Vector2 _touchStartPos;
    private float _touchStartTime;

    // Singleton
    public static PlayerInputManager Instance;

    protected virtual void Awake() {
        // Singleton Setup
        if (Instance != null) throw new System.Exception("Two instances of a singleton exist at the same time. Go fix it dumbass.");
        Instance = this;
    }

    protected virtual void Start() {
        BeatmapManager.ON_TriggerNote.AddListener(AllowDash);
        _mainCamera = Camera.main;
    }

    protected virtual void Update() {
        HandleMobile();
    }

    private void HandleMobile() {
        for (int i = Input.touchCount - 1; i >= 0; i--) {
            if (Input.GetTouch(i).phase == TouchPhase.Began) {
                AttackInput = true;
                _touchStartPos = Utils.ScreenToWorld(_mainCamera, Input.GetTouch(i).position); // Record the starting position of the touch
                _touchStartTime = Time.time; // Record the starting time of the touch
                return;
            }
        }
        AttackInput = false;
    }

    protected virtual void AllowDash() {
        _canDash = true;
        StartCoroutine(DashCoroutine());
    }

    protected virtual IEnumerator DashCoroutine() {
        float lElapsedTime = 0;

        while (lElapsedTime <= _maxSwipeTime && _canDash) {
            if (Input.touchCount > 0) {
                Touch touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Ended) break;  // Exit the loop because of a new input

                if (touch.phase == TouchPhase.Moved) {
                    Vector3 currentPos = Utils.ScreenToWorld(_mainCamera, touch.position);
                    Vector3 startPos = _touchStartPos;

                    if (Vector3.Distance(startPos, currentPos) >= _minSwipeDistance && Time.time - _touchStartTime <= _maxSwipeTime) {
                        ON_DashInput.Invoke((currentPos - startPos).normalized);
                        _canDash = false;
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
