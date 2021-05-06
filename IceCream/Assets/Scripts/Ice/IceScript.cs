using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Helper;
using static IceManager;
using static Cone2Script;
using static CameraScript;

public class IceScript : MonoBehaviour, ICone
{
    Vector3 prevPos;
    [HideInInspector]
    public Vector2 velocity;
    [HideInInspector]
    public int id = -1;

    Rigidbody2D prevIce;
    Vector2 diff_vel, diff_pos;

    Rigidbody2D rb;
    private Vector2 posInCone;

    private SpriteRenderer sprite;

    public IceAttribute attribute;
    IceAttribute _attribute;

    private void Start()
    {
        ResetTouch += ResetTouchingCone;

        //Init:
        rb = GetComponent<Rigidbody2D>();
        prevPos = transform.position;

        //Setze alle Attribute:
        _attribute = ScriptableObject.CreateInstance<IceAttribute>();
        //Aktiv/Passiv:
        _attribute.Set_Attribute(attribute);
        transform.localScale = Vector3.one * _attribute.scale;
        rb.mass = _attribute.mass;
        rb.sharedMaterial = _attribute.material;
        //Visuell:
        //name = _attribute.nameIce;
        sprite = GetComponent<SpriteRenderer>();
        sprite.sprite = _attribute.primSprite.sprite;
        sprite.sortingOrder = 1000;

        //Folgendes Segment sollte noch optimiert werden:
        //Lösche alte Texturen:
        for (int i = transform.childCount - 1; i > 0; i--) Destroy(transform.GetChild(i).gameObject);

        //Generiere neue Texturen:
        GameObject secObj;
        SpriteRenderer secSprite;
        for(int i = 0; i < _attribute.secSprites.Count;)
        {
            secObj = new GameObject("sec_" + i);
            secObj.transform.parent = transform;
            secObj.transform.localPosition = Vector3.zero;
            secSprite = secObj.AddComponent<SpriteRenderer>();
            secSprite.sprite = _attribute.secSprites[i].sprite;
            secSprite.sortingLayerName = "Ice";
            secSprite.sortingOrder = sprite.sortingOrder + ++i;
        }

        StartCoroutine(RunSim());
    }

    private void OnDestroy()
    {
        ResetTouch -= ResetTouchingCone;
    }

    //Accessor:
    public int GetID() => id;
    public void SetID(int _id)
    {
        id = _id;
        sprite.sortingOrder = -id * 1000;
        for (int i = 1; i < transform.childCount; i++) transform.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = sprite.sortingOrder + i;
    }
    public IceAttribute Get_attribute() => _attribute;
    public Rigidbody2D Get_rb() => rb;
    public Vector2 Get_posInCone() => posInCone;
    public void Set_posInCone(Vector2 position) => posInCone = position;
    public void Set_prevIce(Rigidbody2D _prev) { prevIce = _prev; }
    public Vector2 Get_virtPosInCone() => virtPosInCone;
    public Transform Get_transform() => transform;
    public string Get_name() => name;
    public void ResetTouchingCone() { id = -1; transform.parent = null; prevIce = null; }

    IEnumerator RunSim()
    {
        //yield return new WaitForSeconds(2);
        _attribute.runSim = true;
        while (_attribute.runSim)
        {
            //Waffel-Physik:
            if (id > 0)
            {
                Vector2 diff_pos = transform.position - cone.transform.position;
                float rot = Mathf.Atan2(diff_pos.y, diff_pos.x) * Mathf.Rad2Deg - 90;
                while (Mathf.Abs(rot) > 180) rot -= 360 * Mathf.Sign(rot);

                transform.localPosition = posInCone * (1 - Mathf.Abs(rot) / 360);
                if(!fillingSpace) virtPosInCone = posInCone;
            }
            else
                rb.velocity += iceGravity;
            
            prevPos = transform.position;
            yield return new WaitForFixedUpdate();
        }

        //Am Boden Kleben Bleiben:
        if (_attribute.stickyParent != null)
        {
            //block_ice_enter = true;
            while (!_attribute.runSim) { transform.position = posInCone + (Vector2)_attribute.stickyParent.position; yield return new WaitForFixedUpdate(); }
            _attribute.stickyParent = null;
            StartCoroutine(RunSim());
            yield break;
        }

        yield break;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        int _layer = other.gameObject.layer;
        if(id > 0)//Eis ist auf der Waffel
        {
            if (other.tag == "Col" || _layer == 8 || _layer == 12 || _layer == 13) return;//Falls von einem anderen Eis/Waffel getroffen, dann ignoriere es: Eingehendes Eis handelt sich selbst

            //Eis wird runtergeschubst:
            Debug.Log(name + " leaving: posInCone: " + posInCone + ", virt: " + virtPosInCone);
            cone.RemoveIce(id);
            id = -1;
            _attribute.RemoveFromCone();
            cScript.offset = new Vector2(0, --coneIceCount * .3f);
            Camera.main.orthographicSize = 6 + .3f * coneIceCount;
            fillingSpace = false;//beende evtl filling
            StartCoroutine(PushIceOut());
            return;
        }
        else//Eis fliegt durch die Gegend
        {
            if (block_ice_enter) return;//ist in der nicht-berühr-Phase

            if (_layer != 8 && _layer != 12 || other.tag == "Col") //Weder Eis noch Cone getroffen
            {
                //Hier den Code einfügen, der beschreibt, was passiert, wenn fliegendes Eis in Kontakt mit der Umgebung kommt: 

                //Sticking on ground:
                if (_attribute.sticky)
                {
                    Debug.Log(name + " hits ground");
                    OrderStickyIce(other.transform);
                    _attribute.onGround = true;
                }

                return;
            }

            ICone other_Ice = other.GetComponent<ICone>();
            if (other_Ice.GetID() < 0) //falls das getroffene Eis nicht auf der waffel ist
            {
                //sticky:
                if (other_Ice.Get_attribute().sticky && !_attribute.onGround && (_attribute.stickyParent == null || !_attribute.stickyParent.GetComponent<ICone>().Get_attribute().onGround))
                {
                    Debug.Log(name + " hits " + other.name);
                    OrderStickyIce(other.transform);

                    StartCoroutine(Set_sticky());
                }
                return;
            }

            //Eis wird in den Turm integriert:
            //Gruppe lösen:
            for (int i = transform.GetChild(0).childCount - 1; i >= 0; i--) { transform.GetChild(i).GetComponent<ICone>().Get_attribute().runSim = true; transform.GetChild(i).parent = null; }
            //Update den Turm:
            transform.parent = cone.transform;
            id = other_Ice.GetID()+1;
            _attribute.runSim = true;
            _attribute.AddToCone();
            cScript.offset = new Vector2(0, ++coneIceCount * .3f);
            Camera.main.orthographicSize = 6 + .3f * coneIceCount;
            cone.iceTower.Insert(id, this);
            cone.UpdateConeTower(id);  
        }
    }
    void OrderStickyIce(Transform t_other)
    {
        _attribute.runSim = false;
        Transform myStickyParent = transform;
        ICone stickyIce = this;
        while (true)
        {
            if (stickyIce.Get_attribute().stickyParent == null) break;
            myStickyParent = stickyIce.Get_attribute().stickyParent;
            if (myStickyParent.GetComponent<ICone>() == null) break;
            stickyIce = myStickyParent.GetComponent<ICone>();
        }
        stickyIce.Get_attribute().stickyParent = t_other;
        stickyIce.Set_posInCone(stickyIce.Get_transform().position - t_other.position);
    }

