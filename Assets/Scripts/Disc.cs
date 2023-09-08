using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Disc : MonoBehaviour
{
    [SerializeField]
    protected bool _reverse = false;

    public abstract bool IsBlack();
    public abstract void ReverseMotion();
    public void Reverse()
    {
        _reverse = true;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_reverse)
        {
            ReverseMotion();
            _reverse = false;
        }
    }
    public void Remove()
    {
        Destroy(this);
    }
}
