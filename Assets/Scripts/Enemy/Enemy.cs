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

    [Header("Screen Boundary Offset")]
    [SerializeField] private float _screenOffset = 0.5f;

    [SerializeField] LayerMask _playerLayer;

    RaycastHit2D[] _raycastHit2D;






    protected override void Start()

    {
        base.Start();        
        if (_player == null)
        {
            Debug.Log("Player component is NULL");
        }
        _raycastHit2D = new RaycastHit2D[1];
        InvokeRepeating("CheckPlayerBehindEnemy", 0f, 0.3f);
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
        Vector2 origin = transform.position;
        

        Physics2D.RaycastNonAlloc(origin, Vector2.up , _raycastHit2D, _rayDetectionDistance, _playerLayer);

        if (_raycastHit2D[0].collider != null)
        {
            Debug.Log("raycast detected Player:  " + _raycastHit2D[0].collider.tag);
        }

        //RaycastHit2D hit = Physics2D.Raycast(origin, direction, _rayDetectionDistance, _playerLayer, 0f, Mathf.Infinity);

        //if (hit.collider != null)
        //{
        //    Debug.Log("Player detectado pelo Raycast!");
        //}
        Debug.DrawRay(transform.position, Vector3.up * _rayDetectionDistance, Color.red);
        
    }
    

    //fires Laser for now
    //TODO 
    //implement projectile interface
    protected override void Fire()
    {
        
        _laserOffset = transform.position + _laserOffsetPosition;

        Laser laserObject = Instantiate(_laserPrefab, _laserOffset, Quaternion.identity);        
        laserObject.Initialize(Vector3.down, true);
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
