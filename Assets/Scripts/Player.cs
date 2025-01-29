using System.Collections;
using System.Runtime.CompilerServices;
using TMPro.EditorUtilities;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 2.0f;
    [SerializeField]
    private float _speedWithBoost = 4.5f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleLaserPrefab;
    private Vector3 _laserOffset;
    [SerializeField]
    private Vector3 _laserOffsetPosition = new Vector3(0, 1.05f, 0);   


    [SerializeField]
    private float _fireRate = 0.5f;
    private float _fireDelayControl = -1f;

    [SerializeField]
    private int _lives = 3; 
    private SpawnManager _spawnManager;
    private UIManager _uiManager;
    private Vector3 _direction = new Vector3();
    private float verticalMove;
    private float horizontalMove;


    [Header("Scene Axis Limit")]
    [SerializeField]
    float _yLimitUp = 1.5f;
    [SerializeField]
    float _yLimitDown = -3.5f;    
    [SerializeField] 
    float _xLimit = 9.5f;

    [Header("PowerUps Enable")]
    [SerializeField]
    private bool _isTripleLaserEnable = false;
    [SerializeField]
    private bool _isSpeedBoostEnable = false;
    [SerializeField]
    private bool _isShieldEnable = false;

    [SerializeField]
    private GameObject _shieldVisualizer;    

    [SerializeField] private GameObject[] _fireOnEngine;

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

    // Update is called once per frame
    void Update()
    {        
        CalculateMovement();
        CheckScreenBoundaries();
        if(Input.GetKeyDown(KeyCode.Space) && Time.time > _fireDelayControl)
        {
            FireLaser();
        }        
        
    }

    void CalculateMovement()
    {        
        
        horizontalMove = Input.GetAxis("Horizontal");

        verticalMove = Input.GetAxis("Vertical");       

        _direction.Set(horizontalMove, verticalMove, 0);

        if(_isSpeedBoostEnable )
        {
            transform.Translate(_direction * _speedWithBoost * Time.deltaTime);
        }
        else
        {
            transform.Translate(_direction * _speed * Time.deltaTime);
        }

        

        
    }

    void CheckScreenBoundaries()
    {
        Vector3 currentPosition = transform.position;
        
        

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
        _audioSource.Play(); 
        
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


    public void EnableTripleLaser()
    {
        _isTripleLaserEnable = true;
        StartCoroutine(TripleLaserCooldownRoutine());
    }

    IEnumerator TripleLaserCooldownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isTripleLaserEnable = false;
    }

    public void EnableSpeedBoost()
    {
        _isSpeedBoostEnable = true;
        StartCoroutine(SpeedBoostCooldownRoutine());
    }

    IEnumerator SpeedBoostCooldownRoutine()
    {
        yield return new WaitForSeconds(7f);
        _isSpeedBoostEnable = false;
    }

    public void EnableShield()
    {
        _isShieldEnable = true;
        _shieldVisualizer.SetActive(true);
    }

    public void UpdatePlayerScore(int points)
    {
        _playerScore += points;
        _uiManager.UpdateScoreText();
    }

    
}



