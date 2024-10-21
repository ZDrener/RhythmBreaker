using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class Player : MonoBehaviour
{
	public static Player Instance;

	protected const int _MAX_HEALTH = 5;
	protected int _health;

	public delegate void SimpleEvent();
	public static event SimpleEvent PlayerDeathEvent;

	public delegate void IntEvent(int pInt);
	public static event IntEvent PlayerHealthChange;

	public static UnityEvent PlayerFinisherStart;

	protected Animator _animator;
	protected const string _DAMAGED_TRIGGER = "Damaged";

	protected bool _isInvuln = false;
	public bool IsFinishing {get; protected set;} = false; 
	protected bool IsDashing => PlayerMovement.IsDashing;

	private void Awake() {
		if (Instance != null) throw new Exception("Two instances of a singleton exist at the same time");
		Instance = this;
		_health = _MAX_HEALTH;
		_animator = GetComponent<Animator>();
    }

	private void Start()
	{
		if (HealthBar.Instance != null)
			HealthBar.Instance.InitHealth(_health, _MAX_HEALTH);
	}

	public void TakeDamage(int pDamage)
	{
		if (_isInvuln || IsDashing)
			return; // Invincible, baby

		if (_health > 0)
		{
            _health -= pDamage;
            PlayerHealthChange?.Invoke(_health);
			_animator.SetTrigger(_DAMAGED_TRIGGER);

			if (_health <= 0)
				PlayerDeathEvent?.Invoke(); // YOU DIED
			else
				InvulnStart(); // Called in code and not through animator because if multiple projectiles hit it causes problems
        }
	}

	protected void InvulnStart()
	{
		_isInvuln = true;
    }

	protected void InvulnEnd()
	{
        _isInvuln = false;
    }

	protected virtual void StartFinisher()
	{
		IsFinishing = true;
		PlayerFinisherStart?.Invoke();
    }

	protected virtual void EndFinisher()
	{
		IsFinishing = false;
	}

	private void OnDestroy() {
		if (Instance == this) Instance = null;
	}
}
