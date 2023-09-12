using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateController : AbstractStateController
{
    public enum StateType { Lookahead, Think, Wait}

    public override void Initialize(int initializeStateType)
    {
        //stateDic[(int)StateType.Lookahead] = gameObject.AddComponent<>
    }
}
