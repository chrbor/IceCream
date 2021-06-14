using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Helper;
using static IceManager;
using static Cone2Script;
using static CameraScript;
using static GameManager;

public class IceScript : MonoBehaviour, ICone
{
    Vector2 prevVel;
    [HideInInspector]
    public Vector2 velocity;
    [HideInInspector]
    public int id = -1;

    Rigidbody2D prevIce;
    Vector2 diff_vel, diff_pos;

    Rigidbody2D rb;
    private Vector2 posInCone;

    private SpriteRenderer sprite;
    private Material mat;
    public GameObject DestroyEffect;

    public IceAttribute attribute;
    IceAttribute _attribute;
    private bool touchingGround;

    private void Start()
    {
        ResetTouch += ResetTouchingCone;

        //Init:
        rb = GetComponent<Rigidbody2D>();

        //Visuell:
        sprite = GetComponent<SpriteRenderer>();
        sprite.sortingOrder = 1000;
        mat = sprite.material;

        ResetAttributes();

        StartCoroutine(RunSim());
        StartCoroutine(UpdateLife());
    }

    bool isGettingDestroyed = false;
    private void OnDestroy()
    {
        isGettingDestroyed = true;

        //if(cone != null) cone.blockCone();
        ResetTouch -= ResetTouchingCone;
    }


    //Accessor:
    public int GetID() => id;
    public void SetID(int _id)
    {
        //if (isGettingDestroyed) return;

        id = _id;
        sprite.sortingOrder = -id * 1000;
        Transform Ttex = transform.GetChild(transform.childCount - 1);
        for (int i = 0; i < Ttex.childCount; i++) Ttex.GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = sprite.sortingOrder + 1 + i;
    }
    public IceAttribute Get_attribute() => _attribute;
    public void Set_Attribute(IceAttribute attribute)
    {
        _attribute = ScriptableObject.CreateInstance<IceAttribute>();
        _attribute.Set_Attribute(attribute);
    }
    public Rigidbody2D Get_rb() => rb;
    public Vector2 Get_posInCone() => posInCone;
    public void Set_posInCone(Vector2 position) => posInCone = position;
    public void Set_prevIce(Rigidbody2D _prev) { prevIce = _prev; }
    //public Vector2 Get_virtPosInCone() => virtPosInCone;
    //public void Set_virtPosInCone(Vector2 position) => virtPosInCone = position;
    public Transform Get_transform() => transform;
    public string Get_name() => name;
    public void ResetTouchingCone() { id = -1; transform.parent = null; prevIce = null; }
    public void ResetAttributes()
    {
        //Setze alle Attribute:
        if (_attribute == null) Set_Attribute(attribute);

        name = _attribute.name;

        //Aktiv/Passiv:            
        transform.localScale = Vector3.one * _attribute.scale;
        rb.mass = _attribute.mass;
        rb.sharedMaterial = _attribute.material;

        //Generiere neue Texturen:
        sprite.sprite = _attribute.primSprite.sprite;

        if(transform.childCount == 3) Destroy(transform.GetChild(2).gameObject);
        Transform Ttex = new GameObject("secTex").transform;
        Ttex.parent = transform;
        Ttex.localPosition = Vector3.zero;
        Ttex.localScale = Vector3.one;
        SpriteRenderer secSprite;
        GameObject secObj;
        for (int i = 0; i < _attribute.secSprites.Count;)
        {
            secObj = new GameObject("sec_" + i);
            secObj.transform.parent = Ttex;
            secObj.transform.localPosition = Vector3.zero;
            secObj.transform.localScale = Vector3.one;
            secSprite = secObj.AddComponent<SpriteRenderer>();
            secSprite.sprite = _attribute.secSprites[i].sprite;
            secSprite.sortingLayerName = "Ice";
            secSprite.sortingOrder = sprite.sortingOrder + ++i;
        }
    }
    public bool GettingDestroyed() => isGettingDestroyed;

