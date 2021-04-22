using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Helper;

public class ProcMove : MonoBehaviour
{
    static int mask = (1 << 10) | (1 << 11);

    public LineRenderer leg_left;
    public LineRenderer leg_right;
    GameObject foot_left;
    GameObject foot_right;

    public float boneLength;
    public float stepLength;
    float direction, speed;
    float boneSqrLength;
    public float stepTime;

    public AnimationCurve stepMovement;
    [Header("Positionen der Beine in der Luft wenn nach rechts springend:")]
    public Vector2 rightJumpPos, leftJumpPos;
    public float rightJumpAngle, leftJumpAngle;

    float prev_x;
    bool block_left, block_right;
    bool falling;
    bool stop;


    void Start()
    {
        boneSqrLength = boneLength * boneLength;

        foot_left = leg_left.transform.GetChild(0).gameObject;
        foot_right = leg_right.transform.GetChild(0).gameObject;
        stop = false;


        StartCoroutine(UpdateFalling());
        StartCoroutine(Update_Leg(leg_left, leg_right, false));
        StartCoroutine(Update_Leg(leg_right, leg_left, true));
    }

    private void FixedUpdate()
    {
        speed = transform.position.x - prev_x;
        direction = Mathf.Sign(speed);
        speed = Mathf.Abs(speed);
        if (speed < .05f) direction = 0;
        prev_x = transform.position.x;
    }

