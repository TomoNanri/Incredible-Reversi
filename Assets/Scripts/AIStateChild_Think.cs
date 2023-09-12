using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateChild_Think : AbstractStateChild
{
    private bool _isActive = false;

    public override void OnEnter()
    {
        _isActive = true;
    }

    public override void OnExit()
    {
        throw new System.NotImplementedException();
    }

    public override int StateUpdate()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_isActive)
        {

        }
    }
}
