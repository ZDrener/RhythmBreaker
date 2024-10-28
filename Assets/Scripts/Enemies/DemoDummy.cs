using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
public class DemoDummy : EntityFollowingBeat
{
	public static List<DemoDummy> DummyList = new List<DemoDummy>();

	[Space]
	[Header("STATS")]
	[SerializeField] protected int _maxHealth;
	public int Health { get; protected set; }
	[Space]
	[Header("COMBAT")]
	[SerializeField] protected bool _attackPeriodically = false;
	[SerializeField] protected Vector2 _attackRateRange;
	protected float _attackRate;
	protected float _currentAttackTimer;
	[SerializeField] protected GameObject _projectilePrefab;
	[SerializeField] protected FinisherManager _finisherManager;
	protected bool Weakened
	{
		get { return _weakened; }
		set 
		{ 
			_weakened = value;
			_animator.SetBool(_WEAKENED_BOOL, _weakened);
		}
	}
	protected bool _weakened = false;
	protected bool _playerFinishing = false;
	protected Player GetPlayer => Player.Instance;
	[Space]
	[Header("REFERENCES")]
	[SerializeField] protected Image _lifeBar;
	[SerializeField] protected Image _lifeBar2;
	[SerializeField] protected Rect _spawnRect;
	[SerializeField] protected BoxCollider _collider;
	[SerializeField] protected GameObject _finisherVirtualCamera;
	[Space]
	[Header("JUICE")]
	[SerializeField] protected float _lifeBar2DecreaseForce;
	[SerializeField] protected SpriteRenderer _spriteRenderer;
	[SerializeField] protected float _HitHueChangeDuration;
	[SerializeField] protected GameObject _deathParticlePrefab;
	[SerializeField] protected GameObject _finisherSlashParticlePrefab;
	protected Animator _animator;
	protected const string _CHARGE_FLOAT = "Charge";
	protected const string _HURT_TRIGGER = "Hurt";
	protected const string _ATTACK_TRIGGER = "Attack";
	protected const string _WEAKENED_BOOL = "Weak";
	protected const string _DEATH_TRIGGER = "Death";
	[Space]
	[Header("PREDICTION")]
	[SerializeField] protected LineRenderer _laserLineRenderer;
	protected float _predictionTimeRange = 1.25f;
	protected Color _laserColor;
	protected float _predictionCharge = 0f;

	public bool ChangePosOnFirstSpawn;
	public bool ChangePosOnRespawn;
	public bool SpecialFinisher = true;

	protected Coroutine _enemyCoroutine;

	override protected void Awake()
	{
		base.Awake();
		if (_attackPeriodically) BeatmapManager.ON_TriggerNote.RemoveListener(OnNotePlayed);
		_animator = GetComponent<Animator>();
		Player.PlayerDeathEvent += OnPlayerDeath;
		BeatmapManager.SongStartEvent += OnSongStart;
		BeatmapManager.SongStopEvent += OnSongStop;
		_finisherManager.FinisherSuccess.AddListener(OnFinisherDefeat);
		_finisherManager.FinisherSlash.AddListener(OnFinisherSlash);
		Player.PlayerFinisherStart.AddListener(OnPlayerFinisherStart);
		Player.PlayerFinisherEnd.AddListener(OnPlayerFinisherEnd);
	}

	public void Start() {

		DummyList.Add(this);
		_laserColor = _laserLineRenderer.startColor;
		Health = _maxHealth;
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
		DemoDummy lTarget = null;
		// Set target
		if (DummyList.Count == 0) lTarget = null;
		else {
			DummyList.Sort(delegate (DemoDummy n1, DemoDummy n2) {
				return Vector3.Distance(pPosition, n1.transform.position).CompareTo(Vector3.Distance(pPosition, n2.transform.position));
			});
			
			for (int i = 0; i < DummyList.Count; i++)
			{
				if (DummyList[i].Health > 0)
				{
					lTarget = DummyList[i];
					break;
				}
			}
		}

		return lTarget;
	}

	public void Damage(int pDamage) {
		if (Health <= 0) return;
		Health -= pDamage;
		_lifeBar.fillAmount = (float)Health / (float)_maxHealth;
		_animator.SetTrigger(_HURT_TRIGGER);

		if (Health <= 0)
			Defeat();
	}

	protected void Defeat()
	{
		print("defeated");
		StopEnemy();
		ResetPrediction();
		Weakened = true;
	}

	protected void Death()
	{
		_animator.SetTrigger(_DEATH_TRIGGER);
        _finisherVirtualCamera.SetActive(false);
    }

	protected void PlayDeathParticles()
	{
		Instantiate(_deathParticlePrefab, transform.position, Quaternion.identity);
	}

	protected void DeathAnimationEnded()
	{
		Respawn();
	}

	protected void FinisherStart()
	{
		_finisherManager.SetupFinisher();
		Player.Instance.StartFinisher();
		_collider.enabled = false;
		_finisherVirtualCamera.SetActive(true);
    }

	protected void OnFinisherDefeat()
	{
		Player.Instance.EndFinisher();
		Death();
	}

