using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour

{
    [Header("BaseEnemy variables")]
    [SerializeField] protected int _enemyScoreValue = 10;
    [SerializeField] protected float _enemySpeed;
    [SerializeField] protected float _enemyChargeSpeed = 7f;
    [SerializeField] protected float _chanceToEnableShield;
    [SerializeField] protected bool _isShieldEnabled = false;
    [SerializeField] protected float _detectionRadius = 3f;
    [SerializeField] protected float _projectileSpeed = 8f;
    protected Animator _animator;
    [SerializeField] protected float _delayToDestroyEnemy = 2.6f;
    protected AudioManager _audioManager;
    protected SpawnManager _spawnManager;
    protected Collider2D _myCollider2D;
    protected Player _player;
    protected Camera _mainCamera;
    protected ParticleSystem _shieldParticle;
    protected GameObject _engineObject;

    private bool _isPlayerNearby = false;
    private Vector3 _targetPosition;
    private float _ramAttackCooldown = 2f;
    private float _ramLastAttackTime = -Mathf.Infinity;

    private enum EnemyDeathState { Move, Explode, Dead };
    private EnemyDeathState _enemyDeathState = EnemyDeathState.Move;


    protected virtual void Start()
    {
        _engineObject = transform.GetChild(0).gameObject;
        _animator = GetComponent<Animator>();
        _audioManager = GameObject.FindGameObjectWithTag("Audio_Manager").GetComponent<AudioManager>();        
        _spawnManager = GameObject.FindGameObjectWithTag("Spawn_Manager").GetComponent<SpawnManager>();        
        
        _mainCamera = Camera.main;
        _shieldParticle = GetComponent<ParticleSystem>();
        EnableShieldOnStart();
        InvokeRepeating("DetectPlayerNearby", 0f, 0.5f);

    }

    protected virtual void Update()
    {

        if (_isPlayerNearby)
        {
            ChargeAtPlayer();
        }else
        {
            MoveEnemy();
        }           
        
    }
    protected abstract void MoveEnemy();
    protected abstract void Fire();
    
    protected abstract void CheckIfEnemyHasLeftScreen();

    protected virtual void DetectPlayerNearby()
    {

        // check if enough time has passed since last attack
        if (Time.time - _ramLastAttackTime < _ramAttackCooldown)
        {
            return;
        }   
        Collider2D otherCollider = Physics2D.OverlapCircle(transform.position, _detectionRadius);

        if (otherCollider != null && otherCollider.CompareTag("Player"))
        {
            _targetPosition = otherCollider.transform.position;
            _isPlayerNearby = true;
            _ramLastAttackTime = Time.time;
            CancelInvoke("DetectPlayerNearby");
        }
    }        

    protected virtual void ChargeAtPlayer()
    {
        Vector3 direction = (_targetPosition - transform.position).normalized;
        transform.Translate(direction * _enemyChargeSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, _targetPosition) < 0.1f)
        {
            _isPlayerNearby = false;
            CancelInvoke("DetectPlayerNearby");
            InvokeRepeating("DetectPlayerNearby", 0f, 0.5f);
        }
    }

    public virtual void StartOnDeathEffects()
    {
        if (_isShieldEnabled)
        {
            DisableShield();
            return;
        }

        if (_enemyDeathState == EnemyDeathState.Move)
        {
            try
            {
                _enemyDeathState = EnemyDeathState.Explode;
                _engineObject.SetActive(false);
                CancelInvoke(nameof(DetectPlayerNearby));
                _myCollider2D.enabled = false;
                _animator.SetTrigger("OnEnemyDeath");
                _enemySpeed = 0f;
                _audioManager.PlayExplosionAudio();
                UIManager.Instance.UpdateScoreText(_enemyScoreValue);
                WaveManager.Instance.ReduceEnemyCount();
                StopAllCoroutines();
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error on StartOnDeathEffects: " + e.Message);
            }
            finally
            {
                _enemyDeathState = EnemyDeathState.Dead;
                Destroy(gameObject, _delayToDestroyEnemy);
            }
        }
        
        

        
    }

    protected virtual void EnableShieldOnStart()
    {
        if (Random.value < _chanceToEnableShield)
        {
            _isShieldEnabled = true;            
            _shieldParticle.Play();
        }
    }

    protected void DisableShield() 
    {
        _isShieldEnabled = false;
        _shieldParticle.Stop();
    }
    

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        string tag = other.tag;

        switch (tag)
        {
            case "Player":
                Player player = other.GetComponent<Player>();
                if (player != null)
                {
                    player.DamagePlayer();
                }
                StartOnDeathEffects();
                break;
            case "Laser":
                Debug.Log("Laser hit enemy");   
                Laser laser = other.GetComponent<Laser>();
                if (!laser.IsEnemyLaser)
                {

                    Destroy(other.gameObject);
                    StartOnDeathEffects();
                }
                break;
            case "Bomb":
                Destroy(other.gameObject);
                StartOnDeathEffects();
                break;
        }
        return;

                //if (other.CompareTag("Player"))
                //{

                //    Player player = other.GetComponent<Player>();

                //    if (player != null)
                //    {
                //        player.DamagePlayer();
                //    }
                //    StartOnDeathEffects();

                //}
                //else if (other.CompareTag("Laser"))
                //{


                //}
        }
}
