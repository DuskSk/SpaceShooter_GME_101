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

    [SerializeField] private Player _player;
    [SerializeField] private Sprite[] _livesSpritesList;
    [SerializeField] private Image _livesImage;
    private GameManager _gameManager;

    [Header("Slider Section")]
    [SerializeField] private Slider _chargeSlider;
    [SerializeField] private float _chargeSpeed, _depleteChargeSpeed;
    private bool _isThrusterCharged = false;
    private float _maxChargeValue = 100;
    
        
    public bool IsThrusterCharged
    {
        get { return _isThrusterCharged; }
    }

    void Start()
    {
        _scoreText.text = $"Score: {0}";
        _ammoText.text = $"Ammo: {15}";
        _gameOverText.gameObject.SetActive(false);
        _reloadText.gameObject.SetActive(false);
        _gameManager = GameObject.FindWithTag("Game_Manager").GetComponent<GameManager>();        
        _maxChargeValue = _chargeSlider.maxValue;

        if (_gameManager == null)
        {
            Debug.Log("Game Manager is NULL");
        }


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
        _scoreText.text = $"Score: {playerScore}";
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

    private void StartGameOverSequence()
    {
        StartCoroutine(GameOverFlickerRoutine());
        _reloadText.gameObject.SetActive(true);
        _gameManager.GameOver();
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
