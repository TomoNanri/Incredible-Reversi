using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Threading.Tasks;
public enum CellState { None, Black, White, FixedBlack, FixedWhite }
public class ThinkAI : MonoBehaviour
{
    public int PredictPos => _predictPos;
    public DiscType PredictDiscType => _predictDiscType;
    public Action ThinkEnd;

    // ８方向を示す基底ベクトル
    private List<Vector2Int> _directionList;

    private DiscColor _myColor, _yourColor;
    private SpecialDiscItem _mySpecialDisc;
    private GameManager _gm;
    private GameBoard _gameBoard;
    private int _boardSize;
    //private VertualGameBoard _vertualGameBoard;

    private int _predictPos;
    private DiscType _predictDiscType;
    private int _maxDepth;
    private int _currentDepth;

    void Awake()
    {
        _directionList = new List<Vector2Int>();    // 探索方向を示すベクトルを格納
        _directionList.Add(new Vector2Int(0, 1));   // 上
        _directionList.Add(new Vector2Int(1, 1));   // 右上
        _directionList.Add(new Vector2Int(1, 0));   // 右
        _directionList.Add(new Vector2Int(1, -1));  // 右下
        _directionList.Add(new Vector2Int(0, -1));  // 下
        _directionList.Add(new Vector2Int(-1, -1)); // 左下
        _directionList.Add(new Vector2Int(-1, 0));  // 左
        _directionList.Add(new Vector2Int(-1, 1));  // 左上
    }

