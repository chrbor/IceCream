using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerScript : MonoBehaviour
{
    public event UnityAction<Collider2D> On_T_Enter;

    private void OnTriggerEnter2D(Collider2D other)
    {
        On_T_Enter.Invoke(other);
    }
}
