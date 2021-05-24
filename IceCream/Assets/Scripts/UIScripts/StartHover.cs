using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartHover : MonoBehaviour
{

    Animator anim;
    [Range(0,1)]
    public float offset;
    // Start is called before the first frame update
    private void OnEnable()
    {
        anim = GetComponent<Animator>();
        anim.Play("hover",0,offset);        
    }
}
