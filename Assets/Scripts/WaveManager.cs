using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private int _currentWave;
    [SerializeField] private int _maxWaveAmount;    
    [SerializeField] private float _maxEnemyCount;
    
    public float MaxEnemyCount
    {
        get { return _maxEnemyCount; }
    }

    public int CurrentWave
    {
        get { return _currentWave; }
    }

    public int MaxWaveAmount
    {
        get { return _maxWaveAmount; }
    }

    void Start()
    {
        _currentWave = 1;
    }


    public void IncreaseWave()
    {
        _currentWave++;
        Debug.Log(_currentWave);
    }

    public void UpdateWaveWeight(float weight)
    {
        _maxEnemyCount *= weight;
        Debug.Log(_maxEnemyCount);
    }


    
   
}
