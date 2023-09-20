using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public enum DiscColor { Black, White}
public enum GameState { StartUp, Intro, InitialSetting, InGame, GameOver}
public enum GameTurn { You, Me}

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    protected override bool dontDestroyOnLoad { get { return true; } }
    public Action OnInitializeGame;
    public Action OnStartGame;
    public GameState GameState => _gameState;
    public GameTurn GameTurn => _gameTurn;
    public float AI_Level => _AILevel;
    public bool UseSpecialDisc => _useSpecialDisc;
    public bool IsOnBGM => _isOnBGM;
    public bool IsOnSE => _isOnSE;
    public float SoundLevel => _soundLevel;
    public DiscColor PlayerColor => _playerColor;

    private bool _useSpecialDisc = true;
    [SerializeField]
    private DiscColor _playerColor = DiscColor.Black;
    [SerializeField]
    private GameState _gameState;
    private bool _isGoButtonOn = false;
    private bool _isExitButtonOn = false;
    private bool _isInitialSettingCompleted = false;
    private bool _isGameOver = false;
    private bool _isOkButtonOn = false;
    [SerializeField]
    private GameTurn _gameTurn = default;
    private GameObject _introPanel;
    private GameObject _gameOverPanel;
    private float _AILevel = 0;
    private bool _isOnBGM = false;
    private bool _isOnSE = false;
    private float _soundLevel = 0;
    [SerializeField]
    private bool _isPassLastTurn = false;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        var _gameBoard = GameObject.Find("GameBoard").GetComponent<GameBoardCommon>();
        _gameBoard.OnCompleteSetting += DoAfterSetting;
        _gameState = GameState.StartUp;
        _introPanel = GameObject.Find("IntroCanvas/Panel");
        //_introPanel.SetActive(true);
        _gameOverPanel = GameObject.Find("GameOverCanvas/Panel");
        //_gameOverPanel.SetActive(false);
        _introPanel.GetComponent<IntroPanel>().OnChangeVolume += ChangeBGMVolume;
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = _soundLevel;
    }

    // Update is called once per frame
    void Update()
    {
        switch (_gameState)
        {
            case GameState.StartUp:     // 全オブジェクト起動後に初期化を実行
                InitializeGame();       // イントロ画面表示
                _gameState = GameState.Intro;
                break;

            case GameState.Intro:
                if (_isExitButtonOn)
                {
                    Application.Quit();
                }
                if (_isGoButtonOn)
                {
                    _isGoButtonOn = false;

                    // イントロ画面の設定値を読み込む
                    _useSpecialDisc = _introPanel.transform.Find("Panel (1)/Select Yes").GetComponent<Toggle>().isOn;
                    _playerColor = _introPanel.transform.Find("Panel (2)/SelectBlack").GetComponent<Toggle>().isOn ? DiscColor.Black : DiscColor.White;
                    _AILevel = _introPanel.transform.Find("Panel (3)/Slider").GetComponent<Slider>().value;
                    _isOnSE = _introPanel.transform.Find("Panel (5)/Select On").GetComponent<Toggle>().isOn;
                    _soundLevel = _introPanel.transform.Find("Panel (6)/Slider").GetComponent<Slider>().value;

                    // BGMを消す
                    audioSource.Stop();

                    // イントロ画面を消す
                    _introPanel.SetActive(false);

                    // 各オブジェクトへのゲーム開始通知（AIPlayer/GameBoard）
                    if (OnStartGame != null)
                    {
                        OnStartGame.Invoke();
                    }

                    // ターン設定
                    _gameTurn = _playerColor == DiscColor.Black ? GameTurn.You : GameTurn.Me;
                    Debug.Log($"[GameManager]_gameTurn={_gameTurn}");
                    _gameState = GameState.InitialSetting;
                }
                break;

            case GameState.InitialSetting:
                if (_isInitialSettingCompleted)
                {
                    _isInitialSettingCompleted = false;
                    _gameState = GameState.InGame;
                }
                break;

            case GameState.InGame:
                if (_isGameOver)
                {
                    // Game Over 画面表示
                    if (_isOnBGM)
                    {
                        audioSource.Play();
                    }
                    GameOverProc();
                    _gameState = GameState.GameOver;
                }

                break;

            case GameState.GameOver:
                if (_isOkButtonOn)
                {
                    // Intro へもどる処理
                    InitializeGame();
                    _gameState = GameState.Intro;
                }
                break;
        }
    }
    private void ChangeBGMVolume(float value)
    {
        GetComponent<AudioSource>().volume = value;
        _soundLevel = value;
    }

    private void DoAfterSetting()
    {
        _isInitialSettingCompleted = true;
    }
    private void InitializeGame()
    {
        if (OnInitializeGame!= null)
        {
            OnInitializeGame.Invoke();
        }
        _introPanel.SetActive(true);
        if (GameObject.Find("IntroCanvas/Panel/Panel (4)/Select On").GetComponent<Toggle>().isOn) 
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
        _gameOverPanel.SetActive(false);
    }
    private void GameOverProc()
    {
        _gameOverPanel.SetActive(true);
    }
    public void ToggleBGM()
    {
        var BGM_Panel = GameObject.Find("IntroCanvas/Panel/Panel (4)/Select On");
        if(BGM_Panel == null)
        {
            Debug.Log("BGM Error" + BGM_Panel);
        }
        var BGM_Toggle = BGM_Panel.GetComponent<Toggle>();
        if(BGM_Toggle == null)
        {
            Debug.Log("BGM Error" + BGM_Toggle);
        }
        if (BGM_Toggle.isOn)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }
    public void OnStartGameButton()
    {
        _isGoButtonOn = true;
    }
    public void OnExitButton()
    {
        _isExitButtonOn = true;
    }
    public void OnOkButton()
    {
        _isOkButtonOn = true;
    }
    public void TurnEnd(bool isPass)
    {
        if (!isPass || !_isPassLastTurn)
        {
            // 続行
            _isPassLastTurn = isPass;
            _gameTurn = _gameTurn == GameTurn.Me ? GameTurn.You : GameTurn.Me;
        }
        else
        {
            _isGameOver = true;
        }
    }
}
