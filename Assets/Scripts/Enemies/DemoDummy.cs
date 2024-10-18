using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
public class DemoDummy : EntityFollowingBeat
{
	public static List<DemoDummy> DummyList = new List<DemoDummy>();

	[Space]
	[Header("STATS")]
	[SerializeField] protected int _maxHealth;
	protected int _health;
	[Space]
	[Header("COMBAT")]
	[SerializeField] protected bool _attackPeriodically = false;
	[SerializeField] protected Vector2 _attackRateRange;
	protected float _attackRate;
	protected float _currentAttackTimer;
	[SerializeField] protected GameObject _projectilePrefab;
	protected Player GetPlayer => Player.Instance;
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
	protected Animator _animator;
	protected const string _CHARGE_FLOAT = "Charge";
	protected const string _HURT_TRIGGER = "Hurt";
	protected const string _ATTACK_TRIGGER = "Attack";
	[Space]
	[Header("PREDICTION")]
	[SerializeField] protected LineRenderer _laserLineRenderer;
	protected float _predictionTimeRange = 1.5f;
	protected Color _laserColor;
	protected float _predictionCharge = 0f;

	public bool ChangePosOnFirstSpawn;
	public bool ChangePosOnRespawn;

	override protected void Awake()
	{
		base.Awake();
		_animator = GetComponent<Animator>();
		Player.PlayerDeathEvent += OnPlayerDeath;
		BeatmapManager.TriggerNoteEndEvent += OnNoteEnd;
		BeatmapManager.SongStopEvent += OnSongStop;
	}

	public void Start() {

		DummyList.Add(this);
		BeatmapManager.SongStartEvent += OnSongStart;
		_laserColor = _laserLineRenderer.startColor;

		if (ChangePosOnFirstSpawn)
		{
			transform.position = new Vector3(
			Random.Range(_spawnRect.xMin, _spawnRect.xMax),
			Random.Range(_spawnRect.yMin, _spawnRect.yMax),
			0);
		}
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
		_lifeBar.fillAmount = (float)_health / (float)_maxHealth;
		_animator.SetTrigger(_HURT_TRIGGER);

		if (_health <= 0) Respawn();
	}

	private void Respawn() {
		_health = _maxHealth;
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
	}

	protected virtual IEnumerator ManageAttacks()
	{
		_currentAttackTimer = 0f;

		while (GetPlayer != null)
		{
			_currentAttackTimer += Time.deltaTime;

			if (_currentAttackTimer > _attackRate)
			{
				AttackPlayer();
				_currentAttackTimer -= _attackRate;
			}

			yield return null;
		}

		yield break;
	}

	protected virtual void AttackPlayer()
	{
		EnemyProjectile lProjectile = Instantiate(_projectilePrefab, transform.position, Quaternion.identity).GetComponent<EnemyProjectile>();
		lProjectile.InitAndStart(GetPlayer.gameObject);
		_animator.SetTrigger(_ATTACK_TRIGGER);
		ResetPredicitonLaser();
	}

	protected virtual void OnSongStart()
	{
		if (_attackPeriodically)
		{
			_attackRate = Random.Range(_attackRateRange.x, _attackRateRange.y);
			StartCoroutine(ManageAttacks());
			StartCoroutine(ManageActionPredictionPeriod());
		}
		else
			StartCoroutine(ManageActionPredictionBeat());
	}

	protected virtual void OnSongStop()
	{
		StopAllCoroutines();
	}

	protected virtual void OnPlayerDeath()
	{
		StopAllCoroutines();
	}

	protected IEnumerator ManageActionPredictionPeriod()
	{
		float lTimeBeforeAttack;

		while (true)
		{
			lTimeBeforeAttack = _attackRate - _currentAttackTimer;

			if ((_spriteRenderer.isVisible || _predictionCharge > 0) && lTimeBeforeAttack <= _predictionTimeRange)
			{
				_predictionCharge = 1 - lTimeBeforeAttack / _predictionTimeRange;
				_animator.SetFloat(_CHARGE_FLOAT, _predictionCharge);
				DrawPredictionLaser(_predictionCharge);

                if (_predictionCharge > 0.2f)
                    DrawPredictionLaser(_predictionCharge);
            }

			yield return null;
		}
	}

	protected IEnumerator ManageActionPredictionBeat()
	{
		while (true)
		{
			if ((_spriteRenderer.isVisible || _predictionCharge > 0) && m_CurrentNoteCount - m_NotesForAction == -1)
			{
                _predictionCharge = BeatmapManager.Instance.GetNotePrediction(_predictionTimeRange, m_NoteAwaited);
				_animator.SetFloat(_CHARGE_FLOAT, _predictionCharge);

				if(_predictionCharge > 0.2f)
					DrawPredictionLaser(_predictionCharge);
			}

			yield return null;
		}
	}

	protected override void PlayAction()
	{
		if (!_attackPeriodically && (_spriteRenderer.isVisible || _predictionCharge > 0))
		{
			base.PlayAction();
			_animator.SetFloat(_CHARGE_FLOAT, 0f);
			_predictionCharge = 0f;
            AttackPlayer();
		}
	}

	protected virtual void DrawPredictionLaser(float pChargeRatio)
	{
		_laserLineRenderer.SetPosition(0, transform.position);
		_laserLineRenderer.SetPosition(1, GetPlayer.transform.position);
		_laserLineRenderer.startColor = new Color(_laserColor.r, _laserColor.g, _laserColor.b, Mathf.Lerp(0,1, pChargeRatio));
	}


	protected virtual void ResetPredicitonLaser()
	{
		_laserLineRenderer.SetPosition(1, transform.position);
		_laserLineRenderer.startColor = new Color(_laserColor.r, _laserColor.g, _laserColor.b, 0f);
	}

	protected virtual void OnNoteEnd()
	{

	}

	override protected void OnDestroy()
	{
		DummyList.Remove(this);
		Player.PlayerDeathEvent -= OnPlayerDeath;
		BeatmapManager.SongStartEvent -= OnSongStart;
		BeatmapManager.TriggerNoteEndEvent -= OnNoteEnd;
	}
}
