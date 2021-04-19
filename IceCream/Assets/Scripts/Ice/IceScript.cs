using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Helper;
using static IceManager;
using static Cone2Script;

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
    public void ResetTouchingCone() { coneTouch = false; transform.parent = null; contacts.Clear(); }
    public void UpdateConeTouch()
    {
        if (!coneTouch)
        {
            coneIceCount++;
            coneTouch = true;
            transform.parent = cone.transform;
            List<Collider2D> cols = new List<Collider2D>();
            for(int i = 0; i < cols.Count; i++)
            {
                contacts.Add(cols[i].GetComponent<Rigidbody2D>());
                contacts[i].GetComponent<ICone>().UpdateConeTouch();
            }
        }
    }

    IEnumerator RunSim()
    {
        yield return new WaitForSeconds(2);

        while (true)
        {
            velocity = rb.velocity + iceGravity;
            Vector2 virt_Vel = Vector2.zero;

            int substract = 0;
            foreach(var contact in contacts)
            {
                if (!contact.GetComponent<ICone>().TouchingCone()) { substract++; continue; }


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
            if (contacts.Count > substract)
            {
                velocity = virt_Vel / (contacts.Count - substract);
                if (!coneTouch) velocity += iceGravity;//evtl. ohne Kondition?
            }
            
            //Halte Position bei Thresh:
            if (velocity.sqrMagnitude < threshMove && coneTouch) rb.velocity = Vector2.zero;
            else rb.velocity = velocity;

            if (coneTouch)
            {
                
                transform.position += (Vector3)cone.rb.velocity * .0015f;// * Time.fixedDeltaTime;

                //Drehe bei rotation der Waffel mit:
                transform.localPosition = new Vector2(
                    rotMatrix_cone.x * transform.localPosition.x + rotMatrix_cone.y * transform.localPosition.y,
                    rotMatrix_cone.z * transform.localPosition.x + rotMatrix_cone.w * transform.localPosition.y);
                //*
                //Wenn Maus/Touch benutzt wird:
                //transform.position = (Vector2)transform.position + cone.rb.velocity;

                //Helfende Kraft:
                //*
                diff_pos = transform.position - cone.transform.position;
                float dist = 10/(10 + diff_pos.sqrMagnitude);
                float rot = Mathf.Atan2(diff_pos.y, diff_pos.x) - 90 * Mathf.Deg2Rad;

                //rb.velocity += new Vector2(Mathf.Sin(rot), 1-Mathf.Cos(rot)) * dist * cone.helpForceMagnitude;
                //rb.position += new Vector2(Mathf.Sin(rot), 1-Mathf.Cos(rot)) * dist * cone.helpForceMagnitude;
                //rb.AddForce(new Vector2(Mathf.Sin(rot), 1 - Mathf.Cos(rot)) * dist * cone.helpForceMagnitude);
                //rb.velocity += Vector2.right * Mathf.Sin(rot) * dist * cone.helpForceMagnitude;
                //rb.position += Vector2.up * (1-Mathf.Cos(rot)) * dist * cone.helpForceMagnitude;
                //Debug.Log(new Vector2(Mathf.Sin(rot), 1 - Mathf.Cos(rot)));

                //if (name == "Ice (6)") Debug.Log(new Vector2(Mathf.Sin(rot), 1 - Mathf.Cos(rot)) * dist * cone.helpForceMagnitude);
                //*/

                //*

            }

            prevPos = transform.position;
            yield return new WaitForFixedUpdate();
        }

        yield break;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        int _layer = other.gameObject.layer;
        if (_layer != 8 && _layer != 12 || other.tag == "Col") return;//Weder Eis noch Cone

        if (!other.GetComponent<ICone>().TouchingCone() ^ coneTouch) coneIceCount++;
        coneTouch |= other.GetComponent<ICone>().TouchingCone();
        transform.parent = coneTouch? cone.transform : null;
        contacts.Add(other.GetComponent<Rigidbody2D>());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        int _layer = other.gameObject.layer;
        if (_layer != 8 && _layer != 12 || !other.isTrigger || !coneTouch || other.tag == "Col") return;

        CallTouchReset();
        cone.SetTouchingCone();
    }

}
