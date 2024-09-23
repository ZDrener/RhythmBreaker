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
	public static bool MainFireKeyHold;
	public static bool SecondaryFireKeyHold;
	public static bool DashKeyHold;
	public static string LastWeaponInput;

	// Singleton
	public static PlayerInputManager Instance;

	private void Start() {
		if (Instance != null) throw new System.Exception("Two instance of a singleton exists at the same time. Go fix it dumbass.");
		Instance = this;
	}

	// Update is called once per frame
	void Update() {
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

		MainFireKeyHold = Input.GetButton("Fire1");
		SecondaryFireKeyHold = Input.GetButton("Fire2");
		DashKeyHold = Input.GetButton("Dash");
	}

	private void OnDestroy() {
		if (Instance == this) Instance = null;
	}
}
