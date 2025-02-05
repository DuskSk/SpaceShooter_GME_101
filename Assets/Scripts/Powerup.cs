﻿
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private float _powerupSpeed = 3f;
    [SerializeField] private float _yLimitToDestroy = -6f;    

    [SerializeField] private AudioClip _powerUpAudioClip;


    
    public enum PowerUpType {TripleLaser, SpeedBoost, Shield, AmmoRefil, LifeRegeneration, AoeBomb};
    [SerializeField] private PowerUpType _powerUpType;
    
    
     
       
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

            AudioSource.PlayClipAtPoint(_powerUpAudioClip, transform.position);
            Player player = collision.GetComponent<Player>();
            if (player != null) 
            {
                
                switch (_powerUpType)
                {
                    case PowerUpType.TripleLaser:
                        player.EnableTripleLaser();
                        break;
                    case PowerUpType.SpeedBoost:
                        player.EnableSpeedBoost();
                        break;
                    case PowerUpType.Shield:
                        player.EnableShield();
                        break;
                    case PowerUpType.AmmoRefil:
                        player.RefillAmmo();
                        break;
                    case PowerUpType.LifeRegeneration:
                        player.RegenerateLife();
                        break;
                    case PowerUpType.AoeBomb:
                        player.EnableAoeBomb();
                        break;
                    
                }          
                
                Destroy(this.gameObject);
            }
            
        }
    }
}
