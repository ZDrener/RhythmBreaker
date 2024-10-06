using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHands : MonoBehaviour
{
	[Header("HANDS REFERENCES")]
	[SerializeField] protected Weapon _mainWeapon;
	[SerializeField] protected Weapon _secondaryWeapon;
	[Space]
	[Header("COLORS")]
	[SerializeField] protected PlayerColorsSO _playerColors;
	[Space]
	[Header("SPRITES")]
	[SerializeField] protected SpriteRenderer _bodySprite;
	[Space]
	[Header("OTHERS")]
	[SerializeField] protected Camera _camera;
	[SerializeField] protected float _weaponSmoothRotation = .5f;
	[SerializeField] float _secondaryWeaponRange = 2;

	protected Weapon _lastUsedWeapon;

	protected virtual void Start() {
		_camera = Camera.main;

		_mainWeapon.SetWeaponColor(_playerColors.mainColor);
		_secondaryWeapon.SetWeaponColor(_playerColors.secondaryColor);

		// Disable visuals of weapons on start
		_mainWeapon.gameObject.SetActive(false);
		_secondaryWeapon.gameObject.SetActive(false);

		//PlayerInputManager.ON_MainFireKeyPressed.AddListener(FireMain);
		//PlayerInputManager.ON_SecondaryFireKeyPressed.AddListener(FireSecondary);
		BeatmapManager.ON_TriggerNote.AddListener(OrderFire);
	}

	protected virtual void AimAtCursor() {
		// Calculate target
		DemoDummy lTarget = DemoDummy.GetClosestDummy(transform.position);

		if (lTarget) {
			Vector3 lDirection = lTarget.transform.position - transform.position;
			float lAngle = Mathf.Atan2(lDirection.y, lDirection.x) * Mathf.Rad2Deg;
			Quaternion lEndRotation = Quaternion.Euler(Vector3.forward * lAngle);

			if (_lastUsedWeapon) {
				// Rotate weapon towards target
				_lastUsedWeapon.transform.rotation = lEndRotation;

				// Flip Sprites
				bool lDotPositive = (Vector3.Dot(Vector3.right, Vector3.Normalize(lDirection))) > 0;
				_lastUsedWeapon.transform.localScale = lDotPositive ? Vector3.one : new Vector3(1, -1, 1);
				_bodySprite.flipX = !lDotPositive;
			}
		}
	}

	protected virtual void OrderFire() {
		DemoDummy lTarget = DemoDummy.GetClosestDummy(transform.position);
		if (Vector3.Distance(lTarget.transform.position, transform.position) < _secondaryWeaponRange) FireSecondary();
		else FireMain();
	}

	protected virtual void FireMain() {
		_mainWeapon.gameObject.SetActive(true);
		_lastUsedWeapon = _mainWeapon;
		_mainWeapon.sortingGroup.sortingOrder = 1;
		_secondaryWeapon.sortingGroup.sortingOrder = 0;
		AimAtCursor();
		_mainWeapon.Fire();
	}

	protected virtual void FireSecondary() {
		_secondaryWeapon.gameObject.SetActive(true);
		_lastUsedWeapon = _secondaryWeapon;
		_secondaryWeapon.sortingGroup.sortingOrder = 1;
		_mainWeapon.sortingGroup.sortingOrder = 0;
		AimAtCursor();
		_secondaryWeapon.Fire();
	}
}
