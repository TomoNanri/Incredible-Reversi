using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DiscType { NORMAL_DISC = 0, BLACK_DISC = 1, WHITE_DISC = 2}

public class GameBoard : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _discPrefabs;
    [SerializeField]
    private int _boardSize = 8;
    private bool _isInitialized;
    private GameObject[,] _discs;
    private bool _isBoardChanged;
    [SerializeField]
    private int _row;
    [SerializeField]
    private int _col;
    [SerializeField]
    private DiscType _discType;
    [SerializeField]
    private DiscColor _discColor; 
    [SerializeField]
    private bool _do;
    [SerializeField]
    private List<GameObject> _reversible;    // �R�}��ł�����̔��]�\�R�}�̃��X�g


    void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        _discs = new GameObject[_boardSize, _boardSize];
        _isBoardChanged = false;
        _isInitialized = false;
        _reversible = new List<GameObject>();
        _do = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isInitialized && GameManager.Ga)
        {
            // �����S�R�}��z�u����iStart �ŃC�x���g�T�u�X�N�����ɂ���InGame�˓���ɂ��ׂ��H�j
            StartCoroutine(BoardInitialize(0.1f));
            _isInitialized = true;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetDisc(_discType, _discColor, _row, _col);
            _do = false;
            //Debug.Log("_do: " + _do);
            _isBoardChanged = true;
        } 
        else if (_isBoardChanged)
        {
            // �Ԃ������Ȃ��烊�o�[�X�����J�n
            
            StartCoroutine(BoardUpdate(0.1f));
            _isBoardChanged = false;
        }
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
        foreach(GameObject disc in _reversible)
        {
            disc.GetComponent<Disc>().Reverse();
            yield return new WaitForSeconds(seconds);
        }
    }

    void MakeReversibleList(int row, int col, bool isDiscBlack)
    {
        _reversible.Clear();
        List<GameObject> _lineResult = new List<GameObject>();

        // ��T��
        _lineResult.Clear();
        SearchLine(ref _lineResult, row, col, new Vector2Int(0, 1), isDiscBlack);
        if (_lineResult.Count > 0)
            _reversible.AddRange(_lineResult);

        // �E��T��
        _lineResult.Clear();
        SearchLine(ref _lineResult, row, col, new Vector2Int(1, 1), isDiscBlack);
        if (_lineResult.Count > 0)
            _reversible.AddRange(_lineResult);

        // �E�T��
        _lineResult.Clear();
        SearchLine(ref _lineResult, row, col, new Vector2Int(1, 0), isDiscBlack);
        if (_lineResult.Count > 0)
            _reversible.AddRange(_lineResult);

        // �E���T��
        _lineResult.Clear();
        SearchLine(ref _lineResult, row, col, new Vector2Int(1, -1), isDiscBlack);
        if (_lineResult.Count > 0)
            _reversible.AddRange(_lineResult);

        // ���T��
        _lineResult.Clear();
        SearchLine(ref _lineResult, row, col, new Vector2Int(0, -1), isDiscBlack);
        if (_lineResult.Count > 0)
            _reversible.AddRange(_lineResult);

        // �����T��
        _lineResult.Clear();
        SearchLine(ref _lineResult, row, col, new Vector2Int(-1, -1), isDiscBlack);
        if (_lineResult.Count > 0)
            _reversible.AddRange(_lineResult);

        // ���T��
        _lineResult.Clear();
        SearchLine(ref _lineResult, row, col, new Vector2Int(-1, 0), isDiscBlack);
        if (_lineResult.Count > 0)
            _reversible.AddRange(_lineResult);

        // ����T��
        _lineResult.Clear();
        SearchLine(ref _lineResult, row, col, new Vector2Int(-1, 1), isDiscBlack);
        if (_lineResult.Count > 0)
            _reversible.AddRange(_lineResult);
    }

    void SearchLine(ref List<GameObject> results, int row, int col, Vector2Int dirVec, bool isBlack)
    {
        bool foundMe = false;
        bool foundSpecial = false;
        int nextRow = row;
        int nextCol = col;

        // ���̃��C����ɑ���̃R�}�������āA���̐�Ɏ����̃R�}�����邩�T��
        nextRow += dirVec.y;
        nextCol += dirVec.x;
        while (nextRow >= 0 && nextRow < _boardSize && nextCol >= 0 && nextCol < _boardSize)
        {
            if (_discs[nextRow, nextCol] == null) break;
            if (isBlack == _discs[nextRow, nextCol].GetComponent<Disc>().IsBlack())
            {
                foundMe = true;
                break;  // ���F�ōs���~�܂�
            }
            else
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
            nextRow += dirVec.y;
            nextCol += dirVec.x;
        }
        if (!foundMe)
        {
            results.Clear();
        }
    }

    public void SetDisc(DiscType dt, DiscColor color,int row,int col)
    {
        // Board �� child �Ƃ��ăC���X�^���X�𐶐�����
        GameObject prefab = _discPrefabs[(int)dt];
        Debug.Log("Prefab Name/Color = " + prefab + color);
        GameObject clone = Instantiate(prefab, this.transform);
        clone.name = $"Disk{row * _boardSize + col}";
        clone.tag = dt.ToString();

        // row/column �̑��Έʒu�Ɉړ�����
        Vector3 newPos = new Vector3((float)col - (float)3.5, (float)row - (float)3.5,  0);
        clone.transform.localPosition = newPos;
        clone.transform.localRotation = Quaternion.identity;

        // �w�肳�ꂽ�F�ɐݒ肷��
        if (dt == DiscType.NORMAL_DISC && color == DiscColor.Black)
        {
            clone.GetComponent<Disc>().Reverse();
        }

        Debug.Log("Instance = " + clone);
        Debug.Log($"_discs[{row}, {col}] = {_discs[row,col]}");
        _discs[row, col] = clone;

        // ���]�\�ȃR�}�̃��X�g�����
        MakeReversibleList(row, col, DiscColor.Black == color);
    }
}

