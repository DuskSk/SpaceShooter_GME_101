using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Variables
    [Header("Player Speed")]
    [SerializeField] private float _baseSpeed = 3.0f;
    [SerializeField] private float _speedWithBoost = 7.0f;
    [SerializeField] private float _speedWithThruster = 5.0f;

    [Header("Player Projectile Speeds")]
    [SerializeField] private float _laserSpeed = 8.0f;
    [SerializeField] private float _homingSpeed = 5.0f;
    [SerializeField] private float _aoeBombSpeed = 5.0f;

    [SerializeField] private Vector3 _laserOffsetPosition = new Vector3(0, 1.05f, 0);
    private Vector3 _weaponOffset;


    [SerializeField] private float _fireRate = 0.5f;
    private float _fireDelayControl = -1f;

    [SerializeField] private int _maxAmmoAmount = 15;
    private int _currentAmmo;

    [SerializeField] private int _lives = 3; 
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private Vector3 _direction = new Vector3();
    private float _verticalMove;
    private float _horizontalMove;

    

    [Header("Scene Axis Limit")]
    [SerializeField] float _yLimitUp = 1.5f;
    [SerializeField] float _yLimitDown = -3.5f;    
    [SerializeField] float _xLimit = 9.5f;

    [Header("PowerUps Enable")]
    [SerializeField] private bool _isTripleLaserEnable = false;
    [SerializeField] private bool _isSpeedBoostEnable = false;
    [SerializeField] private bool _isShieldEnable = false;
    [SerializeField] private bool _isThrusterEnable = false;
    [SerializeField] private bool _isAoeBombEnable = false;
    [SerializeField] private bool _isHomingShootEnable = false;

    [Header("Attached GameObjects")]
    [SerializeField] private Laser _laserPrefab;
    [SerializeField] private GameObject _tripleLaserPrefab;
    [SerializeField] private AoeBomb _aoeBombPrefab;
    [SerializeField] private HomingShoot _homingShootPrefab;
    [SerializeField] private GameObject _shieldVisualizer; 
    [SerializeField] private GameObject[] _fireOnEngine;
    [SerializeField] private GameObject _thrusterVisualizer;
    [SerializeField] private AudioClip _laserAudioClip;

    [Header("PowerUp Attraction")]
    [SerializeField] private float _attractionRadius = 3.5f;
    [SerializeField] private float _attractionSpeed = 3f;
    private Collider2D[] _powerupColliderList;
    


    private AudioManager _audioManager;
    private AudioSource _audioSource;
    private int _playerScore;

    private SpriteRenderer _shieldSpriteRenderer;
    private Color _shieldColor = Color.white;
    private int _shieldLives = 3;

    private CameraShake _cameraShake;
    #endregion
    public bool IsThrusterEnable
    {
        get { return _isThrusterEnable; }    }

    



    void Start()
    {        
        _spawnManager = GameObject.FindWithTag("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioManager = GameObject.FindWithTag("Audio_Manager").GetComponent<AudioManager>();
        _audioSource = GetComponent<AudioSource>();
        _shieldSpriteRenderer = _shieldVisualizer.GetComponent<SpriteRenderer>();
        _cameraShake = GameObject.FindWithTag("MainCamera").GetComponent<CameraShake>();
        
        _currentAmmo = _maxAmmoAmount;

        
              
        if (_spawnManager == null)
        {
            Debug.Log("Spawn Manager component is NULL");
        }

        if (_uiManager == null)
        {
            Debug.Log("UI manager component is NULL");
        }

        if( _audioSource == null)
        {
            Debug.LogError("Audio Source on PLayer is NULL");
        }
        else
        {
            _audioSource.clip = _laserAudioClip;
        }
    }

    
    void Update()
    {        
        if (Input.GetKey(KeyCode.LeftShift) && _uiManager.IsThrusterCharged)
        {
            EnableThruster(true);
        }
        else
        {
            EnableThruster(false);
        }

        if (Input.GetKey(KeyCode.C))
        {
            AttractNearbyPowerUp();
        }
       
        CalculateMovement();

        CheckScreenBoundaries();

        if(Input.GetKeyDown(KeyCode.Space) && Time.time > _fireDelayControl)
        {
            //TODO
            //create a enum to handle current active weapon
            //only one weapon can be active at a time
            if (_isAoeBombEnable)
            {
                FireAoeBomb();
            }
            else if (_isHomingShootEnable)
            {
                FireHomingShoot();
            }
            else if (_currentAmmo > 0)
            {
                FireLaser();
            }
                        
        }        
        
    }

    void CalculateMovement()
    {        
        
        _horizontalMove = Input.GetAxis("Horizontal");

        _verticalMove = Input.GetAxis("Vertical");       

        _direction.Set(_horizontalMove, _verticalMove, 0);

        if(_isSpeedBoostEnable)
        {
            transform.Translate(_direction * _speedWithBoost * Time.deltaTime);
            Debug.Log("speedboost active");
        }
        else if(_isThrusterEnable)
        {
            transform.Translate(_direction * _speedWithThruster * Time.deltaTime);
            Debug.Log("thruster active");
        }
        else
        {
            transform.Translate(_direction * _baseSpeed * Time.deltaTime);            
        }

        

        
    }

    void CheckScreenBoundaries()
    {

               
        if (transform.position.y > _yLimitUp)
        {

            transform.position = new Vector3(transform.position.x, _yLimitUp, 0);
        }
        else if (transform.position.y < _yLimitDown)
        {

            transform.position = new Vector3(transform.position.x, _yLimitDown, 0);
        }
        

        if (transform.position.x >= _xLimit)
        {

            transform.position = new Vector3(-_xLimit, transform.position.y, 0);
        }
        else if (transform.position.x <= -_xLimit)
        {
            transform.position = new Vector3(_xLimit, transform.position.y, 0);
        }
    }

    private void FireLaser()
    {
        
        _fireDelayControl = Time.time + _fireRate;
        _weaponOffset = transform.position + _laserOffsetPosition;

        _currentAmmo--;
        _uiManager.UpdateAmmoText(_currentAmmo);
       
        if (_isTripleLaserEnable)
        {
             GameObject tripleLaser = Instantiate(_tripleLaserPrefab, transform.position, Quaternion.identity);
            foreach (Transform child in tripleLaser.transform)
            {
                Laser laser = child.GetComponent<Laser>();
                if (laser != null)
                {
                    laser.Initialize(Vector3.up, _laserSpeed,false);
                }
            }
        }
        else
        {
            Laser laserObject = Instantiate(_laserPrefab, _weaponOffset, Quaternion.identity);
            laserObject.Initialize(Vector3.up, _laserSpeed, false);
        }        
        
        
    }
    
    private void FireAoeBomb()
    {
        _fireDelayControl = Time.time + _fireRate;
        _weaponOffset = transform.position + _laserOffsetPosition;

        AoeBomb aoebombObject = Instantiate(_aoeBombPrefab, _weaponOffset, Quaternion.identity);        
        aoebombObject.Initialize(Vector3.up, _aoeBombSpeed, false);
    }

    private void FireHomingShoot()
    {
        _fireDelayControl = Time.time + _fireRate;
        _weaponOffset = transform.position + _laserOffsetPosition;

        HomingShoot homingObject = Instantiate(_homingShootPrefab, _weaponOffset, Quaternion.identity);
        homingObject.Initialize(Vector3.up, _homingSpeed, false);
    }    

    public void DamagePlayer()
    {
        
        if (_isShieldEnable)
        {
            DamageShield();
            return;

        }

        _lives--;
        UpdateEngineFailureAnimation(_lives);
        _uiManager.UpdateLivesImage(_lives);
        _cameraShake.StartCameraShake();

        if (_lives < 1)
        {
            _audioManager.PlayExplosionAudio();
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    private void DamageShield()
    {
        _shieldLives--;

        switch (_shieldLives)
        {
            case 2:
                _shieldColor.a = 0.6f;
                _shieldSpriteRenderer.color = _shieldColor;
                break;
            case 1:
                _shieldColor.a = 0.3f;
                _shieldSpriteRenderer.color = _shieldColor;
                break;
            case 0:
                _isShieldEnable = false;
                _shieldVisualizer.SetActive(false);
                break;

        }
    }

    //Update the animation randomly based on current player lives
    private void UpdateEngineFailureAnimation(int lives)
    {
        //int randomEngine = Random.Range(0, 2);

        switch (lives)
        {
            case 2:
                _fireOnEngine[0].SetActive(true);
                if (_fireOnEngine[1].activeSelf)
                {
                    _fireOnEngine[1].SetActive(false);
                } 
                break;
            case 1:
                _fireOnEngine[1].SetActive(true);
                break;
            default:
                foreach(GameObject gameObject in _fireOnEngine)
                {
                    gameObject.SetActive(false);
                }
                break;


        }
    }
    private void EnableThruster(bool enable)
    {
        _isThrusterEnable= enable;
        _thrusterVisualizer.SetActive(enable);

    }

    private void AttractNearbyPowerUp()
    {
        _powerupColliderList = Physics2D.OverlapCircleAll(transform.position, _attractionRadius);
        //Debug.DrawLine(transform.position, transform.position + Vector3.up * _attractionRadius, Color.red);
        foreach (Collider2D collider in _powerupColliderList)
        {
            if (collider.CompareTag("Powerup"))
            {                
                collider.transform.position = Vector3.MoveTowards(collider.transform.position, transform.position, _attractionSpeed * Time.deltaTime);
            }
        }
    }
    #region PowerUp effects

    public void EnableSpeedBoost()
    {
        _isSpeedBoostEnable = true;
        StartCoroutine(SpeedBoostCooldownRoutine());
    }

    public void EnableTripleLaser()
    {
        _isTripleLaserEnable = true;
        StartCoroutine(TripleLaserCooldownRoutine());
    }    

    public void EnableAoeBomb()
    {
        _isAoeBombEnable = true;
        StartCoroutine(AoeBombCooldownRoutine());
    }

    public void EnableHomingShoot()
    {
        _isHomingShootEnable = true;
        StartCoroutine(HomingShootCooldownRoutine());
    }

    public void EnableShield()
    {
        
        _isShieldEnable = true;
        _shieldVisualizer.SetActive(true);
        if (_shieldLives < 3)
        {
            _shieldLives = 3;
            _shieldSpriteRenderer.color = Color.white;
        }

    }

    public void RefillAmmo()
    {
        _currentAmmo = _maxAmmoAmount;
        _uiManager.UpdateAmmoText(_currentAmmo);
    }

    public void RegenerateLife()
    {
        if (_lives >= 3)
        {
            return;
        }
        _lives++;
        _uiManager.UpdateLivesImage(_lives);
        UpdateEngineFailureAnimation(_lives);

    }

    #endregion

    #region Debuff Effects

    public void ApplySlow()
    {
        StartCoroutine(SlowDebuffCooldownRoutine()); 
    }

    #endregion
    public void UpdatePlayerScore(int points)
    {
        _playerScore += points;
        _uiManager.UpdateScoreText(_playerScore);
    }

    #region Coroutines
    IEnumerator SpeedBoostCooldownRoutine()
    {
        yield return new WaitForSeconds(7f);
        _isSpeedBoostEnable = false;
    }

    IEnumerator TripleLaserCooldownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isTripleLaserEnable = false;
    }

    IEnumerator AoeBombCooldownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isAoeBombEnable = false;
    }

    IEnumerator HomingShootCooldownRoutine()
    {
        yield return new WaitForSeconds(3f);
        _isHomingShootEnable = false;
    }

    IEnumerator SlowDebuffCooldownRoutine()
    {
         float currentSpeed = _baseSpeed;
        _baseSpeed *= 0.2f;
        yield return new WaitForSeconds(7f);
        _baseSpeed = currentSpeed;
    }
    #endregion
}



