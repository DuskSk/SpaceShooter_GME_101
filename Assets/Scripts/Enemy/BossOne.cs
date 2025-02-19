using System.Collections;
using UnityEngine;

public class BossOne : BaseBoss
{
    [SerializeField] protected float _speed = 2f;
    [SerializeField] protected GameObject _laserPrefab;
    [SerializeField] protected GameObject _beamPrefab;
    private Laser _laser;

    [Header("Spiral Attack Cnfiguration")]
    [SerializeField] protected float _angle = 270f; // 🔹 Começa atirando para baixo (270°)
    [SerializeField] protected float _angleIncrease = 15f; // 🔹 Define o quanto o ângulo gira a cada tiro
    [SerializeField] protected float _fireRate = 0.3f; // 🔹 Tempo entre cada tiro
    [SerializeField] protected float _fireRatePhase2 = 0.1f;

    [Header("Beam Attack Configuration")]
    [SerializeField] protected float _beamTimeToLive = 5f;
    [SerializeField] protected float _beamCooldown = 5f;
    [SerializeField] protected bool _beamClockwise = false;     
    [SerializeField] protected float _rightBeamInitialRotation = 30f;
    [SerializeField] protected float _leftBeamInitialRotation = 30f;

    [SerializeField]protected GameObject[] _beamOffset;

    void Start()
    {

        StartCoroutine(SpiralAttackPattern());
        StartCoroutine(LaserBeamAttackPattern(false,_rightBeamInitialRotation , 0));
        StartCoroutine(LaserBeamAttackPattern(true, _leftBeamInitialRotation, 1));
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

    protected void FireLaserBeam()
    {
        GameObject beam1 = Instantiate(_beamPrefab, _beamOffset[0].transform.position, Quaternion.Euler(0,0,30f));
        beam1.GetComponent<Beam>().Initialize(_beamClockwise);
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

    IEnumerator LaserBeamAttackPattern(bool isClockWise,float initialRotation,  int beamOffsetIndex = 0)
    {
        while (true)
        {
            yield return new WaitForSeconds(_beamCooldown);
            GameObject beam1 = Instantiate(_beamPrefab, _beamOffset[beamOffsetIndex].transform.position, Quaternion.Euler(0, 0, initialRotation));
            beam1.GetComponent<Beam>().Initialize(isClockWise);
            yield return new WaitForSeconds(_beamTimeToLive);
            Destroy(beam1);


        }
        
       
    }
}
