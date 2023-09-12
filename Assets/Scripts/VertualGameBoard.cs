using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertualGameBoard : MonoBehaviour
{
    public int BoardSize { get => _boardSize; }
    
    private int _boardSize = 8;

    private GameBoardCommon _boardCommon;
    private void Awake()
    {
        _boardCommon = GetComponent<GameBoardCommon>();
        _boardCommon.BoardSize = _boardSize;
    }
    public int BlackCount
    {
        get { return _boardCommon.BlackCount; }
    }
    public int WhiteCount
    {
        get { return _boardCommon.WhiteCount; }
    }
    public void SetDisc(DiscType dt, DiscColor color, int row, int col)
    {
        _boardCommon.SetDisc(dt, color, row, col);
    }
    public void RemoveDisc(int row, int col)
    {
        _boardCommon.RemoveDisc(row, col);
    } 
    public bool IsSettable(DiscColor color, int row, int col)
    {
        return _boardCommon.IsSettable(color, row, col);
    }
}
