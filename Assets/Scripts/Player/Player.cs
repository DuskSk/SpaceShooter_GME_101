using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Speed")]
    [SerializeField] private float _baseSpeed = 3.0f;
    [SerializeField] private float _speedWithBoost = 7.0f;
    [SerializeField] private float _speedWithThruster = 5.0f;

    
    private Vector3 _laserOffset;
    [SerializeField] private Vector3 _laserOffsetPosition = new Vector3(0, 1.05f, 0);   


    [SerializeField]
    private float _fireRate = 0.5f;
    private float _fireDelayControl = -1f;

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

    [Header("Attached GameObjects")]
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject _tripleLaserPrefab;
    [SerializeField] private GameObject _shieldVisualizer; 
    [SerializeField] private GameObject[] _fireOnEngine;
    [SerializeField] private GameObject _thrusterVisualizer;
    

    [SerializeField] private AudioClip _laserAudioClip;

    private AudioManager _audioManager;
    private AudioSource _audioSource;
    private int _playerScore;



    public int PlayerScore
    {
        get { return _playerScore; }        
    }



    void Start()
    {        
        _spawnManager = GameObject.FindWithTag("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioManager = GameObject.FindWithTag("Audio_Manager").GetComponent<AudioManager>();
        _audioSource = GetComponent<AudioSource>();


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
        if (Input.GetKey(KeyCode.LeftShift))
        {
            EnableThruster(true);            
            
        }else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            EnableThruster(false);
        }
        
        CalculateMovement();

        CheckScreenBoundaries();

        if(Input.GetKeyDown(KeyCode.Space) && Time.time > _fireDelayControl)
        {
            FireLaser();
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
            Debug.Log("none active");
        }

        

        
    }

    void CheckScreenBoundaries()
    {

        Vector3 currentPosition = transform.position;


        //Rename to transform.position
        if (currentPosition.y > _yLimitUp)
        {

            transform.position = new Vector3(currentPosition.x, _yLimitUp, 0);
        }
        else if (currentPosition.y < _yLimitDown)
        {

            transform.position = new Vector3(currentPosition.x, _yLimitDown, 0);
        }
        

        if (currentPosition.x >= _xLimit)
        {

            transform.position = new Vector3(-_xLimit, currentPosition.y, 0);
        }
        else if (currentPosition.x <= -_xLimit)
        {
            transform.position = new Vector3(_xLimit, currentPosition.y, 0);
        }
    }

    void FireLaser()
    {
        _fireDelayControl = Time.time + _fireRate;
        _laserOffset = transform.position + _laserOffsetPosition;

        if (_isTripleLaserEnable)
        {
            Instantiate(_tripleLaserPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, _laserOffset, Quaternion.identity);
        }        
        //_audioSource.Play(); 
        
    }

    public void DamagePlayer()
    {
        if (_isShieldEnable)
        {
            _isShieldEnable = false;
            _shieldVisualizer.SetActive(false);
            return;
        }

        _lives--;
        ActivateEngineFailureAnimation(_lives);
        _uiManager.UpdateLivesImage(_lives);

        if (_lives < 1)
        {
            _audioManager.PlayExplosionAudio();
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    private void ActivateEngineFailureAnimation(int lives)
    {
        int randomEngine = Random.Range(0, 2);

        switch (lives)
        {
            case 2:
                _fireOnEngine[randomEngine].SetActive(true);
                break;
            case 1:
                if (_fireOnEngine[0].activeSelf)
                {
                    _fireOnEngine[1].SetActive(true);
                }
                else
                {
                    _fireOnEngine[0].SetActive(true);
                }
                break;

        }
    }
    private void EnableThruster(bool enable)
    {
        _isThrusterEnable= enable;
        _thrusterVisualizer.SetActive(enable);

    }
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

    public void EnableShield()
    {
        _isShieldEnable = true;
        _shieldVisualizer.SetActive(true);
    }

    public void UpdatePlayerScore(int points)
    {
        _playerScore += points;
        _uiManager.UpdateScoreText(_playerScore);
    }

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

    

    
}



