using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickExpScript : BrickScript
{
    public GameObject explosionPrefab;
    public float size = 1;
    protected override void DoSpecial()
    {
        GameObject explsn = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explsn.transform.localScale = Vector3.one * size;
        Destroy(gameObject);
    }
}
