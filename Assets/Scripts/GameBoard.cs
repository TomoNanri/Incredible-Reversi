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
        // �����S�R�}��z�u����iInGame�˓��オ�ɂ��ׂ��H�j
        _isBoardChange = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isBoardChange)
        {
            // �Ԃ������Ȃ��烊�o�[�X�����J�n
        }
    }

    public void SetDisc(DiscType dt, DiscColor color,int row,int col)
    {
        // Board �� child �Ƃ��ăC���X�^���X�𐶐�����
        _discs[row, col].Disc = Instantiate(_discPrefabs[(int)dt], this.transform);
        // �w�肳�ꂽ�F�ɐݒ肷��
        // row/column �̑��Έʒu�Ɉړ�����
        //_discs[row,col].
        _isBoardChange = true;
    }
}

