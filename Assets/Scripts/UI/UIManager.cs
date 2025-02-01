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

    [SerializeField] private Player _player;
    [SerializeField] private Sprite[] _livesSpritesList;
    [SerializeField] private Image _livesImage;
    private GameManager _gameManager;
    

    void Start()
    {
        _scoreText.text = $"Score: {0}";
        _gameOverText.gameObject.SetActive(false);
        _reloadText.gameObject.SetActive(false);
        _gameManager = GameObject.FindWithTag("Game_Manager").GetComponent<GameManager>();

        if (_gameManager == null)
        {
            Debug.Log("Game Manager is NULL");
        }


    }




    public void UpdateScoreText(int playerScore)
    {
        _scoreText.text = $"Score: {playerScore}";
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
