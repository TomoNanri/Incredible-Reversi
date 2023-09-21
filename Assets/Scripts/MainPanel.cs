using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainPanel : MonoBehaviour
{
    private TextMeshProUGUI _turnText;
    private TextMeshProUGUI _blackPoint;
    private TextMeshProUGUI _whitePoint;
    private GameBoard _gameBoard;
    private GameManager _gm;
    [SerializeField]
    private float _blinkingInterval = 2.0f;
    private float _blinkingTimer;
    private Color _currentColor;

    // Start is called before the first frame update
    void Start()
    {
        _turnText = transform.Find("TurnText").GetComponent<TextMeshProUGUI>();

        _blackPoint = transform.Find("BlackPoint").GetComponent<TextMeshProUGUI>();
        _whitePoint = transform.Find("WhitePoint").GetComponent<TextMeshProUGUI>();
        _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        _gm.OnInitializeGame += InitMainPanel;
        _gameBoard = GameObject.Find("GameBoard").GetComponent<GameBoard>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_gm.GameState != GameState.InGame)
        {
            return;
        }
            _blackPoint.SetText($"Black: {_gameBoard.BlackCount}");
            _whitePoint.SetText($"White: {_gameBoard.WhiteCount}");
        if (_gm.GameTurn == GameTurn.You)
        {
            _turnText.SetText($"Your Turn({_gm.PlayerColor})");
            _currentColor = _turnText.color;
            _currentColor.r = 1f;
            _currentColor.g = 1f;
            _currentColor.b = 1f;
            _turnText.color = _currentColor;
        }
        else
        {
            _turnText.SetText($"Thinking...");
            _currentColor.r = 0f;
            _currentColor.g = 0.1f;
            _currentColor.b = 1f;
            _turnText.color = _currentColor;
        }
            _blinkingTimer -= Time.deltaTime;
        if (_blinkingTimer <= 0)
        {
            _currentColor = _turnText.color;
            _blinkingTimer = _blinkingInterval;
            if (_currentColor.a < 1.0f)
            {
                _currentColor.a = 1.0f;
            }
            else
            {
                _currentColor.a = 0.2f;
            }
            _turnText.color = _currentColor;
        }
    }
    void InitMainPanel()
    {
        _turnText.SetText($"");
    }
}
