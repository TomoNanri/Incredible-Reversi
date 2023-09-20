using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class IntroPanel : MonoBehaviour
{
    public Action<float> OnChangeVolume;
    private GameManager _gm;
    private Slider _slider;

    // Start is called before the first frame update
    void Start()
    {
        _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        _slider = transform.Find("Panel (6)/Slider").GetComponent<Slider>();
        _slider.value = _gm.SoundLevel;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeVolume()
    {
        if (OnChangeVolume != null)
        {
            OnChangeVolume.Invoke(_slider.value);
        }
    }
}
