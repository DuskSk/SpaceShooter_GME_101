using System.Collections;
using UnityEngine;

public class WaveEnemy : BaseEnemy
{
    public enum Direction {LeftToRight, RightToLeft }

    [Header("Wave Movement Parameters")]
    [SerializeField] private float _waveFrequency = 1f;
    [SerializeField] private float _waveAmplitude = 1f;
    [SerializeField] private Laser _laserPrefab;
    [SerializeField] private Direction _waveDirection;
    [SerializeField] private float _screenOffset = 0.5f;
    private Vector3 _startPosition;
    


    protected override void Start()
    {
        base.Start();
        _startPosition = transform.position;        
        StartCoroutine(LaserShootingCoroutine());
        
    }

    private void Update()
    {
        MoveEnemy();
    }

    protected override void MoveEnemy()
    {
        float _xCalculation = (_waveDirection == Direction.LeftToRight ? 1 : -1) * _enemySpeed * Time.deltaTime;
        float _yCalculation = Mathf.Sin(Time.time * _waveFrequency) * _waveAmplitude;

        transform.position = new Vector3(_xCalculation + transform.position.x, _yCalculation + _startPosition.y, 0);

        

        // Check if the enemy has left the screen horizontally using viewport coordinates
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
        if (viewportPosition.x > 1 + _screenOffset || viewportPosition.x < 0 - _screenOffset)
        {
            float newXPosition = (_waveDirection == Direction.LeftToRight) ? -_screenOffset : 1 + _screenOffset;
            Vector3 newWorldPosition = Camera.main.ViewportToWorldPoint(new Vector3(newXPosition, viewportPosition.y, viewportPosition.z));
            transform.position = new Vector3(newWorldPosition.x, transform.position.y, transform.position.z);
        }
    }

    protected override void Fire()
    {
        Vector3[] directions = new Vector3[]
        {
            Vector3.up,
            Vector3.down,
            Vector3.left,
            Vector3.right
        };

        

        foreach (var direction in directions)
        {            
            
            Vector3 laserOffset = transform.position + direction * 0.5f; // Adjust offset as needed
            Laser laserObject = Instantiate(_laserPrefab, laserOffset, Quaternion.identity);
            Debug.Log("Enemy script: " + direction);
            laserObject.InitializeLaser(direction, true);
            
        }
    }

    

    private IEnumerator LaserShootingCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 2.0f)); // Random delay between shots
            if (!_myCollider2D.enabled)
            {
                Debug.Log("Collider not enabled");
                break;
            }
            Fire();
        }
    }

    
}
