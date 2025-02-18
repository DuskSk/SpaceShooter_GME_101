using System.Collections;
using UnityEngine;

public class BossOne : BaseBoss
{
    [SerializeField] protected float _speed = 2f;
    [SerializeField] protected GameObject _laserPrefab;
    private Laser _laser;

    [Header("Spiral Attack Cnfiguration")]
    [SerializeField] protected float _angle = 270f; // 🔹 Começa atirando para baixo (270°)
    [SerializeField] protected float _angleIncrease = 15f; // 🔹 Define o quanto o ângulo gira a cada tiro
    [SerializeField] protected float _fireRate = 0.3f; // 🔹 Tempo entre cada tiro
    [SerializeField] protected float _fireRatePhase2 = 0.1f;

    void Start()
    {
        StartCoroutine(SpiralAttackPattern());
    }

    
    void Update()
    {
        if (transform.position.y >= 4.5f)
        {
            MoveBoss();
        }
    }


    protected override void MoveBoss()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }

    protected override void Fire()
    {
        Fire(Vector3.down, Quaternion.identity);

    }

    protected void Fire(Vector3 direction, Quaternion rotation)
    {
        if (_laserPrefab == null)
        {
            Debug.LogError("No Laser prefab detect on Boss");
            return;
        }

        _laser = Instantiate(_laserPrefab, transform.position, rotation).GetComponent<Laser>();

        if (_laser != null)
        {
            _laser.Initialize(direction, true, Space.World);
        }
        //GameObject _projectileInstance = Instantiate(_laserPrefab, transform.position, Quaternion.identity);

        //IProjectile projectile = _projectileInstance.GetComponent<IProjectile>();



        //if (projectile != null)
        //{
        //    Laser projectileLaser = projectile as Laser;

        //    if (projectileLaser != null)
        //    {
        //        projectileLaser.Initialize(direction, true, Space.World);
        //    }
        //    else
        //    {
        //        projectile.Initialize(direction, true);

        //    }
        //}
    }

    IEnumerator AttackPatternRoutine()
    {
        while (true)
        {
            Fire();
            yield return new WaitForSeconds(3f);
        }
    }

    IEnumerator SpiralAttackPattern()
    {
        
        while (true)
        {
            
            // Convert the angle to a direction (unit vector)
            Vector3 direction = new Vector3(Mathf.Cos(_angle * Mathf.Deg2Rad), Mathf.Sin(_angle * Mathf.Deg2Rad), 0);
            Quaternion rotation = Quaternion.Euler(0, 0, _angle - 90f);
            Fire(direction, rotation);
            _angle += _angleIncrease;
            if (_angle >= 360) _angle -= 360f; // Turn the angle counterclockwise
            switch (_bossPhase)
            {
                case BossPhase.Phase1:
                    yield return new WaitForSeconds(_fireRate);
                    break;
                case BossPhase.Phase2:
                    yield return new WaitForSeconds(_fireRatePhase2);
                    break;
            }
            

        }
    }
}
