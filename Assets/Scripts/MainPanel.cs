using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainPanel : MonoBehaviour
{
    public int BlackPoint = 0;
    public int WhitePoint = 0;

    private TextMeshProUGUI _turnText;
    private TextMeshProUGUI _blackPoint;
    private TextMeshProUGUI _whitePoint;

    // Start is called before the first frame update
    void Start()
    {
        _turnText = transform.Find("TurnText").GetComponent<TextMeshProUGUI>();
        _blackPoint = transform.Find("BlackPoint").GetComponent<TextMeshProUGUI>();
        _whitePoint = transform.Find("WhitePoint").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        _blackPoint.SetText($"Black: {BlackPoint}");
        _whitePoint.SetText($"Black: {WhitePoint}");
    }
}
