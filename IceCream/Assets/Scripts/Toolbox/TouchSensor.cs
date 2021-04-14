using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class TouchSensor : MonoBehaviour
{
    [HideInInspector]
    public bool tipped;
    [HideInInspector]
    public int fingerID;

    public void Initialize(int _fingerID)
    {
        tipped = true;
        fingerID = _fingerID;
        StartCoroutine(StopTipp());
    }

    IEnumerator StopTipp()
    {
        yield return new WaitForSeconds(0.1f);
        tipped = false;
        yield break;
    }
}
