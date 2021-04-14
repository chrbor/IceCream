using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMoveScript : MonoBehaviour
{
    ProcMove proc;

    private void Start()
    {
        proc = GetComponent<ProcMove>();
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.LeftArrow)) transform.position += Vector3.left * .1f;
        if (Input.GetKey(KeyCode.RightArrow)) transform.position += Vector3.right * .1f;
    }
}
