using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DiscType { Normal=0, DoubleBlack=1,DoubleWhite=2}

public class GameCell
{
    public GameObject Disc { get; set; }
}

public class GameBoard : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _discPrefabs;
    [SerializeField]
    private int _boardSize = 8;
    private GameCell[,] _discs;
    private bool _isBoardChange;

    private void Awake()
    {
        _discs = new GameCell[_boardSize, _boardSize];
    }

    // Start is called before the first frame update
    void Start()
    {
        // 初期４コマを配置する（InGame突入後がにすべき？）
        _isBoardChange = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isBoardChange)
        {
            // 間をあけながらリバース処理開始
        }
    }

    public void SetDisc(DiscType dt, DiscColor color,int row,int col)
    {
        // Board の child としてインスタンスを生成する
        _discs[row, col].Disc = Instantiate(_discPrefabs[(int)dt], this.transform);
        // 指定された色に設定する
        // row/column の相対位置に移動する
        //_discs[row,col].
        _isBoardChange = true;
    }
}

