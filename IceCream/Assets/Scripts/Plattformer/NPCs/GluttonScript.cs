using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class GluttonScript : MonoBehaviour
{
    [Header("LaufVektor:")]
    public float range = 10;
    public float offset = 0;

    [Header("Generelles:")]
    [Tooltip("einheiten per tick")]
    public float velocity = .1f;

    private float x_center;
    protected bool pauseMovement;
    protected bool invincible;

    protected Animator anim;
    protected AudioSource aSrc;
    protected Rigidbody2D rb;
    protected CapsuleCollider2D col;

    private int mask = (1 << 10) | (1 << 11) | (1 << 21);
    private float prev_x;

    protected virtual void Start()
    {
        transform.GetChild(1).GetComponent<TriggerScript>().On_T_Enter += ViewTriggered;
        transform.GetChild(2).GetComponent<ColliderScript>().On_C_Enter += HeadTriggered;


        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        aSrc = GetComponent<AudioSource>();

        x_center = transform.position.x + offset;
        StartCoroutine(MoveAround());
    }

    private void OnDestroy()
    {
        transform.GetChild(1).GetComponent<TriggerScript>().On_T_Enter -= ViewTriggered;
        transform.GetChild(2).GetComponent<ColliderScript>().On_C_Enter -= HeadTriggered;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer != 8 || collision.gameObject.GetComponent<IceScript>().id > 0) return;
        StartCoroutine(PlayHit(collision.gameObject));
    }

    private void HeadTriggered(Collider2D other)
    {
        if (other.gameObject.layer != 9 || invincible) return;

        StartCoroutine(GetHeadHit(other.gameObject));
    }
    private void ViewTriggered(Collider2D other)
    {
        if (other.gameObject.layer != 8 || pauseMovement || invincible || Physics2D.Raycast(transform.position, Vector2.down, 2.5f, mask).collider == null ) return;
        if (other.GetComponent<IceScript>().id < 0) return;

        StartCoroutine(Attack(other.gameObject));
    }

    IEnumerator MoveAround()
    {
        while (transform.position.y > -9999)
        {
            if (pauseGame || pauseMovement) yield return new WaitWhile(() => pauseGame || pauseMovement);

            if ((transform.localScale.x > 0 && (transform.position.x > x_center + range)) || (transform.localScale.x < 0 && (transform.position.x < x_center - range)) || Mathf.Sign(transform.position.x - prev_x) != Mathf.Sign(transform.localScale.x))
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1);

            prev_x = transform.position.x;
            transform.position += Vector3.right * Mathf.Sign(transform.localScale.x) * velocity;
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
        yield break;
    }

    IEnumerator Attack(GameObject other)
    {
        pauseMovement = true;
        yield return new WaitForSeconds(.5f);
        if (invincible) { pauseMovement = false; yield break; }
        invincible = true;

        //Berechne Startgeschw. 
        Vector2 diff = other.transform.position - transform.position + Vector3.up;
        if (diff.y == 0) yield break;
        float doubleDist_y = Mathf.Abs(diff.y * 2);
        float _t = Mathf.Sqrt(doubleDist_y/-Physics2D.gravity.y);

        col.size = Vector2.one * 1.5f;
        rb.velocity = new Vector2(diff.x, doubleDist_y) / _t;

        //Warte, bis der Char wieder still steht:
        while (rb.velocity.sqrMagnitude > .125f)
        {
            rb.rotation = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg + 90;
            yield return new WaitForEndOfFrame();
        }

        //Stehe wieder auf und laufe weiter:
        rb.simulated = false;
        Vector2 step = Vector2.up * 2.5f * Time.fixedDeltaTime;
        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            rb.rotation *= count;
            rb.position += step;
            yield return new WaitForFixedUpdate();
        }
        rb.simulated = true;
        col.size = new Vector2(1.5f, 4);
        transform.rotation = Quaternion.identity;
        x_center = transform.position.x;
        pauseMovement = false;
        invincible = false;
        yield break;
    }
    IEnumerator GetHeadHit(GameObject other)
    {
        pauseMovement = true;
        invincible = true;
        yield return new WaitForSeconds(3f);
        invincible = false;
        pauseMovement = false;

        yield break;
    }
    IEnumerator PlayHit(GameObject other)
    {
        pauseMovement = true;
        invincible = true;
        yield return new WaitForSeconds(5f);
        invincible = false;

        pauseMovement = false;
        yield break;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position + Vector3.right * (offset - range), transform.position + Vector3.right * (offset + range));
    }
}
