using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickExpScript : BrickScript
{
    public GameObject explosionPrefab;
    public float size = 1;
    protected override void DoSpecial()
    {
        if (blocked) return;
        StartCoroutine(Timed());
    }

    bool blocked = false;
    IEnumerator Timed()
    {
        blocked = true;
        yield return new WaitForSeconds(Random.Range(.05f, .3f));

        GameObject explsn = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explsn.transform.localScale = Vector3.one * size;
        Destroy(transform.parent.gameObject);
    }
}
