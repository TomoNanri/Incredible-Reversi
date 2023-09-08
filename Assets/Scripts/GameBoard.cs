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
    private GameObject[,] _discs;
    private bool _isBoardChange;
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

    void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        _discs = new GameObject[_boardSize, _boardSize];
        // �����S�R�}��z�u����i�C�x���g�T�u�X�N�����ɂ���InGame�˓��オ�ɂ��ׂ��H�j
        _isBoardChange = false;
        _do = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetDisc(_discType, _discColor, _row, _col);
            _do = false;
            Debug.Log("_do: " + _do);
        }
        if (_isBoardChange)
        {
            // �Ԃ������Ȃ��烊�o�[�X�����J�n
            //StartCoroutine(BoardUpdate(0.1f));
            _isBoardChange = false;
        }
    }

    private IEnumerator BoardUpdate(float seconds)
    {
        foreach(GameObject disc in reversible)
        {
            disc.GetComponent<Disc>().Reverse();
            yield return new WaitForSeconds(seconds);
        }
    }

    private List<GameObject> reversible;
    void MakeReversibleList(int row, int col, bool isDiscBlack)
    {
        List<GameObject> _lineResult = new List<GameObject>();

        // ��T��
        _lineResult.Clear();
        SearchLine(ref _lineResult, row, col, new Vector2Int(0, 1), isDiscBlack);
        if (_lineResult.Count > 0)
            reversible.AddRange(_lineResult);

        // �E��T��
        _lineResult.Clear();
        SearchLine(ref _lineResult, row, col, new Vector2Int(1, 1), isDiscBlack);
        if (_lineResult.Count > 0)
            reversible.AddRange(_lineResult);

        // �E�T��
        _lineResult.Clear();
        SearchLine(ref _lineResult, row, col, new Vector2Int(1, 0), isDiscBlack);
        if (_lineResult.Count > 0)
            reversible.AddRange(_lineResult);

        // �E���T��
        _lineResult.Clear();
        SearchLine(ref _lineResult, row, col, new Vector2Int(1, -1), isDiscBlack);
        if (_lineResult.Count > 0)
            reversible.AddRange(_lineResult);

        // ���T��
        _lineResult.Clear();
        SearchLine(ref _lineResult, row, col, new Vector2Int(0, -1), isDiscBlack);
        if (_lineResult.Count > 0)
            reversible.AddRange(_lineResult);

        // �����T��
        _lineResult.Clear();
        SearchLine(ref _lineResult, row, col, new Vector2Int(-1, -1), isDiscBlack);
        if (_lineResult.Count > 0)
            reversible.AddRange(_lineResult);

        // ���T��
        _lineResult.Clear();
        SearchLine(ref _lineResult, row, col, new Vector2Int(-1, 0), isDiscBlack);
        if (_lineResult.Count > 0)
            reversible.AddRange(_lineResult);

        // ����T��
        _lineResult.Clear();
        SearchLine(ref _lineResult, row, col, new Vector2Int(-1, 1), isDiscBlack);
        if (_lineResult.Count > 0)
            reversible.AddRange(_lineResult);
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

        //// ���]�\�ȃR�}�̃��X�g�����
        //MakeReversibleList(row, col, DiscColor.Black == color);

        ////_discs[row,col].
        //_isBoardChange = true;
    }
}

