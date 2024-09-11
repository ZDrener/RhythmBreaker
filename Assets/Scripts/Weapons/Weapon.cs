using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class Weapon : MonoBehaviour
{
	[SerializeField] protected WeaponStatSO _weaponStats;
	[SerializeField] protected Transform _projectileSpawn;
	[SerializeField] protected SpriteRenderer _weaponSprite;
	[SerializeField] protected AudioClip[] _audioClips;
	public SortingGroup sortingGroup;
	protected Color _color;
	protected Animator _animator;
	protected AudioSource _audioSource;

	#region MAIN / SECONDARY
	public bool isMainWeapon {
		get { return _isMainWeapon; }
		set {
			_isMainWeapon = value;
			if (value) PlayerInputManager.ON_MainFireKeyPressed.AddListener(Fire);
			else PlayerInputManager.ON_MainFireKeyPressed.AddListener(Fire);
		}
	}
	protected bool _isMainWeapon;
	public bool isSecondaryWeapon {
		get { return _isSecondaryWeapon; }
		set {
			_isSecondaryWeapon = value;
			if (value) PlayerInputManager.ON_MainFireKeyPressed.AddListener(Fire);
			else PlayerInputManager.ON_MainFireKeyPressed.AddListener(Fire);
		}
	}
	protected bool _isSecondaryWeapon;
	#endregion

	protected virtual void Start() {
		_animator = transform.GetComponent<Animator>();
		_audioSource = transform.GetComponent<AudioSource>();
	}

	public virtual void SetWeaponColor(Color pColor) {
		_color = _weaponSprite.color = pColor;
	}

	public virtual void Fire() {
		_animator.SetTrigger("fire");

		// Projectile
		GameObject lProjectile = Instantiate(_weaponStats.projectilePrefab);
		lProjectile.transform.position = _projectileSpawn.position;
		lProjectile.transform.rotation = _projectileSpawn.rotation;
		lProjectile.GetComponent<Projectile>().ProjectileInit(_color);

		// MuzzleFlash
		if (_weaponStats.muzzlePrefab) {
			GameObject lMuzzle = Instantiate(_weaponStats.muzzlePrefab);
			lMuzzle.transform.position = _projectileSpawn.position;
			lMuzzle.transform.rotation = _projectileSpawn.rotation;
			lMuzzle.GetComponent<ParticleRecolor>().InitRecolor(_color);
		}

		// Sound
		_audioSource.pitch = UnityEngine.Random.Range(.7f, 1);
		_audioSource.volume = UnityEngine.Random.Range(.7f, 1);
		_audioSource.PlayOneShot(_audioClips[UnityEngine.Random.Range(0, _audioClips.Length - 1)]);
	}

	private void OnDestroy() {
		// Remove this weapon from hands and stop listening at events
		isMainWeapon = isSecondaryWeapon = false;
	}
}
