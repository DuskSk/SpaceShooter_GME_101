
using UnityEngine;
//TO-DO
//Separate this in Collectible abstract Base Class
//Create A class for Powerup and Debuffs
public class Powerup : BaseCollectible

{
   

    private enum PowerUpType {TripleLaser, SpeedBoost, Shield, AmmoRefil, LifeRegeneration, AoeBomb, HomingShoot};
    [SerializeField] private PowerUpType _powerUpType;
     
    void Update()
    {
        Move();

    }


    protected override void ApplyEffect(Player player)
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
            case PowerUpType.HomingShoot:
                player.EnableHomingShoot();
                break;

        }

        Destroy(this.gameObject);
            
    }
}

