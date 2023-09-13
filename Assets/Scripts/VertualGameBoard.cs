using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertualGameBoard : MonoBehaviour
{
    public int BoardSize => _boardSize;
    [SerializeField]
    private int _boardSize;
    private GameBoardCommon _boardCommon;
    private void Awake()
    {
        _boardCommon = GetComponent<GameBoardCommon>();
    }
    private void Start()
    {
        _boardSize = _boardCommon.BoardSize;
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
