using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{

    [SerializeField]
    private DiscType _myDiscType = DiscType.NORMAL_DISC;
    [SerializeField]
    private DiscColor _myDiscColor = DiscColor.Black;

    private GameManager _gm;
    private GameBoard _gameBoard;
    private GameObject _warningMessage;

    [SerializeField]
    private float _warningDisplyTime = 1.0f;
    private float _timer;

    // Start is called before the first frame update
    void Start()
    {
        _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
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
        if (Input.GetMouseButtonDown(0))
        {
            //  Rayで押された対象物を判定しイベント送る
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray,out hitObj))
            {
                Debug.Log("Position = " + hitObj.point + " Object = "+ hitObj.collider.gameObject.name);
                int row = (int)hitObj.point.y;
                int col = (int)hitObj.point.x;
                if (_gameBoard.IsSettable(_myDiscColor, row, col))
                {
                    _gameBoard.SetDisc(_myDiscType, _myDiscColor, row, col);
                }
                else
                {
                    _timer = _warningDisplyTime;
                    _warningMessage.SetActive(true);
                }
            }
        }
    }
}