    IEnumerator Update_Leg(LineRenderer leg, LineRenderer otherLeg, bool right_leg)
    {
        yield return new WaitForFixedUpdate();
        GameObject foot = leg.transform.GetChild(0).gameObject;
        GameObject otherFoot = otherLeg.transform.GetChild(0).gameObject;
        Vector3 footPos = foot.transform.position;//wenn der Fuß verankert ist

        RaycastHit2D hit, hit2;
        Vector2 hitpoint = Vector2.zero, hit2point = Vector2.zero;
        bool onMoveStop = false;
        bool animateFall = false;
        float fallDir = 1;
        //bool afterFall = false;

        //leg.useWorldSpace = true;
        while (!stop)
        {
            //Halte Fuß auf Position:
            if (animateFall)
            {
                animateFall = false;
                //afterFall = true;
                onMoveStop = false;
                if(right_leg ^ block_right)
                    for (float counter = 0; counter < .1f; counter += Time.fixedDeltaTime)
                    {
                        footPos = foot.transform.position;
                        UpdateUpperBone(leg, foot.transform.localPosition);
                        yield return new WaitForFixedUpdate();
                    }
            }
            else
            {
                if (right_leg)
                {
                    while(((direction * (leg.transform.position.x - foot.transform.position.x) <= stepLength && Mathf.Abs(direction) > 0) || block_right) && !falling)
                    {
                        foot.transform.position = footPos;
                        UpdateUpperBone(leg, foot.transform.localPosition);
                        yield return new WaitForFixedUpdate();
                    }
                }
                else
                {
                    while (((direction * (leg.transform.position.x - foot.transform.position.x) <= stepLength && Mathf.Abs(direction) > 0) || block_left) && !falling)
                    {
                        foot.transform.position = footPos;
                        UpdateUpperBone(leg, foot.transform.localPosition);
                        yield return new WaitForFixedUpdate();
                    }
                }
            }


            if (falling)
            {
                //Falls der Char fällt, dann spiele Fallanimation ab:
                //Die Fallanimation nährt sich schnell einer vordefinierten position an
                fallDir = direction == 0 ? 1 : direction;
                if (right_leg)
                {
                    Vector2 jumpPos = (fallDir == 1 ? rightJumpPos : leftJumpPos) * new Vector2(fallDir, 1);
                    hitpoint = jumpPos  + (Vector2)leg_right.transform.position;
                    hit2point = jumpPos + RotToVec(fallDir * rightJumpAngle);
                }
                else
                {
                    Vector2 jumpPos = (fallDir == 1 ? leftJumpPos : rightJumpPos) * new Vector2(fallDir, 1);
                    hitpoint = jumpPos + (Vector2)leg_left.transform.position;
                    hit2point = jumpPos + RotToVec(fallDir * leftJumpAngle);
                }
                animateFall = true;
            }
            else
            {
                if (direction == 0)
                {
                    if (onMoveStop) { yield return new WaitForFixedUpdate(); continue; }
                    else onMoveStop = true;
                }
                else onMoveStop = false;

                //Setze Fuß nach vorne:
                //if(Mathf.Abs(leg.transform.position.x - foot.transform.position.x) < Mathf.Abs(otherLeg.transform.position.x - otherFoot.transform.position.x)) { yield return new WaitForFixedUpdate(); continue; }
                if (right_leg) block_left = true;
                else block_right = true;

                //Überprüfe, ob der Boden existiert:
                float rayStart = stepLength * (1 + speed * 5);
                Vector2 origin = leg.transform.position + Vector3.right * rayStart * direction;
                hit = Physics2D.Raycast(origin, Vector2.down, 3.5f * boneLength, mask);
                if (!hit.collider) hit = Physics2D.Raycast(new Vector2(origin.x - direction, leg.transform.position.y), Vector2.down, 3.5f * boneLength, mask);
                

                hit2 = Physics2D.Raycast(new Vector2(hit.point.x + .01f, leg.transform.position.y), Vector2.down, 5 * boneLength, mask);
                if (!(hit.collider || hit2.collider))
                {
                    block_right = false;
                    block_left = false;
                    foot.transform.position = footPos;
                    UpdateUpperBone(leg, foot.transform.localPosition);
                    yield return new WaitForFixedUpdate();
                    continue;
                }
                hitpoint = hit.point;
                hit2point = hit2.point;
            }

            //Spiele Loop ab, mit dem der Fuß nach vorne gesetzt wird:
            float foot_rot_start = foot.transform.eulerAngles.z;
            Vector2 diff2 = hitpoint - hit2point;
            float foot_rot_goal = Mathf.Atan2(diff2.y, diff2.x) * Mathf.Rad2Deg;
            float foot_rot_diff = Mathf.Atan2(diff2.y, diff2.x) * Mathf.Rad2Deg - foot_rot_start;
            while (Mathf.Abs(foot_rot_diff) > 180) foot_rot_diff -= 360 * Mathf.Sign(foot_rot_diff);

            Vector3 start = foot.transform.position;
            Vector3 diff = ((Vector3)hitpoint - foot.transform.position);
            float y_factor = foot.transform.position.y - hitpoint.y;
            Vector3 virt_Pos = foot.transform.position;
            Vector3 prev = transform.position;
            float loopDir = direction;
            float count = 0;
            while (count < 1)
            {
                if (animateFall)
                {
                    if (!falling) break;
                    start += transform.position - prev;
                    count += (1 - count) / 10;
                }
                else
                {
                    if (direction != loopDir || falling) break;
                    count += direction == 0? .1f : stepTime * speed * Time.fixedDeltaTime;
                }

                virt_Pos = start + diff * count;
                footPos = virt_Pos + stepMovement.Evaluate(count) * Vector3.up;

                foot.transform.position = footPos;
                foot.transform.eulerAngles = Vector3.forward * (foot_rot_start + foot_rot_diff * count);
                UpdateUpperBone(leg, foot.transform.localPosition);
                prev = transform.position;
                yield return new WaitForFixedUpdate();
            }

            if (animateFall)
            {
                block_right = direction > 0;
                block_left = !block_right;
            }
            else if (direction == loopDir)
            {
                footPos = start + diff;
                foot.transform.eulerAngles = Vector3.forward * (foot_rot_start + foot_rot_diff);
                block_right = false;
                block_left = false;
            }
        }

        yield break;
    }

    IEnumerator UpdateFalling()
    {
        Vector3 halfDiff = (leg_right.transform.position - leg_left.transform.position)/2;
        float legLength = 2.75f * boneLength;
        falling = false;
        yield return new WaitUntil(() => !stop);
        while (!stop)
        {
            falling = Physics2D.Raycast(leg_left.transform.position + halfDiff, Vector2.down, legLength, mask).collider == null;
            yield return new WaitForFixedUpdate();
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
                middle + (Vector3)RotToVec(Mathf.Atan2(diff.y, diff.x) + Mathf.PI/2 * (direction == 0 ? 1 : direction)) //Richtung vom Mittelpunkt
                * Mathf.Sqrt(boneSqrLength - diff_sqrBone));//Weite vom Mittelpunkt
    }
}
