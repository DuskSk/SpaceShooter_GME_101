using UnityEngine;

public abstract class BaseBoss : MonoBehaviour
{
    protected enum BossPhase { Phase1, Phase2, Phase3 };
    [SerializeField] protected BossPhase _bossPhase = BossPhase.Phase1;
    [SerializeField] protected float _bossHealth = 1000f;
    
    public float BossHealth
    {
        get { return _bossHealth; }
    }

    protected abstract void MoveBoss();

    protected abstract void Fire();

    protected abstract void CheckBossPhase();

    protected virtual void TakeDamage(float damage)
    {
        _bossHealth -= damage;
        UIManager.Instance.UpdateBossHealthSlider(_bossHealth);
        if (_bossHealth <= 0)
        {
            Die();
        }

        if (_bossHealth <= _bossHealth * 0.3f && _bossPhase == BossPhase.Phase1)
        {
            ActivatePhase(BossPhase.Phase2);
        }
    }        

    protected virtual void ActivatePhase(BossPhase bossPhase) 
    {
        _bossPhase = bossPhase;
    }
    protected virtual void Die()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        string tag = other.tag;

        switch (tag)
        {
            case "Player":
                Player player = other.GetComponent<Player>();
                if (player != null)
                {
                    player.DamagePlayer();
                }                
                break;
            case "Laser":
                Laser laser = other.GetComponent<Laser>();
                if (!laser.IsEnemyLaser)
                {
                    Destroy(other.gameObject);
                    TakeDamage(laser.LaserDamageToBoss);
                }
                break;
            case "Bomb":
                //TODO
                //Implement bomb damage
                TakeDamage(30f);
                break;
        }
    }
}
