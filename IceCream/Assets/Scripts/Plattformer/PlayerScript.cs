using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CameraScript;
using static GameManager;

public class PlayerScript : MonoBehaviour
{
    public float thresh_run = 5;
    public float thresh_maxAngle = 35;

    public float thresh_jump = .3f;
    public float thresh_jump_max = .4f;
    public PlayerAttribute attribute;
    public bool jumpReady { get; private set; }

    private Rigidbody2D rb;
    private ProcMove pMove;
    private bool isMoving;
    [HideInInspector]
    public float angle { get; private set; }
    [HideInInspector]
    public float camSize = 8;

    bool prevTapp = false;

    private int mask = 1 << 12;//Ground

    void Awake()
    {
        player = gameObject;
        pAttribute = attribute;
        attribute.Set_vel();
        rb = GetComponent<Rigidbody2D>();
        pMove = transform.GetChild(0).GetComponent<ProcMove>();
        Input.gyro.enabled = true;
        jumpReady = true;
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        //Feuer Eis:
        if (Input.touchCount > 0 && !prevTapp) IceManager.CallFireIce();
        prevTapp = Input.touchCount > 0;

        //Laufen:
        isMoving = false;
        if(Input.gyro.gravity.y > -0.2f) { if(!staticCam) Camera.main.transform.rotation = Quaternion.identity; return; }

        angle = Mathf.Atan(Input.gyro.gravity.x / -Input.gyro.gravity.y) * Mathf.Rad2Deg;
        if (Mathf.Abs(angle) > thresh_maxAngle) angle = Mathf.Sign(angle) * thresh_maxAngle;

        float moveStrength = angle / 90;
        if (!staticCam)
        {
            cScript.offset = new Vector2(moveStrength * 40, 2 + Mathf.Abs(moveStrength) * 5);
            Camera.main.orthographicSize = camSize + Mathf.Abs(moveStrength) * 10;  
            Camera.main.transform.eulerAngles = new Vector3(0, 0, -angle);
        }

        if (Mathf.Abs(angle) > thresh_run && !pauseMove)
        {
            isMoving = true;
            RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.up * 0.5f, Vector3.right * moveStrength, 1.5f, mask);
            rb.position += Vector2.right * attribute.real_vel * moveStrength;
            if (hit.collider) rb.position += Vector2.up * 0.2f;
        }
    }

    private void Update()
    {
        //Springen:
        if (Input.gyro.userAcceleration.z > thresh_jump && jumpReady && !pauseMove)
        {
            jumpReady = false;
            //rb.AddForce(Vector2.up * (Input.gyro.userAcceleration.z > thresh_jump_max ? thresh_jump_max : Input.gyro.userAcceleration.z) * jumpStrength);
            rb.velocity = new Vector2(rb.velocity.x, (Input.gyro.userAcceleration.z > thresh_jump_max ? 1 : Input.gyro.userAcceleration.z/thresh_jump_max) * attribute.jumpPower);
        }
        //anim.SetBool("moving", isMoving && !pauseMove);
    }

    int holdHeight;
    float y_hold;
    private void OnCollisionEnter2D(Collision2D other)
    {
        jumpReady = true;

        //if (other.gameObject.layer == 13) { holdHeight++; y_hold = transform.position.y; } 
    }
    /*
    private void OnCollisionStay2D(Collision2D other)
    {
        if (holdHeight <= 0 || other.gameObject.layer != 13) return;//falls nicht ice_col dann return
        transform.position = new Vector3(transform.position.x, y_hold);
        IceScript ice = other.gameObject.GetComponent<IceScript>();
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.layer == 13) holdHeight--;
    }
    //*/
}
