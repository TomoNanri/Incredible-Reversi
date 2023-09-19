using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AIPlayer : MonoBehaviour
{
    public enum AIState { Predicting, StartThink, Thinking, Complete }
    private AIState _aiState;

    [SerializeField]
    private DiscColor _myColor,_yourColor;
    private GameManager _gm;
    private GameBoard _gameBoard;
    private AIStateController _stateController;
    private GameObject _passMessage;
    private ThinkAI _thinkAI;

    [SerializeField]
    private float _messageDisplyTime = 1.0f;
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
        //_stateController.Initialize((int)AIStateController.StateType.Wait);
        _gameBoard = GameObject.Find("GameBoard").GetComponent<GameBoard>();
        //_vertualGameBoard = GameObject.Find("VertualBoard").GetComponent<VertualGameBoard>();
        _passMessage = transform.Find("PassCanvas").gameObject;
        _passMessage.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (_gm.GameState != GameState.InGame)
        {
            return;
        }
        if (_passMessage.activeSelf)
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0)
            {
                _passMessage.SetActive(false);
            }
        }
        if (_gm.GameTurn == GameTurn.You)
        {
            _aiState = AIState.Predicting;
        }
        else
        {
            if(_aiState == AIState.Predicting)
            {
                _aiState = AIState.StartThink;
            }
        }

        switch(_aiState)
        {
            case AIState.Predicting:
                break;

            case AIState.StartThink:
                // 先読みを開始する
                _aiState = AIState.Thinking;
                var result = _thinkAI.StartThinking(_ai_Level);
                break;

            case AIState.Thinking:
                // 先読みを継続する
                break;

            case AIState.Complete:
                // 決まった手（passを含む）を打つ
                if (_thinkAI.PredictPos > -1)
                {
                    int row = _thinkAI.PredictPos / _gameBoard.BoardSize;
                    int col = _thinkAI.PredictPos % _gameBoard.BoardSize;
                    _gameBoard.SetDisc(_thinkAI.PredictDiscType, _myColor, row, col);
                }
                else
                {
                    // パスの場合
                    _timer = _messageDisplyTime;
                    _passMessage.SetActive(true);
                }
                _aiState = AIState.Predicting;
                _gm.TurnEnd();

                break;
        }

    }
    private void StartGameHandler()
    {
        _passMessage.SetActive(false);
        _myColor = _gm.PlayerColor==DiscColor.Black?DiscColor.White:DiscColor.Black;
        _yourColor = _gm.PlayerColor!= DiscColor.Black?DiscColor.White:DiscColor.Black;
        _aiState = AIState.Predicting;
        _ai_Level = (int)(_gm.AI_Level * 9.0f +1.0f);
        Debug.Log($"AI Level = {_ai_Level}");
    }

    private void ThinkEndHandler()
    {
        if(_aiState == AIState.Thinking)
        {
            Debug.Log($"[{this.name}] Thinking End Handled! AIState = {_aiState}");
            _aiState = AIState.Complete;
        }
        else
        {
            Debug.Log($"[{this.name}] State Error!");
        }
    }
}
