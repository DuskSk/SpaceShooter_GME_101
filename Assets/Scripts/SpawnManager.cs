﻿using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject[] _powerupPrefab;

    [Header("Spawn rate")]
    [SerializeField] private float _enemySpawnRateInSeconds = 5f;    
    [SerializeField] private float _delayToStartSpawnRoutine = 3.0f;
    private bool _isSpawning = true;

    [Header("Spawn Position")]
    [SerializeField] private float _minSpawnRangeX, _maxSpawnRangeX, _spawnRangeY;



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
            int powerupRandomId = Random.Range(0, _powerupPrefab.Length);
            Instantiate(_powerupPrefab[powerupRandomId], spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(7f, 9f));
        }

    }

    public void OnPlayerDeath()
    {
        _isSpawning = false;
    }

}
