﻿using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    
    [SerializeField] private int _totalWaves;    
    [SerializeField] private int _totalEnemies;
    [SerializeField] private float _waveCooldown;    
    [SerializeField] private float _waveMultiplier;
    private int _enemiesRemaining;
    private int _currentWave;

    public static WaveManager Instance { get; private set; }



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    

    public void StartWave()
    {
        if (_currentWave >= _totalWaves)
        {
            Debug.Log(" All waves concluded");            
            return;
        }

        _enemiesRemaining = _totalEnemies;
        _currentWave++;
        Debug.Log($"Wave {_currentWave} has started with {_totalEnemies} enemies");
        
        if (IsBossWave())
        {
            SpawnManager.Instance.SpawnBoss();
        }
        else
        {
            SpawnManager.Instance.SpawnEnemies(_totalEnemies);
              

        }
        _totalEnemies = (int)(_totalEnemies * _waveMultiplier);

    }

    public void StartPowerupSpawn()
    {
        SpawnManager.Instance.SpawnPowerups();
        SpawnManager.Instance.SpawnDebuffs();
    }



    private void EndWave()
    {
        if(_currentWave < _totalWaves)
        {
            
            StartCoroutine(StartWaveWithCooldown());
        }
    }

    public void ReduceEnemyCount()
    {
        _enemiesRemaining--;
        Debug.Log($"Enemies remaining: {_enemiesRemaining}");
        if (_enemiesRemaining <= 0)
        {
            EndWave();
        }
    }

    private IEnumerator StartWaveWithCooldown()
    {
        Debug.Log($"Awaiting {_waveCooldown}s for next wave to start.. ");
        yield return new WaitForSeconds(_waveCooldown);
        StartWave();        
    }    
    public bool IsBossWave()
    {
        return _currentWave == _totalWaves;
    }

    public int GetRemainingEnemies()
    {
        return _enemiesRemaining;
    }

    public int GetTotalEnemies()
    {
        return _totalEnemies;
    }     
   
}
