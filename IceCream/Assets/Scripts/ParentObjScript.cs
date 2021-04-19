using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentObjScript : MonoBehaviour
{
    [Tooltip("Soll die relative position am Start genutzt werden?")]
    public bool autoPosition;

    public Vector3 relativePosition;
    public GameObject target;


    void OnEnable()
    {
        if (target == null) { Debug.Log("Error_ParentObjScript: target is null!"); return; }
        if (autoPosition) relativePosition = transform.position - target.transform.position;
    }

    void FixedUpdate()
    {
        transform.position = target.transform.position + relativePosition;
    }
}
