using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Helper;
using static IceManager;
using static CameraScript;
using static GameManager;

public class Cone2Script : MonoBehaviour, ICone
{
    public static Cone2Script cone;
    public static Vector4 rotMatrix_cone;
    float diff_rot_cone;

    public bool run;
    public float blockTime = 0;
    private bool isGettingDestroyed;

    public float helpForce = 40;
    public Vector2 coneGravity;
    public Vector2 tilt_move;
    public float dropSpeed;

    [HideInInspector]
    public Vector2 windForce;

    Vector2 prevPosition, diff;
    public Rigidbody2D rb { get; protected set; }

    [HideInInspector]
    public List<ICone> iceTower = new List<ICone>();

    public float camSize;


    private void Awake()
    {
        cone = this;
        iceTower.Add(this);

        rb = GetComponent<Rigidbody2D>();
        prevPosition = rb.position;

        StartCoroutine(StartCone());

        FireIce += FiringIce;
    }

    private void OnDestroy()
    {
        isGettingDestroyed = true;
        FireIce -= FiringIce;
    }

    //Accessor:
    public int GetID() => 0;
    public void SetID(int _id) { /*id der Waffel ist immer gleich 0*/}
    public IceAttribute Get_attribute() => null;
    public void ResetAttributes() { }
    public Vector2 Get_posInCone() => Vector2.zero;
    public void Set_posInCone(Vector2 position) { /*redundant*/}
    public void Set_prevIce(Rigidbody2D _prev) { /*redundant*/}
    public Vector2 Get_virtPosInCone() => Vector2.zero;
    public void Set_virtPosInCone(Vector2 position) { }
    public IEnumerator ShootIce(ICone prev) { yield break; }
    public Rigidbody2D Get_rb() => rb;
    public string Get_name() => name;
    public Transform Get_transform() => transform.GetChild(0);
    public bool GettingDestroyed() => isGettingDestroyed;

    //Aufgerufen, wenn eis hinzugefügt wird:
    public void UpdateConeTower(int startPtr)
    {
        for (int i = startPtr; i < iceTower.Count; i++)
        {
            iceTower[i].SetID(i);

            //if (i == 1) iceTower[i].Get_transform().localPosition = Vector3.up * 1.5f;//Positioniere die erste Kugel auf die Waffel
            if (i == iceTower.Count - 1)
            {
                iceTower[i].Set_posInCone(i == 1 ?
                    Vector2.up * (iceTower[1].Get_transform().localScale.x + iceTower[0].Get_transform().localScale.x) * .65f ://Positioniere die erste Kugel auf die Waffel
                    i == startPtr ?//Wenn falsch: aufgerutscht
                        iceTower[i - 1].Get_posInCone() + (iceTower[i - 1].Get_posInCone() - iceTower[i - 2].Get_posInCone()).normalized * (iceTower[i].Get_transform().localScale.x + iceTower[i - 1].Get_transform().localScale.x) * .3f ://Spitze des Eis
                        iceTower[i].Get_posInCone() + (iceTower[i].Get_posInCone() - iceTower[i - 2].Get_posInCone()).normalized * (iceTower[i].Get_transform().localScale.x + iceTower[i - 1].Get_transform().localScale.x) * .3f
                        );
            }
            else /*if (i != 1)*/ iceTower[i].Set_posInCone(iceTower[i + 1].Get_posInCone());//Lasse alle anderen Kugeln um eine Kugel nach oben wandern

            iceTower[i].Set_prevIce(iceTower[i-1].Get_rb());

            //Überprüfe auf Misch- Attribut:
            if(i > 2 && iceTower[i - 2].Get_attribute().isMelange && !(iceTower[i - 1].Get_attribute().isMelange || iceTower[i].Get_attribute().isMelange))
            {
                //Kombiniere die mittlere Kugel mit der oberen:
                iceTower[i - 1].Get_attribute().Combine(iceTower[i].Get_attribute());
                iceTower[i - 1].ResetAttributes();
                //Lösche anschließend die oberste und die unterste Kugel:
                RemoveCombinedIce(i);
                i -= 2;
                //Hier evtl. Effekte hinzufügen:
            }
        }
    }

