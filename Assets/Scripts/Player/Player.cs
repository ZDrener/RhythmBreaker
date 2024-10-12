using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
	public static Player Instance;

	protected const int _MAX_HEALTH = 5;
	protected int _health;

	public delegate void SimpleEvent();
	public static event SimpleEvent PlayerDeathEvent;

	public delegate void IntEvent(int pInt);
	public static event IntEvent PlayerHealthChange;

	private void Awake() {
		if (Instance != null) throw new Exception("Two instances of a singleton exist at the same time");
		Instance = this;

		_health = _MAX_HEALTH;
	}

	private void Start()
	{
		if (HealthBar.Instance != null)
			HealthBar.Instance.InitHealth(_health, _MAX_HEALTH);
	}

	public void TakeDamage(int pDamage)
	{
		if (_health > 0)
		{
            _health -= pDamage;
            PlayerHealthChange?.Invoke(_health);

            if (_health <= 0)
            {
                PlayerDeathEvent?.Invoke();
                // Die
            }
        }
	}

	private void OnDestroy() {
		if (Instance == this) Instance = null;
	}
}
