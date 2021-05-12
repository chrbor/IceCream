using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
        yield break;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (exclusions.Contains(other.gameObject)) return;

        Vector2 diff = other.transform.position - transform.position;
        float distFactor = transform.localScale.x / (1 + diff.sqrMagnitude);
        other.GetComponent<Rigidbody2D>().velocity += diff * power * distFactor;

        if (other.gameObject.layer == 8 && other.GetComponent<IceScript>().id < 0) other.GetComponent<IceScript>().Get_attribute().life_current -= distFactor;
    }
}
