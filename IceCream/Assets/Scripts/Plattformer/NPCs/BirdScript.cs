using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class BirdScript : MonoBehaviour
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

    // Start is called before the first frame update
    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        aSrc = GetComponent<AudioSource>();

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

    IEnumerator PlayHit(GameObject other)
    {
        if(other.layer == 8 && other.GetComponent<IceScript>().id > 0)
        {
            invincible = true;
            yield return new WaitForSeconds(.25f);
            invincible = false;
            yield break;
        }

        //anim.SetTrigger("Hit");
        //aSrc.Play();

        pauseMovement = true;
        invincible = true;

        if (--life > 0)
        {
            yield return new WaitForSeconds(.25f);
            pauseMovement = false;
            invincible = false;
            yield break;
        }
        GetComponent<Collider2D>().enabled = false;

        if(other.layer == 8)//Eis
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
