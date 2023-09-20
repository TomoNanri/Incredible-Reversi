using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AIPlayer : MonoBehaviour
{
    public enum AIState { Waiting, StartThink, Thinking, ThinkComplete, DiscSetting, Passing }
    private AIState _aiState;

    [SerializeField]
    private DiscColor _myColor,_yourColor;
    private GameManager _gm;
    private GameBoard _gameBoard;
    private AIStateController _stateController;
    private GameObject _passMessage;
    private ThinkAI _thinkAI;

    [SerializeField]
    private float _messageDisplyTime = 2.0f;
    [SerializeField]
    private float _turnEndDelay = 2.0f;
    [SerializeField]
    private int _ai_Level;
    private float _timer;

    // Start is called before the first frame update
    void Start()
    {
        _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        _gm.OnStartGame += StartGameHandler;
        //_stateController = transform.Find("State").GetComponent<AIStateController>();
        _thinkAI = GetComponent<ThinkAI>();
        _thinkAI.ThinkEnd += ThinkEndHandler;
        _aiState = AIState.Waiting;

        //_stateController.Initialize((int)AIStateController.StateType.Wait);
        _gameBoard = GameObject.Find("GameBoard").GetComponent<GameBoard>();
        //_vertualGameBoard = GameObject.Find("VertualBoard").GetComponent<VertualGameBoard>();
        _passMessage = transform.Find("PassCanvas").gameObject;
        _passMessage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch(_aiState)
        {
            case AIState.Waiting:
                if (_gm.GameState != GameState.InGame)
                {
                    return;
                }
                if (_gm.GameTurn == GameTurn.You)
                {
                    return;
                }
                _aiState = AIState.StartThink;
                break;

            case AIState.StartThink:
                // 先読みを開始する
                _aiState = AIState.Thinking;
                var result = _thinkAI.StartThinking(_ai_Level);
                break;

            case AIState.Thinking:
                // 先読みを継続する(イベントはハンドラで受信する)
                break;

            case AIState.ThinkComplete:
                // 決まった手（passを含む）を打つ
                if (_thinkAI.PredictPos > -1)
                {
                    // パスしない
                    int row = _thinkAI.PredictPos / _gameBoard.BoardSize;
                    int col = _thinkAI.PredictPos % _gameBoard.BoardSize;
                    _gameBoard.SetDisc(_thinkAI.PredictDiscType, _myColor, row, col);
                    _timer = _turnEndDelay;
                    _aiState = AIState.DiscSetting;
                }
                else
                {
                    // パスの場合
                    _timer = _messageDisplyTime;
                    _passMessage.SetActive(true);
                    _aiState = AIState.Passing;
                }
                break;

            case AIState.DiscSetting:
                // 盤面変化中
                _timer -= Time.deltaTime;
                if (_timer <= 0)
                {
                    _gm.TurnEnd(false);
                    _aiState = AIState.Waiting;
                }
                break;

            case AIState.Passing:
                // パスメッセージ掲示中
                _timer -= Time.deltaTime;
                if (_timer <= 0)
                {
                    _passMessage.SetActive(false);
                    _gm.TurnEnd(true);
                    _aiState = AIState.Waiting;
                }
                break;

            default:
                Debug.LogError($"[{this.name}] State Error!!!");
                break;
        }

    }
    private void StartGameHandler()
    {
        _passMessage.SetActive(false);
        _myColor = _gm.PlayerColor==DiscColor.Black?DiscColor.White:DiscColor.Black;
        _yourColor = _gm.PlayerColor!= DiscColor.Black?DiscColor.White:DiscColor.Black;
        _aiState = AIState.Waiting;
        _ai_Level = (int)(_gm.AI_Level * 9.0f +1.0f);
        Debug.Log($"AI Level = {_ai_Level}");
    }

    private void ThinkEndHandler()
    {
        if(_aiState == AIState.Thinking)
        {
            Debug.Log($"[{this.name}] Thinking End Handled! AIState = {_aiState}");
            _aiState = AIState.ThinkComplete;
        }
        else
        {
            Debug.Log($"[{this.name}] State Error!");
        }
    }
}
