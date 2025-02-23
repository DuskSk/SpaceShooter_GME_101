using UnityEngine;
using UnityEngine.SceneManagement;

public  class GameManager : MonoBehaviour
{
        

    private void Awake()
    {      

        
    }

    private void Start()
    {        
        if (WaveManager.Instance == null)
        {
            Debug.LogError("WaveManager is NULL");
        }

        if (SpawnManager.Instance == null)
        {
            Debug.LogError("SpawnManager is NULL");
        }
        WaveManager.Instance.StartWave();
        WaveManager.Instance.StartPowerupSpawn();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(1);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    
}
