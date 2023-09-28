using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
public enum DiscColor { Black, White}
public enum GameState { StartUp, Intro, InitialSetting, InGame, Judging, TurenInterval, GameOver}
public enum GameTurn { You, Me}

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    protected override bool dontDestroyOnLoad { get { return true; } }
    public Action OnInitializeGame;
    public Action OnStartGame;
    public Action<BadgeEventArgs> OnBadgeEvent;

    public GameState GameState => _gameState;
    public GameTurn GameTurn => _gameTurn;
    public float AI_Level => _AILevel;
    public bool UseSpecialDisc => _useSpecialDisc;
    public bool IsOnBGM => _isOnBGM;
    public bool IsOnSE => _isOnSE;
    public float SoundLevel => _soundLevel;
    public DiscColor PlayerColor => _playerColor;
    private GameBoardCommon _gameBoardCommon;

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
    [SerializeField]
    private float _turnInterval = 0.5f;
    private GameObject _introPanel;
    private GameObject _gameOverPanel;
    private float _AILevel = 0;
    [SerializeField]
    private bool _isOnBGM = false;
    private bool _isOnSE = false;
    private float _soundLevel = 0.2f;
    [SerializeField]
    private bool _isPassLastTurn = false;
    private bool _isPass = false;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        _gameBoardCommon = GameObject.Find("GameBoard").GetComponent<GameBoardCommon>();
        _gameBoardCommon.OnCompleteSetting += DoAfterSetting;
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
                    _isInitialSettingCompleted = false;

                    // player の選択に従ってターン設定する
                    _gameTurn = _playerColor == DiscColor.Black ? GameTurn.You : GameTurn.Me;
                    Debug.Log($"[{this.name}] Start Initialize! _gameTurn={_gameTurn}");
                    _gameState = GameState.InitialSetting;

                    // 各オブジェクトへのゲーム開始通知（AIPlayer/GameBoard）
                    if (OnStartGame != null)
                    {
                        OnStartGame.Invoke();
                    }
                }
                break;

            case GameState.InitialSetting:
                if (_isInitialSettingCompleted)
                {
                    Debug.Log($"[{this.name}] Initialize complete! _gameTurn={_gameTurn}");

                    _isInitialSettingCompleted = false;
                    _gameState = GameState.InGame;
                }
                break;

            case GameState.TurenInterval:
                break;

            case GameState.InGame:
                if (_isGameOver)
                {
                    OnBadgeEvent.Invoke(new BadgeEventArgs(BadgeEventType.GAMEOVER));
                    _isGameOver = false;
                    // Game Over 画面を表示し、BGMを戻す。
                    if (_isOnBGM)
                    {
                        audioSource.Play();
                    }
                    GameOverProc();
                    _gameState = GameState.GameOver;
                }
                break;

            case GameState.Judging:
                if (_gameBoardCommon.IsExisting(DiscColor.Black) || _gameBoardCommon.IsExisting(DiscColor.White))
                {
                    if (_isPass && _isPassLastTurn)
                    {
                        _isGameOver = true;
                    }
                    else
                    {
                        // 続行
                        _isPassLastTurn = _isPass;
                        _gameTurn = _gameTurn == GameTurn.Me ? GameTurn.You : GameTurn.Me;
                    }
                }
                else
                {
                    _isGameOver = true;
                }


                _gameState = GameState.TurenInterval;
                StartCoroutine(StartNextTurn(_turnInterval));
                break;

            case GameState.GameOver:
                if (_isOkButtonOn)
                {
                    _isOkButtonOn = false;
                    // Intro へもどる処理
                    InitializeGame();
                    _gameState = GameState.StartUp;
                }
                break;
        }
    }

    /// <summary>
    /// ターン交代時の間を取る
    /// </summary>
    /// <param name="sec"></param>
    /// <returns></returns>
    private IEnumerator StartNextTurn(float sec)
    {
        Debug.Log($"[{this.name}]Waiting for Next turn Start... Current State = {_gameState}]");
        yield return new WaitForSeconds(sec);
        _gameState = GameState.InGame;
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
            _isOnBGM = true;
            audioSource.Play();
        }
        else
        {
            _isOnBGM = false;
            audioSource.Stop();
        }
        _isGameOver = false;
        _isPass = false;
        _isPassLastTurn = false;
        _gameOverPanel.SetActive(false);
    }
    private void GameOverProc()
    {
        var _gameResultText = _gameOverPanel.transform.Find("Result Text").GetComponent<TextMeshProUGUI>();
        if (PlayerColor == DiscColor.Black)
        {
            if(_gameBoardCommon.BlackCount > _gameBoardCommon.WhiteCount)
            {
                _gameResultText.SetText("You win!!!");
            }
            else
            {
                _gameResultText.SetText("You lose!!!");
            }
        }
        else
        {
            if (_gameBoardCommon.BlackCount > _gameBoardCommon.WhiteCount)
            {
                _gameResultText.SetText("You lose!!!");
            }
            else
            {
                _gameResultText.SetText("You win!!!");
            }
        }
        if (_isOnBGM)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
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
        _isPass = isPass;
        Debug.Log($"[{this.name}]TurnEnd is called! Current State = {_gameState}]");
        _gameState = GameState.Judging;
    }
}
