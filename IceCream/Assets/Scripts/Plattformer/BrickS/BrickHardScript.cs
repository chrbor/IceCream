using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickHardScript : BrickScript
{
    public int life = 3;
    protected override void DoSpecial()
    {
        if (--life <= 0) Destroy(gameObject);
    }
}
