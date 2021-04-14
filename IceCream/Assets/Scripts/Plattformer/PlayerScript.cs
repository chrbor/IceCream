using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CameraScript;
using static GameManager;

public class PlayerScript : MonoBehaviour
{
    public float thresh_run = 0.1f;
    public float thresh_maxAngle = 35;
    public float moveVeloctity;
    private float realVel;

    public float thresh_jump;
    public float thresh_jump_max;
    public float jumpStrength;
    public bool jumpReady { get; private set; }

    private Rigidbody2D rb;
    private Animator anim;
    private bool isMoving;
    [HideInInspector]
    public float angle { get; private set; }


    private int mask = 1 << 12;//Ground

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = transform.GetChild(0).GetComponent<Animator>();
        Input.gyro.enabled = true;
        realVel = moveVeloctity * Time.fixedDeltaTime;
        jumpReady = true;
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        //Laufen:
        isMoving = false;
        if(Input.gyro.gravity.y > -0.2f) { if(!staticCam) Camera.main.transform.rotation = Quaternion.identity; return; }

        angle = Mathf.Atan(Input.gyro.gravity.x / -Input.gyro.gravity.y) * Mathf.Rad2Deg;
        if (Mathf.Abs(angle) > thresh_maxAngle) angle = Mathf.Sign(angle) * thresh_maxAngle;

        float moveStrength = angle / 90;
        if (!staticCam)
        {
            cScript.offset = new Vector2(moveStrength * 40, 2 + Mathf.Abs(moveStrength) * 5);
            Camera.main.orthographicSize = 8 + Mathf.Abs(moveStrength) * 10;  
            Camera.main.transform.eulerAngles = new Vector3(0, 0, -angle);
        }

        if (Mathf.Abs(angle) > thresh_run && !pauseMove)
        {
            isMoving = true;
            RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.up * 0.5f, Vector3.right * moveStrength, 1.5f, mask);
            rb.position += Vector2.right * realVel * moveStrength;
            if (hit.collider) rb.position += Vector2.up * 0.2f;
        }
    }

    private void Update()
    {
        //Springen:
        if (Input.gyro.userAcceleration.z > thresh_jump && jumpReady && !pauseMove)
        {
            jumpReady = false;
            rb.AddForce(Vector2.up * (Input.gyro.userAcceleration.z > thresh_jump_max ? thresh_jump_max : Input.gyro.userAcceleration.z) * jumpStrength);
            anim.SetTrigger("jump");
        }
        anim.SetBool("moving", isMoving && !pauseMove);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        jumpReady = true;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        anim.SetBool("inAir", false);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        anim.SetBool("inAir", true);
    }
}
