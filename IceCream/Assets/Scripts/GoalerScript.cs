using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cone2Script;
using static GameManager;

public class GoalerScript : MonoBehaviour
{
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
                //yield return new WaitForFixedUpdate();
                yield return new WaitForSeconds(.2f);
            }
            //Hier Punkte hinzufügen:
            iceDelivered.Add(iceScript.Get_attribute());
        }
        if(iceScript != null)
            Destroy(iceScript.gameObject);
        yield break;
    }
}
