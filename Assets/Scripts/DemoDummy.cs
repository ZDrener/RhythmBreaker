using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DemoDummy : MonoBehaviour
{
	public static List<DemoDummy> DummyList = new List<DemoDummy>();

	[Space]
	[Header("STATS")]
	[SerializeField] protected int _maxHealth;
	protected int _health;
	[Space]
	[Header("REFERENCES")]
	[SerializeField] protected Image _lifeBar;
	[SerializeField] protected Image _lifeBar2;
	[SerializeField] protected Rect _spawnRect;
	[Space]
	[Header("JUICE")]
	[SerializeField] protected float _lifeBar2DecreaseForce;
	[SerializeField] protected SpriteRenderer _spriteRenderer;
	[SerializeField] protected float _HitHueChangeDuration;

	protected float _timeSinceLastHit;

	public bool ChangePosOnRespawn;

	public void Start() {
		Respawn();
		DummyList.Add(this);
	}

	public static DemoDummy GetClosestDummy(Vector3 pPosition) {
		DemoDummy Target;
		// Set target
		if (DummyList.Count == 0) Target = null;
		else {
			DummyList.Sort(delegate (DemoDummy n1, DemoDummy n2) {
				return Vector3.Distance(pPosition, n1.transform.position).CompareTo(Vector3.Distance(pPosition, n2.transform.position));
			});
			Target = DummyList[0];
		}

		return Target;
	}

	public void Damage(int pDamage) {
		_health -= pDamage;
		_timeSinceLastHit = 0;
		_lifeBar.fillAmount = (float)_health / (float)_maxHealth;
		_spriteRenderer.color = Color.red;

		if (_health <= 0) Respawn();
	}

	private void Respawn() {
		_health = _maxHealth;
		_spriteRenderer.color = Color.clear;
		_lifeBar.fillAmount = 1;
		if (ChangePosOnRespawn) transform.position = new Vector3(
			Random.Range(_spawnRect.xMin, _spawnRect.xMax),
			Random.Range(_spawnRect.yMin, _spawnRect.yMax),
			0);
	}

	private void Update() {
		// Update lifebar2
		if (_lifeBar.fillAmount != _lifeBar2.fillAmount)
			_lifeBar2.fillAmount = Mathf.Lerp(_lifeBar2.fillAmount, _health / _maxHealth, _lifeBar2DecreaseForce * Time.deltaTime);

		// Update SpriteRenderer
		if (_spriteRenderer.color != Color.white)
			_spriteRenderer.color = Color.Lerp(_spriteRenderer.color, Color.white, _HitHueChangeDuration * Time.deltaTime);
	}
}
