using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float _laserSpeed = 8f, _yLaserLimit = 7.5f;
    private bool _isEnemyLaser = false;

    [SerializeField] AudioClip _laserAudioClip;
    
    public bool IsEnemyLaser
    {
        get { return _isEnemyLaser; }
    }
    
    void Start()
    {
        AudioSource.PlayClipAtPoint(_laserAudioClip, transform.position);
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
                transform.Translate(Vector3.down * _laserSpeed * Time.deltaTime);
                if (transform.position.y <= -_yLaserLimit)
                {
                    DestroyLaser();
                }
                break;
            case false:
                transform.Translate(Vector3.up * _laserSpeed * Time.deltaTime);
                if (transform.position.y >= _yLaserLimit)
                {
                    DestroyLaser();
                }
                break;
        }
        
    }

    public void EnableEnemyLaser()
    {
        _isEnemyLaser = true;
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
