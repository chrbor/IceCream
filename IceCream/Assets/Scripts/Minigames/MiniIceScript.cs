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

    private List<Collider2D> cols = new List<Collider2D>();

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

        if (miniGame_current.iceOnCone.Count > 0 && !miniGame_current.stoppingGame && miniGame_current.runGame)
        {
            if ((transform.position - miniGame_current.iceOnCone[miniGame_current.iceOnCone.Count - 1].transform.position).sqrMagnitude < 1)
                miniGame_current.AddIce(gameObject);
        }
        else if(Mathf.Abs(transform.position.x - miniGame_current.cone_mini.transform.position.x) < 1 && Mathf.Abs(transform.position.y - miniGame_current.cone_mini.transform.position.y - 1) < .25f)
            miniGame_current.AddIce(gameObject);

        if (Camera.main.transform.position.y - transform.position.y > Camera.main.orthographicSize)
        {
            Debug.Log("GameOver durch " + attribute.name);
            miniGame_current.stoppingGame = true;
            Destroy(gameObject);
        }
    }
}
