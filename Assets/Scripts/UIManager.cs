using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private Player _player;
    [SerializeField] private Sprite[] _livesSpritesList;
    [SerializeField] private Image _livesImage;
    [SerializeField] private TMP_Text _gameOverText;
    [SerializeField] private TMP_Text _reloadText;    
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




    public void UpdateScoreText()
    {
        _scoreText.text = $"Score: {_player.PlayerScore}";
    }

    public void UpdateLivesImage(int currentPlayerLives)
    {
        _livesImage.sprite = _livesSpritesList[currentPlayerLives];

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
