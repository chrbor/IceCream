using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CameraScript;
using static Helper;

public class JumpPadScript : MonoBehaviour
{
    public float jumpForce = 15;

    Animator anim;
    AudioSource aSrc;


    private void Start()
    {
        anim = GetComponent<Animator>();
        aSrc = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Vector2 diff = transform.position - Camera.main.transform.position;
        if (Mathf.Abs(diff.x) < camWindow.x && Mathf.Abs(diff.y) < camWindow.y)
        {
            anim.SetTrigger("Squeesh");
            aSrc.panStereo = diff.x / camWindow.x;
            aSrc.volume = Mathf.Abs(aSrc.panStereo) * .5f + .25f;
            aSrc.pitch = Random.Range(.9f, 1.1f);

            aSrc.Play();
        }


        Rigidbody2D rb_other = other.GetComponent<Rigidbody2D>();
        rb_other.velocity = new Vector2(rb_other.velocity.x, Mathf.Abs(rb_other.velocity.y) > jumpForce ? Mathf.Abs(rb_other.velocity.y) : jumpForce);
    }
}
