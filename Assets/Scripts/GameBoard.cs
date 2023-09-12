using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GameBoard : MonoBehaviour
{
    public int BoardSize { get => _boardSize; }
    private int _boardSize = 8;

    private GameBoardCommon _boardCommon;
    private GameBoardCommon _vertualGameBoardCommon;

    void Awake()
    {
        _boardCommon = GetComponent<GameBoardCommon>();
        _boardCommon.BoardSize = _boardSize;
    }
    private void Start()
    {
        _vertualGameBoardCommon = GameObject.Find("AIPlayer/VertualBoard").GetComponent<GameBoardCommon>();
    }

    public void SetDisc(DiscType dt, DiscColor color, int row, int col)
    {
        _boardCommon.SetDisc(dt, color, row, col);
        _vertualGameBoardCommon.SetDisc(dt, color, row, col);
    }
    public bool IsSettable(DiscColor color, int row, int col)
    {
        return _boardCommon.IsSettable(color, row, col);
    }
}

