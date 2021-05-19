using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static IceStandScript;
using static Cone2Script;

public class MiniGameScript : MonoBehaviour
{
    public static MiniGameScript miniGame_current;
    public bool runGame, stoppingGame;

    private Vector2 startPoint;

    [HideInInspector]
    public GameObject cone_mini;
    [HideInInspector]
    public List<GameObject> iceOnCone;
    protected int startID;

    protected GameObject inGameUI, txtField;
    protected Animator anim_CountDown, anim_Success, anim_Failure;

    public void StartGame()
    {
        miniGame_current = this;
        inGameUI = iceStand.transform.parent.parent.GetChild(1).gameObject;
        txtField = inGameUI.transform.GetChild(0).gameObject;
        anim_CountDown = inGameUI.transform.GetChild(1).GetComponent<Animator>();
        anim_Success = inGameUI.transform.GetChild(2).GetComponent<Animator>();
        anim_Failure = inGameUI.transform.GetChild(3).GetComponent<Animator>();

        startPoint = Camera.main.transform.position;
        transform.position = Vector3.up * (1 << 15);

        StartCoroutine(RunningGame());
    }

    IEnumerator RunningGame()
    {
        //Erstelle Eis: 
        cone_mini = Instantiate(iceStand.MiniConePrefab, transform.position, Quaternion.identity, transform);
        Vector2 icePos = cone_mini.transform.position + Vector3.up * .5f;
        startID = addedIce.Count;

        iceOnCone = new List<GameObject>();
        GameObject secSpriteObj;
        if(cone != null)
        {
            startID += cone.iceTower.Count;
            GameObject myIcePrefab = new GameObject("displayIce");
            myIcePrefab.AddComponent<SpriteRenderer>().sortingLayerName = "Ice";
            for(int i = 1; i < startID; i++)
            {
                icePos += Vector2.up * .6f;
                iceOnCone.Add(Instantiate(myIcePrefab, icePos, Quaternion.identity, cone_mini.transform));

                //Sprites entsprechend der Attribute:
                IceAttribute attribute = i < cone.iceTower.Count ? cone.iceTower[i].Get_attribute() : addedIce[i - cone.iceTower.Count];

                SpriteRenderer primSpriteRenderer = iceOnCone[i - 1].GetComponent<SpriteRenderer>();
                primSpriteRenderer.sprite = attribute.primSprite.sprite;
                primSpriteRenderer.sortingOrder = -i * 1000;

                int j = 0;
                foreach(var secSprite in attribute.secSprites)
                {
                    secSpriteObj = new GameObject("sec_" + j);

                    SpriteRenderer secSpriteRenderer = secSpriteObj.AddComponent<SpriteRenderer>();
                    secSpriteRenderer.sprite = secSprite.sprite;
                    secSpriteRenderer.sortingLayerName = "Ice";
                    secSpriteRenderer.sortingOrder = -i * 1000 + j;

                    secSpriteObj.transform.parent = iceOnCone[i-1].transform;
                    secSpriteObj.transform.localPosition = Vector3.zero;
                }
            }
            Destroy(myIcePrefab);
        }


        //Gehe zum Spielfeld:
        StartCoroutine(MoveCamera(true));
        yield return new WaitForSeconds(1);

        //starte Spiel:
        runGame = true;
        StartCoroutine(PlayGame());
        yield return new WaitWhile(() => runGame);
        Debug.Log("spiel beendet!");

        //Beende das Spiel:
        yield return new WaitForSeconds(1);
        plannedIce.Clear();
        for (int i = iceStand.iceDisplay.childCount - 1; i > 0; i--) Destroy(iceStand.iceDisplay.GetChild(i).gameObject);
        yield return new WaitForSeconds(.1f);
        iceStand.UpdateIceDisplay();
        StartCoroutine(MoveCamera(false));
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
        yield break;
    }

    public IEnumerator MoveCamera(bool moveToGame, float transitionTime = 1)
    {
        //Gleite zum Spiel über:
        RectTransform Tgelateria = iceStand.transform.parent.GetComponent<RectTransform>();
        Vector2 endPixelPoint = 2 * Camera.main.pixelWidth * Vector2.right;
        AnimationCurve animCurve = moveToGame ? AnimationCurve.EaseInOut(0, 0, 1, 1) : AnimationCurve.EaseInOut(0,1,1,0);

        float timeStep = Time.fixedDeltaTime / transitionTime;
        for(float count = 0; count < 1; count += timeStep)
        {
            Camera.main.transform.position = new Vector3(startPoint.x * animCurve.Evaluate(1 - count), startPoint.y, -10);
            Tgelateria.anchoredPosition = endPixelPoint * animCurve.Evaluate(count);
            yield return new WaitForFixedUpdate();
        }
        Camera.main.transform.position = new Vector3(0, 1<<15, -10);

        yield break;
    }

    protected virtual IEnumerator PlayGame()
    {
        Debug.Log("Game not implemented yet");
        yield break;
    }

    public virtual void AddIce(GameObject iceObj)
    {
        Debug.Log("Adding not implemented yet");
    }
}
