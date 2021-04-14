using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICone
{
    bool TouchingCone();
    float GetDamping();
    void UpdateConeTouch();
}
