using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CameraScript;
using static Helper;

public class ParasolScript : MonoBehaviour
{
    public bool isOpen;
    private bool block;

    Animator anim;
    AudioSource aSrc;
    BoxCollider2D col;

    private void Start()
    {
        anim = GetComponent<Animator>();
        aSrc = GetComponent<AudioSource>();
        col = GetComponent<BoxCollider2D>();

        if (isOpen)
        {
            anim.SetBool("opened", true);
            col.offset = Vector2.up * 4;
            col.size = new Vector2(4, 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (block) return;

        int _layer = other.gameObject.layer;
        if (!isOpen)
        {
            if (_layer == 9 || _layer == 14)//Player oder Effekt (wie zB Explosion)
                OpenParasol();
            return;
        }
        if(_layer != 8)//Falls nicht vom Eis getroffen, dann mache den Schirm wieder zu
            CloseParasol();
        //else
            anim.SetTrigger("squeesh");

        Vector2 diff = transform.position - Camera.main.transform.position;
        if (Mathf.Abs(diff.x) < camWindow.x && Mathf.Abs(diff.y) < camWindow.y)
        {
            aSrc.panStereo = diff.x / camWindow.x;
            aSrc.volume = Mathf.Abs(aSrc.panStereo) * .5f + .25f;
            aSrc.pitch = Random.Range(.9f, 1.1f);

            aSrc.Play();
        }

        Rigidbody2D rb_other = other.GetComponent<Rigidbody2D>();
        Vector4 rotMat = GetRotationMatrix(2 * ((transform.eulerAngles.z + 90) * Mathf.Deg2Rad - Mathf.Atan2(-rb_other.velocity.y, -rb_other.velocity.x)) + Mathf.PI);
        rb_other.velocity = new Vector2(rotMat.x * rb_other.velocity.x + rotMat.y * rb_other.velocity.y,
                                        rotMat.z * rb_other.velocity.x + rotMat.w * rb_other.velocity.y) * 1.05f;
    }

    private void OpenParasol()
    {       
        anim.SetBool("opened", true);
        col.offset = Vector2.up * 4.5f;
        col.size = new Vector2(4, 1);
        isOpen = true;
    }

    private void CloseParasol()
    {
        StartCoroutine(Cooldown());
        anim.SetBool("opened", false);
        col.offset = Vector2.up * 2.5f;
        col.size = new Vector2(1, 5);
        isOpen = false;
    }

    IEnumerator Cooldown()
    {
        block = true;
        yield return new WaitForSeconds(1);
        block = false;
        yield break;
    }
}
