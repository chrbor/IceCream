using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class IceManager : MonoBehaviour
{
    public Vector2 gravity;
    public static Vector2 iceGravity;
    public static int coneIceCount;

    public static event UnityAction ResetTouch;

    private void Start()
    {
        iceGravity = gravity * Time.fixedDeltaTime;
        coneIceCount = 0;
    }

    public static void CallTouchReset() { coneIceCount = 0; ResetTouch.Invoke(); }
}
