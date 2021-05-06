using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cone2Script;
using static GameManager;

[CreateAssetMenu(fileName = "newIceAttribute", menuName = "IceAttribute")]
public class IceAttribute : ScriptableObject
{
    [System.Serializable]
    public class SpriteData
    {
        public Sprite sprite;
        public int dominance;

        public SpriteData(Sprite _sprite, int _dominance)
        {
            sprite = _sprite;
            dominance = _dominance;
        }
        public SpriteData(SpriteData data)
        {
            sprite = data.sprite;
            dominance = data.dominance;
        }
    }

    [HideInInspector]
    public bool runSim;
    [HideInInspector]
    public Transform stickyParent;

    [Header("Visuelle Attribute")]
    [Tooltip("Name des Eis, wenn blank, dann gleich dem Name des Objektes")]
    public string nameIce;
    [Tooltip("Sprite der Eiskugel")]
    public SpriteData primSprite;
    [Tooltip("Sprite der ursprünglichen Dekoration des primären Sprites")]
    public SpriteData prim_secSprite;
    [HideInInspector]
    public List<SpriteData> secSprites;//aktuelle(r) Sprite(s) der Dekoration der Eiskugel, können mehrere sein
    [Tooltip("Gibt an wie dominant das PrimärSprite ist")]
    public int prim_dominance;
    [Tooltip("Gibt an wie dominant das SekundärSprite ist")]
    public int sec_dominance;


    [Header("Aktiv (Effekte auf die Kugel):")]
    [Tooltip("Größe des Eis")]
    public float scale;
    [Tooltip("Masse des Eis (gesammt)")]
    public float mass;
    [Tooltip("Geschwindigkeit, mit der das Eis geschossen werden kann")]
    public float shootPower;
    [Tooltip("Bestimmt, wie oft das Eis auf den Boden fallen oder wie lange durch die Luft fliegen kann")]
    public float life;
    [Tooltip("Umkreis, in den das Eis explodiert")]
    public float explosionRange;
    [Tooltip("Wenn wahr, dann wird diese Kugel mit den zwei oberen Eiskugeln kombiniert")]
    public bool isMelange;
    [Tooltip("Bestimmt, ob das nächste Eis gleich mit verschossen wird")]
    public bool sticky;
    [HideInInspector]
    public bool onGround;
    [Tooltip("Bestimmt, ob das Eis direct bei Impact explodiert/zersplittert/etc oder bei  life < 0")]
    public bool reactOnImpact;
    [Tooltip("Änderung der Größe per Tick")]
    public float growing;
    [Tooltip("Maximale/Minimale Größe")]
    public float endScale;
    [Tooltip("Anzahl, wie oft das Eis zersplittern kann")]
    public int splitCount;

    [Header("Material:")]
    [Tooltip("Das Material, dass bestimmt, wie sich das Eis verhält, wenn es durch die Gegend fliegt")]
    public PhysicsMaterial2D material;

    [Header("Passiv (Effekte auf den Spieler in %):")]
    [Tooltip("Ändert wie schnell der Spieler sich bewegen kann")]
    public float speed;
    [Tooltip("Ändert wie hoch der Spieler springen kann")]
    public float jumpForce;
    [Tooltip("Ändert wie schnell der Turm durch Bewegung gekippt werden kann")]
    public float agility;
    [Tooltip("Ändert wie schnell der Turm kippen kann")]
    public float instability;
    [Tooltip("Ändert wie schnell der Turm sich wieder aufrichten kann")]
    public float upForce;
    [Tooltip("Ändert wie schnell Eiskugeln wieder nachfallen ")]
    public float dropSpeed;

    public void Set_Attribute(IceAttribute attribute)
    {
        nameIce = attribute.nameIce == "" ? attribute.name : attribute.nameIce;
        primSprite = attribute.primSprite;
        secSprites = attribute.secSprites;
        prim_dominance = attribute.prim_dominance;
        sec_dominance = attribute.sec_dominance;

        scale = attribute.scale;
        mass = attribute.mass;
        material = attribute.material;
        life = attribute.life;

        shootPower = attribute.shootPower;
        explosionRange = attribute.explosionRange;
        isMelange = attribute.isMelange;
        sticky = attribute.sticky;
        reactOnImpact = attribute.reactOnImpact;
        growing = attribute.growing;
        endScale = attribute.endScale;
        splitCount = attribute.splitCount;

        speed = attribute.speed;
        jumpForce = attribute.jumpForce;
        agility = attribute.agility;
        instability = attribute.instability;
        upForce = attribute.upForce;
        dropSpeed = attribute.dropSpeed;
    }

    public void Combine(IceAttribute attribute)
    {
        isMelange = false;

        //Visuell:
        primSprite = attribute.primSprite.dominance > primSprite.dominance ? attribute.primSprite : primSprite;
        int index;
        foreach(var spriteData in attribute.secSprites)
        {
            for (index = 0; index < secSprites.Count; index++) if (spriteData.dominance <= secSprites[index].dominance) break;
            if(spriteData.dominance != secSprites[index].dominance) secSprites.Insert(index, attribute.secSprites[index]);
        }

        //Aktiv:
        scale = (scale + attribute.scale) / 1.5f;
        mass = (mass + attribute.mass) / 1.5f;
        life = life + attribute.life;

        shootPower = (shootPower + attribute.shootPower) / 1.5f;
        explosionRange = (explosionRange + attribute.explosionRange) / 1.25f;
        sticky |= attribute.sticky;
        reactOnImpact |= attribute.reactOnImpact;
        growing += attribute.growing;

        PhysicsMaterial2D tmp = new PhysicsMaterial2D("combo_mat");
        tmp.bounciness = (material.bounciness + attribute.material.bounciness) / 2;
        tmp.friction = (material.friction + attribute.material.friction) / 2;
        material = tmp;

        //Passiv:
        speed *= attribute.speed;
        jumpForce *= attribute.jumpForce;
        instability *= attribute.instability * 1.1f;
        upForce *= attribute.upForce;
        dropSpeed *= attribute.dropSpeed;
    }

    public void AddToCone()
    {
        cone.helpForce *= upForce;
        cone.tilt_move *= agility;
        cone.coneGravity *= instability;
        cone.helpForce *= upForce;
        cone.fillTime_real /= dropSpeed;

        if (pAttribute == null) return;
        pAttribute.velocity *= speed;
        pAttribute.Set_vel();
        pAttribute.jumpPower *= jumpForce;
    }

    public void RemoveFromCone()
    {
        cone.helpForce /= upForce;
        cone.tilt_move /= agility;
        cone.coneGravity /= instability;
        cone.helpForce /= upForce;
        cone.fillTime_real *= dropSpeed;

        if (pAttribute == null) return;
        pAttribute.velocity /= speed;
        pAttribute.Set_vel();
        pAttribute.jumpPower /= jumpForce;
    }

    [ExecuteInEditMode]
    void OnValidate()
    {
        //Update der List an sekundären Sprites:
        secSprites.Clear();
        secSprites.Add(new SpriteData(prim_secSprite));
    }
}
