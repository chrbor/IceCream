using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcMove : MonoBehaviour
{
    static int mask = (1 << 10) | (1 << 11);

    public LineRenderer leg_left;
    public LineRenderer leg_right;
    GameObject foot_left;
    GameObject foot_right;

    public float boneLength;
    public float stepLength;
    float direction;
    float boneSqrLength;
    public float stepTime;
    float timeStep;

    public AnimationCurve stepMovement;

    float prev_x;
    bool block_left, block_right;
    bool stop;


    void Start()
    {
        boneSqrLength = boneLength * boneLength;

        timeStep = Time.fixedDeltaTime / stepTime;
        foot_left = leg_left.transform.GetChild(0).gameObject;
        foot_right = leg_right.transform.GetChild(0).gameObject;
        stop = false;

        StartCoroutine(Update_Leg(leg_left, false));
        StartCoroutine(Update_Leg(leg_right, true));
    }

    private void FixedUpdate()
    {
        direction = Mathf.Sign(transform.position.x - prev_x);
        prev_x = transform.position.x;
    }

    IEnumerator Update_Leg(LineRenderer leg, bool right_leg)
    {
        GameObject foot = leg.transform.GetChild(0).gameObject;

        Vector3 footPos = foot.transform.position;//wenn der Fuß verankert ist
        //leg.useWorldSpace = true;
        while (!stop)
        {
            //Halte Fuß auf Position:
            while(direction * (leg.transform.position.x - foot.transform.position.x) <= stepLength || (right_leg && block_right) || (!right_leg && block_left))
            {
                foot.transform.position = footPos;
                UpdateUpperBone(leg, foot.transform.localPosition);
                yield return new WaitForFixedUpdate();
            }

            //Setze Fuß nach vorne:
            if (right_leg) block_left = true;
            else block_right = true;

            //Überprüfe, ob der Boden existiert:
            float rayStart = Mathf.Sign(leg.transform.position.x - foot.transform.position.x) * stepLength;
            RaycastHit2D hit = Physics2D.Raycast(leg.transform.position + Vector3.right * rayStart, Vector2.down, 3 * boneLength, mask);
            if(!hit.collider) hit = Physics2D.Raycast(leg.transform.position, Vector2.down, 3 * boneLength, mask);

            if (!hit.collider)
            {
                block_right = false;
                block_left = false;
                foot.transform.position = footPos;
                UpdateUpperBone(leg, foot.transform.localPosition);
                yield return new WaitForFixedUpdate();
                continue;
            }

            //Spiele Loop ab, mit dem der Fuß nach vorne gesetzt wird:
            Vector3 step = ((Vector3)hit.point - foot.transform.position) * timeStep;
            float y_factor = foot.transform.position.y - hit.point.y;
            Vector3 virt_Pos = foot.transform.position;
            for (float count = 0; count < 1; count += timeStep)
            {
                virt_Pos += step;
                footPos = virt_Pos + stepMovement.Evaluate(count) * Vector3.up;

                foot.transform.position = footPos;
                UpdateUpperBone(leg, foot.transform.localPosition);
                yield return new WaitForFixedUpdate();
            }

            block_right = false;
            block_left = false;
        }

        yield break;
    }

    void UpdateUpperBone(LineRenderer leg, Vector3 footPos)
    {
        //Update des Fußes und des Ankers:
        leg.SetPosition(2, footPos);


        //Update des Knies:
        Vector3 diff = leg.GetPosition(2) - leg.GetPosition(0);
        Vector3 middle = leg.GetPosition(0) + diff / 2;
        float diff_sqrBone = diff.sqrMagnitude / 4;
        if (diff_sqrBone >= boneSqrLength) leg.SetPosition(1, middle);
        else
            leg.SetPosition(1, 
                middle + (Vector3)RotToVec(Mathf.Atan2(diff.y, diff.x) + Mathf.PI/2 * direction) //Richtung vom Mittelpunkt
                * Mathf.Sqrt(boneSqrLength - diff_sqrBone));//Weite vom Mittelpunkt
    }

    /// <summary>
    /// Transforms a rotation to a vector in wspace
    /// </summary>
    /// <param name="angle">in radians</param>
    /// <returns></returns>
    public static Vector2 RotToVec(float angle) => new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
}
