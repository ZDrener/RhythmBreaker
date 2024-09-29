using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInputManager : MonoBehaviour
{
	// Events
	public static UnityEvent ON_DashKeyPressed = new UnityEvent();
	public static UnityEvent ON_MainFireKeyPressed = new UnityEvent();
	public static UnityEvent ON_SecondaryFireKeyPressed = new UnityEvent();

	// Bools (for buffering)
	public static Vector2 MovementInput;
	public static bool MainFireKeyInput;
	public static bool SecondaryFireKeyInput;
	public static bool DashKeyHold;
	public static string LastWeaponInput;

	// Mobile inputs shit
	private Vector2 touchStartPos;  // To track where the touch begins
	private bool isTouching = false; // To track if the user is dragging

	// Singleton
	public static PlayerInputManager Instance;

	private void Start() {
		if (Instance != null) throw new System.Exception("Two instance of a singleton exists at the same time. Go fix it dumbass.");
		Instance = this;
	}

	// Update is called once per frame
	void Update() {
		if (DebugGameMode.Instance.MobileInputs) HandleMobileInput();
		else HandlePCInput();
	}

    private void HandleMobileInput() {
        // Handle Touch Input
        foreach (Touch touch in Input.touches) {
            // Check if the touch is on the left side of the screen for movement
            if (touch.position.x < Screen.width / 2) {
                if (touch.phase == TouchPhase.Began) {
                    // Start tracking the touch
                    touchStartPos = touch.position;
                    isTouching = true;
                }
                else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) {
                    if (isTouching) {
                        // Calculate movement direction based on difference
                        Vector2 touchCurrentPos = touch.position;
                        Vector2 direction = (touchCurrentPos - touchStartPos).normalized;  // Normalized for direction

                        // Update movement input
                        MovementInput = direction;

                        // Visualize with Debug.DrawLine
                        Debug.DrawLine(Camera.main.ScreenToWorldPoint(touchStartPos),
                                       Camera.main.ScreenToWorldPoint(touchCurrentPos),
                                       Color.red);
                    }
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
                    // Reset movement when touch is released
                    MovementInput = Vector2.zero;
                    isTouching = false;
                }
            }

            // Right half of the screen for firing
            if (touch.position.x > Screen.width / 2) {
                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary) {
                    if (touch.position.y > Screen.height / 2) {
                        // Top right half - Main Weapon
                        ON_MainFireKeyPressed.Invoke();
                        LastWeaponInput = "Fire1";
                        MainFireKeyInput = true;
                    }
                    else {
                        // Bottom right half - Secondary Weapon
                        ON_SecondaryFireKeyPressed.Invoke();
                        LastWeaponInput = "Fire2";
                        SecondaryFireKeyInput = true;
                    }
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
                    // Reset firing inputs when touch is released
                    if (touch.position.y > Screen.height / 2) {
                        // Reset MainFire input
                        MainFireKeyInput = false;
                    }
                    else {
                        // Reset SecondaryFire input
                        SecondaryFireKeyInput = false;
                    }
                }
            }
        }
    }
private void HandlePCInput() {
		MovementInput = Input.GetAxis("Horizontal") * Vector2.right + Input.GetAxis("Vertical") * Vector2.up;

		if (Input.GetButtonDown("Dash")) {
			ON_DashKeyPressed.Invoke();
		}
		if (Input.GetButtonDown("Fire1")) {
			ON_MainFireKeyPressed.Invoke();
			LastWeaponInput = "Fire1";
		}
		if (Input.GetButtonDown("Fire2")) {
			ON_SecondaryFireKeyPressed.Invoke();
			LastWeaponInput = "Fire2";
		}

		// Hold Gamemode
		if (DebugGameMode.Instance.AllowHold) {
			MainFireKeyInput = Input.GetButton("Fire1");
			SecondaryFireKeyInput = Input.GetButton("Fire2");
			DashKeyHold = Input.GetButton("Dash");
		}
		// Press Gamemode
		else {
			MainFireKeyInput = Input.GetButtonDown("Fire1");
			SecondaryFireKeyInput = Input.GetButtonDown("Fire2");
			DashKeyHold = Input.GetButtonDown("Dash");
		}
	}

	private void OnDestroy() {
		if (Instance == this) Instance = null;
	}
}
