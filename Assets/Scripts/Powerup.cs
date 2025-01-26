using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _powerupSpeed = 3f;
    [SerializeField]
    private float _yLimitToDestroy = -6f;

    [SerializeField]
    private int _powerupId;
    
    void Start()
    {
        
    }

    
    void Update()
    {
        transform.Translate(Vector3.down * _powerupSpeed * Time.deltaTime);
        if (transform.position.y < _yLimitToDestroy)
        {
            Destroy(this.gameObject);
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.GetComponent<Player>();
            if (player != null) 
            {
                //Grab powerUp based on it`s ID
                switch (_powerupId)
                {
                    case 0:
                        player.EnableTripleLaser();
                        break;
                    case 1:
                        player.EnableSpeedBoost();
                        break;
                    case 2:
                        player.EnableShield();
                        break;
                }
                
                Destroy(this.gameObject);
            }
            
        }
    }
}
