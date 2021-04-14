using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TouchAndScreen;

public class ConeScript : MonoBehaviour, ICone
{
    public static ConeScript cone;
    public static Vector4 rotMatrix_cone;
    private float diff_rot;


    public float tilt_gravity;
    public Vector2 tilt_move;

    Vector2 prevPosition, diff;
    public Rigidbody2D rb { get; protected set; }

    List<IceScript> contacts = new List<IceScript>();

    public bool run;

    private void Awake()
    {
        cone = this;

        rb = GetComponent<Rigidbody2D>();
        prevPosition = rb.position;

        StartCoroutine(UpdateCone());
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
        foreach (var contact in contacts) contact.UpdateConeTouch();
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

        rb.position = PixelToWorld(Input.mousePosition);
        rb.velocity = rb.position - prevPosition;
        UpdateRotation();

        prevPosition = rb.position;
    }

    IEnumerator UpdateCone()
    {
        //Startphase:
        for (float count = 0; count < .5f; count += Time.fixedDeltaTime){
            rb.position = PixelToWorld(Input.mousePosition);
            yield return new WaitForFixedUpdate();
        }

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

        if(Mathf.Abs(rb.rotation) > 45 && Mathf.Sign(rb.rotation) == Mathf.Sign(diff_rot)) { diff_rot = 0; rotMatrix_cone = new Vector4(1, 0, 0, 1); return; }
        rb.rotation += diff_rot;

        //Aktualisiere Rotationsmatrix:
        diff_rot *= Mathf.Deg2Rad;
        rotMatrix_cone = new Vector4(Mathf.Cos(diff_rot), -Mathf.Sin(diff_rot), Mathf.Sin(diff_rot), Mathf.Cos(diff_rot));

    }
}
