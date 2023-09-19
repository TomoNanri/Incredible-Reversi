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
    public float AI_Level => _AILevel;
    public bool UseSpecialDisc => _useSpecialDisc;
    public bool IsOnBGM => _isOnBGM;
    public bool IsOnSE => _isOnSE;
    public float SoundLevel => _soundLevel;

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
    private bool _isOnBGM = false;
    private bool _isOnSE = false;
    private float _soundLevel = 0;
    [SerializeField]
    bool _init = false;

    // Start is called before the first frame update
    void Start()
    {
        var _gameBoard = GameObject.Find("GameBoard").GetComponent<GameBoardCommon>();
        _gameBoard.OnCompleteSetting += DoAfterSetting;
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
    private void DoAfterSetting()
    {
        _gameState = GameState.InGame;
    }
    private void InitializeGame()
    {
        if (OnInitializeGame!= null)
        {
            OnInitializeGame.Invoke();
        }
        _introPanel.SetActive(true);
        _gameOverPanel.SetActive(false);
        _gameState = GameState.Intro;
    }
    public void OnStartGameButton()
    {
        // イントロ画面の設定値を読み込む
        _useSpecialDisc = _introPanel.transform.Find("Panel (1)/Select Yes").GetComponent<Toggle>().isOn;
        _playerColor = _introPanel.transform.Find("Panel (2)/SelectBlack").GetComponent<Toggle>().isOn ? DiscColor.Black : DiscColor.White;
        _AILevel = _introPanel.transform.Find("Panel (3)/Slider").GetComponent<Slider>().value;
        _isOnBGM = _introPanel.transform.Find("Panel (4)/Select On").GetComponent<Toggle>().isOn;
        _isOnSE = _introPanel.transform.Find("Panel (5)/Select On").GetComponent<Toggle>().isOn;
        _soundLevel = _introPanel.transform.Find("Panel (6)/Slider").GetComponent<Slider>().value;

        // イントロ画面を消す
        _introPanel.SetActive(false);

        // 各オブジェクトの起動処理
        if (OnStartGame != null)
        {
            OnStartGame.Invoke();
        }

        // ターン設定
        _gameTurn = _playerColor == DiscColor.Black ? GameTurn.You : GameTurn.Me;
        Debug.Log($"[GameManager]_gameTurn={_gameTurn}");
    }
    public void TurnEnd()
    {
        _gameTurn = _gameTurn == GameTurn.Me ? GameTurn.You : GameTurn.Me;
    }
}
