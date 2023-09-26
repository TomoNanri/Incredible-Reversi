using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public Action<BadgeEventArgs> OnBadgeEvent;
    public int PlayerPoint => _reverseCount;
    [SerializeField]
    private DiscType _myDiscType = DiscType.NORMAL_DISC;
    [SerializeField]
    private DiscColor _myColor = DiscColor.Black;

    private GameManager _gm;
    private GameBoard _gameBoard;
    private GameBoardCommon _gameBoardCommon;
    private GameObject _warningMessage;
    private SpecialDiscItem _mySpecialDisc;

    [SerializeField]
    private float _warningDisplyTime = 1.0f;
    [SerializeField]
    private float _turnEndDelay = 2.0f;
    private float _timer;
    private bool _done = false;
    private bool _isPassed = false;
    private int _reverseCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        _gm.OnStartGame += StartGameHandler;

        _gameBoard = GameObject.Find("GameBoard").GetComponent<GameBoard>();
        _gameBoardCommon = FindAnyObjectByType<GameBoardCommon>();

        _warningMessage = transform.Find("WarningCanvas").gameObject;
        _warningMessage.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if(_gm.GameState != GameState.InGame)
        {
            return;
        }
        if(_gm.GameTurn != GameTurn.You)
        {
            return;
        }
        RaycastHit hitObj;
        if(_warningMessage.activeSelf)
        {
            _timer -= Time.deltaTime;
            if(_timer <= 0)
            {
                _warningMessage.SetActive(false);
            }
        }
        if (_done)
        {
            if (_gameBoard.IsReversingCompleted)
            {
                _done = false;
                _gm.TurnEnd(_isPassed);
            }
            return;
        }
        
        if (Input.GetMouseButtonDown(0))
        {
            //  Ray‚Å‰Ÿ‚³‚ê‚½‘ÎÛ•¨‚ð”»’è‚µƒCƒxƒ“ƒg‘—‚é
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray,out hitObj))
            {
                Debug.Log("Position = " + hitObj.point + " Object = "+ hitObj.collider.gameObject.name);
                int row = (int)hitObj.point.y;
                int col = (int)hitObj.point.x;
                if (_gameBoard.IsSettable(_myColor, row, col))
                {
                    if (_myDiscType != DiscType.NORMAL_DISC)
                    {
                        _mySpecialDisc.Use();
                    }
                    _reverseCount = _gameBoard.SetDisc(_myDiscType, _myColor, row, col);
                    _timer = _turnEndDelay;
                    _isPassed = false;
                    _done = true;
                }
                else
                {
                    _timer = _warningDisplyTime;
                    _warningMessage.SetActive(true);
                }
            }
        }
    }
    public void OnChangeSpecialDiscUse(bool state)
    {
        //if(state)
        if(_myDiscType == DiscType.NORMAL_DISC)
        {
            if(_mySpecialDisc.ItemCount >0)
            {
                _myDiscType = (_myColor == DiscColor.Black) ? DiscType.BLACK_DISC : DiscType.WHITE_DISC;
            }
            else
            {
                _myDiscType = DiscType.NORMAL_DISC;
            }
        }
        else
        {
            _myDiscType = DiscType.NORMAL_DISC;
        }
        Debug.Log($"[{this.name}] +++ Toggle Changed +++  Color={_myDiscType}");
    }
    private void StartGameHandler()
    {
        _myColor = _gm.PlayerColor;
        var SpecialItemName = (_myColor == DiscColor.Black) ? "SBlack" : "SWhite";
        _mySpecialDisc = GameObject.Find(SpecialItemName).GetComponent<SpecialDiscItem>();

        _warningMessage.SetActive(false);
        _reverseCount = 0;
    }
    public void PassButton()
    {
        _reverseCount = 0;
        _timer = 2.0f;
        _isPassed = true;
        _done = true;
        if(OnBadgeEvent != null)
        {
            OnBadgeEvent.Invoke(new BadgeEventArgs(BadgeEventType.PASS));
        }
    }
}
