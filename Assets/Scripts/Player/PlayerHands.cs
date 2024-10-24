using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHands : MonoBehaviour
{
	public static UnityEvent<GameObject> ON_WeaponPickUp = new UnityEvent<GameObject>();

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
	[SerializeField] protected bool _ImmobileToShoot;

	protected Weapon _lastUsedWeapon;
	protected DemoDummy _lastTarget;

	protected virtual void Start() {
		_camera = Camera.main;

		if (_mainWeapon) {
			_mainWeapon.SetWeaponColor(_playerColors.mainColor);
			_mainWeapon.gameObject.SetActive(false);
		}
		if (_secondaryWeapon) {
			_secondaryWeapon.SetWeaponColor(_playerColors.secondaryColor);
			_secondaryWeapon.gameObject.SetActive(false);
		}

		ON_WeaponPickUp.AddListener(GetNewWeapon);

		if (PlayerInputManager.Instance.ArcheroGameplay)
			BeatmapManager.ON_TriggerNote.AddListener(OrderFire);
		//PlayerInputManager.ON_ArcheroAttackInput.AddListener(OrderFire);
		else
			BeatmapManager.ON_TriggerNote.AddListener(OrderFire);
	}

	public void GetNewWeapon(GameObject pWeapon) {
		if (_secondaryWeapon) Destroy(_secondaryWeapon.gameObject);

		_secondaryWeapon = Instantiate(pWeapon, transform.position, Quaternion.identity, transform).GetComponent<Weapon>();
		_secondaryWeapon.SetWeaponColor(_playerColors.secondaryColor);
		_secondaryWeapon.gameObject.SetActive(false);
	}

	protected virtual void AimAtCursor() {
        // Calculate target
        _lastTarget = DemoDummy.GetClosestDummy(transform.position);

		if (_lastTarget) {
			Vector3 lDirection = _lastTarget.transform.position - transform.position;
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

	protected virtual void OrderFire(NoteType pType) {
		// Archero
		if (PlayerInputManager.Instance.ArcheroGameplay && pType != NoteType.Yellow && PlayerInputManager.AttackInput || !_ImmobileToShoot) {
			if ((pType == NoteType.Red)) FireMain();
			else if ((pType == NoteType.Blue)) FireSecondary();
		}
		// Hold
		else if (!PlayerInputManager.Instance.ArcheroGameplay && PlayerInputManager.AttackInput || !_ImmobileToShoot) {
			if ((pType == NoteType.Red) && (PlayerInputManager.AttackType == NoteType.Red)) FireMain();
			else if ((pType == NoteType.Blue) && (PlayerInputManager.AttackType == NoteType.Blue)) FireSecondary();
		}
	}

	protected virtual void FireMain() {
		if (!_mainWeapon) return;
		_mainWeapon.gameObject.SetActive(true);
		_lastUsedWeapon = _mainWeapon;
		_mainWeapon.sortingGroup.sortingOrder = 1;
		AimAtCursor();
		if (!_lastTarget) return;
		if (_secondaryWeapon) _secondaryWeapon.sortingGroup.sortingOrder = 0;
		_mainWeapon.Fire();
	}

	protected virtual void FireSecondary() {
		if (!_secondaryWeapon || _lastTarget == null) return;
		_secondaryWeapon.gameObject.SetActive(true);
		_lastUsedWeapon = _secondaryWeapon;
		_secondaryWeapon.sortingGroup.sortingOrder = 1;
		AimAtCursor();
		if (!_lastTarget) return;
		if (_mainWeapon) _mainWeapon.sortingGroup.sortingOrder = 0;
		_secondaryWeapon.Fire();
	}
}
