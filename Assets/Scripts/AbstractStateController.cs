using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractStateController : MonoBehaviour
{
    protected Dictionary<int, AbstractStateChild> stateDic = new Dictionary<int, AbstractStateChild>();

    public int CurrentState { protected set; get; }

    public abstract void Initialize(int initializeStateType);

    public void UpdateSequence()
    {
        int nextState = (int)stateDic[CurrentState].StateUpdate();
        AutoStateTransitionSequence(nextState);
    }

    protected void AutoStateTransitionSequence(int nextState)
    {
        if (CurrentState == nextState)
        {
            return;
        }
        stateDic[CurrentState].OnExit();
        CurrentState = nextState;
        stateDic[CurrentState].OnEnter();
    }
}
