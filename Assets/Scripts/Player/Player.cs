using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public static Player Instance;

	private void Awake() {
		if (Instance != null) throw new Exception("Two instances of a singleton exist at the same time");
		Instance = this;
	}

	private void OnDestroy() {
		if (Instance == this) Instance = null;
	}
}
