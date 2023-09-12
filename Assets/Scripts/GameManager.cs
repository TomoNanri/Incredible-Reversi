using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public enum DiscColor { Black, White}
public enum GameState { Intro, InGame, GameOver}
public enum GameTurn { You, Me}

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    protected override bool dontDestroyOnLoad { get { return true; } }
    public Action OnInitializeGame;
    public Action OnStartGame;
    public DiscColor PlayerColor => _playerColor;
    public GameState GameState => _gameState;
    public GameTurn GameTurn => _gameTurn;

    private bool _useSpecialDisc = true;
    [SerializeField]
    private DiscColor _playerColor = DiscColor.Black;
    [SerializeField]
    private GameState _gameState;
    [SerializeField]
    private GameTurn _gameTurn = default;
    private GameObject _introPanel;
    private GameObject _gameOverPanel;
    private float _AILevel = 0;
    [SerializeField]
    bool _init = false;

    // Start is called before the first frame update
    void Start()
    {
        _gameState = GameState.Intro;
        _introPanel = GameObject.Find("IntroCanvas/Panel");
        //_introPanel.SetActive(true);
        _gameOverPanel = GameObject.Find("GameOverCanvas/Panel");
        //_gameOverPanel.SetActive(false);
        InitializeGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (_init)
        {
            _init = false;
            InitializeGame();
        }
    }
    public void InitializeGame()
    {
        if (OnInitializeGame!= null)
        {
            OnInitializeGame.Invoke();
        }
        _introPanel.SetActive(true);
        _gameOverPanel.SetActive(false);
        _gameState = GameState.Intro;
    }
    public void StartGame()
    {
        _useSpecialDisc = _introPanel.transform.Find("Panel (1)/Select Yes").GetComponent<Toggle>().isOn;
        _playerColor = _introPanel.transform.Find("Panel (2)/SelectBlack").GetComponent<Toggle>().isOn ? DiscColor.Black : DiscColor.White;
        _introPanel.SetActive(false);
        if (OnStartGame != null)
        {
            OnStartGame.Invoke();
        }
        _gameTurn = _playerColor == DiscColor.Black ? GameTurn.You : GameTurn.Me;
        _gameState = GameState.InGame;
    }
    public void Pass()
    {
        _gameTurn = _gameTurn == GameTurn.Me ? GameTurn.You : GameTurn.Me;
    }
}
