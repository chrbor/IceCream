using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Helper;
using static IceManager;
using static CameraScript;

public class Cone2Script : MonoBehaviour, ICone
{
    public static Cone2Script cone;
    public static Vector4 rotMatrix_cone;
    public static Vector2 moveForce_cone;
    public float diff_rot_cone;

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

    public List<ICone> iceTower = new List<ICone>();

    public bool run;


    private void Awake()
    {
        cone = this;
        iceTower.Add(this);

        rb = GetComponent<Rigidbody2D>();
        prevPosition = rb.position;

        StartCoroutine(StartCone());
    }

    public int GetID() => 0;
    public void SetID(int _id) { /*id der Waffel ist immer gleich 0*/}
    public Vector2 Get_posInCone() => Vector2.zero;
    public void Set_posInCone(Vector2 position) { /*redundant*/}
    public void Set_prevIce(Rigidbody2D _prev) { /*redundant*/}

    public Rigidbody2D Get_rb() => rb;
    public string Get_name() => name;
    public Transform Get_transform() => transform.GetChild(0);

    public void UpdateConeTower(int startPtr)
    {
        for(int i = startPtr; i < iceTower.Count; i++)
        {
            iceTower[i].SetID(i);

            //if (i == 1) iceTower[i].Get_transform().localPosition = Vector3.up * 1.5f;//Positioniere die erste Kugel auf die Waffel
            if (i == iceTower.Count - 1)
            {
                iceTower[i].Get_transform().localPosition = i == 1 ?
                    iceTower[i].Get_transform().localPosition = Vector3.up * (iceTower[i].Get_transform().localScale.x + iceTower[i - 1].Get_transform().localScale.x) * .9f ://Positioniere die erste Kugel auf die Waffel
                    i == startPtr ?
                        iceTower[i - 1].Get_transform().localPosition + (iceTower[i - 1].Get_transform().localPosition - iceTower[i - 2].Get_transform().localPosition).normalized * (iceTower[i].Get_transform().localScale.x + iceTower[i - 1].Get_transform().localScale.x) * .4f ://Spitze des Eis
                        iceTower[i].Get_transform().localPosition + (iceTower[i].Get_transform().localPosition - iceTower[i - 1].Get_transform().localPosition).normalized * (iceTower[i].Get_transform().localScale.x + iceTower[i - 1].Get_transform().localScale.x) * .75f;
                //2 * iceTower[i].Get_transform().localPosition - iceTower[i - 1].Get_transform().localPosition;//Positioniere die letzte Kugel auf die vorletzte
                //Debug.Log();
            }
            else if(i != 1) iceTower[i].Get_transform().localPosition = iceTower[i + 1].Get_transform().localPosition;//Lasse alle anderen Kugeln um eine Kugel nach oben wandern

            iceTower[i].Set_posInCone(iceTower[i].Get_transform().localPosition);
            iceTower[i].Set_prevIce(iceTower[i-1].Get_rb());
        }
    }

    
    void FixedUpdate()
    {
        if (!run) return;

        rb.velocity = rb.position - prevPosition;
        UpdateRotation();
        UpdateIce();

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
        /*
        diff_rot_cone = 0;

        float iceCountFactor = 10 / (10f + coneIceCount);
        //Tilt_move:
        if (Mathf.Abs(rb.velocity.y) < 100)
            diff_rot_cone = iceCountFactor *
                (Mathf.Cos(rb.rotation * Mathf.Deg2Rad) * rb.velocity.x * tilt_move.x
                +Mathf.Sin(rb.rotation * Mathf.Deg2Rad) * rb.velocity.y * tilt_move.y);

        //tilt_gravity:
        diff_rot_cone += Mathf.Sin(rb.rotation * Mathf.Deg2Rad) * tilt_gravity;

        //Garantiere, dass sich das Eis wieder aufstellen kann:
        if (rb.velocity.x != 0 && Mathf.Sign(rb.velocity.x) != Mathf.Sign(rb.rotation) && -diff_rot_cone * Mathf.Sign(rb.rotation) < .1f ) diff_rot_cone = -Mathf.Sign(rb.rotation) * .1f; 
        //Schränke den Winkel ein:
        if (Mathf.Abs(rb.rotation) > maxRotation && Mathf.Sign(rb.rotation) == Mathf.Sign(diff_rot_cone)) { diff_rot_cone = 0; rotMatrix_cone = new Vector4(1, 0, 0, 1); return; }

        rb.rotation += diff_rot_cone;
        //*/
        //Aktualisiere Rotationsmatrix:
        rotMatrix_cone = GetRotationMatrix(0);// diff_rot_cone * Mathf.Deg2Rad);


    }

