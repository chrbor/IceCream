using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cone2Script;
using static CameraScript;

public class LavaScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        int _layer = other.gameObject.layer;
        
        if(_layer == 9)//Player
        {
            //Springe in die Luft:
            Rigidbody2D rb_other = other.GetComponent<Rigidbody2D>();
            rb_other.velocity = new Vector2(rb_other.velocity.x, Mathf.Clamp(other.GetComponent<PlayerScript>().attribute.jumpPower, 15, 20));
            cScript.DoShake();

            //Verliere den Eisturm:
            if (cone == null) return;
            IceScript iceScript;
            for(int i = cone.iceTower.Count - 1; i > 0; i--)
            {
                iceScript = cone.iceTower[i].Get_transform().GetComponent<IceScript>();
                cone.RemoveIce(iceScript.id);
                iceScript.RemoveFromCone();
                iceScript.Get_rb().velocity = rb_other.velocity + Random.insideUnitCircle * new Vector2(10, 2);
            }
        }
        if(_layer == 8)//Eis
        {
            IceScript iceScript = other.GetComponent<IceScript>();
            if (iceScript.id < 0)
                iceScript.Get_attribute().life_current -= 2;
        }
    }
}
