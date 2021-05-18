using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MiniGameScript;

public class MiniIceScript : MonoBehaviour
{
    public int id;
    public IceAttribute attribute;
    public float fallSpeed;

    private SpriteRenderer primSprite, secSprite;

    void Start()
    {
        if (attribute == null) return;

        //Setze Texturen:
        primSprite = GetComponent<SpriteRenderer>();
        primSprite.sprite = attribute.primSprite.sprite;
        primSprite.sortingOrder = id * -1000;

        GameObject obj;
        for(int i = 0; i < attribute.secSprites.Count; i++)
        {
            obj = new GameObject("sec_" + i);
            obj.transform.parent = transform;
            obj.transform.localPosition = Vector3.zero;

            secSprite = obj.AddComponent<SpriteRenderer>();
            secSprite.sprite = attribute.secSprites[i].sprite;
            secSprite.sortingOrder = id * -1000 + i;
        }
    }

    private void FixedUpdate()
    {
        transform.position += Vector3.down * fallSpeed;

        if(Camera.main.transform.position.y - transform.position.y > Camera.main.orthographicSize)
        {
            miniGame_current.runGame = false;
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(miniGame_current.iceOnCone.Count > 0)
            if (other.gameObject != miniGame_current.iceOnCone[miniGame_current.iceOnCone.Count - 1]) return;
        else
            if (other.gameObject != miniGame_current.cone_mini.gameObject) return;

        miniGame_current.AddIce(id);
    }
}
