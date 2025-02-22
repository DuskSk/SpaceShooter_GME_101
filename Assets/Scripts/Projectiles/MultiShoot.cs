
using UnityEngine;

public class MultiShoot : MonoBehaviour, IProjectile
{
    [SerializeField] private float _speed;
    [SerializeField] AudioClip _laserAudioClip;
    private Vector3 _direction;
    private Vector3 _viewportPosition;

    private void Start()
    {
        AudioSource.PlayClipAtPoint(_laserAudioClip, transform.position);
    }

    private void Update()
    {
        Move();
    }

    public void Move()
    {
        transform.Translate(_direction * _speed * Time.deltaTime, Space.World);
        _viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
        if (_viewportPosition.x > 1 || _viewportPosition.x < 0 || _viewportPosition.y > 1 || _viewportPosition.y < 0)
        {
            Destroy(this.gameObject);
        }
    }
    

    public void Initialize(Vector3 direction, float speed, bool isEnemy = true)
    {
        _direction = direction;      
        
        //sets the rotation in order to spawn  in 4 directions
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
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            player.DamagePlayer();
            Destroy(this.gameObject);

        }
    }

}
