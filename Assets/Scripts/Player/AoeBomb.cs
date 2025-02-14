using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeBomb : MonoBehaviour
{
    [SerializeField] private float _bombSpeed;
    [SerializeField] private float _bombRadiusIncreaseRate = 1f, _bombMaxRadius = 2.5f;
    [SerializeField] private float _yBombLimit = 7.5f;
    
    private CircleCollider2D _circleCollider;
    private Transform _imageTransform;

    private Coroutine _radiusCoroutine;
    void Start()
    {
        _circleCollider = GetComponent<CircleCollider2D>();
        _imageTransform = GetComponentInChildren<Transform>();
    }

   
    void Update()
    {
        
        MoveBomb();
        
    }

    private void MoveBomb()
    {
        transform.Translate(Vector3.up * _bombSpeed * Time.deltaTime);
        if (transform.position.y >= _yBombLimit)
        {
            Destroy(this.gameObject);
        }
        
    }

    private void IncreaseBombImageScale()
    {
        this._imageTransform.localScale = new Vector3(5, 5, 0);
    }

    //Increases the collider radius in a steady rate
    private IEnumerator IncreaseBombRadiusRoutine()
    {
        while (true) 
        {
            _circleCollider.radius += _bombRadiusIncreaseRate * Time.deltaTime;
            if (_circleCollider.radius >= _bombMaxRadius)
            {
                _circleCollider.radius = _bombMaxRadius;
                break;
            }
            yield return new WaitForEndOfFrame();
        }    
        
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

            
            Destroy(this.gameObject);
            
        }
        



    }





}
