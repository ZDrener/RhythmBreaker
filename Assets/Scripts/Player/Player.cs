using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(BoxCollider))]
public class Player : MonoBehaviour
{
	public static Player Instance;

	protected const int _MAX_HEALTH = 5;
	protected int _health;

	public delegate void SimpleEvent();
	public static event SimpleEvent PlayerDeathEvent;

	public delegate void IntEvent(int pInt);
	public static event IntEvent PlayerHealthChange;

	protected Animator _animator;
	protected const string _DAMAGED_TRIGGER = "Damaged";
	protected const string _INVULN_TRIGGER = "Invuln";

	protected BoxCollider _boxCollider;
	protected bool _isInvuln;

	private void Awake() {
		if (Instance != null) throw new Exception("Two instances of a singleton exist at the same time");
		Instance = this;

		_health = _MAX_HEALTH;
		_animator = GetComponent<Animator>();
        _boxCollider = GetComponent<BoxCollider>();
    }

	private void Start()
	{
		if (HealthBar.Instance != null)
			HealthBar.Instance.InitHealth(_health, _MAX_HEALTH);
	}

	public void TakeDamage(int pDamage)
	{
		if (_isInvuln)
			return;

		if (_health > 0)
		{
            _health -= pDamage;
            PlayerHealthChange?.Invoke(_health);
			_animator.SetTrigger(_DAMAGED_TRIGGER);

			if (_health <= 0)
			{
				PlayerDeathEvent?.Invoke();
				// Die
			}
			else
				InvulnStart();
        }
	}

	protected void InvulnStart()
	{
		_boxCollider.enabled = false;
		_isInvuln = true;
        _animator.SetTrigger(_INVULN_TRIGGER);
    }

	protected void InvulnEnd()
	{
		_boxCollider.enabled = true;
        _isInvuln = false;
    }

	private void OnDestroy() {
		if (Instance == this) Instance = null;
	}
}
