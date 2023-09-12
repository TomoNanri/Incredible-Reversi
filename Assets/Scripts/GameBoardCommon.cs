using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DiscType { NORMAL_DISC = 0, BLACK_DISC = 1, WHITE_DISC = 2 }
public enum SearchMode { Normal, IgnoreSpecial }

public class GameBoardCommon : MonoBehaviour
{
    public int BoardSize { get; set; }
    public int BlackCount => _blackCount;
    public int WhiteCount => _whiteCount;

    private int _boardSize;
    private GameManager _gm;

    [SerializeField]
    private List<GameObject> _discPrefabs;
    private bool _isInitialized;
    private GameObject[,] _discs;
    private bool _isBoardChanged;
    private int _blackCount = 0;
    private int _whiteCount = 0;
    [SerializeField]
    private List<GameObject> _reversible;    // コマを打った後の反転可能コマのリスト
    // ８方向を示す基底ベクトル
    private List<Vector2Int> _directionList;

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
        _gm.OnInitializeGame += _initializeHandler;

        _discs = new GameObject[_boardSize, _boardSize];
        _reversible = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_gm.GameState == GameState.InGame)
        {
            if (!_isInitialized)
            {
                // 初期４コマを配置する（Start でイベントサブスクだけにしてInGame突入後にすべき？）
                StartCoroutine(BoardInitialize(0.1f));
                _isInitialized = true;
            }
            if (_isBoardChanged)
            {
                // 間をあけながらリバース処理開始
                StartCoroutine(BoardUpdate(0.1f));
                _isBoardChanged = false;
            }
            _blackCount = 0;
            _whiteCount = 0;
            foreach(Transform child in transform)
            {
                switch (child.tag)
                {
                    case "NORMAL_DISC":
                        break;
                    case "BLACK_DISC":
                        break;
                    case "WHITE_DISC":
                        break;
                    default:
                        break;
                }
            }
        }
    }
    private void _initializeHandler()
    {
        foreach (Transform child in this.transform)
        {
            switch (child.tag){
                case "BLACK_DISC":
                case "WHITE_DISC":
                case "NORMAL_DISC":
                    Destroy(child.gameObject);
                    break;

                default:
                    break;
            }
        }
        _isBoardChanged = false;
        _isInitialized = false;
    }
    private IEnumerator BoardInitialize(float sec)
    {
        SetDisc(DiscType.NORMAL_DISC, DiscColor.Black, 3, 3);
        yield return new WaitForSeconds(sec);
        SetDisc(DiscType.NORMAL_DISC, DiscColor.White, 3, 4);
        yield return new WaitForSeconds(sec);
        SetDisc(DiscType.NORMAL_DISC, DiscColor.Black, 4, 4);
        yield return new WaitForSeconds(sec);
        SetDisc(DiscType.NORMAL_DISC, DiscColor.White, 4, 3);
    }

    private IEnumerator BoardUpdate(float seconds)
    {
        foreach (GameObject disc in _reversible)
        {
            disc.GetComponent<Disc>().Reverse();
            yield return new WaitForSeconds(seconds);
        }
    }

    private void MakeReversibleList(ref List<GameObject> reversible, int row, int col, bool isDiscBlack, SearchMode mode)
    {
        List<GameObject> _lineResult = new List<GameObject>();

        foreach (Vector2Int dir in _directionList)
        {
            _lineResult.Clear();
            SearchLine(ref _lineResult, row, col, dir, isDiscBlack, mode);
            if (_lineResult.Count > 0)
                reversible.AddRange(_lineResult);
        }
    }

    private void SearchLine(ref List<GameObject> results, int row, int col, Vector2Int dirVec, bool isBlack, SearchMode mode)
    {
        bool foundMe = false;
        bool foundSpecial = false;
        int nextRow = row;
        int nextCol = col;

        // そのライン上に相手のコマがあって、その先に自分のコマがあるか探す
        nextRow += dirVec.y;
        nextCol += dirVec.x;
        while (nextRow >= 0 && nextRow < _boardSize && nextCol >= 0 && nextCol < _boardSize)
        {
            if (_discs[nextRow, nextCol] == null) break;
            if (isBlack == _discs[nextRow, nextCol].GetComponent<Disc>().IsBlack())
            {
                foundMe = true;
                break;  // 同色で行き止まり
            }
            else
            {
                if (mode == SearchMode.Normal)
                {
                    if (_discs[nextRow, nextCol].tag != "NORMAL_DISC")
                    {
                        foundSpecial = true;
                    }
                    else
                    {
                        if (!foundSpecial)
                        {
                            results.Add(_discs[nextRow, nextCol]);
                        }
                    }
                }
                else
                {
                    results.Add(_discs[nextRow, nextCol]);
                }
            }
            nextRow += dirVec.y;
            nextCol += dirVec.x;
        }
        if (!foundMe)
        {
            results.Clear();
        }
    }

    public void SetDisc(DiscType dt, DiscColor color, int row, int col)
    {
        // Board の child としてインスタンスを生成する
        GameObject prefab = _discPrefabs[(int)dt];
        Debug.Log("Prefab Name/Color = " + prefab + color);
        GameObject clone = Instantiate(prefab, this.transform);
        clone.name = $"Disk{row * _boardSize + col}";
        clone.tag = dt.ToString();

        // row/column の相対位置に移動する
        Vector3 newPos = new Vector3((float)col - (float)3.5, (float)row - (float)3.5, 0);
        clone.transform.localPosition = newPos;
        clone.transform.localRotation = Quaternion.identity;

        // 指定された色に設定する
        if (dt == DiscType.NORMAL_DISC && color == DiscColor.Black)
        {
            clone.GetComponent<Disc>().Reverse();
        }

        Debug.Log("Instance = " + clone);
        _discs[row, col] = clone;

        // 反転可能なコマのリストを作る
        _reversible.Clear();
        MakeReversibleList(ref _reversible, row, col, DiscColor.Black == color, SearchMode.Normal);
        _isBoardChanged = true;
    }
    public void RemoveDisc(int row, int col)
    {
        Destroy(_discs[row, col]);
        _discs[row, col] = null;
    }
    public bool IsSettable(DiscColor color, int row, int col)
    {
        List<GameObject> checkWork = new List<GameObject>();
        MakeReversibleList(ref checkWork, row, col, DiscColor.Black == color, SearchMode.IgnoreSpecial);
        if (checkWork.Count > 0)
            return true;
        return false;
    }
}
