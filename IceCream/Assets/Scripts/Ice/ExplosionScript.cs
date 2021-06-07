using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cone2Script;
using static CameraScript;

public class ExplosionScript : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> exclusions;

    [Tooltip("Faktor mit welchem bestimmt wird wieviel Geschwindigkeit den betroffenen Objekten hinzugefügt wird")]
    public float power;
    [Tooltip("Dauer der Explosion in Sekunden")]
    public float duration;

    private void Start()
    {
        StartCoroutine(DelayedDestroy());
        Vector2 diff = transform.position - Camera.main.transform.position;
        if (Mathf.Abs(diff.x) < camWindow.x && Mathf.Abs(diff.y) < camWindow.y)
        {
            AudioSource aSrc = GetComponent<AudioSource>();
            aSrc.panStereo = diff.x / camWindow.x;
            aSrc.volume = Mathf.Abs(aSrc.panStereo) * .5f + .5f;
            aSrc.pitch = Random.Range(.9f, 1.1f);

            cScript.DoShake(3000 + 50 * diff.sqrMagnitude - 500 * (transform.localScale.x > 5.5f ? 5.5f : transform.localScale.x));
        }
    }

    IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
        yield break;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Rigidbody2D rb_other = other.GetComponent<Rigidbody2D>();
        if (exclusions.Contains(other.gameObject) || rb_other == null) return;

        Vector2 diff = other.transform.position - transform.position;
        float distFactor = transform.localScale.x / (2 + diff.sqrMagnitude);

        if (other.gameObject.layer == 8)
        {
            IceScript iceScript = other.GetComponent<IceScript>();
            if (iceScript.id < 0)
                iceScript.Get_attribute().life_current -= distFactor;
            else
            {
                cone.RemoveIce(iceScript.id);
                iceScript.RemoveFromCone();
            }
        }
        else distFactor *= rb_other.gravityScale/2;
        rb_other.velocity += diff * power * distFactor;
    }
}
