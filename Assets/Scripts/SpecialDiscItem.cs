using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpecialDiscItem : MonoBehaviour
{
    public int ItemCount => _itemCount;
    [SerializeField]
    private int _itemCount = 1;
    private DiscColor _colorSelf;
    private TextMeshProUGUI _countText;
    private GameObject _toggle;
    private GameManager _gm;

    // Start is called before the first frame update
    void Start()
    {
        if(this.name == "SBlack")
        {
            _colorSelf = DiscColor.Black;
        }
        else
        {
            _colorSelf = DiscColor.White;
        }
        Debug.Log($"[{this.name}] _colorSelf = {_colorSelf}");

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
        _gm.OnStartGame += StartUp;
    }

    // Update is called once per frame
    void Update()
    {
        if (_countText == null) return;

        _countText.SetText($"{_itemCount}");
    }
    private void StartUp()
    {
        if (_gm.UseSpecialDisc && _colorSelf == _gm.PlayerColor)
        {
            Debug.Log($"[{this.name}] _toggle = {_toggle} IsActive={_toggle.activeSelf}");
            _toggle.SetActive(true); 
            //_toggle.GetComponent<Toggle>().interactable = true; 
            Debug.Log($"[{this.name}] _toggle = {_toggle} IsActive={_toggle.activeSelf}");
        }
        else
        {
            Debug.Log($"[{this.name}] _toggle = {_toggle} IsActive={_toggle.activeSelf}");
            _toggle.SetActive(false);
            //_toggle.GetComponent<Toggle>().interactable = false; 
            Debug.Log($"[{this.name}] _toggle = {_toggle} IsActive={_toggle.activeSelf}");
        }
    }
    
    public bool Use()
    {
        if (_itemCount > 0)
        {
            _itemCount--;
            return true;
        }
        else
        {
            return false;
        }
    }
}
