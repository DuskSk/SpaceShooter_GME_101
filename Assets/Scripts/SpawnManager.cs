using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;
    

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

    [Header("Spawn Position")]
    [SerializeField] private float _minSpawnRangeX;
    [SerializeField] private float _maxSpawnRangeX;
    [SerializeField] private float _spawnRangeY;


    [Header("Rarity Range Control")]
    [Tooltip("Set between 0 and 1")][SerializeField] private float _commonMaxPercentage;
    [Tooltip("Set it higher then Common range")][SerializeField] private float _rareMaxPercentage;
    private float _spawnRarityControl;

    private void Start()
    {
        //StartSpawning();
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemyRoutine(_enemySpawnRateInSeconds));
        StartCoroutine(SpawnPowerupRoutine());
    }
   

    IEnumerator SpawnEnemyRoutine(float enemySpawnRate)
    {
        yield return new WaitForSeconds(_delayToStartSpawnRoutine);

        while (_isSpawning) 
        {   
            Vector3 spawnPosition = new Vector3 (Random.Range(_minSpawnRangeX, _maxSpawnRangeX), _spawnRangeY, 0);
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
