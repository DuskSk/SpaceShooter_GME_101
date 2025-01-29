using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _enemySpeed = 4f;

    private float _yBottomLimit = -6f;
    private float _yTopRespawnPoint = 8f;
    [SerializeField] private int _enemyScoreValue = 10;
    private Player _player;
    private Animator _animator;
    [SerializeField] private float _delayToDestroyEnemy = 2.6f;
    private AudioManager _audioManager;
    
    void Start()
    {
        _player = FindObjectOfType<Player>().GetComponent<Player>();
        _audioManager = GameObject.FindWithTag("Audio_Manager").GetComponent<AudioManager>();
        if (_player == null)
        {
            Debug.Log("Player component is NULL");
        }

        _animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime);
        CheckPosition();
        
    }

    void CheckPosition()
    {
        if (transform.position.y <= _yBottomLimit) 
        {
            
            transform.position = new Vector3(Random.Range(-10f, 10f), _yTopRespawnPoint, 0);
        
        }
    }
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            Player player = other.GetComponent<Player>();

            if (player != null) 
            { 
                player.DamagePlayer(); 
            }            
            _animator.SetTrigger("OnEnemyDeath");
            _enemySpeed = 0f;
            _audioManager.PlayExplosionAudio();
            Destroy(this.gameObject, _delayToDestroyEnemy);
        }
        else if (other.CompareTag("Laser"))
        {
            _player.UpdatePlayerScore(_enemyScoreValue);
            Destroy(other.gameObject);
            _animator.SetTrigger("OnEnemyDeath");
            _enemySpeed = 0f;
            _audioManager.PlayExplosionAudio();
            Destroy(this.gameObject, _delayToDestroyEnemy);
        }
        
    }
}
