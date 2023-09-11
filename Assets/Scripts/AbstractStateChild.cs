using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class AbstractStateChild : MonoBehaviour
{
    protected AbstractStateController controller;

    protected int StateType { get; set; }
    public virtual void Initialize(int stateType)
    {
        StateType = stateType;
        controller = GetComponent<AbstractStateController>();
    }

    public abstract void OnEnter();
    public abstract void OnExit();
    public abstract int StateUpdate();
}
