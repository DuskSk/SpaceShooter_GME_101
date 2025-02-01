using System.Collections;
using System.Threading;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Movement")]
    [SerializeField] private float _enemySpeed = 4f;
    [SerializeField] private float _xMinLimit = -10f, _xMaxLimit = 10f;
    private float _yBottomLimit = -6f;
    private float _yTopRespawnPoint = 8f;

    [SerializeField] private int _enemyScoreValue = 10;
    private Player _player;
    private Animator _animator;
    [SerializeField] private float _delayToDestroyEnemy = 2.6f;
    private AudioManager _audioManager;


    [Header("Enemy Laser Configuration")]
    [SerializeField] private Vector3 _laserOffsetPosition = new Vector3(0, -0.9f, 0);    
    [SerializeField] private Laser _laserPrefab;
    [SerializeField] private float _minDelayToShoot = 0.5f, _maxDelayToShoot = 2.0f;
    private Vector3 _laserOffset;

    private Collider2D _myEnemyCollider2d;



    void Start()
    {
        _player = FindObjectOfType<Player>().GetComponent<Player>();
        _audioManager = GameObject.FindWithTag("Audio_Manager").GetComponent<AudioManager>();
        if (_player == null)
        {
            Debug.Log("Player component is NULL");
        }

        _animator = GetComponent<Animator>();
        _myEnemyCollider2d = GetComponent<Collider2D>();

        StartCoroutine(LaserShootingCoroutine());
    }
    
    void Update()
    {
        
        CalculateMovement();
        
    }

    void CalculateMovement()
    {
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);

        if (transform.position.y <= _yBottomLimit) 
        {
            
            transform.position = new Vector3(Random.Range(_xMinLimit, _xMaxLimit), _yTopRespawnPoint, 0);
        
        }
    }

    void FireEnemyLaser()
    {
        
        _laserOffset = transform.position + _laserOffsetPosition;

        Laser laserObject = Instantiate(_laserPrefab, _laserOffset, Quaternion.identity);        
        laserObject.EnableEnemyLaser();
    }

    private IEnumerator LaserShootingCoroutine()
    {
        while (true)
        {
            
            
            yield return new WaitForSeconds(Random.Range(_minDelayToShoot, _maxDelayToShoot));
            if (!_myEnemyCollider2d.enabled)
            {
                break;
            }
            FireEnemyLaser();
            
                
            
            
        }

        
    }

    private void StartOnDeathEffects()
    {
        _animator.SetTrigger("OnEnemyDeath");
        _enemySpeed = 0f;
        _audioManager.PlayExplosionAudio();
        Destroy(this.gameObject, _delayToDestroyEnemy);
    }


    private void OnTriggerEnter2D(Collider2D other)
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
                StopAllCoroutines();

                _player.UpdatePlayerScore(_enemyScoreValue);
                Destroy(other.gameObject);                
                _myEnemyCollider2d.enabled = false;
                StartOnDeathEffects();                
                
            }

        }
        
    }

    
}
