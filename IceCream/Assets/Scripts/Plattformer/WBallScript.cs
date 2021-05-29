using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CameraScript;

public class WBallScript : MonoBehaviour
{
    //*
    Rigidbody2D rb;
    AudioSource aSrc;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        aSrc = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        int _layer = other.gameObject.layer;
        Vector2 diff = transform.position - other.transform.position;
        if ((_layer == 11 || _layer == 21) && (Mathf.Abs(diff.x) < camWindow.x && Mathf.Abs(diff.y) < camWindow.y))
        {
            aSrc.panStereo = diff.x / camWindow.x;
            aSrc.volume = Mathf.Abs(aSrc.panStereo) * .5f + .25f;
            aSrc.pitch = Random.Range(.9f, 1.1f);
            aSrc.Play();
        }
        else if(_layer == 9)//Player
            rb.velocity += Vector2.right * (Mathf.Sign(diff.x) * other.transform.GetChild(0).GetComponent<ProcMove>().speed * 20 - rb.velocity.x) * 2;
    }
    //*/
}
