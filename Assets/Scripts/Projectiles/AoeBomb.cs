using UnityEngine;

public class AoeBomb : MonoBehaviour, IProjectile
{
    [SerializeField] private float _bombSpeed;
    [SerializeField] private float _bombMaxRadius = 2.5f;
    [SerializeField] private float _yBombLimit = 7.5f;
    private Vector3 _direction;

    private CircleCollider2D _circleCollider;
    private Transform _bombSprite;
    
    void Start()
    {
        _circleCollider = GetComponent<CircleCollider2D>();
        _bombSprite = GetComponentInChildren<Transform>();
    }

   
    void Update()
    {
        
        Move();
        
    }

    public void Move()
    {
        transform.Translate(_direction * _bombSpeed * Time.deltaTime);
        if (transform.position.y >= _yBombLimit)
        {
            Destroy(this.gameObject);
        }
        
    }

    public void Initialize(Vector3 direction, float speed, bool isEnemy = false)
    {
        _bombSpeed = speed;
        _direction = direction;
    }



    private void IncreaseBombImageScale()
    {
        _bombSprite.transform.localScale = new Vector3(9f, 9f, 0);
    }
    

    private void OnTriggerEnter2D(Collider2D other)
    {
          
        if (other.CompareTag("Enemy"))
        {
            _bombSpeed = 0f;            
            //if(_radiusCoroutine == null)
            //{                
            //    _radiusCoroutine = StartCoroutine(IncreaseBombRadiusRoutine());
            //    Debug.LogWarning("New radius Coroutine started");
            //}            
            Collider2D[] colliderList = Physics2D.OverlapCircleAll(transform.position, _bombMaxRadius);
            IncreaseBombImageScale();
            for (int i = 0; i < colliderList.Length;i++)
            {
                Debug.Log(colliderList[i].tag);
                if (colliderList[i].CompareTag("Enemy"))
                {
                    colliderList[i].gameObject.GetComponent<BaseEnemy>().StartOnDeathEffects();

                }
            }

            _circleCollider.enabled = false;
            Destroy(this.gameObject, 0.5f);
            
        }
        



    }





}