    IEnumerator RunSim()
    {
        //yield return new WaitForSeconds(2);
        _attribute.runSim = true;
        while (_attribute.runSim)
        {
            if (pauseGame) yield return new WaitWhile(() => pauseGame);

            //Waffel-Physik:
            if (id > 0)
            {
                Vector2 diff_pos = transform.position - cone.transform.position;
                float rot = Mathf.Atan2(diff_pos.y, diff_pos.x) * Mathf.Rad2Deg - 90;
                while (Mathf.Abs(rot) > 180) rot -= 360 * Mathf.Sign(rot);

                transform.localPosition = posInCone * (1 - Mathf.Abs(rot) / 360);
                //if(!fillingSpace) virtPosInCone = posInCone;
            }
            else
            {
                //Laufende Effekte, wenn fliegend:
                //Gravity:
                rb.velocity += iceGravity;

                if (!_attribute.reactOnImpact) UpdateContinousAttributes();
            }
            
            prevVel = rb.velocity;
            yield return new WaitForFixedUpdate();
        }

        //Am Boden Kleben bleiben:
        if (_attribute.stickyParent != null)
        {
            //block_ice_enter = true;
            ICone parentIce = _attribute.stickyParent.GetComponent<ICone>();
            while (_attribute.stickyParent != null && (parentIce == null || parentIce.GetID() < 0))
            {
                if (pauseGame) yield return new WaitWhile(() => pauseGame);

                transform.position = posInCone + (Vector2)_attribute.stickyParent.position;
                if (!_attribute.reactOnImpact) UpdateContinousAttributes();
                yield return new WaitForFixedUpdate();
            }
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            _attribute.runSim = true;
            _attribute.stickyParent = null;
            StartCoroutine(RunSim());
            yield break;
        }

        yield break;
    }
    [HideInInspector]
    public bool resetUpdateLife;
    IEnumerator UpdateLife()
    {
        float timeCount;
        float timeGoal;

        while(_attribute.life_current > 0)
        {
            yield return new WaitUntil(() => id < 0);
            while (id < 0 && _attribute.life_current > 0)
            {
                mat.SetInt("_Blink", 0);
                transform.GetChild(2).gameObject.SetActive(true);

                timeCount = 0;
                timeGoal = _attribute.life_current * .2f;
                while (_attribute.life_current > 0 && id < 0 && (timeCount < timeGoal || _attribute.life_current > 10) && !resetUpdateLife)
                {
                    if (pauseGame) yield return new WaitWhile(() => pauseGame);

                    timeCount += Time.fixedDeltaTime;
                    _attribute.life_current -= Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();
                }

                mat.SetInt("_Blink", 1);
                transform.GetChild(2).gameObject.SetActive(false);

                timeCount = 0;
                timeGoal = _attribute.life_current * .025f;
                while (_attribute.life_current > 0 && id < 0 && timeCount < timeGoal && !resetUpdateLife)
                {
                    timeCount += Time.fixedDeltaTime;
                    _attribute.life_current -= Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();
                }

                if(resetUpdateLife) { resetUpdateLife = false; yield return new WaitForSeconds(.25f); continue; }

                //Reagiere nur bei Lebensabzug:
                GetComponent<Collider2D>().GetContacts(new List<Collider2D>());
                if (_attribute.reactOnImpact && _attribute.life_current > 0 && _attribute.life_current < 1 && !touchingGround) _attribute.life_current = .5f;
                touchingGround = false;
            }

            mat.SetInt("_Blink", 0);
            transform.GetChild(1).gameObject.SetActive(true);
        }

        //Explosion:
        GameObject explsn = null;
        if(_attribute.explosionRange > 0)
        {
            explsn = Instantiate(iceManager.explosionPrefab, transform.position, Quaternion.identity);
            explsn.transform.localScale = Vector3.one * _attribute.explosionRange * transform.localScale.x;
            explsn.GetComponent<ExplosionScript>().exclusions.Add(gameObject);
        }

        //Split:
        if(_attribute.splitCount > 0)
        {
            _attribute.life_current = _attribute.life;
            _attribute.splitCount--;
            _attribute.scale *= .75f;
            _attribute.mass *= .75f;
            transform.localScale *= .75f;
            rb.mass = _attribute.mass;

            IceScript splitIce = Instantiate(gameObject).GetComponent<IceScript>();
            splitIce.Set_Attribute(_attribute);
            splitIce.Get_attribute().ResetIce();
            rb.velocity = -prevVel;

            if (explsn != null) explsn.GetComponent<ExplosionScript>().exclusions.Add(splitIce.gameObject);

            Rigidbody2D split_rb = splitIce.GetComponent<Rigidbody2D>();
            split_rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            split_rb.velocity = prevVel;
            iceManager.SplitIgnore(splitIce.gameObject, transform.GetChild(0).gameObject);

            StartCoroutine(UpdateLife());
            yield break;
        }

        //Instantiiere Effekt:
        GameObject obj = Instantiate(DestroyEffect, transform.position, Quaternion.identity);
        ParticleSystem.MainModule main = obj.GetComponent<ParticleSystem>().main;
        main.startColor = _attribute.color;

        Destroy(gameObject);
        yield break;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        int _layer = other.gameObject.layer;
        if (isGettingDestroyed || _layer == 19/*iceOnly*/) return;

        if(id > 0)//Eis ist auf der Waffel
        {
            if (other.tag == "Col" || _layer == 8 || (_layer > 11 && _layer < 20)) return;//Falls von einem anderen Eis/Waffel getroffen, dann ignoriere es: Eingehendes Eis handelt sich selbst

            //Eis wird runtergeschubst:
            //Debug.Log(name + " leaving: posInCone: " + posInCone);
            if (_attribute.explosionRange > 0)
            {
                GameObject explsn = Instantiate(iceManager.explosionPrefab, transform.position, Quaternion.identity);
                explsn.transform.localScale = Vector3.one * _attribute.explosionRange * transform.localScale.x * .5f;
                explsn.GetComponent<ExplosionScript>().exclusions.Add(gameObject);
            }

            cone.blockCone();
            cone.RemoveIce(id);
            RemoveFromCone();
            StartCoroutine(PushIceOut());
            cScript.DoShake(2000);
            return;
        }
        else//Eis fliegt durch die Gegend
        {
            if (block_ice_enter || _layer == 13 || _layer == 14 ) return;//ist in der nicht-berühr-Phase oder von einem Effekt betroffen
            
            //Weder Eis noch Cone getroffen:
            if (_layer != 8 && _layer != 12) 
            {
                //Hier den Code einfügen, der beschreibt, was passiert, wenn fliegendes Eis in Kontakt mit der Umgebung kommt: 

                if (_attribute.reactOnImpact)
                {
                    if (_attribute.life_current > 1)
                    {
                        //Growth
                        if (_attribute.growing != 0 && Mathf.Sign(_attribute.growing) == Mathf.Sign(_attribute.endScale - transform.localScale.x))
                        {
                            _attribute.scale += _attribute.growing * 50;
                            transform.localScale = Vector3.one * _attribute.scale;
                        }
                        //Explosion:
                        GameObject explsn = null;
                        if (_attribute.explosionRange > 0)
                        {
                            explsn = Instantiate(iceManager.explosionPrefab, transform.position, Quaternion.identity);
                            explsn.transform.localScale = Vector3.one * _attribute.explosionRange * transform.localScale.x * .5f;
                            explsn.GetComponent<ExplosionScript>().exclusions.Add(gameObject);
                        }
                        //Split:
                        if (_attribute.splitCount > 0)
                        {

                            IceScript splitIce = Instantiate(gameObject).GetComponent<IceScript>();
                            splitIce.Set_Attribute(_attribute);
                            splitIce.Get_attribute().ResetIce();
                            splitIce.Get_attribute().life_current = _attribute.life_current;
                            rb.velocity = -prevVel;

                            _attribute.life_current = _attribute.life;
                            _attribute.reactOnImpact = false;
                            _attribute.splitCount--;
                            _attribute.scale *= .75f;
                            _attribute.mass *= .75f;
                            transform.localScale *= .75f;
                            rb.mass = _attribute.mass;

                            if (explsn != null) explsn.GetComponent<ExplosionScript>().exclusions.Add(splitIce.gameObject);

                            Rigidbody2D split_rb = splitIce.GetComponent<Rigidbody2D>();
                            split_rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                            split_rb.velocity = prevVel;
                            iceManager.SplitIgnore(splitIce.gameObject, transform.GetChild(0).gameObject);
                        }
                    }
                }

                //Sticking on ground:
                ICone iceOrigin = GetHighestIce(this);
                if (_attribute.sticky && !_attribute.onGround)
                {
                    _attribute.onGround = true;
                    rb.constraints = RigidbodyConstraints2D.FreezeAll;
                    OrderStickyIce(other.transform, iceOrigin);
                    /*
                    Debug.Log(name + " hits ground");
                    Debug.Log("pos: " + transform.position + ", calcPos: " + (other.transform.position + (Vector3)posInCone));
                    Debug.Log("posInCone: " + posInCone);
                    Debug.Break();
                    //*/
                    return;
                }
                //Ground- Bounce einer Gruppe:
                if (!(_attribute.stickyParent == null || iceOrigin.Get_attribute().reflecting))
                {
                    StartCoroutine(iceOrigin.Get_attribute().SetReflectBlock());
                    iceOrigin.Get_rb().velocity *= -1;//simple reflection (pls dont kill me XP)
                }

                _attribute.life_current--;
                return;
            }

            ICone other_Ice = other.GetComponent<ICone>();
            //falls das getroffene Eis nicht auf der waffel ist:
            if (other_Ice.GetID() < 0) 
            {

                //sticky:
                if(((_attribute.sticky)) && //!_attribute.onGround)) &&// || (other_Ice.Get_attribute().sticky && !other_Ice.Get_attribute().onGround)) && //Falls sticky und nicht auf Boden berührt Eis und
                    ((_attribute.stickyParent == null) ||
                    (_attribute.stickyParent != other_Ice.Get_attribute().stickyParent) //Origin ungleich other.Origin (gegen doppelkontakt)
                    ))
                {
                    //Ermittle höchstes Eis:
                    Transform myStickyParent = transform;
                    ICone iceOrigin = GetHighestIce(this);
                    ICone iceOrigin_other = GetHighestIce(other_Ice);
                    if (iceOrigin == iceOrigin_other) return;//haben schon die gleiche Quelle

                    if (iceOrigin.Get_attribute().onGround)//Wenn die ein am Boden klebendes Eis ist, dann wird das andere eis diesem zugeordnet 
                    {
                        iceOrigin_other.Get_rb().constraints = RigidbodyConstraints2D.FreezeAll;
                        OrderStickyIce(transform, iceOrigin_other);
                        StartCoroutine(Set_sticky(iceOrigin_other.Get_attribute()));
                        return;
                    }
                    //Debug.Log("origin: " + iceOrigin.Get_transform() + "\nother: " + other.transform);
                    /*
                    //Debug.Log("sticky pre-parent: " + _attribute.stickyParent);
                    if (_attribute.stickyParent != null)
                    {
                        Debug.Log("parent: " + _attribute.stickyParent + ", parent-origin: " + _attribute.stickyParent.GetComponent<ICone>().Get_attribute().stickyParent
                             + "\nother: " + other.transform);
                        Debug.Log("connect: " + ((_attribute.stickyParent != other.transform) && (_attribute.stickyParent.GetComponent<ICone>().Get_attribute().stickyParent != other.transform)));
                    }
                    //*/
                    iceOrigin.Get_rb().constraints = RigidbodyConstraints2D.FreezeAll;
                    OrderStickyIce(other.transform, iceOrigin);
                    /*
                    Debug.Log(name + " hits " + other.name);
                    //Debug.Log("other: " + other.transform.position + ", self: " + transform.position + "\ndiff: " + (transform.position - _attribute.stickyParent.position) + "posInCone: " + posInCone);
                    Debug.Log("runSim of parent: " + _attribute.stickyParent.GetComponent<ICone>().Get_attribute().runSim);
                    Debug.Log("sticky parent " + _attribute.stickyParent + " follows " + _attribute.stickyParent.GetComponent<ICone>().Get_attribute().stickyParent);
                    //Debug.Log("is already paired to it: " + (_attribute.stickyParent == other.transform));
                    Debug.Break();
                    //*/
                    StartCoroutine(Set_sticky(_attribute));
                    return;
                }
                //*
                //Bounce
                else if (!_attribute.sticky && !other_Ice.Get_attribute().sticky && other_Ice.Get_attribute().stickyParent != null && _attribute.stickyParent == null)
                {
                    ICone iceOrigin = GetHighestIce(other_Ice);
                    if (iceOrigin.Get_attribute().onGround)
                    {
                        Vector2 diff = ((Vector2)(transform.position - other.transform.position)).normalized;
                        rb.velocity = rb.velocity.magnitude * diff;
                    }
                    else
                    {
                        Debug.Log(name + " bounced " + other.name);
                        Vector2 vel = iceOrigin.Get_rb().velocity;
                        iceOrigin.Get_rb().velocity += rb.velocity;// / iceOrigin.Get_attribute().stickyCount;
                        rb.velocity = vel;
                        //rb.velocity = (vel * iceOrigin.Get_attribute().stickyCount + rb.velocity) / (iceOrigin.Get_attribute().stickyCount + 1);
                    }
                }
                //*/
                return;
            }

            //Eis wird in den Turm integriert:
            //Update den Turm:
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            transform.parent = cone.transform;
            //id = other_Ice.GetID()+1;
            SetID(other_Ice.GetID() + 1);
            _attribute.ResetIce();
            _attribute.AddToCone();
            cScript.offset = new Vector2(0, ++coneIceCount * .3f);
            Camera.main.orthographicSize = cone.camSize + .3f * coneIceCount;
            cone.iceTower.Insert(id, this);
            cone.UpdateConeTower(id);
            StartCoroutine(TriggerBlink());//Damit Gruppen aufgelöst werden können
        }
    }
    private void OnTriggerStay2D(Collider2D other) { touchingGround |= other.gameObject.layer == 11; }
    IEnumerator TriggerBlink()
    {
        GetComponent<Collider2D>().enabled = false;
        yield return new WaitForFixedUpdate();
        GetComponent<Collider2D>().enabled = true;
        transform.GetChild(2).gameObject.SetActive(true);//idk wo der fehler liegt XP
        yield break;
    }
    void UpdateContinousAttributes()
    {
        //Growth:
        if (_attribute.growing != 0 && Mathf.Sign(_attribute.growing) == Mathf.Sign(_attribute.endScale - transform.localScale.x))
        {
            _attribute.scale += _attribute.growing;
            transform.localScale = Vector3.one * _attribute.scale;
        }
    }
    void OrderStickyIce(Transform t_other, ICone stickyOrigin)
    {
        _attribute.runSim = false;
        Transform myStickyParent = transform;

        ICone otherIce = t_other.GetComponent<ICone>();
        IceAttribute stickyOrigin_Attribute = stickyOrigin.Get_attribute();


        stickyOrigin_Attribute.stickyParent = t_other;
        stickyOrigin.Set_posInCone(stickyOrigin.Get_transform().position - t_other.position);
        stickyOrigin_Attribute.onGround |= otherIce == null || otherIce.Get_attribute().onGround;
        stickyOrigin_Attribute.runSim &= !stickyOrigin_Attribute.onGround && stickyOrigin == this;//false;
        if (stickyOrigin_Attribute.onGround) return;


        //Setze Impuls und entsprechend der Position zum Zentrum und setze anschließend das Zentrum neu:
        IceAttribute otherIce_Attribute = otherIce.Get_attribute();

        //Geschwindigkeit und Dreh-impuls:
        Vector2 diff = ((Vector2)(otherIce.Get_transform().position - stickyOrigin.Get_transform().position)).normalized;
        Vector2 velocity = otherIce.Get_rb().velocity - stickyOrigin.Get_rb().velocity;
        //Debug.Log("diff_vel: " + velocity + "\nother_vel: " + otherIce.Get_rb().velocity + ", vel: " + stickyOrigin.Get_rb().velocity);

        float diff_angle = Mathf.Atan2(velocity.y, velocity.x) - Mathf.Atan2(diff.y, diff.x);
        float vel_mag = stickyOrigin.Get_rb().velocity.magnitude;
        float countFactor = stickyOrigin_Attribute.stickyCount / (float)(stickyOrigin_Attribute.stickyCount + otherIce_Attribute.stickyCount);
        float otherCountFactor = 1 - countFactor;

        t_other.GetComponent<ICone>().Get_rb().velocity = t_other.GetComponent<ICone>().Get_rb().velocity * otherCountFactor - diff * vel_mag * Mathf.Cos(diff_angle) * countFactor;
        //otherIce_Attribute.stickyRotation += Mathf.Sin(diff_angle) * vel_mag * countFactor * 0.05f;//Drehimpuls

        //Update des Zentrums:
        otherIce_Attribute.stickyCenter = otherIce_Attribute.stickyCenter * otherCountFactor + stickyOrigin_Attribute.stickyCenter * countFactor;
        otherIce_Attribute.stickyCount += stickyOrigin_Attribute.stickyCount;
    }
    ICone GetHighestIce(ICone startIce)
    {
        Transform myStickyParent = transform;
        ICone stickyIce = startIce;
        int x;
        for (x = 0; x < 20; x++)
        {
            if (stickyIce.Get_attribute().stickyParent == null) break;
            myStickyParent = stickyIce.Get_attribute().stickyParent;
            if (myStickyParent.GetComponent<ICone>() == null) break;
            stickyIce = myStickyParent.GetComponent<ICone>();
        }
        if (x == 20) Debug.Log("Error: overload (no runback)");

        return stickyIce;
    }

