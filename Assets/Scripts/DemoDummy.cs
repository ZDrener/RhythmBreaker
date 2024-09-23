using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DemoDummy : MonoBehaviour
{
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
