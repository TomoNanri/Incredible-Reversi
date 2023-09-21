using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class IntroPanel : MonoBehaviour
{
    public Action<float> OnChangeVolume;
    private GameManager _gm;
    private Slider _volumeSlider;
    private Slider _AILevelSlider;

    // Start is called before the first frame update
    void Start()
    {
        _gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        _volumeSlider = transform.Find("Panel (6)/Slider").GetComponent<Slider>();
        _volumeSlider.value = _gm.SoundLevel;
        _AILevelSlider = transform.Find("Panel (3)/Slider").GetComponent<Slider>();
        _AILevelSlider.value = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeVolume()
    {
        if (OnChangeVolume != null)
        {
            OnChangeVolume.Invoke(_volumeSlider.value);
        }
    }
}
