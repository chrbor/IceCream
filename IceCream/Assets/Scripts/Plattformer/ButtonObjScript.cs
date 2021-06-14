using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonObjScript : MonoBehaviour
{
    public bool isSwitch;
    private int pressedCount;
    public UnityEvent ButtonPressed, ButtonReleased;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pressedCount++ != 0) return;
        ButtonPressed.Invoke();
        anim.SetBool("pressed", true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if ( --pressedCount != 0 || isSwitch) return;
        ButtonReleased.Invoke();
        anim.SetBool("pressed", false);
    }

    public void ResetSwitch()
    {
        if (!isSwitch || pressedCount > 0) return;
        ButtonReleased.Invoke();
    }
}
