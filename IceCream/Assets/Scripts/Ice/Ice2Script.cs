using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Helper;
using static IceManager;
using static Cone2Script;

public class Ice2Script : MonoBehaviour, ICone
{
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
    public float GetDamping() => 0;
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
            if (coneTouch)
            {
                foreach (var contact in contacts)
                {
                    //Setze rotation entsprechend coneGravity:

                    //Ermittle locale Position in Bezug auf die cone-Gravity:
                    if (contact == cone.rb) continue;
                    Vector2 diff_pos = (Vector2)transform.position - contact.position;
                    float g_rotation = Mathf.Atan2(cone.coneGravity.y, cone.coneGravity.x) + 90;//Winkel, um den sich die Gravitation durch die Bewegung des Spielers verändert hat
                    Vector4 trafoMatrix = GetRotationMatrix(g_rotation);//Drehe Koordinatensystem entsprechend der Beschleunigung:
                    Vector2 localPos = new Vector2(trafoMatrix.x * diff_pos.x + trafoMatrix.y * diff_pos.y, trafoMatrix.z * diff_pos.x + trafoMatrix.w * diff_pos.y);

                    float local_rot = Mathf.Atan2(localPos.y, localPos.x) + 90;

                    Vector4 rotMatrix = GetRotationMatrix(Mathf.Cos(local_rot) * cone.coneGravityFactor);
                    
                    transform.position = contact.position + new Vector2(rotMatrix.x * diff_pos.x + rotMatrix.y * diff_pos.y, rotMatrix.z * diff_pos.x + rotMatrix.w * diff_pos.y);
                }

                transform.localPosition -= (Vector3)cone.rb.velocity * Time.fixedDeltaTime;
                
                //Rotiere mit der Waffel mit:
                transform.localPosition = new Vector2(
                    rotMatrix_cone.x * transform.localPosition.x + rotMatrix_cone.y * transform.localPosition.y,
                    rotMatrix_cone.z * transform.localPosition.x + rotMatrix_cone.w * transform.localPosition.y);
                    
            }
            else
            {
                rb.velocity += iceGravity;
            }

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
        transform.parent = coneTouch? cone.transform : null;
        contacts.Add(other.GetComponent<Rigidbody2D>());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        int _layer = other.gameObject.layer;
        if (_layer != 8 && _layer != 12 || !coneTouch) return;

        contacts.Remove(other.GetComponent<Rigidbody2D>());
        CallTouchReset();
        cone.SetTouchingCone();

    }
}