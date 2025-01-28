using UnityEngine;

public class Asteroid : MonoBehaviour
{

    [SerializeField] private float _rotationSpeed = 3.0f;
    [SerializeField] private GameObject _explosionPrefab;
    private SpawnManager _spawnManager;

    void Start()
    {
        _spawnManager = GameObject.FindGameObjectWithTag("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null) { Debug.LogError("Spawn Manager is NULL for Asteroid"); }
    }

    
    void Update()
    {
        transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);
        ;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser")) 
        {  
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            _spawnManager.StartSpawning();
            Destroy(this.gameObject);
            Destroy(other.gameObject);            
        }
    }
}
