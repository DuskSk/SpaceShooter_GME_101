using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerupPrefab;


    [SerializeField]
    private float _secondsToSpawn = 5f;
    private bool _isSpawning = true;
    
    void Start()
    {
        StartCoroutine(SpawnEnemyRoutine(_secondsToSpawn));
        StartCoroutine(SpawnPowerupRoutine());
        
        
    }

    
    void Update()
    {
        
    }

    IEnumerator SpawnEnemyRoutine(float enemySpawnRate)
    {
        while (_isSpawning) 
        {   
            Vector3 spawnPosition = new Vector3 (Random.Range(-10f, 10f), 8f, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, spawnPosition, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(enemySpawnRate);            
            
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        while (_isSpawning) 
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-10f, 10f), 8f, 0);
            int powerupRandomId = Random.Range(0, 3);
            GameObject newPowerup = Instantiate(_powerupPrefab[powerupRandomId], spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(5f, 8f));
        }

    }

    public void OnPlayerDeath()
    {
        _isSpawning = false;
    }

}