    // Start is called before the first frame update
    void Start()
    {
        _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        _gm.OnStartGame += StartGameHandler;
        _gameBoard = GameObject.Find("GameBoard").GetComponent<GameBoard>();
        //_vertualGameBoard = GameObject.Find("VertualBoard").GetComponent<VertualGameBoard>();
        _boardSize = _gameBoard.BoardSize;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    /// <summary>
    /// 現在の盤面を読む
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    private CellState[,] ReadBoard(int size)
    {
        char[] _trimChar = { 'D', 'i', 's', 'c' }; 
        CellState[,] _board = new CellState[size, size];
        for(int row = 0; row < size; row++)
        {
            for(int col = 0; col < size; col++)
            {
                _board[row, col] = CellState.None;
            }
        }
        //Debug.Log($"[ReadBoard/{this.name}] Started!");
        foreach(Transform child in _gameBoard.transform)
        {
            //Debug.Log($" - ReadBoard - object name = {child.name}/{child.tag}");

            switch (child.tag)
            {
                case "BLACK_DISC":
                    int No = int.Parse(child.name.Trim(_trimChar));
                    int row = No / size;
                    int col = No % size;
                    _board[row, col] = CellState.FixedBlack;
                    break;

                case "WHITE_DISC":
                    No = int.Parse(child.name.Trim(_trimChar));
                    row = No / size;
                    col = No % size;
                    _board[row, col] = CellState.FixedWhite;
                    break;

                case "NORMAL_DISC":
                    No = int.Parse(child.name.Trim(_trimChar));
                    row = No / size;
                    col = No % size;
                    if (child.GetComponent<Disc>().IsBlack())
                    {
                        _board[row, col] = CellState.Black;
                    }
                    else
                    {
                        _board[row, col] = CellState.White;
                    }
                    break;

                default:
                    break;
            }
        }
        return _board;
    }
    /// <summary>
    /// 指定したマスが自分の色であれば True を返す
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    private bool IsMyColor(CellState s)
    {
        if (_myColor == DiscColor.Black)
        {
            return IsBlack(s);
        }
        return IsWhite(s);
    }

    /// <summary>
    /// 指定したマスが黒であれば True を返す
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    private bool IsBlack(CellState s)
    {
        bool ret = false;
        switch(s){
            case CellState.Black:
            case CellState.FixedBlack:
                ret = true;
                break;
            default:
                break;
        }
        return ret;
    }

    /// <summary>
    /// 指定したマスが白であれば True を返す
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    private bool IsWhite(CellState s)
    {
        bool ret = false;
        switch (s)
        {
            case CellState.White:
            case CellState.FixedWhite:
                ret = true;
                break;
            default:
                break;
        }
        return ret;
    }
    
    /// <summary>
    /// 指定したマスが特殊コマなら True を返す
    /// </summary>
    /// <param name="s"></param>
    /// <returns></returns>
    private bool IsSpecial(CellState s)
    {
        bool ret = false;
        switch (s)
        {
            case CellState.FixedBlack:  
            case CellState.FixedWhite:
                ret = true;
                break;
            default:
                break;
        }
        return ret;
    }
    
    /// <summary>
    /// 指定したマスがコマを打てるマスなら True を返す
    /// </summary>
    /// <param name="cells"></param>
    /// <param name="color"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    public bool IsSettable(CellState[,] cells, DiscColor color, int row, int col)
    {
        if (cells[row, col] != CellState.None)
            return false;

        List<int> checkWork = new List<int>();
        MakeReversibleList(ref checkWork, cells, row, col, DiscColor.Black == color, SearchMode.IgnoreSpecial);
        if (checkWork.Count > 0)
            return true;
        return false;
    }

    /// <summary>
    /// 指定したマスにコマを打ったときに裏返し可能なコマのリストを返す
    /// </summary>
    /// <param name="reversible"></param>
    /// <param name="cells"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="isDiscBlack"></param>
    /// <param name="mode"></param>
    private void MakeReversibleList(ref List<int> reversible, CellState[,] cells, int row, int col, bool isDiscBlack, SearchMode mode)
    {
        List<int> _lineResult = new List<int>();

        foreach (Vector2Int dir in _directionList)
        {
            _lineResult.Clear();
            SearchLine(ref _lineResult, cells, row, col, dir, isDiscBlack, mode);
            if (_lineResult.Count > 0)
            {
                reversible.AddRange(_lineResult);
            }
        }
    }

    /// <summary>
    /// 指定されたマスから dirVec で指定された方向へ裏返し可能なコマのリストを返す
    /// </summary>
    /// <param name="results"></param>
    /// <param name="cells"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="dirVec"></param>
    /// <param name="isBlack"></param>
    /// <param name="mode"></param>
    private void SearchLine(ref List<int> results, CellState[,] cells, int row, int col, Vector2Int dirVec, bool isBlack, SearchMode mode)
    {
        bool foundMyColor = false;
        bool foundSpecial = false;

        // そのライン上に相手のコマがあって、その先に自分のコマがあるか調べる
        row += dirVec.y;
        col += dirVec.x;
        while (row >= 0 && row < _boardSize && col >= 0 && col < _boardSize)
        {
            if (cells[row, col] == CellState.None) break;   // その方向にコマは無い
            if (isBlack == IsBlack(cells[row, col]))
            {
                foundMyColor = true;
                break;  // 同色で行き止まり
            }
            else
            {
                if (mode == SearchMode.Normal)
                {
                    if (IsSpecial(cells[row, col]))
                    {
                        foundSpecial = true;
                    }
                    else
                    {
                        if (!foundSpecial)
                        {
                            results.Add(row * _boardSize + col);
                        }
                    }
                }
                else
                {
                    results.Add(row * _boardSize + col);
                }
            }
            row += dirVec.y;
            col += dirVec.x;
        }
        if (!foundMyColor)
        {
            results.Clear();
        }
    }

    /// <summary>
    /// コマを打てる総てのマスのリストを返す
    /// </summary>
    /// <param name="result"></param>
    /// <param name="board"></param>
    /// <param name="color"></param>
    private void SearchSettable(ref List<int> result, CellState[,] board, DiscColor color)
    {
        for (int i = 0; i < _gameBoard.BoardSize * _gameBoard.BoardSize; i++)
        {
            int row = i / _boardSize;
            int col = i % _boardSize;
            if (IsSettable(board, color, row, col))
            {
                result.Add(i);
            }
        }
    }

    /// <summary>
    /// 指定されたマスにコマを配置し、影響を受けるコマを裏返した結果の盤面を返す
    /// </summary>
    /// <param name="board"></param>
    /// <param name="type"></param>
    /// <param name="color"></param>
    /// <param name="row"></param>
    /// <param name="col"></param>
    private void SetDisc(ref CellState[,] board, DiscType type, DiscColor color, int row, int col)
    {
        // 指定された色に設定する
        if (type == DiscType.NORMAL_DISC)
        {
            if(color == DiscColor.Black)
            {
                board[row, col] = CellState.Black;
            }
            else
            {
                board[row, col] = CellState.White;
            }
        }
        else if(type == DiscType.BLACK_DISC)
        {
            board[row, col] = CellState.FixedBlack;
        }
        else
        {
            board[row, col] = CellState.FixedWhite;
        }


        // 反転可能なコマのリストを作る
        List<int> _reversible = new List<int>();
        MakeReversibleList(ref _reversible, board, row, col, DiscColor.Black == color, SearchMode.Normal);
        //ShowReversibleList(_reversible);
        foreach (int child in _reversible)
        {
            row = child / _boardSize;
            col = child % _boardSize;
            if(board[row, col] == CellState.Black)
            {
                board[row, col] = CellState.White;
            }
            else
            {
                board[row, col] = CellState.Black;
            }
        }
    }

    /// <summary>
    /// ゲーム開始時に呼ばれ、AI 側の扱うコマを記憶する
    /// </summary>
    private void StartGameHandler()
    {
        _myColor = _gm.PlayerColor == DiscColor.Black ? DiscColor.White : DiscColor.Black;
        _yourColor = _gm.PlayerColor != DiscColor.Black ? DiscColor.White : DiscColor.Black;
        var SpecialItemName = (_myColor == DiscColor.Black) ? "SBlack" : "SWhite";
        _mySpecialDisc = GameObject.Find(SpecialItemName).GetComponent<SpecialDiscItem>();
    }

    /// <summary>
    /// AI Player から呼ばれる先読み処理本体
    /// </summary>
    /// <param name="depth"></param>
    /// <returns></returns>
    public async Task StartThinking(int depth)
    {
        _maxDepth = depth;
        _currentDepth = 0;
        _predictPos = -1;
        _predictDiscType = DiscType.NORMAL_DISC;

        // 検討用ボードを作成する
        CellState[,] _cells = ReadBoard(_gameBoard.BoardSize);
        Debug.Log($"[{this.name}] !!! Start of Thinking !!! current={_currentDepth} max={_maxDepth}");
        //ShowBoard(_cells);
        List<int> _candidates = new List<int>();
        SearchSettable(ref _candidates, _cells, _myColor);
        if (!KillerCheck(_candidates, _cells))
        {
            int alpha = int.MinValue;
            int beta = int.MaxValue;

            var result = await Task.Run(() => Proceed(_cells, _currentDepth, _maxDepth, GameTurn.Me, alpha, beta));
            if(_predictPos != -1 && _gm.UseSpecialDisc)
            {
                int nextRow = _predictPos / _boardSize;
                int nextCol = _predictPos % _boardSize;
                if (nextRow > 1 && nextRow <= 5 && nextCol > 1 && nextCol <= 5)
                {
                    if (_mySpecialDisc.ItemCount > 0)
                    {
                        _mySpecialDisc.Use();
                        _predictDiscType = (_myColor == DiscColor.Black) ? DiscType.BLACK_DISC : DiscType.WHITE_DISC;
                    }
                }
            }
        }
        Debug.Log($"[{this.name}] !!! End of Thinking !!!");
        ThinkEnd.Invoke();
        return;
    }

    /// <summary>
    /// 無条件で有利な手筋の判定
    /// </summary>
    /// <param name="candidates"></param>
    /// <param name="board"></param>
    /// <returns></returns>
    private bool KillerCheck(List<int> candidates, CellState[,] board)
    {
        int _maxValue = int.MinValue;
        CellState[,] nextBoard = (CellState[,])board.Clone();

        foreach (int e in candidates)
        {
            int row = e / _boardSize;
            int col = e % _boardSize;
            if (row == 0 || row == _boardSize - 1)
            {
                if(col == 0 || col == _boardSize - 1)
                {
                    _predictPos = e;
                    return true;
                }
                if (board[row,col-1]==CellState.None  && board[row, col + 1] == CellState.None)
                {
                    nextBoard = (CellState[,])board.Clone();
                    SetDisc(ref nextBoard, DiscType.NORMAL_DISC, _myColor, row, col);
                    var v = Evaluate(nextBoard);
                    if(v > _maxValue)
                    {
                        _maxValue = v;
                        _predictPos = e;
                    }
                }
                if (_gm.UseSpecialDisc && _mySpecialDisc.ItemCount>0)
                {
                    _predictPos = e;
                    _mySpecialDisc.Use();
                    _predictDiscType = (_myColor == DiscColor.Black) ? DiscType.BLACK_DISC : DiscType.WHITE_DISC;
                    return true;
                }
            }
            if(col == 0 || col == _boardSize - 1)
            {
                if (board[row - 1, col] == CellState.None && board[row + 1, col] == CellState.None)
                {
                    nextBoard = (CellState[,])board.Clone();
                    SetDisc(ref nextBoard, DiscType.NORMAL_DISC, _myColor, row, col);
                    var v = Evaluate(nextBoard);
                    if (v > _maxValue)
                    {
                        _maxValue = v;
                        _predictPos = e;
                    }
                }
                if (_gm.UseSpecialDisc && _mySpecialDisc.ItemCount > 0)
                {
                    _predictPos = e;
                    _mySpecialDisc.Use();
                    _predictDiscType = (_myColor == DiscColor.Black) ? DiscType.BLACK_DISC : DiscType.WHITE_DISC;
                    return true;
                }
            }
        }
        if (_maxValue > int.MinValue)
        {
            Debug.Log($"[{this.name}] *** Killer *** ");
            return true;
        }
        return false;
    }

    /// <summary>
    /// αβ法による先読み処理（別スレッド）
    /// </summary>
    /// <param name="currentBoard"></param>
    /// <param name="currentDepth"></param>
    /// <param name="maxDepth"></param>
    /// <param name="turn"></param>
    /// <param name="alpha"></param>
    /// <param name="beta"></param>
    /// <returns></returns>
    private int Proceed(CellState[,] currentBoard, int currentDepth, int maxDepth, GameTurn turn, int alpha, int beta)
    {
        int _subTreeValue;
        int _stageValue;
        int _minValue = int.MaxValue;
        int _maxValue = int.MinValue;


        // 終端まで来たら局面評価を実行する

        if (currentDepth == maxDepth)
        {
            return Evaluate(currentBoard);
        }

        // この局面で置くコマの色を決める
        var targetColor = (turn == GameTurn.Me) ? _myColor : _yourColor;
        GameTurn nextTurn = (turn == GameTurn.Me) ? GameTurn.You : GameTurn.Me;

        List<int> _candidateList = new List<int>();

        // 打つことができる場所のリストを作る
        SearchSettable(ref _candidateList, currentBoard, targetColor);

        if (_candidateList.Count > 0)
        {
            // 打てるところがあるので各手について先読み継続
            foreach (int next in _candidateList)
            {
                int row = next / _gameBoard.BoardSize;
                int col = next % _gameBoard.BoardSize;

                if (turn == GameTurn.Me)
                {
                    alpha = _maxValue;
                    if (currentDepth == 0)
                    {
                        Debug.Log($"=>Start Checking [{row},{col}] alpha={alpha} beta={beta}");
                    }
                    if (alpha >= beta)
                        return beta;    // βカット

                    // 局面のコピー
                    CellState[,] nextBoard = (CellState[,])currentBoard.Clone();   
                    SetDisc(ref nextBoard, DiscType.NORMAL_DISC, targetColor, row, col);
                    // 先読み実行
                    _subTreeValue = Proceed(nextBoard, currentDepth + 1, maxDepth, nextTurn, alpha, beta);

                    // 自分のターンなので最大値を選択する
                    if (_maxValue < _subTreeValue)
                    {
                        _maxValue = _subTreeValue;
                        if (currentDepth == 0)
                        {
                            _subTreeValue = _maxValue;
                            _predictPos = next;
                            Debug.Log($"   ++++++ Find new Max Value [{row},{col}] = {_subTreeValue} ++++++");
                        }
                    }
                }
                else
                {
                    beta = _minValue;
                    if (beta <= alpha)
                        return alpha;   // αカット

                    // 局面のコピー
                    CellState[,] nextBoard = (CellState[,])currentBoard.Clone();
                    SetDisc(ref nextBoard, DiscType.NORMAL_DISC, targetColor, row, col);
                    // 先読み実行
                    _subTreeValue = Proceed(nextBoard, currentDepth + 1, maxDepth, nextTurn, alpha, beta);

                    // 相手のターンなので最小値を選択する
                    if (_minValue > _subTreeValue)
                    {
                        _minValue = _subTreeValue;
                    }
                }
            }
            if (turn == GameTurn.Me)
            {
                _stageValue = _maxValue;
            }
            else
            {
                _stageValue = _minValue;
            }
        }
        else
        {
            // パスしかないので打たない。サブツリーの値がそのまま局面の評価値になる。
            _stageValue = Proceed(currentBoard, currentDepth + 1, maxDepth, nextTurn, alpha, beta);   // 先読み実行
        }

        //Debug.Log($"+++ Current Candiate => [{_predictPos / _boardSize},{_predictPos % _boardSize}]");
        return _stageValue;
    }

/// <summary>
/// 盤面のコマごとの重み
/// </summary>
    private int[,] _cellValue = { {  30, -12,   0,  -1,  -1,   0, -12,  30},
                                  { -12, -15,  -3,  -3,  -3,  -3, -15, -12},
                                  {   0,  -3,   0,  -1,  -1,   0,  -3,   0},
                                  {  -1,  -3,  -1,  -1,  -1,  -1,  -3,  -1},
                                  {  -1,  -3,  -1,  -1,  -1,  -1,  -3,  -1},
                                  {   0,  -3,   0,  -1,  -1,   0,  -3,   0},
                                  { -12, -15,  -3,  -3,  -3,  -3, -15, -12},
                                  {  30, -12,   0,  -1,  -1,   0, -12,  30}};
    
    /// <summary>
    /// 局面評価関数
    /// </summary>
    /// <param name="currentBoard"></param>
    /// <returns></returns>
    private int Evaluate(CellState[,] currentBoard)
    {
        int _value = 0;
        for(int row = 0; row < _boardSize; row++) {
            for (int col = 0; col < _boardSize; col++)
            {
                if (IsMyColor(currentBoard[row,col]))
                {
                    _value += _cellValue[row, col];
                }
            }
        }
        return _value;
    }

    /// <summary>
    /// 盤面をテキストで表示する（デバッグ用）
    /// </summary>
    /// <param name="board"></param>
    private void ShowBoard(CellState[,] board)
    {
        for (int r = 7; r >= 0; r--)
        {
            Debug.Log($"  {board[r, 0]} {board[r, 1]} {board[r, 2]} {board[r, 3]} {board[r, 4]} {board[r, 5]} {board[r, 6]} {board[r, 7]}");
        }
    }

    /// <summary>
    /// コマを打てるか所をテキスト表示する（デバッグ用）
    /// </summary>
    /// <param name="vs"></param>
    private void ShowReversibleList(List<int> vs)
    {
        foreach(int e in vs)
        {
            Debug.Log($" revesible elem = cell[{e/_boardSize},{e%_boardSize}]");
        }
    }
}
