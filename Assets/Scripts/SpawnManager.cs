﻿using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    #region Variables
    
    [SerializeField] private GameObject _enemyContainer;    
    private UIManager _uiManager;

    [Header("Common PowerUp Spawns")]
    [SerializeField] private GameObject[] _commonPowerupPrefab;

    [Header("Uncommon PowerUp Spawns")]
    [SerializeField] private GameObject[] _uncommonPowerupPrefab;

    [Header("Rare PowerUp Spawns")]
    [SerializeField] private GameObject[] _rarePowerupPrefab;

    [Header("Debuff Spawns")]
    [SerializeField] private GameObject[] _debuffPrefab;

    [Header("Spawn rate")]
    [SerializeField] private float _enemySpawnRateInSeconds = 5f;    
    [SerializeField] private float _delayToStartSpawnRoutine = 3.0f;
    [SerializeField] private float _delayToStartPowerUpSpawnRoutine = 4.0f;
    [SerializeField] private float _delayToStartDebuffSpawnRoutine = 3.0f;
    private bool _isSpawning = true;

    [Header("Spawn rate Power Up")]
    [SerializeField] private float _minPowerUpSpawnRate;
    [SerializeField] private float _maxPowerUpSpawnRate;

    [Header("Spawn rate Debuff")]
    [SerializeField] private float _minDebuffSpawnRate;
    [SerializeField] private float _maxDebuffSpawnRate;

    [Header("Common Enemy Spawn")]
    [SerializeField] private GameObject _commonEnemyPrefab;

    [Header("Uncommon Enemy Spawn")]
    [SerializeField] private GameObject _uncommonEnemyPrefab;

    [Header("Enemy Spawn Position - Vertical")]
    [SerializeField] private float _minSpawnRangeX;
    [SerializeField] private float _maxSpawnRangeX;
    [SerializeField] private float _spawnRangeY;

    [Header("Enemy Spawn Position - ZigZag")]
    [SerializeField] private float _zzMinSpawnRangeY;
    [SerializeField] private float _zzMaxSpawnRangeY;
    [SerializeField] private float _zzSpawnRangeX;

    [Header("Wave Configuration")]    
    [SerializeField] private float _waveWeight;
    [SerializeField] private int _currentWave;
    [SerializeField] private int _maxWaveAmount;
    [SerializeField] private float _maxEnemyCount;    
    private int _enemySpawnCount;
    private int _enemiesToNextWave;
    private bool _isGameOver = false;


    [Header("PowerUp Rarity Range Control")]
    [Tooltip("Set between 0 and 1")][SerializeField] private float _commonMaxPercentage;
    [SerializeField] private float _uncommonMaxPercentage;
    [Tooltip("Set it higher then Common range")][SerializeField] private float _rareMaxPercentage;

    [Header("Enemy Rarity Range Control")]
    [Tooltip("Set between 0 and 1")][SerializeField] private float _commonEnemyMaxPercentage;
    [SerializeField] private float _uncommonEnemyMaxPercentage;
    [Tooltip("Set it higher then Common range")][SerializeField] private float _rareEnemyMaxPercentage;
    private float _spawnRarityControl;

    private Camera _maincamera;

    private WaveState _currentWaveState;
    #endregion

    public enum WaveState 
    { 
        Spawning, 
        Waiting, 
        Transitioning 
    };

    private void Start()
    {
                
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _enemiesToNextWave = (int)_maxEnemyCount;
        _currentWave = 1;
        _currentWaveState = WaveState.Waiting;
        _maincamera = Camera.main;  
    }

    private void Update()
    {
        switch (_currentWaveState)
        {
            case WaveState.Spawning:
                if (_enemySpawnCount == Mathf.RoundToInt(_maxEnemyCount))
                {
                    CheckCurrentWave();                    
                }
                break;
            case WaveState.Waiting:
                if (_enemiesToNextWave <= 0)
                {                    
                    _currentWaveState = WaveState.Transitioning;
                }
                break;
            case WaveState.Transitioning:
                PrepareNextWave();
                break;
        }
        

               

        if (_isGameOver)
        {
            _uiManager.StartWinSequence();
            _isGameOver = false;
        }
    }

    

    public void StartSpawning()
    {
        _enemySpawnCount = 0;
        _isSpawning = true;
        _currentWaveState = WaveState.Spawning;
        StartCoroutine(SpawnEnemyRoutine(_enemySpawnRateInSeconds));
        StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(SpawnDebuffRoutine());

    }

    public void UpdateAvailableEnemies()
    {
        _enemiesToNextWave--;
        Debug.Log($"Avaialable enemies: {_enemiesToNextWave}");
    }

    public void OnPlayerDeath()
    {
        _isSpawning = false;
    }

    private void CheckCurrentWave()
    {    
        _isSpawning = false;
        StopCoroutine("SpawnEnemyRoutine");
        _currentWaveState = WaveState.Waiting;

    }
    private void PrepareNextWave()
    {
        IncreaseWave();
        if (_currentWave > _maxWaveAmount)
        {
            _isGameOver = true;
            return;
        }
        UpdateWaveWeight(_waveWeight);
        _enemiesToNextWave = (int)_maxEnemyCount;
        StartSpawning();
    }    

    private void IncreaseWave()
    {
        _currentWave++;
        Debug.Log(_currentWave);
    }

    private void UpdateWaveWeight(float weight)
    {
        _maxEnemyCount *= weight;
        Debug.Log(_maxEnemyCount);
        
    }


    #region Routines
    IEnumerator SpawnEnemyRoutine(float enemySpawnRate)
    {
        
        yield return new WaitForSeconds(_delayToStartSpawnRoutine);
        Vector3 spawnPosition;
        Vector3 viewportSpawnPosition;

        while (_isSpawning)
        {

            GameObject enemyPrefab;
            _spawnRarityControl = Random.Range(0f, 1.01f);
            // TODO
            // Implement a variable on enemy to hold the Vector3 position for the spawn
            if (_spawnRarityControl <= _commonEnemyMaxPercentage)
            {
                enemyPrefab = _commonEnemyPrefab;
                viewportSpawnPosition = new Vector3(Random.Range(0.05f, 1f), 1.1f, 1f);
            }
            else if (_spawnRarityControl > _commonEnemyMaxPercentage && _spawnRarityControl <= _uncommonEnemyMaxPercentage)
            {
                enemyPrefab = _uncommonEnemyPrefab;                
                viewportSpawnPosition = new Vector3(-0.1f, Random.Range(0.5f, 0.9f), 1f);
            }
            else
            {
                enemyPrefab = _commonEnemyPrefab;
                viewportSpawnPosition = new Vector3(Random.Range(0.05f, 1f), 1.1f, 1f);
                Debug.Log("Rare Enemy Spawned");
            }
            spawnPosition = _maincamera.ViewportToWorldPoint(viewportSpawnPosition);
            _enemySpawnCount++;
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(enemySpawnRate);
             
                      
            
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(_delayToStartPowerUpSpawnRoutine);

        while (_isSpawning) 
        {
            GameObject powerupPrefab;
            int powerupRandomId;
            Vector3 spawnPosition = new Vector3(Random.Range(_minSpawnRangeX, _maxSpawnRangeX), _spawnRangeY, 0);

            _spawnRarityControl = Random.Range(0f, 1.01f);
            if (_spawnRarityControl <= _commonMaxPercentage)
            {
                powerupRandomId = Random.Range(0, _commonPowerupPrefab.Length);
                powerupPrefab = _commonPowerupPrefab[powerupRandomId];
                
            }
            else if(_spawnRarityControl > _commonMaxPercentage && _spawnRarityControl <= _uncommonMaxPercentage)
            {
                powerupRandomId = Random.Range(0, _uncommonPowerupPrefab.Length);
                powerupPrefab = _uncommonPowerupPrefab[powerupRandomId];
            }
            else
            {
                powerupRandomId = Random.Range(0, _rarePowerupPrefab.Length);
                powerupPrefab = _rarePowerupPrefab[powerupRandomId];   
            }

            Instantiate(powerupPrefab, spawnPosition, Quaternion.identity);

            yield return new WaitForSeconds(Random.Range(_minPowerUpSpawnRate, _maxPowerUpSpawnRate));
        }

    }

    IEnumerator SpawnDebuffRoutine()
    {
        yield return new WaitForSeconds(_delayToStartDebuffSpawnRoutine);
        while (_isSpawning)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(_minSpawnRangeX, _maxSpawnRangeX), _spawnRangeY, 0);
            int debuffRandomId = Random.Range(0, _debuffPrefab.Length);
            Instantiate(_debuffPrefab[debuffRandomId], spawnPosition, Quaternion.identity);

            yield return new WaitForSeconds(Random.Range(_minDebuffSpawnRate, _maxDebuffSpawnRate));
        }
    }

    #endregion

}
