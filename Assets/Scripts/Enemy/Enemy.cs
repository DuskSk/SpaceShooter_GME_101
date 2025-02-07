using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    [Header("Enemy Current Movement Type")]
    [SerializeField] private Movement _movementType;
    public enum Movement { Vertical, ZigZag };

    [Header("Enemy Vertical Movement")]
    [SerializeField] private float _enemyVerticalSpeed = 4f;
    [SerializeField] private float _xMinLimit = -10f, _xMaxLimit = 10f;
    [SerializeField] private float _yBottomLimitToRespawn = -6f;
    [SerializeField] private float _yTopRespawnPoint = 8f;

    [Header("Enemy ZigZag Movement")]
    [SerializeField] private float _zzSpeed = 4f;
    [SerializeField] private float _frequency;
    [SerializeField] private float _amplitude;
    [SerializeField] private Direction _zigZagDirection;
    public enum Direction {Right, Left}
    private float _xCalculation, _yCalculation;

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
    private Vector3 _startPosition;


    public Movement MovementType
    {
        get { return _movementType; }
    }

    public Direction ZigZagDirection
    {
        get { return _zigZagDirection; }
    }

    


    void Start()

    {
         _startPosition = transform.position;

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

    //create enum for different movements
    //if move = this, then do it
    //zigzag? circles? horizontal? diagonal?
    void CalculateMovement()    
    {
        switch (_movementType)
        {
            case Movement.Vertical:
                transform.Translate(Vector3.down * _enemyVerticalSpeed * Time.deltaTime);

                if (transform.position.y <= _yBottomLimitToRespawn)
                {

                    transform.position = new Vector3(UnityEngine.Random.Range(_xMinLimit, _xMaxLimit), _yTopRespawnPoint, 0);

                }
                break;
            case Movement.ZigZag:

                _xCalculation = (_zigZagDirection == Direction.Right ? 1 : -1) * _zzSpeed * Time.deltaTime;
                _yCalculation = Mathf.Sin(Time.time * _frequency) * _amplitude;

                transform.position = new Vector3(_xCalculation + transform.position.x, _yCalculation + _startPosition.y, 0);                
                
                if(transform.position.x > _xMaxLimit || transform.position.x < -_xMaxLimit)
                {
                    transform.position = _startPosition;
                }
                break;
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
            
            
            yield return new WaitForSeconds(UnityEngine.Random.Range(_minDelayToShoot, _maxDelayToShoot));
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
        _enemyVerticalSpeed = 0f;
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
