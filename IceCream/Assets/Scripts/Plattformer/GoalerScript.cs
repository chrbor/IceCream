using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cone2Script;
using static ProgressScript;

public class GoalerScript : MonoBehaviour
{
    Animator anim;

    private void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        StartCoroutine(EatIce(other.GetComponent<IceScript>()));
    }

    IEnumerator EatIce(IceScript iceScript)
    {
        if(iceScript != null)
        {
            if (iceScript.id > 0)
            {
                iceScript.Get_rb().constraints = RigidbodyConstraints2D.FreezeAll;
                iceScript.gameObject.GetComponent<Collider2D>().enabled = false;
                cone.RemoveIce(iceScript.id);
                iceScript.RemoveFromCone();

                progressDisplay.UpdateProgressDisplay(iceScript.Get_attribute());
                anim.SetTrigger("squeesh");

                yield return new WaitForSeconds(.2f);
            }           
        }
        if(iceScript != null)
            Destroy(iceScript.gameObject);
        yield break;
    }
}