    private void UpdateIce()
    {
        Vector2 diff_pos;
        Vector2 posInCone;
        float rot_prev = 0;

        //for(int i = 2; i < iceTower.Count; i++)
        for(int i = iceTower.Count -1; i >= 2; i--)
        {
            //Manueller-Ansatz:

            //Diff zur Waffel:
            diff_pos = iceTower[i].Get_transform().position - transform.position;
            float diff_rot = Mathf.Atan2(diff_pos.y, diff_pos.x) * Mathf.Rad2Deg - 90;
            while (Mathf.Abs(diff_rot) > 180) diff_rot -= 360 * Mathf.Sign(diff_rot);

            //Diff zur vorherigen Kugel:
            //Vector2 diff_cntct_pos = iceTower[i].Get_transform().position - iceTower[i-1].Get_transform().position;
            Vector2 diff_cntct_pos = i >= iceTower.Count - 1 ? (Vector3)diff_pos : iceTower[i].Get_transform().position - iceTower[i + 1].Get_transform().position;
            //Vector2 diff_pre_pos = iceTower[i-1].Get_transform().position - iceTower[i-2].Get_transform().position;
            Vector2 diff_pre_pos = i >= iceTower.Count - 2 ? (Vector3)diff_cntct_pos : iceTower[i + 1].Get_transform().position - iceTower[i + 2].Get_transform().position;//iceTower[i-1].Get_transform().position - iceTower[i-2].Get_transform().position;
            float rot_cntct = (Mathf.Atan2(diff_cntct_pos.y, diff_cntct_pos.x) - Mathf.Atan2(diff_pre_pos.y, diff_pre_pos.x)) * Mathf.Rad2Deg;
            while (Mathf.Abs(rot_cntct) > 180) rot_cntct -= 360 * Mathf.Sign(rot_cntct);

            float dist_Factor = 1 - 1000 / (1000 + diff_pos.sqrMagnitude);
            //diff_rot *= dist_Factor * 0.1f;
            //rot_cntct *= dist_Factor * 0.1f;


            //Gegenrotation, die bei Schräglage durch Bewegung trotzdem gegendreht: 
            /*
            if (iceTower[i].Get_name() == "Ice (9)")
            {
                Debug.Log((rb.velocity.x * (1 - Mathf.Cos(diff_rot * Mathf.Deg2Rad)) * dist_Factor * 1e7f) + ", added to " + diff_rot + ", is added: " + (Mathf.Sign(rb.velocity.x) != Mathf.Sign(diff_rot)));
                //Debug.Log("rot: " + rot + ", rot_prev: " + rot_prev);
            }
            //*/


            //float rot = diff_rot * dist_Factor * 0.1f + rot_prev + (Mathf.Sign(rb.velocity.x) == Mathf.Sign(diff_rot) ? 0 : rb.velocity.x * (1 - Mathf.Cos(diff_rot * Mathf.Deg2Rad)) * dist_Factor * 2e7f);
            float rot = diff_rot * dist_Factor * 0.1f + rot_prev + 10 * rb.velocity.x * (Mathf.Cos(diff_rot * Mathf.Deg2Rad) - Mathf.Abs(Mathf.Sin(diff_rot * Mathf.Deg2Rad))) * dist_Factor;


            //Stelle sicher, dass der Kontakt zur Vorherigen Kugel aufrecht erhalten wird
            //if (iceTower[i].Get_name() == "Ice (9)") Debug.Log("rot: " + rot + ", rot_cone: " + rb.rotation +  ", dist: " + diff_cntct_pos.sqrMagnitude);
            if ((Mathf.Sign(rot) == Mathf.Sign(rot_cntct) || Mathf.Abs(rot) < Mathf.Abs(rot_prev)) && diff_cntct_pos.sqrMagnitude > .75) rot = rot_prev;
            //if (iceTower[i].Get_name() == "Ice (9)") Debug.Log("cone_rot: " + diff_rot + ", rot: " + rot);
            //Debug.Log("overthrown: " + (Mathf.Sign(rb.rotation) == Mathf.Sign(rot) && diff_cntct_pos.sqrMagnitude > .5));

            //Rotiere:
            posInCone = iceTower[i].Get_posInCone();
            Vector4 rot_Matrix = GetRotationMatrix(rot * Mathf.Deg2Rad);
            iceTower[i].Set_posInCone(new Vector2(rot_Matrix.x * posInCone.x + rot_Matrix.y * posInCone.y,
                                                  rot_Matrix.z * posInCone.x + rot_Matrix.w * posInCone.y));
            rot_prev = rot;
        }
    }
}
