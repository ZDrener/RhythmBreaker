using System;
using UnityEngine;
using UnityEngine.Events;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class PlayerInputManager : MonoBehaviour
{
    // Bools (for buffering)
    public static bool AttackInput;
    public static bool DashKeyHold;
    public static string LastWeaponInput;

    private TouchControls _touchControls;

    // Singleton
    public static PlayerInputManager Instance;

    protected virtual void Start() {
        if (Instance != null) throw new System.Exception("Two instances of a singleton exist at the same time. Go fix it dumbass.");
        Instance = this;
        _touchControls = new TouchControls();
        _touchControls.Enable();
    }

    protected virtual void Update() {
        HandleMobile();
        HandlePC();
        
    }

	private void HandlePC() {
        AttackInput = Input.GetButtonDown("Fire1");
	}

	private void HandleMobile() {
        // Check if any finger started touching the screen this exact frame
        foreach (Touch lTouch in Touch.activeTouches) {
            if (lTouch.began) {
                AttackInput = true;
                return;
            }
        }
        AttackInput = false;
    }

	protected virtual void OnDestroy() {
        if (Instance == this) Instance = null;
    }
}
