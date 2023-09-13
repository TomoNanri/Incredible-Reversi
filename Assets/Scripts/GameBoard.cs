using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameBoard : MonoBehaviour
{
    public int BoardSize => _boardSize;
    public int BlackCount => _boardCommon.BlackCount;
    public int WhiteCount => _boardCommon.WhiteCount;

    [SerializeField]
    private int _boardSize;
    private GameBoardCommon _boardCommon;
    private GameBoardCommon _vertualGameBoardCommon;
    private TextMeshProUGUI _blackPoint;
    private TextMeshProUGUI _whitePoint;

    void Awake()
    {
        _boardCommon = GetComponent<GameBoardCommon>();
    }
    private void Start()
    {
        _boardSize = _boardCommon.BoardSize;
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

