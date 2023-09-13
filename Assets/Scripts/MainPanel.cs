using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainPanel : MonoBehaviour
{
    private TextMeshProUGUI _turnText;
    private TextMeshProUGUI _blackPoint;
    private TextMeshProUGUI _whitePoint;
    private GameBoard _gameBoard;

    // Start is called before the first frame update
    void Start()
    {
        _turnText = transform.Find("TurnText").GetComponent<TextMeshProUGUI>();
        _blackPoint = transform.Find("BlackPoint").GetComponent<TextMeshProUGUI>();
        _whitePoint = transform.Find("WhitePoint").GetComponent<TextMeshProUGUI>();
        _gameBoard = GameObject.Find("GameBoard").GetComponent<GameBoard>();
    }

    // Update is called once per frame
    void Update()
    {
        _blackPoint.SetText($"Black: {_gameBoard.BlackCount}");
        _whitePoint.SetText($"White: {_gameBoard.WhiteCount}");
    }
}
