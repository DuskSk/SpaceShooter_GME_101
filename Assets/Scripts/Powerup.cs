
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] private float _powerupSpeed = 3f;
    [SerializeField] private float _yLimitToDestroy = -6f;
    [SerializeField] private int _powerUpId;

    [SerializeField] private AudioClip _powerUpAudioClip;


    //testing enums
    private enum PowerUpType {TripleLaser, SpeedBoost, Shield};
    [SerializeField] private PowerUpType _powerUpType;
    //
         
    
       
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
                //Grab powerUp based on it`s ID
                //switch (_powerUpId)
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
                }          
                
                Destroy(this.gameObject);
            }
            
        }
    }
}
