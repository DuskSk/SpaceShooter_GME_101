using UnityEngine;

public class Asteroid : MonoBehaviour
{

    [SerializeField] private float _rotationSpeed = 3.0f;
    [SerializeField] private GameObject _explosionPrefab;
    private SpawnManager _spawnManager;
    private AudioManager _audioManager;

    void Start()
    {
        _spawnManager = GameObject.FindGameObjectWithTag("Spawn_Manager").GetComponent<SpawnManager>();
        _audioManager = GameObject.FindWithTag("Audio_Manager").GetComponent<AudioManager>();
        if (_spawnManager == null) { Debug.LogError("Spawn Manager is NULL for Asteroid"); }
    }


    void Update()
    {
        transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser") || other.CompareTag("Player")) 
        {  
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            _audioManager.PlayExplosionAudio();
            WaveManager.Instance.StartWave();                       
            Destroy(this.gameObject);
                        
        }
    }
}
