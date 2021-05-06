using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Cone2Script;

public class IceManager : MonoBehaviour
{
    public static IceManager iceManager;

    public Vector2 gravity;
    public static Vector2 iceGravity;
    public static int coneIceCount;

    public GameObject ice_prefab;

    public static event UnityAction ResetTouch, FireIce;

    private void Start()
    {
        iceManager = this;

        iceGravity = gravity * Time.fixedDeltaTime;
        coneIceCount = 0;
    }

    public static void CallTouchReset() { coneIceCount = 0; ResetTouch.Invoke(); }
    public static void CallFireIce() { if (FireIce != null) FireIce.Invoke(); }


    public void CreateIceTower(List<IceAttribute> attributes)
    {

    }
}
