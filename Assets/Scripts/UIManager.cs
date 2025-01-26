using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    [SerializeField] private Player _player;
    [SerializeField] private Sprite[] _livesSpritesList;
    [SerializeField] private Image _livesImage;
    // Start is called before the first frame update
    void Start()
    {
        _scoreText.text = $"Score: {0}";
    }   

    public void UpdateScoreText()
    {
        _scoreText.text = $"Score: {_player.PlayerScore}";
    }

    public void UpdateLivesImage(int currentPlayerLives)
    {
        _livesImage.sprite = _livesSpritesList[currentPlayerLives];
    }
}
