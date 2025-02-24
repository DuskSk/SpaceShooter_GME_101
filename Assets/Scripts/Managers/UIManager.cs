using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("TMP Text References")]
    [SerializeField] private TMP_Text _scoreText;    
    [SerializeField] private TMP_Text _gameOverText;
    [SerializeField] private TMP_Text _reloadText;
    [SerializeField] private TMP_Text _ammoText;
    private int _playerScore = 0;

    [Header("Player UI")]
    [SerializeField] private Player _player;
    [SerializeField] private Sprite[] _livesSpritesList;
    [SerializeField] private Image _livesImage;
    private GameManager _gameManager;

    [Header("Slider Section")]
    [SerializeField] private Slider _chargeSlider;
    [SerializeField] private float _chargeSpeed, _depleteChargeSpeed;
    private bool _isThrusterCharged = false;
    private float _maxChargeValue = 100;

    [Header("Boss UI")]    
    [SerializeField] private Slider _bossHealthSlider;
    private float _bossMaxHealth;

    public bool IsThrusterCharged
    {
        get { return _isThrusterCharged; }
    }

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        _scoreText.text = $"Score: {0}";        
        _gameOverText.gameObject.SetActive(false);
        _reloadText.gameObject.SetActive(false); 
        _bossHealthSlider.gameObject.SetActive(false);
        _maxChargeValue = _chargeSlider.maxValue;      

        


    }

    private void Update()
    {
        if (_player.IsThrusterEnable)
        {
            DepleteCharge();
        }
        else 
        {
            UpdateChargeValue();
        }        
        
    }

    private void UpdateChargeValue()
    {
        
        _chargeSlider.value += _chargeSpeed * Time.deltaTime;
        if(_chargeSlider.value >= _maxChargeValue)
        {
            _chargeSlider.value = _maxChargeValue;
            _isThrusterCharged = true;
        }

    }

    private void DepleteCharge()
    {
        
        _chargeSlider.value -= _depleteChargeSpeed * Time.deltaTime;        
        if (_chargeSlider.value <= 0)
        {
            _isThrusterCharged = false;
        }
    }    


    public void UpdateScoreText(int playerScore)
    {
        _playerScore += playerScore;
        _scoreText.text = $"Score: {_playerScore}";
    }

    public void UpdateAmmoText(int ammoAmount)
    {
        _ammoText.text = $"Ammo:{ammoAmount}";
    }

    public void UpdateLivesImage(int currentPlayerLives)
    {
        if(currentPlayerLives >= 0)
        {
           _livesImage.sprite = _livesSpritesList[currentPlayerLives];
        }        

        if (currentPlayerLives == 0) 
        {
            StartGameOverSequence();                       
            
        }

    }

   
    public void UpdateBossHealthSlider(float currentHealth)
    {
        _bossHealthSlider.value = currentHealth;
        if (currentHealth <= 0)
        {
            StartWinSequence();
        }
    }

    public void SetBossHealth(float hpValue)
    {
        _bossHealthSlider.gameObject.SetActive(true);
        _bossHealthSlider.maxValue = hpValue;
    }

    private void StartGameOverSequence()
    {
        StartCoroutine(GameOverFlickerRoutine());
        _reloadText.gameObject.SetActive(true);
        
    }

    //placeholder for final wave WIN sequence
    public void StartWinSequence()
    {
        _gameOverText.text = "YOU WIN!!";
        StartGameOverSequence();
    }


    private IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.7f);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.7f);            
        }
    }

   
}
