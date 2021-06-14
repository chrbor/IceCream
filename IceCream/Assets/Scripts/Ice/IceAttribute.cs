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
    public Transform stickyParent;//Ursprungseis/-boden der Gruppe
    [HideInInspector]
    public Vector2 stickyCenter;//ungefähres Massezentrum der Gruppe
    [HideInInspector]
    public int stickyCount;//Anzahl der eiskugeln in der Gruppe
    [HideInInspector]
    public float stickyRotation;//Rotation per Tick in Radianten


    [Tooltip("Dieser Text wird im Infofeld und im Glossar direct unterhalb der Beschreibung angezeigt")]
    public string anecdote;
    [Tooltip("Dieser Text wird im Infofeld und im Glossar angezeigt"), TextArea(5, 7)]
    public string description;

    [Header("Visuelle Attribute")]
    //[Tooltip("Name des Eis, wenn blank, dann gleich dem Name des Objektes")]
    //public string nameIce;
    [Tooltip("Sprite der Eiskugel")]
    public SpriteData primSprite;
    [Tooltip("Sprite der ursprünglichen Dekoration des primären Sprites")]
    public SpriteData prim_secSprite;
    [HideInInspector]
    public List<SpriteData> secSprites;//aktuelle(r) Sprite(s) der Dekoration der Eiskugel, können mehrere sein
    [Tooltip("Farbe für Effekte wie zB Spritzer")]
    public Color color;

    [Header("Aktiv (Effekte auf die Kugel):")]
    [Tooltip("Größe des Eis")]
    public float scale;
    [Tooltip("Masse des Eis (gesammt)")]
    public float mass;
    [Tooltip("Geschwindigkeit, mit der das Eis geschossen werden kann")]
    public float shootPower;
    [Tooltip("Bestimmt, wie oft das Eis auf den Boden fallen oder wie lange durch die Luft fliegen kann")]
    public float life;
    [HideInInspector]
    public float life_current;
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

    public bool reflecting { get; private set; }

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
        //nameIce = attribute.nameIce == "" ? attribute.name : attribute.nameIce;
        name = attribute.name;
        primSprite = attribute.primSprite;
        secSprites = attribute.secSprites;
        color = attribute.color;

        scale = attribute.scale;
        mass = attribute.mass;
        material = attribute.material;
        life = attribute.life;
        life_current = life;

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

        stickyCount = 1;
        stickyRotation = 0;
    }

    public void Combine(IceAttribute attribute)
    {
        Debug.Log("Combine into " + name);
        isMelange = false;

        //Visuell:
        if(attribute.primSprite.dominance > primSprite.dominance)
        {
            primSprite = attribute.primSprite;
            color = attribute.color;
        }
        //primSprite = attribute.primSprite.dominance > primSprite.dominance ? attribute.primSprite : primSprite;
        int i;
        secSprites = new List<SpriteData>(secSprites);
        foreach(var spriteData in attribute.secSprites)
        {
            for (i = 0; i < secSprites.Count; i++) if (spriteData.dominance <= secSprites[i].dominance) break;//Ermittle die Stelle, an die die Textur eingefügt werden muss
            secSprites.Insert(i, spriteData);
        }

        //Aktiv:
        scale = (scale + attribute.scale) / 1.75f;
        mass = (mass + attribute.mass) / 1.5f;
        life = (life + attribute.life) / 2.25f;

        shootPower = (shootPower + attribute.shootPower) / 2;
        explosionRange = (explosionRange + attribute.explosionRange) / 1.5f;
        splitCount += attribute.splitCount;
        sticky |= attribute.sticky;
        reactOnImpact |= attribute.reactOnImpact;
        growing = (growing + attribute.growing) / 1.5f;
        endScale = (endScale + attribute.endScale) / 1.5f;

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
        cone.dropSpeed *= dropSpeed;

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
        cone.dropSpeed /= dropSpeed;

        if (pAttribute == null) return;
        pAttribute.velocity /= speed;
        pAttribute.Set_vel();
        pAttribute.jumpPower /= jumpForce;
    }
    public void ResetIce()
    {
        runSim = true;
        stickyParent = null;
        stickyCount = 1;
        stickyCenter = Vector2.zero;
        stickyRotation = 0;
        onGround = false;
        life_current = life;
    }

    public IEnumerator SetReflectBlock()
    {
        reflecting = true;
        yield return new WaitForSeconds(.1f);
        reflecting = false;
        yield break;
    }

    [ExecuteInEditMode]
    void OnValidate()
    {
        //Update der List an sekundären Sprites:
        secSprites.Clear();
        secSprites.Add(new SpriteData(prim_secSprite));
    }
}
