﻿using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _enemySpeed = 4f;

    private float _yBottomLimit = -6f;
    private float _yTopRespawnPoint = 8f;
    
    void Start()
    {
        
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

            Destroy(this.gameObject);
        }
        else if (other.CompareTag("Laser"))
        {
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }
    }
}