    bool block_ice_enter = false;
    int contactMask = (1 << 10) | (1 << 11) | (1 << 8);//Kontakt nur mit Obstacles, Ground und Eis
    public IEnumerator PushIceOut()
    {
        block_ice_enter = true;

        //Gebe entsprechend der Kollision dem Eis eine Ausgangsgeschwindigkeit:
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, transform.localScale.x / 2, Vector2.up, 666, 1 << 8);
        Vector2 diff = (hit.point - rb.position);
        Vector2 vel_other = hit.collider.GetComponent<Rigidbody2D>() != null ? hit.collider.GetComponent<Rigidbody2D>().velocity : Vector2.zero;
        Vector2 vel = RotToVec(2 * Mathf.Atan2(diff.y, diff.x) - Mathf.Atan2(vel_other.y-cone.rb.velocity.y, vel_other.x-cone.rb.velocity.x)) * rb.velocity.magnitude;
        //Debug.Log("vel: " + vel + ", pos: " + transform.position + "\nparent: " + transform.parent);
        //Debug.Break();
        rb.velocity = vel;

        //Warte, bis die Eiskugel nichts mehr berührt:
        gameObject.layer = 13;//Ice_col
        do { yield return new WaitForSeconds(.1f); } while (Physics2D.CircleCast(transform.position, transform.localScale.x/2, Vector2.up, 666/*Muhahahaha!!!*/, contactMask).collider != null);
        gameObject.layer = 8;//Ice
        block_ice_enter = false;
        yield break;
    }
    public IEnumerator ShootIce(ICone prev)
    {
        id = -1;
        _attribute.RemoveFromCone();
        cScript.offset = new Vector2(0, --coneIceCount * .3f);
        Camera.main.orthographicSize = 6 + .3f * coneIceCount;
        rb.velocity = cone.rb.velocity + (rb.position - prev.Get_rb().position).normalized * _attribute.shootPower;
        transform.parent = null;

        gameObject.layer = 13;
        yield return new WaitForSeconds(.1f);
        gameObject.layer = 8;
        yield break;
    }

    bool fillingSpace, fillOverwrite;
    Vector2 virtPosInCone;
    public IEnumerator FillSpace(Vector2 _endPos)
    {
        virtPosInCone = _endPos;

        //Überlagere vorherige Lückenfüller:
        if(fillingSpace)
        {
            fillOverwrite = true;
            yield return new WaitUntil(() => !fillOverwrite);
            virtPosInCone = _endPos;
            //Debug.Log(name + " overwritten to: " + _endPos);
        }
        fillingSpace = true;

        //Lerp zur nächsten position
        Vector2 startPos = posInCone;
        Vector2 diffPos = virtPosInCone - startPos;
        for (float count = 0; count < 1 && !fillOverwrite && fillingSpace; count += cone.fillTime_real)
        {
            posInCone = startPos + count * diffPos;
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForFixedUpdate();
        //Debug.Log(name + " stopped: overwrite: " + fillOverwrite + ", fillingSpace: " + fillingSpace + ", posInCone: " + posInCone + ", virt: " + virtPosInCone);
        if (!fillOverwrite) fillingSpace = false;
        fillOverwrite = false;
        yield break;
    }
    IEnumerator Set_sticky()
    {
        _attribute.sticky = false;
        yield return new WaitForFixedUpdate();
        _attribute.sticky = true;
    }
}