    public void RemoveCombinedIce(int id_removed)
    {
        //if (iceTower[id_removed].GettingDestroyed()) return;

        for (int i = iceTower.Count - 1; i > id_removed; i--) iceTower[i].SetID(i - 2);
        iceTower[id_removed - 1].SetID(id_removed - 2);


        Destroy(iceTower[id_removed].Get_transform().gameObject);
        iceTower.RemoveAt(id_removed);
        Destroy(iceTower[id_removed - 2].Get_transform().gameObject);
        iceTower.RemoveAt(id_removed - 2);
    }
    public void RemoveIce(int id_removed)
    {
        for (int i = iceTower.Count - 1; i > id_removed; i--) iceTower[i].SetID(i - 1);
        iceTower[id_removed].Get_transform().parent = null;
        iceTower.RemoveAt(id_removed);
    }


    //public void FiringIce() => StartCoroutine(ShootIce());
    public void FiringIce()
    {
        int i = iceTower.Count - 1;
        if (i == 0 || pauseGame) return;
        StartCoroutine(iceTower[i].ShootIce(iceTower[i - 1]));
        iceTower.RemoveAt(i);
    }
    bool block_shoot;
    IEnumerator ShootIce()
    {
        if (block_shoot) yield break;
        block_shoot = true;

        int i = iceTower.Count - 1;
        bool next;
        do
        {
            if (i == 0) break;
            StartCoroutine(iceTower[i].ShootIce(iceTower[i - 1]));
            next = i > 1 ? iceTower[i].Get_attribute().sticky || iceTower[i-1].Get_attribute().sticky : false;
            iceTower.RemoveAt(i--);

            yield return new WaitForFixedUpdate();//WaitForSeconds(.1f);
        } while (next);

        block_shoot = false;
        yield break;
    }


    void FixedUpdate()
    {
        if (!run || pauseGame) { windForce = Vector2.zero; return; }

        rb.velocity = rb.position - prevPosition;
        //Rotiere zur letzten Eiskugel:
        diff_rot_cone = .01f * ((diff_info.Count > 2 ? diff_info[diff_info.Count - 1].rotation : 0) - rb.rotation);
        rb.rotation += diff_rot_cone;

        UpdateIce();

        prevPosition = rb.position;
        windForce = Vector2.zero;

        //Test:
        //for (int i = 1; i < iceTower.Count; i++) iceTower[i].SetID(i);
    }

    IEnumerator StartCone()
    {
        yield return new WaitForFixedUpdate();
        player.transform.GetChild(0).GetComponent<ProcMove>().SetConeActive(1);

        yield return new WaitForSeconds(.5f);
        run = true;
        yield break;
    }
    public void blockCone() { if(!isGettingDestroyed) StartCoroutine(BlockingCone()); }
    IEnumerator BlockingCone()
    {
        if (blockTime > 0) { blockTime += .1f; yield break; }
        run = false;
        blockTime = .1f;
        for (; blockTime > 0; blockTime -= Time.fixedDeltaTime) yield return new WaitForFixedUpdate();
        run = true;
        yield break;
    }

    private class Diff_info
    {
        public Vector2 position;
        public float rotation;

        public Diff_info(Vector2 _position, float _rotation)
        {
            position = _position;
            rotation = _rotation;
        }
    }
    List<Diff_info> diff_info = new List<Diff_info>();

