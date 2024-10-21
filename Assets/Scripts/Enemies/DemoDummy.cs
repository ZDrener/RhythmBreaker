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
	protected bool _weakened = false;
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
	protected const string _WEAKENED_BOOL = "Weak";
	[Space]
	[Header("PREDICTION")]
	[SerializeField] protected LineRenderer _laserLineRenderer;
	protected float _predictionTimeRange = 1.25f;
	protected Color _laserColor;
	protected float _predictionCharge = 0f;

	public bool ChangePosOnFirstSpawn;
	public bool ChangePosOnRespawn;

	override protected void Awake()
	{
		base.Awake();
		_animator = GetComponent<Animator>();
		Player.PlayerDeathEvent += OnPlayerDeath;
		BeatmapManager.SongStopEvent += OnSongStop;
	}

	public void Start() {

		DummyList.Add(this);
		BeatmapManager.SongStartEvent += OnSongStart;
		_laserColor = _laserLineRenderer.startColor;
		_health = _maxHealth;
        _lifeBar.fillAmount = 1;

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
		if (_health <= 0) return;
		_health -= pDamage;
		_lifeBar.fillAmount = (float)_health / (float)_maxHealth;
		_animator.SetTrigger(_HURT_TRIGGER);

		//if (_health <= 0) Respawn();

		if (_health <= 0)
		{
			StopAllCoroutines();
			_weakened = true;
            _animator.SetBool(_WEAKENED_BOOL, true);
			_animator.SetFloat(_CHARGE_FLOAT, 0f);
		}
	}

	protected void Defeat()
	{
		Respawn();
	}

	private void Respawn() {
		_weakened = false;
		_animator.SetBool(_WEAKENED_BOOL, false);
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

	protected virtual void AttackPlayer()
	{
		ResetPrediction();
        EnemyProjectile lProjectile = Instantiate(_projectilePrefab, transform.position, Quaternion.identity).GetComponent<EnemyProjectile>();
		lProjectile.InitAndStart(GetPlayer.gameObject);
		_animator.SetTrigger(_ATTACK_TRIGGER);
		ResetPredictionLaser();
    }

	protected virtual void OnSongStart()
	{
		StartEnemy();
	}

    protected override void PlayAction()
    {
        if (!_attackPeriodically && (_spriteRenderer.isVisible || _predictionCharge > 0))
        {
            ResetPrediction();
            base.PlayAction();
            AttackPlayer();
        }
    }

    protected void StartEnemy()
	{
		StartCoroutine(ManageEnemy());
	}

    protected IEnumerator ManageEnemy()
    {
        if (_attackPeriodically)
        {
            _currentAttackTimer = 0f;
            _attackRate = Random.Range(_attackRateRange.x, _attackRateRange.y);

            while (GetPlayer != null)
            {
                _currentAttackTimer += Time.deltaTime;

                if (_currentAttackTimer > _attackRate)
                {
                    AttackPlayer();
                    _currentAttackTimer = 0f;
                }

                ManagePrediction();
                yield return null;
            }
        }
        else
        {
            while (GetPlayer != null)
            {
                ManagePrediction();
                yield return null;
            }
        }
    }

    protected void ManagePrediction()
    {
        if (_spriteRenderer.isVisible || _predictionCharge > 0f)
        {
            if (_attackPeriodically)
            {
                float lTimeBeforeAttack = Mathf.Clamp(_attackRate - _currentAttackTimer, 0f, _attackRate);

                if (lTimeBeforeAttack <= _predictionTimeRange)
                {
                    _predictionCharge = 1f - lTimeBeforeAttack / _predictionTimeRange;

                    _animator.SetFloat(_CHARGE_FLOAT, _predictionCharge);

                    if (_predictionCharge > 0.2f)
                        DrawPredictionLaser(_predictionCharge);
                }

            }
            else if (m_CurrentNoteCount - m_NotesForAction == -1)
            {
                _predictionCharge = BeatmapManager.Instance.GetNotePrediction(_predictionTimeRange, m_NoteAwaited);
                _animator.SetFloat(_CHARGE_FLOAT, _predictionCharge);

                if (_predictionCharge > 0.2f)
                    DrawPredictionLaser(_predictionCharge);
            }
        }
    }

    protected void ResetPrediction()
    {
        _predictionCharge = 0f;
        _animator.SetFloat(_CHARGE_FLOAT, 0f);
        ResetPredictionLaser();
    }

    protected virtual void DrawPredictionLaser(float pChargeRatio)
    {
        _laserLineRenderer.SetPosition(0, transform.position);
        _laserLineRenderer.SetPosition(1, GetPlayer.transform.position);
        _laserLineRenderer.startColor = new Color(_laserColor.r, _laserColor.g, _laserColor.b, Mathf.Lerp(0, 1, pChargeRatio));
    }


    protected virtual void ResetPredictionLaser()
    {
        _laserLineRenderer.SetPosition(1, transform.position);
        _laserLineRenderer.startColor = new Color(_laserColor.r, _laserColor.g, _laserColor.b, 0f);
    }

    protected virtual void OnSongStop()
	{
		StopAllCoroutines();
	}

	protected virtual void OnPlayerDeath()
	{
		StopAllCoroutines();
	}

	

    private void OnTriggerEnter(Collider pCollision)
    {
        if (_weakened && pCollision.gameObject.GetComponent<Player>() && PlayerMovement.IsDashing)
            Defeat();
    }

    override protected void OnDestroy()
	{
		DummyList.Remove(this);
		Player.PlayerDeathEvent -= OnPlayerDeath;
		BeatmapManager.SongStartEvent -= OnSongStart;
	}
}
