using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

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

    bool prevTapp = false;
    void FixedUpdate()
    {
        //Feuer Eis:
        if (Input.GetKey(KeyCode.Space) && !prevTapp) IceManager.CallFireIce();
        prevTapp = Input.GetKey(KeyCode.Space);

        if (Input.GetKey(KeyCode.LeftArrow)) transform.position += Vector3.left * pAttribute.real_vel * .5f;
        if (Input.GetKey(KeyCode.RightArrow)) transform.position += Vector3.right * pAttribute.real_vel * .5f;
        if(Input.GetKey(KeyCode.UpArrow) && jumpReady) { jumpReady = false; rb.velocity = new Vector2(rb.velocity.x, pAttribute.jumpPower); }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        jumpReady = true;
    }
}
