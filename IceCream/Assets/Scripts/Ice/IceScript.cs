using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IceManager;
using static ConeScript;

public class IceScript : MonoBehaviour, ICone
{
    public float damping;
    public float threshMove;

    Vector3 prevPos;
    [HideInInspector]
    public Vector2 velocity;
    [HideInInspector]
    public bool coneTouch;

    List<Rigidbody2D> contacts = new List<Rigidbody2D>();
    Vector2 diff_vel, diff_pos;
    float realDamping;

    Rigidbody2D rb;

    private void Start()
    {
        ResetTouch += ResetTouchingCone;

        //Init:
        rb = GetComponent<Rigidbody2D>();
        prevPos = transform.position;
        StartCoroutine(RunSim());
    }

    public bool TouchingCone() => coneTouch;
    public float GetDamping() => damping;
    public void ResetTouchingCone() { coneTouch = false; transform.parent = null; }
    public void UpdateConeTouch()
    {
        if (!coneTouch)
        {
            coneTouch = true;
            transform.parent = cone.transform;
            foreach (var contact in contacts) contact.GetComponent<ICone>().UpdateConeTouch();
        }
    }


    IEnumerator RunSim()
    {
        yield return new WaitForSeconds(2);

        while (true)
        {
            velocity = rb.velocity + iceGravity;
            Vector2 virt_Vel = Vector2.zero;

            foreach(var contact in contacts)
            {
                diff_vel = velocity - contact.velocity;
                diff_pos = transform.position - contact.transform.position;


                if (contact.gameObject.layer == 12) realDamping = 1;
                else
                {
                    realDamping = 1 - diff_pos.sqrMagnitude;
                    realDamping *= contact.GetComponent<ICone>().GetDamping();
                    realDamping = realDamping > 1 ? 1 : realDamping;
                }

                virt_Vel +=  Vector2.Lerp(velocity, contact.velocity, realDamping);
            }
            //Setze aktive geschwindigkeit entsprechend der auswertung:
            if (contacts.Count > 0)
            {
                velocity = virt_Vel / contacts.Count;
                if (!coneTouch) velocity += iceGravity;//evtl. ohne Kondition?
            }
            
            if (coneTouch)
            {
                transform.localPosition = new Vector2(
                    rotMatrix_cone.x * transform.localPosition.x + rotMatrix_cone.y * transform.localPosition.y,
                    rotMatrix_cone.z * transform.localPosition.x + rotMatrix_cone.w * transform.localPosition.y);

                transform.position = (Vector2)transform.position + cone.rb.velocity;
            }

            //Halte Thresh:
            if (velocity.sqrMagnitude < threshMove && coneTouch) rb.velocity = Vector2.zero;
            else rb.velocity = velocity;

            prevPos = transform.position;
            yield return new WaitForFixedUpdate();
        }

        yield break;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        int _layer = other.gameObject.layer;
        if (_layer != 8 && _layer != 12) return;//Weder Eis noch Cone

        coneTouch |= other.GetComponent<ICone>().TouchingCone();
        transform.parent = cone.transform;
        contacts.Add(other.GetComponent<Rigidbody2D>());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        int _layer = other.gameObject.layer;
        if (_layer != 8 && _layer != 12) return;

        contacts.Remove(other.GetComponent<Rigidbody2D>());
        CallTouchReset();
        cone.SetTouchingCone();

    }
}
