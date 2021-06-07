using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cone2Script;

public class GoalScript : MonoBehaviour
{
    private List<GameObject> goalerList;

    public float jumpTime;
    private float timeStep;

    private void Start()
    {
        timeStep = Time.fixedDeltaTime / jumpTime;

        //Erfasse Goaler:
        goalerList = new List<GameObject>(transform.childCount-1);
        for (int i = 1; i < transform.childCount; i++) goalerList.Add(transform.GetChild(i).gameObject);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        //Es werden nur Eiskugeln erfasst:

        if (goalerList.Count == 0) return;

        StartCoroutine(JumpToPoint(goalerList[Random.Range(0, goalerList.Count)], other.gameObject));
    }

    private IEnumerator JumpToPoint(GameObject jumper, GameObject ice)
    {
        goalerList.Remove(jumper);

        Rigidbody2D rb_ice = ice.GetComponent<Rigidbody2D>();
        Vector3 ice_vel = Vector3.zero;
        IceScript iceScript = ice.GetComponent<IceScript>();
        Vector2 endPoint = ice.transform.position;

        if (iceScript.id < 0)
        {
            iceScript.Get_attribute().runSim = false;
     
            ice_vel = rb_ice.velocity;
            rb_ice.bodyType = RigidbodyType2D.Kinematic;
            rb_ice.constraints = RigidbodyConstraints2D.FreezeAll;
            ice.gameObject.layer = 0;
        }
        else ice = null;
        endPoint += (Vector2)ice_vel * jumpTime * .5f;
        ice_vel *= Time.fixedDeltaTime;




        Vector2 startPos = jumper.transform.position;
        Vector2 diff = endPoint - (Vector2)jumper.transform.position + Vector2.down;

        Animator anim = jumper.transform.GetChild(0).GetComponent<Animator>();
        anim.Play("Jump", 0);
        jumper.transform.localScale = Mathf.Abs(jumper.transform.localScale.x) * Vector3.one * Mathf.Sign(diff.x);

        //Fliege zum Punkt:
        for(float count = 0; count < 1; count += timeStep)
        {
            if(ice != null) ice.transform.position += ice_vel * (1 - count);
            jumper.transform.position = startPos + new Vector2(diff.x * count, (1 - (1 - count) * (1 - count)) * diff.y);
            yield return new WaitForFixedUpdate();
        }
        //Eis solltee jetzt durch GoalerScript zerstört sein!
        yield return new WaitForSeconds(.5f);
        Vector2 tmpPos = jumper.transform.position;
        diff *= new Vector2(.5f, -1);
        //Falle auf y-start zurück
        float fallStep = timeStep * 2;
        for (float count = 0; count < 1; count += fallStep)
        {
            jumper.transform.position = tmpPos + new Vector2(diff.x * count, (1 - (1 - count) * (1 - count)) * diff.y);
            yield return new WaitForFixedUpdate();
        }

        //Laufe zum Start zurück:
        anim.SetBool("moving", true);
        jumper.transform.localScale = new Vector3(-jumper.transform.localScale.x, jumper.transform.localScale.y, 1) ;//transform.localScale.x * Vector3.one * Mathf.Sign(diff.x);
        tmpPos = jumper.transform.position;
        diff *= new Vector2( 3 * timeStep, 0);
        for (float count = 0; count < 1; count += timeStep)
        {
            jumper.transform.position -= (Vector3)diff;
            yield return new WaitForFixedUpdate();
        }

        jumper.transform.position = startPos;
        goalerList.Add(jumper);
        anim.SetBool("moving", false);
        yield break;
    }
}
