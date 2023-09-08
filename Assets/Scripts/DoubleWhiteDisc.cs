using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleWhiteDisc : Disc
{
    public override bool IsBlack()
    {
        return false;
    }
    public override void ReverseMotion()
    {
        transform.Rotate(new Vector3(1, 0, 0), 180);
        Debug.Log("+++Reversed!+++");
    }
}
