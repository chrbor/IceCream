using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class TestMoveScript : MonoBehaviour
{
    ProcMove proc;
    Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        proc = transform.GetChild(0).GetComponent<ProcMove>();
    }

    bool prevTapp = false;
    void FixedUpdate()
    {
        //Feuer Eis:
        if (Input.GetKey(KeyCode.Space) && !prevTapp) IceManager.CallFireIce();
        prevTapp = Input.GetKey(KeyCode.Space);

        if (Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
        {
            if(Physics2D.Raycast((Vector2)transform.position + Vector2.down, Vector2.left, .6f, 1 << 11).collider == null)
                rb.position += Vector2.left * pAttribute.real_vel * .6f;
        }
        if (Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.LeftArrow))
        {
            if (Physics2D.Raycast((Vector2)transform.position + Vector2.down, Vector2.right, .6f, 1 << 11).collider == null)
                rb.position += Vector2.right * pAttribute.real_vel * .6f;
        }
        if(Input.GetKey(KeyCode.UpArrow) && !proc.falling) { rb.velocity = new Vector2(rb.velocity.x, pAttribute.jumpPower); }
    }
}
