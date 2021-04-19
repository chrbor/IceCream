using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceRainScript : MonoBehaviour
{
    public float width;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(width, 1, 1));
    }
}
