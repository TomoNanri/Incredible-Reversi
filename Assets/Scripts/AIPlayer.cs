using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        _vertualGameBoard = GameObject.Find("VertualGameBoard").GetComponent<VertualGameBoard>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_gm.GameTurn != GameTurn.Me)
            return;
        

    }
    private void StartGameHandler()
    {
        _myColor = _gm.PlayerColor==DiscColor.Black?DiscColor.White:DiscColor.Black;
    }

    List<(int row, int col)> _settable = new List<(int row, int col)>();
    private void Think()
    {
        int maxValue = int.MinValue;
        int newRow = 0;
        int newCol = 0;
        SearchSettable();
        if(_settable.Count == 0)
        {
            _gm.Pass();
            return;
        }
        foreach((int row, int col) child in _settable)
        {
            _vertualGameBoard.SetDisc(DiscType.NORMAL_DISC, _myColor, child.row, child.col);
            var myCount = _myColor == DiscColor.Black ? _vertualGameBoard.BlackCount : _vertualGameBoard.WhiteCount;
            if(maxValue < myCount)
            {
                newRow = child.row;
                newCol = child.col;
                maxValue = myCount;
            }
            _vertualGameBoard.RemoveDisc(child.row, child.col);
        }
        _gameBoard.SetDisc(DiscType.NORMAL_DISC, _myColor, newRow, newCol);
        _gm.Pass();
    }
    private void SearchSettable()
    {
        for (int row = 0; row < _gameBoard.BoardSize; row++)
        {
            for (int col = 0; col < _gameBoard.BoardSize; col++)
            {
                if (_gameBoard.IsSettable(_myColor, row, col))
                {
                    _settable.Add(new(row, col));
                }
            }
        }
    }
}
