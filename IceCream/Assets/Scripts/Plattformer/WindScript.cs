﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Helper;

public class WindScript : MonoBehaviour
{
    public float angle;
    public float strength;
    private Vector2 windVector;

    private Rigidbody2D other_rb;

    private void Start()
    {
        windVector = RotToVec(angle * Mathf.Deg2Rad) * strength;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        int _layer = other.gameObject.layer;
        if(_layer == 8 && other.GetComponent<IceScript>().id > 0)
        {
            //Verhalten des Turms:
            Cone2Script.cone.windForce += windVector / 200;
            return;
        }

        other_rb = other.GetComponent<Rigidbody2D>();
        if (other_rb == null) return;
        other_rb.AddForce(windVector);
    }
}
