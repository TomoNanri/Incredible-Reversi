using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{

    [SerializeField]
    private DiscType _myDiscType = DiscType.NORMAL_DISC;
    [SerializeField]
    private DiscColor _myColor = DiscColor.Black;

    private GameManager _gm;
    private GameBoard _gameBoard;
    private GameObject _warningMessage;

    [SerializeField]
    private float _warningDisplyTime = 1.0f;
    [SerializeField]
    private float _turnEndDelay = 2.0f;
    private float _timer;
    private bool _done = false;
    private bool _isPassed = false;

    // Start is called before the first frame update
    void Start()
    {
        _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        _gm.OnStartGame += StartGameHandler;

        _gameBoard = GameObject.Find("GameBoard").GetComponent<GameBoard>();
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
            _timer -= Time.deltaTime;
            if (_timer <= 0)
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
                    _gameBoard.SetDisc(_myDiscType, _myColor, row, col);
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
    private void StartGameHandler()
    {
        _myColor = _gm.PlayerColor;
        _warningMessage.SetActive(false);
    }
    public void PassButton()
    {
        _timer = 2.0f;
        _isPassed = true;
        _done = true;
    }

}
