using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class BirdScript : MonoBehaviour, IHitable
{
    [Header("Flugvektor:")]
    public float range = 10;
    public float offset = 0;

    [Header("Generelles:")]
    public int life = 3;
    [Tooltip("einheiten per tick")]
    public float velocity = .1f;
    [Tooltip("Attribut von der Eiskugel, die nach dem Tod spawned")]
    public IceAttribute attribute;


    private float x_center;
    protected bool pauseMovement;
    protected bool invincible;

    protected Animator anim;
    protected AudioSource aSrc;
    private Material mat;
    private float timeStep;

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        aSrc = GetComponent<AudioSource>();
        mat = new Material(transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().material);
        transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().material = mat;
        timeStep = Time.deltaTime / .5f;

        x_center = transform.position.x + offset;
        StartCoroutine(FlyAround());
    }

    IEnumerator FlyAround()
    {
        while(life > 0)
        {
            if (pauseGame || pauseMovement) yield return new WaitWhile(() => pauseGame || pauseMovement);

            if((transform.localScale.x > 0 && (transform.position.x > x_center + range)) || (transform.localScale.x < 0 && (transform.position.x < x_center - range)))
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1);

            transform.position += Vector3.right * Mathf.Sign(transform.localScale.x) * velocity;
            yield return new WaitForFixedUpdate();
        }

        Destroy(gameObject);
        yield break;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (pauseMovement || invincible) return;
        StartCoroutine(PlayHit(other.gameObject));        
    }

    public void StartPlayHit(GameObject other) => StartCoroutine(PlayHit(other));
    IEnumerator PlayHit(GameObject other)
    {
        if(other.layer == 8 && other.GetComponent<IceScript>().id > 0)
        {
            invincible = true;
            for(float count = 0; count < 1; count += timeStep)
            {
                mat.SetFloat("_Strength", count);
                yield return new WaitForEndOfFrame();
            }
            invincible = false;
            yield break;
        }

        anim.SetTrigger("Hit");
        //aSrc.Play();

        pauseMovement = true;
        invincible = true;

        if (other.layer == 14) life = 0;//Explosion? => Tod
        if (--life > 0)
        {
            for (float count = 1; count > 0; count -= timeStep)
            {
                mat.SetFloat("_Strength", count);
                yield return new WaitForEndOfFrame();
            }
            pauseMovement = false;
            invincible = false;
            yield break;
        }

        GetComponent<Collider2D>().enabled = false;

        if (other.layer == 8)//Eis
        {
            //Vogel verwandelt sich in eine Eiskugel:
            CreateIce(other.GetComponent<IceScript>());

        }
        else
        {
            //Vogel implodiert in eine Wolke an Federn:
        }

        //pauseMovement = false;
        yield break;
    }



    protected void CreateIce(IceScript iceScript)
    {
        bool isMelange = iceScript.Get_attribute().isMelange;
        iceScript.Get_attribute().Combine(attribute);
        iceScript.Get_attribute().isMelange = isMelange;
        iceScript.Get_attribute().life = 1;
        iceScript.Get_attribute().life_current = 1;

        iceScript.ResetAttributes();
        iceScript.resetUpdateLife = true;
        iceScript.Get_rb().velocity = Vector2.zero;
        iceScript.Get_transform().position = transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position + Vector3.right * (offset - range), transform.position + Vector3.right * (offset + range));
    }
}
