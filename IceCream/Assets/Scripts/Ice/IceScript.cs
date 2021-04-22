using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Helper;
using static IceManager;
using static Cone2Script;
using static CameraScript;

public class IceScript : MonoBehaviour, ICone
{
    Vector3 prevPos;
    [HideInInspector]
    public Vector2 velocity;
    [HideInInspector]
    public int id = -1;

    Rigidbody2D prevIce;
    Vector2 diff_vel, diff_pos;

    Rigidbody2D rb;

    private Vector2 posInCone;


    private void Start()
    {
        ResetTouch += ResetTouchingCone;

        //Init:
        rb = GetComponent<Rigidbody2D>();
        prevPos = transform.position;
        StartCoroutine(RunSim());
    }


    public int GetID() => id;
    public void SetID(int _id) => id = _id;
    public Rigidbody2D Get_rb() => rb;
    public Vector2 Get_posInCone() => posInCone;
    public void Set_posInCone(Vector2 position) => posInCone = position;
    public void Set_prevIce(Rigidbody2D _prev) { prevIce = _prev; }
    public Transform Get_transform() => transform;
    public string Get_name() => name;
    public void ResetTouchingCone() { id = -1; transform.parent = null; prevIce = null; }

    IEnumerator RunSim()
    {
        yield return new WaitForSeconds(2);

        while (true)
        {
            //Waffel-Physik:
            if (id > 0)
            {
                Vector2 diff_pos = transform.position - cone.transform.position;
                float rot = Mathf.Atan2(diff_pos.y, diff_pos.x) * Mathf.Rad2Deg - 90;
                while (Mathf.Abs(rot) > 180) rot -= 360 * Mathf.Sign(rot);

                transform.localPosition = posInCone * (1 - Mathf.Abs(rot) / 360);
            }
            else
            {
                rb.velocity += iceGravity;
            }
            
            prevPos = transform.position;
            yield return new WaitForFixedUpdate();
        }

        yield break;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        int _layer = other.gameObject.layer;
        if(id > 0)//Eis ist auf der Waffel
        {
            if (other.tag == "Col" || _layer == 8 || _layer == 12) return;//Falls von einem anderen Eis/Waffel getroffen, dann ignoriere es: Eingehendes Eis handelt sich selbst

            //Hier Code einfügen, der behandelt, wie eis von der Umgebung runtergeschubst werden kann:

            return;
        }
        else//Eis fliegt durch die Gegend
        {
            if (_layer != 8 && _layer != 12 || other.tag == "Col") //Weder Eis noch Cone getroffen
            {
                //Hier den Code einfügen, der beschreibt, was passiert, wenn fliegendes Eis in Kontakt mit der Umgebung kommt:         

                return;
            }

            //Eis wird in den Turm integriert:

            //Finde den Kontakt mit der niedriegsten ID:
            List<Collider2D> contacts = new List<Collider2D>();
            GetComponent<Collider2D>().GetContacts(contacts);
            ICone tmp = null, lowest_Ice = contacts[0].GetComponent<ICone>();
            for(int i = 1; i < contacts.Count; i++)
            {
                tmp = contacts[i].GetComponent<ICone>();
                if (tmp == null) continue;
                lowest_Ice = tmp.GetID() < lowest_Ice.GetID() ? tmp : lowest_Ice;
            }

            //Update den Turm:
            transform.parent = cone.transform;
            id = lowest_Ice.GetID()+1;
            cScript.offset = new Vector2(0, id * .25f);
            coneIceCount++;
            cone.iceTower.Insert(id, this);
            cone.UpdateConeTower(id);
        }
    }
}
