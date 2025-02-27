using UnityEngine;
using System.Collections;


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

    [Header("Boss Spawn Configuration")]
    [SerializeField] private GameObject _bossPrefab;
    [SerializeField] private Vector3 _bossSpawnPosition;    


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

    #endregion

    #region Variable for Local usage

    private GameObject _enemyPrefab;
    private GameObject _powerupPrefab;
    private Vector3 _enemySpawnPosition, 
        _viewportSpawnPosition, 
        _powerupSpawnPosition,
        _debuffSpawnPosition;
    private int _powerupRandomId,
        _debuffRandomId;

    #endregion

    public static SpawnManager Instance { get; private set; }

    


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _maincamera = Camera.main;
        
    }

    #region Spawn Methods

    public void SpawnEnemies(int enemyCount)
    {
        StartCoroutine(SpawnEnemyRoutine(_enemySpawnRateInSeconds, enemyCount));
    }

    public void SpawnPowerups()
    {
        StartCoroutine(SpawnPowerupRoutine());
    }

    public void SpawnDebuffs()
    {
        StartCoroutine(SpawnDebuffRoutine());
    }

    public void SpawnBoss()
    {

        Instantiate(_bossPrefab, _bossSpawnPosition, Quaternion.identity);        
    }

    #endregion
    public void OnPlayerDeath()
    {
        _isSpawning = false;
    }


    #region Routines
    IEnumerator SpawnEnemyRoutine(float enemySpawnRate, int enemyCount)
    {
        
        yield return new WaitForSeconds(_delayToStartSpawnRoutine);      
        

        while (enemyCount > 0)
        {

            
            _spawnRarityControl = Random.Range(0f, 1.01f);
            // TODO
            // Implement a variable on enemy to hold the Vector3 position for the spawn
            if (_spawnRarityControl <= _commonEnemyMaxPercentage)
            {
                _enemyPrefab = _commonEnemyPrefab;
                _viewportSpawnPosition = new Vector3(Random.Range(0.05f, 1f), 1.1f, 1f);
            }
            else if (_spawnRarityControl > _commonEnemyMaxPercentage && _spawnRarityControl <= _uncommonEnemyMaxPercentage)
            {
                _enemyPrefab = _uncommonEnemyPrefab;
                // _viewportSpawnPosition = new Vector3(-0.1f, Random.Range(0.5f, 0.9f), 1f);
                _viewportSpawnPosition = new Vector3(Random.Range(-0.15f, -0.05f), Random.Range(0.5f, 0.9f), 1f);
            }
            else
            {
                _enemyPrefab = _commonEnemyPrefab;
                _viewportSpawnPosition = new Vector3(Random.Range(0.05f, 1f), 1.1f, 1f);
                Debug.Log("Rare Enemy Spawned");
            }
            _enemySpawnPosition = _maincamera.ViewportToWorldPoint(_viewportSpawnPosition);            
            GameObject newEnemy = Instantiate(_enemyPrefab, _enemySpawnPosition, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            enemyCount--;
            yield return new WaitForSeconds(enemySpawnRate);



        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(_delayToStartPowerUpSpawnRoutine);

        while (_isSpawning)
        {            
            
            _powerupSpawnPosition = new Vector3(Random.Range(_minSpawnRangeX, _maxSpawnRangeX), _spawnRangeY, 0);

            _spawnRarityControl = Random.Range(0f, 1.01f);
            if (_spawnRarityControl <= _commonMaxPercentage)
            {
                _powerupRandomId = Random.Range(0, _commonPowerupPrefab.Length);
                _powerupPrefab = _commonPowerupPrefab[_powerupRandomId];

            }
            else if (_spawnRarityControl > _commonMaxPercentage && _spawnRarityControl <= _uncommonMaxPercentage)
            {
                _powerupRandomId = Random.Range(0, _uncommonPowerupPrefab.Length);
                _powerupPrefab = _uncommonPowerupPrefab[_powerupRandomId];
            }
            else
            {
                _powerupRandomId = Random.Range(0, _rarePowerupPrefab.Length);
                _powerupPrefab = _rarePowerupPrefab[_powerupRandomId];
            }

            Instantiate(_powerupPrefab, _powerupSpawnPosition, Quaternion.identity);

            yield return new WaitForSeconds(Random.Range(_minPowerUpSpawnRate, _maxPowerUpSpawnRate));
        }

    }

    IEnumerator SpawnDebuffRoutine()
    {
        yield return new WaitForSeconds(_delayToStartDebuffSpawnRoutine);
        while (_isSpawning)
        {            
            _debuffSpawnPosition = new Vector3(Random.Range(_minSpawnRangeX, _maxSpawnRangeX), _spawnRangeY, 0);
            _debuffRandomId = Random.Range(0, _debuffPrefab.Length);
            Instantiate(_debuffPrefab[_debuffRandomId], _debuffSpawnPosition, Quaternion.identity);

            yield return new WaitForSeconds(Random.Range(_minDebuffSpawnRate, _maxDebuffSpawnRate));
        }
    }

    #endregion

}
