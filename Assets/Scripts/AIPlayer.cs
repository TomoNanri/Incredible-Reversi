using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AIPlayer : MonoBehaviour
{
    public enum AIState { Waiting, StartThink, Thinking, ThinkComplete, DiscSetting, Passing }
    public Action<BadgeEventArgs> OnBadgeEvent;
    private AIState _aiState;

    [SerializeField]
    private DiscColor _myColor;
    private GameManager _gm;
    private GameBoard _gameBoard;
    private GameBoardCommon _gameBoardCommon;
    private GameObject _passMessage;
    private ThinkAI _thinkAI;

    [SerializeField]
    private float _messageDisplyTime = 2.0f;
    [SerializeField]
    private int _ai_Level;
    private float _timer;

    private Player _player;
    //private float _debugTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        _gm.OnStartGame += StartGameHandler;
        _thinkAI = GetComponent<ThinkAI>();
        _thinkAI.ThinkEnd += ThinkEndHandler;
        _aiState = AIState.Waiting;

        _gameBoard = GameObject.Find("GameBoard").GetComponent<GameBoard>();
        _gameBoardCommon = FindAnyObjectByType<GameBoardCommon>();

        _passMessage = transform.Find("PassCanvas").gameObject;
        _passMessage.SetActive(false);
        _player = FindAnyObjectByType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        //_debugTimer += Time.deltaTime;
        //if (_debugTimer > 0.1f)
        //{
        //    _debugTimer = 0;
        //    Debug.Log($"[{this.name}]   AI State = {_aiState} Game State = {_gm.GameState} Turn = {_gm.GameTurn}");
        //}
        switch(_aiState)
        {
            case AIState.Waiting:
                if (_gm.GameState != GameState.InGame || _gm.GameTurn != GameTurn.Me)
                {
                    return;
                }

                Debug.Log($"[{this.name}]Start my turn!");

                // ����̎�ɂ��o�b�W�C�x���g�`�F�b�N
                if (_player.PlayerPoint > 0)
                {
                    OnBadgeEvent.Invoke(new BadgeEventArgs(BadgeEventType.POINT));
                }
                _aiState = AIState.StartThink;
                break;

            case AIState.StartThink:
                // ��ǂ݂��J�n����
                Debug.Log($"[{this.name}]Start Thinking! AI Level = {_ai_Level}");
                _aiState = AIState.Thinking;
                var result = _thinkAI.StartThinking(_ai_Level);
                break;

            case AIState.Thinking:
                // ��ǂ݂��p������(�C�x���g�̓n���h���Ŏ�M����)
                break;

            case AIState.ThinkComplete:
                // ���܂�����ipass���܂ށj��ł�
                if (_thinkAI.PredictPos > -1)
                {
                    // �p�X���Ȃ�
                    int row = _thinkAI.PredictPos / _gameBoard.BoardSize;
                    int col = _thinkAI.PredictPos % _gameBoard.BoardSize;

                    _gameBoard.SetDisc(_thinkAI.PredictDiscType, _myColor, row, col);
                    _aiState = AIState.DiscSetting;
                    Debug.Log($"[{this.name}]waiting reverse...");
                }
                else
                {
                    // �p�X�̏ꍇ
                    _timer = _messageDisplyTime;
                    _passMessage.SetActive(true);
                    _aiState = AIState.Passing;

                    // �����̃p�X�ɂ��o�b�W�C�x���g�`�F�b�N
                    OnBadgeEvent.Invoke(new BadgeEventArgs(BadgeEventType.PASS));
                }
                break;

            case AIState.DiscSetting:
                //�Ֆʕω���
                if (_gameBoard.IsReversingCompleted)
                {
                    _gm.TurnEnd(false);
                    _aiState = AIState.Waiting;
                }
                break;

            case AIState.Passing:
                // �p�X���b�Z�[�W�f����
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
        _aiState = AIState.Waiting;
        _ai_Level = (int)(_gm.AI_Level * 14.0f + 1.0f);
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
