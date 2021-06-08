using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestroyScript : MonoBehaviour
{
    public float timer = 2;
    private void Awake()
    {
        StartCoroutine(TimedDestroy());
    }

    IEnumerator TimedDestroy()
    {
        yield return new WaitForSeconds(timer);
        Destroy(gameObject);
        yield break;
    }
}
