using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Helper;

public class ProcMove : MonoBehaviour
{
    static int mask = (1 << 13) | (1 << 11) | (1 << 21) | (1 << 10);
    static int procAnimMask = (1 << 11) | (1 << 10);
    private Animator anim;
    bool onGround = true;
    [HideInInspector]
    public float groundDist;
    //bool hasCone = false;
    int coneDir;

    public LineRenderer leg_left;
    public LineRenderer leg_right;
    public Texture moveLeftLeg, moveRightLeg;
    Material mat_leftLeg, mat_rightLeg;
    int tex_nameID;
    GameObject foot_left;
    GameObject foot_right;
    SpriteRenderer foot_left_sprite;
    SpriteRenderer foot_right_sprite;

    public float boneLength;
    public float stepLength;
    float direction;
    [HideInInspector]
    public float speed;
    float boneSqrLength;
    public float stepTime;

    public AnimationCurve stepMovement;
    [Header("Positionen der Beine in der Luft wenn nach rechts springend:")]
    public Vector2 rightJumpPos;
    public Vector2 leftJumpPos;
    public float rightJumpAngle, leftJumpAngle;

    float prev_x;
    bool block_left, block_right;
    float blockVal_left, blockVal_right;
    bool falling;
    bool stop;


    void Start()
    {
        anim = GetComponent<Animator>();
        //SetConeActive(0);

        mat_leftLeg = leg_left.material;
        mat_rightLeg = leg_right.material;
        tex_nameID = mat_leftLeg.shader.GetPropertyNameId(0);

        boneSqrLength = boneLength * boneLength;

        foot_left = leg_left.transform.GetChild(0).gameObject;
        foot_right = leg_right.transform.GetChild(0).gameObject;
        foot_left_sprite = foot_left.GetComponent<SpriteRenderer>();
        foot_right_sprite = foot_right.GetComponent<SpriteRenderer>();
        prev_x = transform.position.x;
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
        if (speed < .0075f) direction = 0;
        prev_x = transform.position.x;

        //foot_left_sprite.flipX = direction < 0;
        //foot_right_sprite.flipX = foot_left_sprite.flipX;
        mat_leftLeg.SetTexture(tex_nameID, direction < 0 ? moveLeftLeg : moveRightLeg);
        mat_rightLeg.SetTexture(tex_nameID, direction < 0 ? moveLeftLeg : moveRightLeg);

        if (direction != 0 && coneDir == 0) transform.localScale = new Vector3(direction, 1, 1);
        anim.SetBool("Moving", direction != 0);
        anim.SetBool("OnGround", onGround);
        anim.SetFloat("speed", (coneDir == 0 ? 1 : direction * coneDir) * speed / Time.fixedDeltaTime);
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
                onGround = true;
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
                    //solange     distanz zum zentrum < stepLength && in Bewegung    ||    geblockt ist,     Ausnahme: Char fällt 
                    blockVal_left = leg.transform.position.x - foot.transform.position.x;
                    while (((direction * blockVal_left <= stepLength && Mathf.Abs(direction) > 0) || blockVal_left * direction < blockVal_right * direction || block_right) && !falling)
                    {
                        foot.transform.position = footPos;
                        UpdateUpperBone(leg, foot.transform.localPosition);

                        yield return new WaitForFixedUpdate();
                        blockVal_left = leg.transform.position.x - foot.transform.position.x;
                    }
                }
                else
                {
                    blockVal_right = leg.transform.position.x - foot.transform.position.x;
                    while (((direction * blockVal_right <= stepLength && Mathf.Abs(direction) > 0) || blockVal_right * direction < blockVal_left * direction || block_left) && !falling)
                    {
                        foot.transform.position = footPos;
                        UpdateUpperBone(leg, foot.transform.localPosition);

                        yield return new WaitForFixedUpdate();
                        blockVal_right = leg.transform.position.x - foot.transform.position.x;
                    }
                    //Debug.Log("left_leg:\nfall: " + falling + ", left: " + blockVal_right * direction + ", left_pure: " + blockVal_right);
                }
            }


            if (falling)
            {
                //Falls der Char fällt, dann spiele Fallanimation ab:
                //Die Fallanimation nährt sich schnell einer vordefinierten position an
                fallDir = coneDir == 0 ? (direction == 0 ? Mathf.Sign(transform.localScale.x) : direction) : coneDir;
                if (right_leg)
                {
                    Vector2 jumpPos =  rightJumpPos * new Vector2(fallDir, 1);// ;
                    hitpoint = jumpPos  + (Vector2)leg_right.transform.position;
                    hit2point = hitpoint + RotToVec(fallDir * /*(fallDir == 1 ? rightJumpAngle : leftJumpAngle)*/rightJumpAngle * Mathf.Deg2Rad);

                    onGround = false;
                }
                else
                {
                    Vector2 jumpPos = leftJumpPos * new Vector2(fallDir, 1);// ;
                    hitpoint = jumpPos + (Vector2)leg_left.transform.position;
                    hit2point = hitpoint + RotToVec(fallDir * /*(fallDir == 1 ? leftJumpAngle : rightJumpAngle)*/leftJumpAngle * Mathf.Deg2Rad);
                }
                animateFall = true;
            }
            else
            {
                if (direction == 0)
                {
                    if (onMoveStop && Mathf.Abs(leg.transform.position.x - foot.transform.position.x) < 0.1f)
                    {
                        yield return new WaitForFixedUpdate();
                        foot.transform.position = footPos;
                        UpdateUpperBone(leg, foot.transform.localPosition);
                        continue;
                    }
                    else onMoveStop = true;
                }
                else
                    onMoveStop = false;

                //Setze Fuß nach vorne:
                //if(Mathf.Abs(leg.transform.position.x - foot.transform.position.x) < Mathf.Abs(otherLeg.transform.position.x - otherFoot.transform.position.x)) { yield return new WaitForFixedUpdate(); continue; }
                if (right_leg) block_left = true;
                else block_right = true;

                //Überprüfe, ob der Boden existiert:
                float rayStart = stepLength;
                Vector2 origin = leg.transform.position + Vector3.right * rayStart * direction;
                hit = Physics2D.Raycast(origin, Vector2.down, 3.5f * boneLength, procAnimMask);
                if (hit.collider == null) hit = Physics2D.Raycast(new Vector2(origin.x - direction, leg.transform.position.y), Vector2.down, 3.5f * boneLength, procAnimMask);
                

                hit2 = Physics2D.Raycast(new Vector2(hit.point.x + .01f, leg.transform.position.y), Vector2.down, 5 * boneLength, procAnimMask);
                hit2 = Physics2D.Raycast(new Vector2(hit.point.x + .01f, leg.transform.position.y), Vector2.down, 5 * boneLength, procAnimMask);
                if (hit.collider == null || hit2.collider == null)
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
            Vector2 diff2 = hit2point - hitpoint;
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

                    if(coneDir == 0 && direction != loopDir && direction != 0)//Bei Richtungswechsel ändere Winkel der Füße und Beine
                    {
                        foot_rot_diff = 0;
                        foot_rot_start = (right_leg ? rightJumpAngle : leftJumpAngle) * direction;
                        //foot_rot_start = right_leg ? ;

                        start = new Vector2(direction, 1) * (right_leg ? rightJumpPos : leftJumpPos);
                        //if (right_leg) Debug.Log((direction > 0));

                        start += (right_leg ? leg_right.transform.position : leg_left.transform.position);
                        diff = Vector2.zero;
                        
                        loopDir = direction;
                    }
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
            else //if (direction == loopDir) //Verursacht den doppeltipp-stretch-Fehler!
            {
                footPos = start + diff;
                foot.transform.eulerAngles = Vector3.forward * (foot_rot_start + foot_rot_diff);
                block_right = false;
                block_left = false;
            }
        }

        yield break;
    }

    RaycastHit2D groundHit1, groundHit2;
    IEnumerator UpdateFalling()
    {
        Vector3 halfDiff = Vector3.down * 1.3f;//(leg_right.transform.position - leg_left.transform.position)/2 + Vector3.down * 1.3f;
        float legLength = 2.5f * boneLength - 1.3f;
        bool hit1IsNull, hit2IsNull;
        falling = false;
        if(stop) yield return new WaitUntil(() => !stop);
        while (!stop)
        {
            //groundHit = Physics2D.Raycast((direction > 0 ? leg_left.transform.position : leg_right.transform.position), Vector2.down, legLength, mask);
            groundHit1 = Physics2D.Raycast(leg_left.transform.position + halfDiff, Vector2.down, legLength, mask);
            groundHit2 = Physics2D.Raycast(leg_right.transform.position + halfDiff, Vector2.down, legLength, mask);
            hit1IsNull = groundHit1.collider == null;
            hit2IsNull = groundHit2.collider == null;
            falling = (hit1IsNull && hit2IsNull) 
                || (hit1IsNull && groundHit2.collider.gameObject.layer != 13) 
                || (hit2IsNull && groundHit1.collider.gameObject.layer != 13);
            groundDist = falling ? 999 : (groundHit1.distance > groundHit2.distance? groundHit1.distance : groundHit2.distance);
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
                middle + (Vector3)RotToVec(Mathf.Atan2(diff.y, diff.x) + Mathf.PI/2)// * (direction == 0 ? 1 : direction)) //Richtung vom Mittelpunkt
                * Mathf.Sqrt(boneSqrLength - diff_sqrBone));//Weite vom Mittelpunkt
    }

    public void SetConeActive(int _coneDir)
    {
        coneDir = _coneDir;
        if(coneDir != 0) transform.localScale = new Vector3(coneDir, 1, 1);
        anim.Play(coneDir == 0 ? "pIdle" : "phIdle", 0);
    }
}
