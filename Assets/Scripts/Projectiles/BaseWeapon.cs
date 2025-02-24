using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    [SerializeField] protected float _speed;
    [SerializeField] protected float _damageToBoss;
    [SerializeField] protected Vector3 _direction;
    [SerializeField] protected bool _isEnemyWeapon;

    protected Vector3 _viewportPosition;
    protected Camera _mainCamera;
    
    
    
    void Start()
    {
        _mainCamera = Camera.main;
    }

    
    void Update()
    {
        
    }

    protected abstract void Move();

    public abstract void Initialize(Vector3 direction, float speed, bool isEnemy = false);

    protected virtual void DestroyOnScreenExit(Vector3 objectPosition, GameObject myGameObject)
    {
        _viewportPosition = _mainCamera.WorldToViewportPoint(objectPosition);
        if (_viewportPosition.x > 1 || _viewportPosition.x < 0 || _viewportPosition.y > 1 || _viewportPosition.y < 0)
        {
            Destroy(myGameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && _isEnemyWeapon)
        {
            Player player = other.GetComponent<Player>();
            player.DamagePlayer();
            Destroy(gameObject);
        }
        
    }
}
