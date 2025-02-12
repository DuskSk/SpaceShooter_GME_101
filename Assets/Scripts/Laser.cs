using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float _laserSpeed = 8f, _yLaserLimit = 7.5f;
    private bool _isEnemyLaser = true;

    [SerializeField] AudioClip _laserAudioClip;
    private Vector3 _direction;
    
    public bool IsEnemyLaser
    {
        get { return _isEnemyLaser; }
    }
    
    void Start()
    {
        AudioSource.PlayClipAtPoint(_laserAudioClip, transform.position);
        //_direction = Vector3.down;
    }
    
    void Update()
    {
        
        MoveLaser(_isEnemyLaser);
            
    }

    void DestroyLaser()
    {
        
        if(transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        Destroy(this.gameObject);        
    }

    private void MoveLaser(bool isEnemy)
    {

        switch (isEnemy)
        {
            case true:                
                transform.Translate(_direction * _laserSpeed * Time.deltaTime, Space.World);
                if (transform.position.y <= -_yLaserLimit)
                {
                    DestroyLaser();
                }
                break;
            case false:
                _direction = Vector3.up;
                transform.Translate(_direction * _laserSpeed * Time.deltaTime);
                if (transform.position.y >= _yLaserLimit)
                {
                    DestroyLaser();
                }
                break;
        }
        
    }

    public void SetEnemyLaser(bool isEnemy)
    {
        _isEnemyLaser = isEnemy;

    }
    public void InitializeLaser(Vector3 direction, bool isEnemyLaser)
    {
        Debug.Log("Laser script: " + _direction);
        _direction = direction;
        _isEnemyLaser = isEnemyLaser;

        if (direction == Vector3.left)
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (direction == Vector3.right)
        {
            transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else if (direction == Vector3.up)
        { 
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (direction == Vector3.down)
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }


    }

    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && _isEnemyLaser)
        {
            Player player = other.GetComponent<Player>();
            player.DamagePlayer();
            DestroyLaser();
            
        }
    }

}
