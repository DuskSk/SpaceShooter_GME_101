using System.Collections;
using UnityEngine;
using static UnityEngine.UI.Image;

public class Enemy : BaseEnemy
{
    
    
    

    [Header("Enemy Vertical Movement")]    
    [SerializeField] private float _xMinLimit = -10f, _xMaxLimit = 10f;    
    [SerializeField] private float _yTopRespawnPoint = 8f;
  
      

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
        MoveEnemy();
        
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
        laserObject.Initialize(direction, true);
    }

    private IEnumerator LaserShootingCoroutine()
    {
        while (true)
        {
            
            yield return new WaitForSeconds(Random.Range(_minDelayToShoot, _maxDelayToShoot));
            if (! _myCollider2D.enabled)
            {
                break;
            }
            Fire();     
        }
        
    }   
    


    

    
}
