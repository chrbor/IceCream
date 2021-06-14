using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IceManager;

public class BrickScript : MonoBehaviour
{
    private bool block;

    private void OnTriggerEnter2D(Collider2D other)
    {
        int _layer = other.gameObject.layer;
        if (_layer != 13 && _layer != 14) return;//IceCol

        StartCoroutine(Crumble(other.transform));
    }

    IEnumerator Crumble(Transform Tother)
    {
        if (block) yield break;
        block = true;
        yield return new WaitForFixedUpdate();
        if (Tother.parent == transform) yield break;
        //GetComponent<Animator>().Play("crumble");
        yield return new WaitForSeconds(.25f);
        block = false;

        DoSpecial();
        yield break;
    }

    protected virtual void DoSpecial()
    {
        Destroy(transform.parent.gameObject);
    }
}
