using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AIPlayer : MonoBehaviour
{
    [SerializeField]
    private DiscColor _myColor;
    private GameManager _gm;
    private GameBoard _gameBoard;
    private VertualGameBoard _vertualGameBoard;
    private AIStateController _stateController;

    // Start is called before the first frame update
    void Start()
    {
        _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        _gm.OnStartGame += StartGameHandler;
        //_stateController = transform.Find("State").GetComponent<AIStateController>();
        //_stateController.Initialize((int)AIStateController.StateType.Wait);
        _gameBoard = GameObject.Find("GameBoard").GetComponent<GameBoard>();
        Debug.Log($"[AIPlayer] BoardSize={_gameBoard.BoardSize}");
        _vertualGameBoard = GameObject.Find("VertualBoard").GetComponent<VertualGameBoard>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_gm.GameTurn != GameTurn.Me)
            return;
        Debug.Log("[AI] Start thinking!");
        Think();

    }
    private void StartGameHandler()
    {
        _myColor = _gm.PlayerColor==DiscColor.Black?DiscColor.White:DiscColor.Black;
    }

    List<int> _settable = new List<int>();
    private void Think()
    {
        int maxValue = int.MinValue;
        int newRow = 0;
        int newCol = 0;
        _settable.Clear();
        SearchSettable();
        Debug.Log("[AI] " + _settable.Count);
        if(_settable.Count == 0)
        {
            _gm.Pass();
            return;
        }
        foreach(int child in _settable)
        {
            int row = child / _gameBoard.BoardSize;
            int col = child % _gameBoard.BoardSize;

            _vertualGameBoard.SetDisc(DiscType.NORMAL_DISC, _myColor, row, col);
            var myCount = _myColor == DiscColor.Black ? _vertualGameBoard.BlackCount : _vertualGameBoard.WhiteCount;
            if(maxValue < myCount)
            {
                newRow = row;
                newCol = col;
                maxValue = myCount;
            }
            _vertualGameBoard.RemoveDisc(row, col);
        }
        _gameBoard.SetDisc(DiscType.NORMAL_DISC, _myColor, newRow, newCol);
        _gm.Pass();
    }
    private void SearchSettable()
    {
        Debug.Log("[Search] Start!");
        Debug.Log($"[Search] BoardSize={_gameBoard.BoardSize}");

        for (int i = 0; i < _gameBoard.BoardSize * _gameBoard.BoardSize; i++)
        {
            int row = i / _gameBoard.BoardSize;
            int col = i % _gameBoard.BoardSize;
            if (_gameBoard.IsSettable(_myColor, row, col))
            {
                _settable.Add(i);
            }
            Debug.Log($"[Search] MyColor={_myColor} Row={row} Col={col}");
        }
    }
}
