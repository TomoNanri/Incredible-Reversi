using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleBlackDisc : Disc
{
    public override bool IsBlack()
    {
        return true;
    }
    public override void ReverseMotion()
    {
        transform.Rotate(new Vector3(1, 0, 0), 180);
        Debug.Log("+++Reversed!+++");
    }
}
