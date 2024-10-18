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
	protected Color _baseColor;
	[Space]
	[Header("COMBAT")]
	[SerializeField] protected bool _attackPeriodically = false;
	[SerializeField] protected Vector2 _attackRateRange;
	protected float _attackRate;
	[SerializeField] protected GameObject _projectilePrefab;
	protected Player GetPlayer => Player.Instance;
	[Space]
	[Header("REFERENCES")]
	[SerializeField] protected Image _lifeBar;
	[SerializeField] protected Image _lifeBar2;
	[SerializeField] protected Rect _spawnRect;
	[SerializeField] protected SpriteRenderer _chargeEffect;
	[Space]
	[Header("JUICE")]
	[SerializeField] protected float _lifeBar2DecreaseForce;
	[SerializeField] protected SpriteRenderer _spriteRenderer;
	[SerializeField] protected float _HitHueChangeDuration;
    protected Animator _animator;
    protected const string _CHARGE_FLOAT = "Charge";
	protected const string _HURT_TRIGGER = "Hurt";
    [Space]
	[Header("PREDICTION")]
	protected float _predictionTimeRange = 1.5f;

	//protected float _timeSinceLastHit;

	public bool ChangePosOnFirstSpawn;
	public bool ChangePosOnRespawn;

    override protected void Awake()
    {
		base.Awake();
        _animator = GetComponent<Animator>();
        Player.PlayerDeathEvent += OnPlayerDeath;
		BeatmapManager.TriggerNoteEndEvent += OnNoteEnd;
		BeatmapManager.SongStopEvent += OnSongStop;
		_baseColor = _spriteRenderer.color;
    }

    public void Start() {

		DummyList.Add(this);
		BeatmapManager.SongStartEvent += OnSongStart;

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
        /*_timeSinceLastHit = 0;
		_spriteRenderer.color = Color.red;*/
        _lifeBar.fillAmount = (float)_health / (float)_maxHealth;

        if (_health <= 0) Respawn();
		else _animator.SetTrigger(_HURT_TRIGGER);
	}

	private void Respawn() {
		_health = _maxHealth;
		_spriteRenderer.color = _baseColor;
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
		/*if (_spriteRenderer.color != Color.white)
			_spriteRenderer.color = Color.Lerp(_spriteRenderer.color, Color.white, _HitHueChangeDuration * Time.deltaTime);*/
	}

    protected virtual IEnumerator ManageAttacks()
    {
        float lTime = 0f;

        while (GetPlayer != null)
        {
            lTime += Time.deltaTime;

            if (lTime > _attackRate)
            {
                AttackPlayer();
                lTime -= _attackRate;
            }

            yield return null;
        }

        yield break;
    }

    protected virtual void AttackPlayer()
	{
		EnemyProjectile lProjectile = Instantiate(_projectilePrefab, transform.position, Quaternion.identity).GetComponent<EnemyProjectile>();
		lProjectile.InitAndStart(GetPlayer.gameObject);
	}

    protected virtual void OnSongStart()
    {
		if (_attackPeriodically)
		{
            _attackRate = Random.Range(_attackRateRange.x, _attackRateRange.y);
            StartCoroutine(ManageAttacks());
        }
		else
            StartCoroutine(ManageActionPrediction());
    }

	protected virtual void OnSongStop()
	{
		StopAllCoroutines();
    }

    protected virtual void OnPlayerDeath()
    {
        StopAllCoroutines();
    }

    protected IEnumerator ManageActionPrediction()
	{
		float lPredictionRatio = 0f;

		while (true)
		{
            if (_spriteRenderer.isVisible && m_CurrentNoteCount - m_NotesForAction == -1)
            {
                lPredictionRatio = BeatmapManager.Instance.GetNotePrediction(_predictionTimeRange, m_NoteAwaited);
				_animator.SetFloat(_CHARGE_FLOAT, lPredictionRatio);
            }

			yield return null;
		}
	}

    protected override void PlayAction()
    {
        if (!_attackPeriodically && _spriteRenderer.isVisible /*&& _hasFired*/)
		{
			base.PlayAction();
            _animator.SetFloat(_CHARGE_FLOAT, 0f);
            _chargeEffect.transform.localScale = Vector3.zero;
			EnemyProjectile lProjectile = Instantiate(_projectilePrefab, transform.position, Quaternion.identity).GetComponent<EnemyProjectile>();
			lProjectile.InitAndStart(GetPlayer.gameObject);
		}
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
