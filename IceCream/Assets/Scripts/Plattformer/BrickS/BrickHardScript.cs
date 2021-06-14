using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickHardScript : BrickScript
{
    public Sprite[] sprites;
    SpriteRenderer sprite;
    private int life;
    private void Start()
    {
        life = sprites.Length + 1;
        sprite = GetComponent<SpriteRenderer>();
    }

    protected override void DoSpecial()
    {
        if (--life <= 0) { Destroy(gameObject); return; }
        sprite.sprite = sprites[life - 1];
    }
}
