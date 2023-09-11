using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DoubleWhiteItem : MonoBehaviour
{
    [SerializeField]
    private int _itemCount = 1;
    private TextMeshProUGUI _countText;
    private GameObject _toggle;
    private GameManager _gm;

    // Start is called before the first frame update
    void Start()
    {
        var obj = transform.Find("Image/CountText");
        if (obj != null)
        {
            _countText = obj.GetComponent<TextMeshProUGUI>();
            if (_countText != null)
            {
                Debug.Log(_countText);
            }
            else
                Debug.Log("Text Component Reference Error!" + this);
        }
        else
            Debug.Log("Text Object Reference Error!" + this);
        _toggle = transform.Find("Toggle").gameObject;
        _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_gm.GameState == GameState.Intro)
        {
            if (_gm.PlayerColor == DiscColor.White)
            {
                _toggle.SetActive(true);
            }
            else
            {
                _toggle.SetActive(false);
            }
        }
        if (_countText == null) return;

        _countText.SetText($"{_itemCount}");
    }

}
