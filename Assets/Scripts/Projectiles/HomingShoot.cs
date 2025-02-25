
using UnityEngine;

public class HomingShoot : MonoBehaviour, IProjectile
{
    private enum HomingState { Move, Homing };
    private HomingState _homingState = HomingState.Move;
    [SerializeField] private float _speed;
    [SerializeField] private float _homingSpeed;
    [SerializeField] private float _detectionRadius = 3f;
    [SerializeField] private float _detectionInterval = 0.2f;
    [SerializeField] LayerMask _enemyLayer;

    private Transform _targetEnemy;
    private Vector3 _direction = Vector3.up;
    private Vector3 _viewportPosition;
    private Camera _mainCamera;

    private Collider2D _enemyCollider;
    void Start()
    {
        _mainCamera = Camera.main;
        InvokeRepeating("DetectEnemyNearby", 0f, _detectionInterval);
    }


    void Update()
    {
        if (_homingState == HomingState.Move)
        {
            Move();
        }
        else if (_homingState == HomingState.Homing && _targetEnemy != null)
        {
            MoveTowardsEnemy();
        }
    }

    public void Move()
    {
        transform.Translate(_direction * _speed * Time.deltaTime);
        _viewportPosition = _mainCamera.WorldToViewportPoint(transform.position);
        if (_viewportPosition.x > 1 || _viewportPosition.x < 0 || _viewportPosition.y > 1 || _viewportPosition.y < 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void Initialize(Vector3 direction, float speed, bool isEnemy = false)
    {
        _direction = direction;
        _speed = speed;
    }

    public void Initialize(Vector3 direction, float speed,float homingSpeed, bool isEnemy = false)
    {
        _direction = direction;
        _speed = speed;
        _homingSpeed = homingSpeed;

    }

    private void MoveTowardsEnemy()
    {
        Vector3 direction = (_targetEnemy.position - transform.position).normalized;
        transform.position = Vector3.Lerp(transform.position, _targetEnemy.position, _homingSpeed * Time.deltaTime);

        //transform.position = Vector3.MoveTowards(transform.position, enemyCollider.transform.position, _homingSpeed * Time.deltaTime);
    }    

    private void DetectEnemyNearby()
    {
        if(_homingState == HomingState.Homing)
        {
            return;
        }

        _enemyCollider = Physics2D.OverlapCircle(transform.position, _detectionRadius, _enemyLayer);
        if (_enemyCollider != null && (_enemyCollider.CompareTag("Enemy") || _enemyCollider.CompareTag("Boss")))
        {
            Debug.Log("Enemy detected");
            _homingState = HomingState.Homing;
            _targetEnemy = _enemyCollider.transform;

        }
    }

    




}
