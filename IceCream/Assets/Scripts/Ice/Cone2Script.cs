using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Helper;
using static IceManager;

public class Cone2Script : MonoBehaviour, ICone
{
    public static Cone2Script cone;
    public static Vector4 rotMatrix_cone;
    public static Vector2 moveForce_cone;
    private float diff_rot;

    public float maxRotation = 45;
    public float helpFactor = 1;
    [HideInInspector]
    public float helpForceMagnitude = 0;
    [HideInInspector]
    public Vector2 coneGravity;
    public float coneGravityFactor;

    public float tilt_gravity;
    public Vector2 tilt_move;
    public float autoRotation;
    float autoRotate;

    Vector2 prevPosition, diff;
    public Rigidbody2D rb { get; protected set; }

    List<IceScript> contacts = new List<IceScript>();

    public bool run;


    private void Awake()
    {
        cone = this;

        rb = GetComponent<Rigidbody2D>();
        prevPosition = rb.position;

        StartCoroutine(StartCone());
    }

    public bool TouchingCone() => true;
    public float GetDamping() => 99;
    public void UpdateConeTouch() { }
    public void SetTouchingCone()
    {
        StartCoroutine(SettingTouchingCone());
    }
    IEnumerator SettingTouchingCone()
    {
        yield return new WaitForFixedUpdate();
        contacts.Clear();
        List<Collider2D> cols = new List<Collider2D>();
        GetComponent<Collider2D>().GetContacts(cols);
        int sub = 0;
        for (int i = 0; i < cols.Count; i++)
        {
            if (cols[i].tag != "Col")
            {
                contacts.Add(cols[i].GetComponent<IceScript>());
                contacts[i - sub].UpdateConeTouch();
            }
            else sub++;
        }
        yield break;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        int _layer = other.gameObject.layer;
        if (_layer != 8) return;//Nur Eis

        contacts.Add(other.GetComponent<IceScript>());
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        int _layer = other.gameObject.layer;
        if (_layer != 8) return;//Nur Eis

        contacts.Remove(other.GetComponent<IceScript>());
    }

    
    void FixedUpdate()
    {
        if (!run) return;

        rb.velocity = rb.position - prevPosition;
        UpdateRotation();

        //Noch im Test:
        helpForceMagnitude = helpFactor * (Mathf.Abs(rb.velocity.x) + 1) * Mathf.Abs(transform.eulerAngles.z / maxRotation);
        coneGravity = iceGravity - rb.velocity;

        prevPosition = rb.position;
    }
    
    IEnumerator StartCone()
    {
        yield return new WaitForSeconds(.5f);
        run = true;
        yield break;
    }


    private void UpdateRotation()
    {
        diff_rot = 0;

        //Tilt_move:
        if (Mathf.Abs(rb.velocity.y) < 100)
            diff_rot =
            Mathf.Cos(rb.rotation * Mathf.Deg2Rad) * rb.velocity.x * tilt_move.x
          + Mathf.Sin(rb.rotation * Mathf.Deg2Rad) * rb.velocity.y * tilt_move.y;

        //tilt_gravity:
        diff_rot += Mathf.Sin(rb.rotation * Mathf.Deg2Rad) * tilt_gravity;
        if (Mathf.Abs(rb.rotation) > maxRotation && Mathf.Sign(rb.rotation) == Mathf.Sign(diff_rot)) { diff_rot = 0; rotMatrix_cone = new Vector4(1, 0, 0, 1); return; }

        //Auto-Ausrichten:
        autoRotate = autoRotation * 10 / (10 + coneIceCount);
        if(Mathf.Abs(rb.rotation) > autoRotate) diff_rot -= autoRotate * Mathf.Sign(rb.rotation) * (2 - Mathf.Cos(rb.rotation));

        rb.rotation += diff_rot;

        //Aktualisiere Rotationsmatrix:
        rotMatrix_cone = GetRotationMatrix(diff_rot * Mathf.Deg2Rad);

    }
}