    private void UpdateIce()
    {
        Vector2 diff_pos;
        Vector2 posInCone;
        float rot_prev = 0;

        diff_info.Clear();
        for (int i = 0; i < iceTower.Count; i++)
        {
            diff_pos = iceTower[i].Get_transform().position - transform.position;
            float diff_rot = Mathf.Atan2(diff_pos.y, diff_pos.x) * Mathf.Rad2Deg - 90;
            while (Mathf.Abs(diff_rot) > 180) diff_rot -= 360 * Mathf.Sign(diff_rot);

            diff_info.Add(new Diff_info(diff_pos, diff_rot));
        }

        //for(int i = 2; i < iceTower.Count; i++)
        for(int i = iceTower.Count -1; i >= 2; i--)
        {
            //Manueller-Ansatz:
            if (iceTower[i].GetID() < 0) continue;

            //Sin-Wave:
            //float weight = iceTower.Count == 3? 1 : Mathf.Sin(Mathf.PI * (i - 2) / (2 * (iceTower.Count - 3f)));
            //float weight = iceTower.Count == 3 ? 1 : (i - 2) / (iceTower.Count - 3f) ;
            float weight = i / (i + 2f);
            float dist_Factor = 1 - 2000 / (2000 + diff_info[i].position.sqrMagnitude);
            //float dist_Factor = .05f * (diff_info[i].position.sqrMagnitude / diff_info[diff_info.Count - 1].position.sqrMagnitude);//1 - 2000 / (2000 + diff_info[i].position.sqrMagnitude);
            //Debug.Log("weight: " + weight);// * (1 - dist_Factor));
            //Debug.Log(iceTower[i].Get_name() + ": " + dist_Factor);

            //Diff zur vorherigen Kugel:
            int ptr = i == iceTower.Count - 1 ? iceTower.Count - 2 : i;
            Vector2 diff_cntct_pos = iceTower[ptr].Get_transform().position - iceTower[ptr + 1].Get_transform().position;

            float rot_cntct = weight * (diff_info[ptr+1].rotation - diff_info[ptr].rotation) + (1 - weight) * (diff_info[ptr - 1].rotation - diff_info[ptr].rotation);
            rot_cntct -= Mathf.Sin(diff_info[ptr + 1].rotation * Mathf.Deg2Rad) * 100 * dist_Factor;
            while (Mathf.Abs(rot_cntct) > 180) rot_cntct -= 360 * Mathf.Sign(rot_cntct);

            //da juicy stuff ;P
            float rot;
            if(i == iceTower.Count - 1)
            {
                Vector2 rot_move = rb.velocity * tilt_move * RotToVec(diff_info[i].rotation * Mathf.Deg2Rad);
                rot = diff_info[i].rotation * dist_Factor * (Mathf.Sign(diff_info[i].rotation) == Mathf.Sign(rb.velocity.x) || rb.velocity.x == 0 ? coneGravity.x : coneGravity.y) //Gravity
                + (10 + Mathf.Sign(rb.velocity.x) == Mathf.Sign(diff_info[i].rotation) ? 0 : helpForce) * (rot_move.x + rot_move.y)//Movement
                + Mathf.Sin((diff_info[i].rotation - 90) * Mathf.Deg2Rad - Mathf.Atan2(windForce.y, windForce.x)) * windForce.magnitude;
                //Debug.Log("coneRot: " + (diff_info[i].rotation + 90) + ", windRot: " + Mathf.Atan2(windForce.y, windForce.x) * Mathf.Rad2Deg + ", diff: " + ((diff_info[i].rotation + 90) * Mathf.Deg2Rad - Mathf.Atan2(windForce.y, windForce.x)) * Mathf.Rad2Deg);
                //Debug.Log(Mathf.Sin((diff_info[i].rotation + 90) * Mathf.Deg2Rad - Mathf.Atan2(windForce.y, windForce.x)));
            }
            else
                rot = rot_cntct * weight * 1 * (1 - weight);

            //Stelle sicher, dass der Kontakt zur Vorherigen Kugel aufrecht erhalten wird
            if (i != iceTower.Count-1 && (Mathf.Sign(rot_prev) == Mathf.Sign(rot_cntct) || Mathf.Abs(rot_prev) > Mathf.Abs(rot_cntct)) && diff_cntct_pos.sqrMagnitude > (iceTower[i + 1].Get_attribute().scale + iceTower[i].Get_attribute().scale) * .325f) rot = rot_prev;

            //Rotiere:
            posInCone = iceTower[i].Get_posInCone();
            Vector4 rot_Matrix = GetRotationMatrix(weight * rot * Mathf.Deg2Rad);
            posInCone = new Vector2(rot_Matrix.x * posInCone.x + rot_Matrix.y * posInCone.y,
                                    rot_Matrix.z * posInCone.x + rot_Matrix.w * posInCone.y);
            rot_prev = rot;

            //Aktualisiere y_position auf der Waffel:
            Vector2 diffToPrev = iceTower[i - 1].Get_transform().position - iceTower[i].Get_transform().position;
            float scaleFactor = (iceTower[i - 1].Get_attribute().scale + iceTower[i].Get_attribute().scale) * .35f;//formerly /2 *.65f
            if(diffToPrev.sqrMagnitude > scaleFactor * scaleFactor)
            {
                posInCone += diffToPrev.normalized * dropSpeed;
            }


            iceTower[i].Set_posInCone(posInCone);
        }

        //Aktualisiere y_position der ersten Eiskugel auf der Waffel:
        if(iceTower.Count > 1)
        {
            iceTower[1].SetID(1);
            Vector2 diffToPrev = Vector3.up * .65f - iceTower[1].Get_transform().localPosition;
            if (diffToPrev.sqrMagnitude > (1 + iceTower[1].Get_attribute().scale) * .35f)//formerly /2 *.65f
            {
                iceTower[1].Set_posInCone(Vector3.up * (iceTower[1].Get_posInCone() + diffToPrev.normalized * dropSpeed));
            }
            else iceTower[1].Set_posInCone(Vector3.up * (1 + iceTower[1].Get_attribute().scale * .35f));
        }
    }
}
