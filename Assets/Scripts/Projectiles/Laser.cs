using UnityEngine;

public class Laser : MonoBehaviour, IProjectile
{
    [SerializeField] private float _laserSpeed = 8f, _yLaserLimit = 7.5f;
    [SerializeField] private float _laserDamageToBoss = 10f;
    private bool _isEnemyLaser = true;

    [SerializeField] AudioClip _laserAudioClip;
    private Vector3 _direction;
    private Space _spaceRelativeTo;

    public bool IsEnemyLaser
    {
        get { return _isEnemyLaser; }
    }
    public float LaserDamageToBoss
    {
        get { return _laserDamageToBoss; }
    }

    void Start()
    {
        AudioSource.PlayClipAtPoint(_laserAudioClip, transform.position);
        
    }
    
    void Update()
    {
        
        Move();
            
    }

    void DestroyLaser()
    {
        
        if(transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        Destroy(this.gameObject);        
    }

    
    public void Move()
    {
        transform.Translate(_direction * _laserSpeed * Time.deltaTime, _spaceRelativeTo);

        if (transform.position.y >= _yLaserLimit || transform.position.y <= -_yLaserLimit)
        {
            DestroyLaser();
        }
        
        
    }
    
    public void Initialize(Vector3 direction, bool isEnemy)
    {
        Debug.Log("Laser script: " + _direction);
        _direction = direction;
        _isEnemyLaser = isEnemy;  

    }

    public void Initialize(Vector3 direction, bool isEnemy,Space spaceRelativeTo = Space.Self)
    {
        Debug.Log("Laser script: " + _direction);
        _direction = direction;
        _isEnemyLaser = isEnemy;
        _spaceRelativeTo = spaceRelativeTo;

    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && _isEnemyLaser)
        {
            Player player = other.GetComponent<Player>();
            player.DamagePlayer();
            DestroyLaser();
            
        }else if (other.CompareTag("Powerup"))
        {
            Destroy(other.gameObject);
            DestroyLaser();
        }
    }

}
