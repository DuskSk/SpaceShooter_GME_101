using System.Collections;
using UnityEngine;
using static UnityEngine.UI.Image;

public class Enemy : BaseEnemy
{
    
    private enum EnemyState { Move, Evading};
    private EnemyState _enemyState = EnemyState.Move;

    [Header("Enemy Vertical Movement")]    
    [SerializeField] private float _xMinLimit = -10f, _xMaxLimit = 10f;    
    [SerializeField] private float _yTopRespawnPoint = 8f;

    [Header("Enemy Evasion Configuration")]
    [SerializeField] private float _evadeSpeed = 4f;
    [SerializeField] private float _evadeDuration = 1.5f;
    [SerializeField] private float _evadeCooldown = 3f;
    private float _lastEvadeTime = 0f;
    private CapsuleCollider2D _evasionCollider;


    [Header("Enemy Laser Configuration")]
    [SerializeField] private Vector3 _laserOffsetPosition = new Vector3(0, -0.9f, 0);    
    [SerializeField] private Laser _laserPrefab;
    [SerializeField] private float _minDelayToShoot = 0.5f, _maxDelayToShoot = 2.0f;    
    private Vector3 _laserOffset;

    [Header("Back Laser Configuration")]
    [SerializeField] private float _rayDetectionDistance = 10f;
    [SerializeField] private float _laserDelay = 1f;
    [SerializeField] LayerMask _playerLayer;
    private float _lastLaserTime;
    private bool _isPlayerBehindEnemy = false;  

    [Header("Screen Boundary Offset")]
    [SerializeField] private float _screenOffset = 0.5f;


    
    [SerializeField] LayerMask _powerupLayer;
    private bool _isPowerupInFrontOfEnemy = false;

    RaycastHit2D[] _raycastHit2D;
    Vector2 originPosition;







    protected override void Start()

    {
        base.Start();
        _evasionCollider = GetComponent<CapsuleCollider2D>();
        _myCollider2D = GetComponent<BoxCollider2D>();
        if (_player == null)
        {
            Debug.Log("Player component is NULL");
        }
        _raycastHit2D = new RaycastHit2D[1];        
        InvokeRepeating("CheckPlayerBehindEnemy", 0f, 0.3f);
        InvokeRepeating("CheckPowerupInFrontOfEnemy", 0f, 0.5f);
        StartCoroutine(LaserShootingCoroutine());
        
    }
    
    protected override void Update()
    {
        base.Update();
        if(_enemyState == EnemyState.Move)
        {
            MoveEnemy();
        }
        
        
    }

    
    protected override void MoveEnemy()    
    {

        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);

        CheckIfEnemyHasLeftScreen();

    }

    protected override void CheckIfEnemyHasLeftScreen()
    {
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
        if (viewportPosition.y < 0 - _screenOffset)
        {
            transform.position = new Vector3(Random.Range(_xMinLimit, _xMaxLimit), _yTopRespawnPoint, 0);
            
        }
    }

    protected void CheckPlayerBehindEnemy()
    {
        originPosition = transform.position;

        int hitCount = Physics2D.RaycastNonAlloc(originPosition, Vector2.up , _raycastHit2D, _rayDetectionDistance, _playerLayer);

        if (hitCount > 0 && _raycastHit2D[0].collider != null && _raycastHit2D[0].collider.CompareTag("Player"))
        {
            _isPlayerBehindEnemy = true;

            if (Time.time >= _lastLaserTime)
            {
                Fire(Vector3.up);
                _lastLaserTime = Time.time + _laserDelay;
                Debug.Log("raycast detected Player:  " + _raycastHit2D[0].collider.tag);
            }
            else
            {
                _isPlayerBehindEnemy = false;
            }         
            
        }
        
        Debug.DrawRay(transform.position, Vector3.up * _rayDetectionDistance, Color.red);
        
    }

    protected void CheckPowerupInFrontOfEnemy()
    {
        originPosition = transform.position;

        int hitCount = Physics2D.RaycastNonAlloc(originPosition, Vector2.down, _raycastHit2D, _rayDetectionDistance, _powerupLayer);

        if (hitCount > 0 && _raycastHit2D[0].collider != null && _raycastHit2D[0].collider.CompareTag("Powerup"))
        {
            _isPowerupInFrontOfEnemy = true;

            if (Time.time >= _lastLaserTime)
            {
                Fire(Vector3.down);
                _lastLaserTime = Time.time + _laserDelay;
                Debug.Log("raycast detected Powerup:  " + _raycastHit2D[0].collider.tag);
            }
            else
            {
                _isPowerupInFrontOfEnemy = false;
            }

        }

        Debug.DrawRay(transform.position, Vector3.down * _rayDetectionDistance, Color.green);
    }
    

    
    protected override void Fire()
    {

        Fire(Vector3.down);
    }

    protected void Fire(Vector3 direction)
    {
        _laserOffset = transform.position + _laserOffsetPosition;
        Laser laserObject = Instantiate(_laserPrefab, _laserOffset, Quaternion.identity);
        laserObject.GetComponent<SpriteRenderer>().color = Color.yellow;
        laserObject.Initialize(direction, _projectileSpeed, true);
    }

    protected void StartEvade()
    {
        _enemyState = EnemyState.Evading;
        _evasionCollider.enabled = false;

        Vector3 evadeDirection = (Random.value > 0.5f) ? Vector3.left : Vector3.right;

        transform.Translate(evadeDirection * _evadeSpeed * Time.deltaTime);

        _lastEvadeTime = Time.time + _evadeCooldown;

        Invoke("StopEvade", _evadeCooldown);
    }

    protected void StopEvade()
    {
        _enemyState = EnemyState.Move;
        _evasionCollider.enabled = true;

    }

    private IEnumerator LaserShootingCoroutine()
    {
        while (true)
        {
            
            yield return new WaitForSeconds(Random.Range(_minDelayToShoot, _maxDelayToShoot));
            if (!_myCollider2D.enabled)
            {
                break;
            }
            Fire();     
        }
        
    }



    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser") && _enemyState == EnemyState.Move && _evasionCollider.isActiveAndEnabled)
        {
            if (other.IsTouching(_evasionCollider))
            {
                if (Time.time > _lastEvadeTime)
                {
                    Laser laser = other.GetComponent<Laser>();
                    if (laser != null && !laser.IsEnemyLaser)
                    {
                        StartEvade();
                        return; // avoid further collision processing

                    }
                }
            }      
            
            

        }
        
        else if(other.IsTouching(_myCollider2D))
        {
            base.OnTriggerEnter2D(other);
        }




    }




}