    bool block_ice_enter = false;
    public IEnumerator PushIceOut()
    {
        block_ice_enter = true;

        //Gebe entsprechend der Kollision dem Eis eine Ausgangsgeschwindigkeit:
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, transform.localScale.x / 2, Vector2.up, 666, 1 << 8);
        Vector2 diff = (hit.point - rb.position);
        Vector2 vel_other = hit.collider != null && hit.collider.GetComponent<Rigidbody2D>() != null ? hit.collider.GetComponent<Rigidbody2D>().velocity : Vector2.zero;
        Vector2 vel = RotToVec(2 * Mathf.Atan2(diff.y, diff.x) - Mathf.Atan2(vel_other.y-cone.rb.velocity.y, vel_other.x-cone.rb.velocity.x)) * rb.velocity.magnitude;
        //Debug.Log("vel: " + vel + ", pos: " + transform.position + "\nparent: " + transform.parent);
        //Debug.Break();
        rb.velocity = vel;

        //Warte, bis die Eiskugel nichts mehr berührt:
        gameObject.layer = 13;//Ice_col
        do { yield return new WaitForSeconds(.2f); } while (Physics2D.CircleCast(transform.position, transform.localScale.x/2, Vector2.up, 666/*Muhahahaha!!!*/, 1<<8).collider != null);
        //yield return new WaitForSeconds(.2f);
        gameObject.layer = 8;//Ice
        block_ice_enter = false;
        yield break;
    }
    public IEnumerator ShootIce(ICone prev)
    {
        RemoveFromCone();
        transform.parent = null;
        rb.velocity = cone.rb.velocity + (rb.position - /*prev.Get_rb().position).normalized*/cone.rb.position).normalized * _attribute.shootPower;

        gameObject.layer = 13;
        yield return new WaitForSeconds(.1f);
        gameObject.layer = 8;
        yield break;
    }

    public void RemoveFromCone()
    {
        id = -1;
        _attribute.RemoveFromCone();
        cScript.offset = new Vector2(0, --coneIceCount * .3f);
        Camera.main.orthographicSize = cone.camSize + .3f * coneIceCount;
    }

    IEnumerator Set_sticky(IceAttribute attribute)
    {
        bool isSticky = attribute.sticky;
        attribute.sticky = false;
        yield return new WaitForFixedUpdate();
        attribute.sticky = isSticky;
    }
}
