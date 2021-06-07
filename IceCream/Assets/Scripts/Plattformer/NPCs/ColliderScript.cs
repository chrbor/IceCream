using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ColliderScript : MonoBehaviour
{
    public event UnityAction<Collider2D> On_C_Enter;

    private void OnCollisionEnter2D(Collision2D other)
    {
        On_C_Enter.Invoke(other.collider);
    }
}
