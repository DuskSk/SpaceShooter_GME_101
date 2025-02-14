using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour

{
    [SerializeField] protected int _enemyScoreValue = 10;
    [SerializeField] protected float _enemySpeed;
    protected Animator _animator;
    protected float _delayToDestroyEnemy = 2.6f;
    protected AudioManager _audioManager;
    protected SpawnManager _spawnManager;
    protected Collider2D _myCollider2D;
    protected Player _player;


    protected virtual void Start()
    {
                
        _animator = GetComponent<Animator>();
        _audioManager = GameObject.FindGameObjectWithTag("Audio_Manager").GetComponent<AudioManager>();        
        _spawnManager = GameObject.FindGameObjectWithTag("Spawn_Manager").GetComponent<SpawnManager>();
        _myCollider2D = GetComponent<Collider2D>();
        _player = FindObjectOfType<Player>().GetComponent<Player>();

    }
    protected abstract void MoveEnemy();
    protected abstract void Fire();

    public virtual void StartOnDeathEffects()
    {
        _myCollider2D.enabled = false;
        _animator.SetTrigger("OnEnemyDeath");
        _enemySpeed = 0f;
        _audioManager.PlayExplosionAudio();
        _spawnManager.UpdateAvailableEnemies();        
        _player.UpdatePlayerScore(_enemyScoreValue);
        StopAllCoroutines();
        Destroy(gameObject, _delayToDestroyEnemy);
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            Player player = other.GetComponent<Player>();

            if (player != null)
            {
                player.DamagePlayer();
            }
            StartOnDeathEffects();

        }
        else if (other.CompareTag("Laser"))
        {
            Laser laser = other.GetComponent<Laser>();
            if (!laser.IsEnemyLaser)
            {   
                
                Destroy(other.gameObject);
                StartOnDeathEffects();

            }

        }
    }
}
