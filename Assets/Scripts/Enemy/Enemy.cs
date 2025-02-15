using System.Collections;
using UnityEngine;

public class Enemy : BaseEnemy
{
    
    
    

    [Header("Enemy Vertical Movement")]
    [SerializeField] private float _enemyVerticalSpeed = 4f;
    [SerializeField] private float _xMinLimit = -10f, _xMaxLimit = 10f;    
    [SerializeField] private float _yTopRespawnPoint = 8f;
  
      

    [Header("Enemy Laser Configuration")]
    [SerializeField] private Vector3 _laserOffsetPosition = new Vector3(0, -0.9f, 0);    
    [SerializeField] private Laser _laserPrefab;
    [SerializeField] private float _minDelayToShoot = 0.5f, _maxDelayToShoot = 2.0f;
    private Vector3 _laserOffset;

    [Header("Screen Boundary Offset")]
    [SerializeField] private float _screenOffset = 0.5f;




    protected override void Start()

    {
        base.Start();        
        if (_player == null)
        {
            Debug.Log("Player component is NULL");
        }

        StartCoroutine(LaserShootingCoroutine());
    }
    
    protected override void Update()
    {
        base.Update();
        MoveEnemy();
        
    }

    
    protected override void MoveEnemy()    
    {
        transform.Translate(Vector3.down * _enemyVerticalSpeed * Time.deltaTime);

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
