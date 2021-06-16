using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class TestMoveScript : MonoBehaviour
{
    ProcMove proc;
    Rigidbody2D rb;

    RaycastHit2D hit;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        proc = transform.GetChild(0).GetComponent<ProcMove>();
    }

    bool prevTapp = false;
    void FixedUpdate()
    {
        if (pauseMove) return;

        //Feuer Eis:
        if (Input.GetKey(KeyCode.Space) && !prevTapp) IceManager.CallFireIce();
        prevTapp = Input.GetKey(KeyCode.Space);

        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.RightArrow))
        {
            hit = Physics2D.Raycast((Vector2)transform.position + Vector2.down, Vector2.left, 1.4f, 1 << 11);
            rb.position += Vector2.left * pAttribute.real_vel * .6f * (hit.collider == null ? 1 : (hit.distance * hit.distance - 1f));
        }
        if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.LeftArrow))
        {
            hit = Physics2D.Raycast((Vector2)transform.position + Vector2.down, Vector2.right, 1.4f, 1 << 11);
            rb.position += Vector2.right * pAttribute.real_vel * .6f * (hit.collider == null ? 1 : (hit.distance * hit.distance - 1f));
        }
        if(Input.GetKey(KeyCode.W) && proc.groundDist < .6f) { rb.velocity = new Vector2(rb.velocity.x, pAttribute.jumpPower); }
    }
}