	protected void OnFinisherSlash(Color pColor, int pDirection)
	{
		float lRotation;

		if (pDirection == 0)
			lRotation = -90f;
		else if (pDirection == 1)
			lRotation = 90f;
		else if (pDirection == 2)
			lRotation = 0;
		else
			lRotation = 180f;

        Instantiate(_finisherSlashParticlePrefab, transform.position, Quaternion.AngleAxis(lRotation, Vector3.forward))
			.GetComponent<ParticleRecolor>().InitRecolor(pColor);
	}


    private void Respawn() {
		if (Weakened)
		{
			StartEnemy();
			_collider.enabled = true;
        }

		Weakened = false;
		Health = _maxHealth;
		_lifeBar.fillAmount = 1;
		ResetPrediction();

		if (ChangePosOnRespawn) transform.position = new Vector3(
			Random.Range(_spawnRect.xMin, _spawnRect.xMax),
			Random.Range(_spawnRect.yMin, _spawnRect.yMax),
			0);
	}

	private void Update() {
		// Update lifebar2
		if (_lifeBar.fillAmount != _lifeBar2.fillAmount)
			_lifeBar2.fillAmount = Mathf.Lerp(_lifeBar2.fillAmount, Health / _maxHealth, _lifeBar2DecreaseForce * Time.deltaTime);
	}

	protected virtual void AttackPlayer()
	{
		ResetPrediction();
		Instantiate(_projectilePrefab, transform.position, Quaternion.identity).GetComponent<EnemyProjectile>().InitAndStart(GetPlayer.gameObject);
		_animator.SetTrigger(_ATTACK_TRIGGER);
	}

	protected override void PlayBeatAction()
	{
		if (!_playerFinishing && !Weakened && (_spriteRenderer.isVisible || _predictionCharge > 0))
		{
			ResetPrediction();
			base.PlayBeatAction();
			AttackPlayer();
		}
	}

	protected void StartEnemy()
	{
		if (_enemyCoroutine != null)
			return;

		_predictionCharge = 0;
		_enemyCoroutine = StartCoroutine(ManageEnemy());
	}

	protected void StopEnemy()
	{
		if (_enemyCoroutine != null)
		{
			StopCoroutine(_enemyCoroutine);
			_enemyCoroutine = null;
		}
	}

	protected IEnumerator ManageEnemy()
	{
		if (_attackPeriodically)
		{
			_currentAttackTimer = 0f;
			_attackRate = Random.Range(_attackRateRange.x, _attackRateRange.y);

			while (GetPlayer != null)
			{
				if (!_playerFinishing)
				{
                    _currentAttackTimer += Time.deltaTime;

                    if (_currentAttackTimer > _attackRate)
                    {
                        AttackPlayer();
                        _currentAttackTimer = 0f;
                    }

                    ManagePrediction();
                }

				yield return null;
			}
		}
		else
		{
			while (GetPlayer != null)
			{
                if (!_playerFinishing)
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
					DrawPredictionLaser();
				}

			}
			else if (m_CurrentNoteCount - m_NotesForAction == -1)
			{
				_predictionCharge = BeatmapManager.Instance.GetNotePrediction(_predictionTimeRange, m_NoteAwaited);
				_animator.SetFloat(_CHARGE_FLOAT, _predictionCharge);
				DrawPredictionLaser();
			}
		}
	}

	protected void ResetPrediction()
	{
		_predictionCharge = 0f;
		_animator.SetFloat(_CHARGE_FLOAT, 0f);
		ResetPredictionLaser();
	}

	protected virtual void DrawPredictionLaser()
	{
		if (_predictionCharge < 0.2f)
			return;

		_laserLineRenderer.SetPosition(0, transform.position);
		_laserLineRenderer.SetPosition(1, GetPlayer.transform.position);
		_laserLineRenderer.startColor = new Color(_laserColor.r, _laserColor.g, _laserColor.b, Mathf.Lerp(0, 1, _predictionCharge));
	}

	protected virtual void ResetPredictionLaser()
	{
		_laserLineRenderer.SetPosition(0, transform.position);
		_laserLineRenderer.SetPosition(1, transform.position);
		_laserLineRenderer.startColor = new Color(_laserColor.r, _laserColor.g, _laserColor.b, 0f);
	}

	protected virtual void OnSongStart()
	{
		StartEnemy();
	}

	protected virtual void OnSongStop()
	{
		StopEnemy();
	}

	protected virtual void OnPlayerFinisherStart()
	{
		_playerFinishing = true;
    }

	protected virtual void OnPlayerFinisherEnd()
	{
		_playerFinishing = false;
    }

	protected virtual void OnPlayerDeath()
	{
		StopEnemy();
	}

	private void OnTriggerEnter(Collider pCollision)
	{
		if (Weakened && pCollision.gameObject.GetComponent<Player>() && PlayerMovement.IsDashing)
		{
			if (SpecialFinisher)
				FinisherStart();
			else
				Death();
		}
	}

	override protected void OnDestroy()
	{
		DummyList.Remove(this);
		Player.PlayerDeathEvent -= OnPlayerDeath;
		BeatmapManager.SongStartEvent -= OnSongStart;
		BeatmapManager.SongStopEvent -= OnSongStop;
	}
}
