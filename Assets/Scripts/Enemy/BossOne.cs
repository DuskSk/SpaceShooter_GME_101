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
    [SerializeField] protected float _fireRate = 0.5f; // 🔹 Tempo entre cada tiro
    [SerializeField] protected float _fireRatePhase2 = 0.3f;
    [SerializeField] protected float _fireRatePhase3 = 0.1f;

    [Header("Beam Attack Configuration")]
    [SerializeField] protected float _beamTimeToLive = 5f;
    [SerializeField] protected float _beamCooldown = 5f;
    [SerializeField] protected bool _beamClockwise = false;     
    [SerializeField] protected float _rightBeamInitialRotation = 30f;
    [SerializeField] protected float _leftBeamInitialRotation = 30f;

    [SerializeField]protected GameObject[] _beamOffset;

    private float _bossMaxHealth;

    void Start()
    {
        UIManager.Instance.SetBossHealth(_bossHealth);
        StartCoroutine(SpiralAttackPattern());  
        InvokeRepeating(nameof(CheckBossPhase), 0, 0.5f);
        _bossMaxHealth = _bossHealth;
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

    protected override void CheckBossPhase()
    {
        
        Debug.Log($"Checking Boss Phase | Current: {_bossPhase} | Current health: {_bossHealth}");
        if (_bossHealth <= (_bossMaxHealth * 0.6f) && _bossPhase == BossPhase.Phase1)
        {
            ActivatePhase(BossPhase.Phase2);
            StartCoroutine(LaserBeamAttackPattern(false, _rightBeamInitialRotation, 0));
        }
        else if(_bossHealth <= (_bossMaxHealth * 0.3f) && _bossPhase == BossPhase.Phase2)
        {
            ActivatePhase(BossPhase.Phase3);
            StartCoroutine(LaserBeamAttackPattern(true, _leftBeamInitialRotation, 1));
        }
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
        
    }

    protected void FireLaserBeam()
    {
        GameObject beam1 = Instantiate(_beamPrefab, _beamOffset[0].transform.position, Quaternion.Euler(0,0,30f));
        beam1.GetComponent<Beam>().Initialize(_beamClockwise);
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
                case BossPhase.Phase3:
                    yield return new WaitForSeconds(_fireRatePhase3);
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
