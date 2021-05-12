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
    public GameObject explosionPrefab;

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

    int contactMask = (1 << 8);//Kontakt nur mit Obstacles, Ground und Eis
    public void SplitIgnore(GameObject splitIce, GameObject col_Object) => StartCoroutine(DoSplitIgnore(splitIce, col_Object));
    IEnumerator DoSplitIgnore(GameObject splitIce, GameObject col_Object)
    {
        splitIce.layer = 0;
        col_Object.SetActive(false);
        //yield return new WaitForSeconds(.22f);
        float count = 0;
        do { count += .1f;  yield return new WaitForSeconds(.1f); } while (Physics2D.CircleCast(splitIce.transform.position, splitIce.transform.localScale.x/2, Vector2.up, 666/*Muhahahaha!!!*/, contactMask).collider != null && count < .5f);
        if (col_Object != null) col_Object.SetActive(true);
        if(splitIce != null) splitIce.layer = 8;
        yield break;
    }
}
