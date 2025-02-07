using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;
    private Enemy _enemy;
    

    [Header("Common PowerUp Spawns")]
    [SerializeField] private GameObject[] _commonPowerupPrefab;

    [Header("Rare PowerUp Spawns")]
    [SerializeField] private GameObject[] _rarePowerupPrefab;

    [Header("Spawn rate")]
    [SerializeField] private float _enemySpawnRateInSeconds = 5f;    
    [SerializeField] private float _delayToStartSpawnRoutine = 3.0f;
    private bool _isSpawning = true;

    [Header("Spawn rate Power Up")]
    [SerializeField] private float _minPowerUpSpawnRate;
    [SerializeField] private float _maxPowerUpSpawnRate;

    [Header("Spawn Position - Vertical")]
    [SerializeField] private float _minSpawnRangeX;
    [SerializeField] private float _maxSpawnRangeX;
    [SerializeField] private float _spawnRangeY;

    [Header("Spawn Position - ZigZag")]
    [SerializeField] private float _zzMinSpawnRangeY;
    [SerializeField] private float _zzMaxSpawnRangeY;
    [SerializeField] private float _zzSpawnRangeX;


    [Header("Rarity Range Control")]
    [Tooltip("Set between 0 and 1")][SerializeField] private float _commonMaxPercentage;
    [Tooltip("Set it higher then Common range")][SerializeField] private float _rareMaxPercentage;
    private float _spawnRarityControl;

    private void Start()
    {
        _enemy = _enemyPrefab.GetComponent<Enemy>();
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine(_enemySpawnRateInSeconds));
        StartCoroutine(SpawnPowerupRoutine());
    }

    
   

    IEnumerator SpawnEnemyRoutine(float enemySpawnRate)
    {
        
        yield return new WaitForSeconds(_delayToStartSpawnRoutine);
        Vector3 spawnPosition;

        while (_isSpawning)
        {
            
            if (_enemy.MovementType == Enemy.Movement.Vertical)
            {
                spawnPosition = new Vector3(Random.Range(_minSpawnRangeX, _maxSpawnRangeX), _spawnRangeY, 0);
            }
            else if (_enemy.MovementType == Enemy.Movement.ZigZag)
            {
                float xLimit = (_enemy.ZigZagDirection == Enemy.Direction.Right ? -_zzSpawnRangeX : _zzSpawnRangeX);
                spawnPosition = new Vector3(xLimit, Random.Range(_zzMinSpawnRangeY, _zzMaxSpawnRangeY), 0);
            }
            else
            {
                Debug.LogError("Enemy movement type is NULL");
                break;
            }


            GameObject newEnemy = Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(enemySpawnRate);
             
                      
            
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(_delayToStartSpawnRoutine);

        while (_isSpawning) 
        {
            Vector3 spawnPosition = new Vector3(Random.Range(_minSpawnRangeX, _maxSpawnRangeX), _spawnRangeY, 0);
            _spawnRarityControl = Random.Range(0f, 1.01f);
            if (_spawnRarityControl <= _commonMaxPercentage)
            {
                int powerupRandomId = Random.Range(0, _commonPowerupPrefab.Length);
                Instantiate(_commonPowerupPrefab[powerupRandomId], spawnPosition, Quaternion.identity);
            }
            else if(_spawnRarityControl > _commonMaxPercentage)
            {
                int powerupRandomId = Random.Range(0, _rarePowerupPrefab.Length);
                Instantiate(_rarePowerupPrefab[powerupRandomId], spawnPosition, Quaternion.identity);
            }

                      
            yield return new WaitForSeconds(Random.Range(_minPowerUpSpawnRate, _maxPowerUpSpawnRate));
        }

    }

    public void OnPlayerDeath()
    {
        _isSpawning = false;
    }

}
