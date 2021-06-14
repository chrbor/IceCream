using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cone2Script;
using static ProgressScript;

public class GoalerScript : MonoBehaviour
{
    Animator anim;
    public bool block;

    private void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
        StartCoroutine(MoveAround());
    }


    IEnumerator MoveAround()
    {
        float x_center = transform.parent.position.x;
        float range = transform.parent.GetComponent<BoxCollider2D>().size.x/2;
        float x_goal;
        Vector3 moveStep;
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 8f));
            if (block) yield return new WaitWhile(() => block);


            x_goal = x_center + Random.Range(-range, range);
            transform.localScale = new Vector3(Mathf.Sign(x_goal - transform.position.x), 1, 1) * Mathf.Abs(transform.localScale.x);
            moveStep = Mathf.Sign(transform.localScale.x) * .1f * Vector2.right;

            anim.SetBool("moving", true);
            while(Mathf.Abs(transform.position.x - x_goal) > 0.1f && !block)
            {
                transform.position += moveStep;
                yield return new WaitForFixedUpdate();
            }
            if (!block) anim.SetBool("moving", false);
        }
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

                yield return new WaitForSeconds(.2f);
            }

            progressDisplay.UpdateProgressDisplay(iceScript.Get_attribute());
            anim.SetTrigger("squeesh");           
        }
        if(iceScript != null)
            Destroy(iceScript.gameObject);
        yield break;
    }
}
