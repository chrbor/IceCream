using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMoveScript : MonoBehaviour
{
    ProcMove proc;
    Rigidbody2D rb;
    private bool jumpReady;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        proc = GetComponent<ProcMove>();
        jumpReady = true;
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.LeftArrow)) transform.position += Vector3.left * .1f;
        if (Input.GetKey(KeyCode.RightArrow)) transform.position += Vector3.right * .1f;
        if(Input.GetKey(KeyCode.UpArrow) && jumpReady) { jumpReady = false; rb.AddForce(Vector2.up * 600); }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        jumpReady = true;
    }
}
