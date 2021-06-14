using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateScript : MonoBehaviour
{
    public bool active;
    private Animator anim;
    private BoxCollider2D col;

    private void Start()
    {
        col = GetComponent<BoxCollider2D>();

        anim = GetComponent<Animator>();
        anim.SetBool("closed", active);
        col.enabled = active;
    }

    public void SetGate(bool _active)
    {
        if (active == _active) return;
        active = _active;

        anim.SetBool("closed", active);
        col.enabled = active;
    }
}
