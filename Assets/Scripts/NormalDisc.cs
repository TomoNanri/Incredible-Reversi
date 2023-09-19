using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalDisc : Disc
{
    public override bool IsBlack()
    {
        if(transform.forward.z > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    public override void ReverseMotion()
    {
        transform.Rotate(new Vector3(1, 0, 0), 180);
        Debug.Log($"{this.name}/{transform.parent.name}+++Reversed!+++");
    }
}
