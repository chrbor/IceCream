using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ProgressScript;

public class WinLoseScript : MonoBehaviour
{
    public GameObject IcePrefab;

    CanvasGroup buttonsGroup, titleGroup, messageGroup;
    RectTransform tower, bronzeLimit, silverLimit, goldLimit;
    bool bronzeWin, silverWin, goldWin;

    public AnimationCurve dropSpeed;
    private AnimationCurve smoothMove;

    // Start is called before the first frame update
    void Start()
    {
        smoothMove = AnimationCurve.EaseInOut(0, 0, 1, 1);
        dropSpeed.MoveKey(1, new Keyframe(.5f, Mathf.Clamp(.5f + .2f * iceDelivered.Count, 1, 10)));
        buttonsGroup = transform.GetChild(1).GetComponent<CanvasGroup>();
        titleGroup = transform.GetChild(2).GetComponent<CanvasGroup>();
        messageGroup = transform.GetChild(3).GetComponent<CanvasGroup>();

        tower = transform.GetChild(0).GetComponent<RectTransform>();
        bronzeLimit = tower.transform.GetChild(1).GetComponent<RectTransform>();
        silverLimit = tower.transform.GetChild(2).GetComponent<RectTransform>();
        goldLimit   = tower.transform.GetChild(3).GetComponent<RectTransform>();

        Canvas.ForceUpdateCanvases();
        //Wenn nicht geschafft, dann verstecke "weiter"-Button
        if(progressPoints < goal_Bronze)
        {
            buttonsGroup.transform.GetChild(1).gameObject.SetActive(true);
            buttonsGroup.transform.GetChild(3).gameObject.SetActive(false);
        }

        //Positioniere Ziele:
        bronzeLimit.anchoredPosition = Vector2.up * goal_Bronze * 70;//formerly  * 100 * .7f
        silverLimit.anchoredPosition = Vector2.up * goal_Silver * 70;
        goldLimit.anchoredPosition = Vector2.up * goal_Gold * 70;


        StartCoroutine(ShowLevelResult());
    }

    IEnumerator ShowLevelResult()
    {
        //Blende dich selbst ein:
        CanvasGroup group = GetComponent<CanvasGroup>();
        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            group.alpha = count;
            yield return new WaitForFixedUpdate();
        }

        //Staple alle Eiskugeln, um das Endergebnis anzuzeigen:
        float yPos_current = 0;
        float timeStep, scrollSpeed;
        for(int i = 0; i< iceDelivered.Count; i++)
        {
            yPos_current += iceDelivered[i].scale * 70;//formerly 100 * .7f
            timeStep = dropSpeed.Evaluate((float)i / iceDelivered.Count) * Time.fixedDeltaTime;
            StartCoroutine(PlaceIce(i, yPos_current, timeStep));

            //Scroll nach Oben:
            scrollSpeed = timeStep + Time.fixedDeltaTime;
            Vector2 step = Vector2.down * iceDelivered[i].scale * 70 * scrollSpeed;
            Vector2 end = Vector2.up * (tower.anchoredPosition.y - iceDelivered[i].scale * 70);
            for(float count = 0; count < 1; count += scrollSpeed)
            {
                tower.anchoredPosition += step;
                yield return new WaitForFixedUpdate();
            }
            tower.anchoredPosition = end;
        }
        yield return new WaitForSeconds(2);

        //Zoome heraus
        timeStep = Time.fixedDeltaTime / 2;
        Vector2 startPos = tower.anchoredPosition;
        Vector2 diffPos = Vector2.down * (tower.anchoredPosition.y / 2 - 100);
        Vector3 diffScale = Vector3.one * (yPos_current > 600 ? .75f - yPos_current / 600 : 0);
        for(float count = 0; count < 1; count += timeStep)
        {
            tower.anchoredPosition = startPos + diffPos * smoothMove.Evaluate(count);
            tower.localScale = Vector3.one + diffScale * smoothMove.Evaluate(count);
            yield return new WaitForFixedUpdate();
        }

        //Setze Message und Title:
        if(yPos_current < bronzeLimit.anchoredPosition.y)
        {
            //titleGroup.GetComponent<Image>().sprite = ;
            //messageGroup.GetComponent<Image>().sprite = ;
        }
        else if(yPos_current < silverLimit.anchoredPosition.y)
        {

        }
        else if(yPos_current < goldLimit.anchoredPosition.y)
        {

        }
        else//Perfekter score:
        {

        }

        //Zeige Buttons und Title:
        titleGroup.gameObject.SetActive(true);
        buttonsGroup.gameObject.SetActive(true);
        Vector3 startScale = .2f * Vector3.one;
        diffScale = 1.3f * Vector3.one;
        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            titleGroup.transform.localScale = startScale + diffScale * smoothMove.Evaluate(count);
            titleGroup.alpha = count;
            buttonsGroup.alpha = count;
            yield return new WaitForFixedUpdate();
        }
        startScale = Vector3.one * 1.5f;
        diffScale = Vector3.one * -.5f;
        for (float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            titleGroup.transform.localScale = startScale + diffScale * smoothMove.Evaluate(count);
            yield return new WaitForFixedUpdate();
        }

        //Message:
        messageGroup.gameObject.SetActive(true);
        startScale = .3f * Vector3.one;
        diffScale = Vector3.one;
        timeStep = Time.fixedDeltaTime / .5f;
        for (float count = 0; count < 1; count += timeStep)
        {
            messageGroup.transform.localScale = startScale + diffScale * smoothMove.Evaluate(count);
            messageGroup.alpha = count;
            yield return new WaitForFixedUpdate();
        }
        startScale = Vector3.one * 1.3f;
        diffScale = Vector3.one * -.3f;
        for (float count = 0; count < 1; count += timeStep)
        {
            messageGroup.transform.localScale = startScale + diffScale * smoothMove.Evaluate(count);
            yield return new WaitForFixedUpdate();
        }
        yield break;
    }

    IEnumerator PlaceIce(int index, float yPos, float timeStep)
    {
        IceAttribute attribute = iceDelivered[index];

        //initialisiere Objekt:
        GameObject ice = Instantiate(IcePrefab, tower);
        ice.transform.SetSiblingIndex(0);

        
        ice.GetComponent<Image>().sprite = attribute.primSprite.sprite;
        GameObject iceTexObj;
        for(int i = 0; i < attribute.secSprites.Count; i++)
        {
            iceTexObj = Instantiate(IcePrefab, ice.transform);
            iceTexObj.name = "sec_" + i;
            iceTexObj.transform.localPosition = Vector3.zero;
            iceTexObj.GetComponent<Image>().sprite = attribute.secSprites[i].sprite;
        }

        ice.transform.localScale *= attribute.scale;

        RectTransform rect_ice = ice.GetComponent<RectTransform>();
        rect_ice.anchoredPosition = new Vector2(index == 0 ? 0 : Random.Range(-attribute.scale, attribute.scale) * 25, yPos + 100);
        
        Vector2 step = Vector2.down * 100 * timeStep;
        CanvasGroup iceGroup = ice.AddComponent<CanvasGroup>();
        for(float count = 0; count < 1; count += timeStep)
        {
            iceGroup.alpha = count;
            rect_ice.anchoredPosition += step;
            yield return new WaitForFixedUpdate();
        }
        rect_ice.anchoredPosition = new Vector2(rect_ice.anchoredPosition.x, yPos);
        iceGroup.alpha = 1;

        if (!bronzeWin && yPos >= bronzeLimit.anchoredPosition.y) { bronzeWin = true; StartCoroutine(ShowTrophy(bronzeLimit)); }
        if (!silverWin && yPos >= silverLimit.anchoredPosition.y) { silverWin = true; StartCoroutine(ShowTrophy(silverLimit)); }
        if (!goldWin && yPos >= goldLimit.anchoredPosition.y) { goldWin = true; StartCoroutine(ShowTrophy(goldLimit)); }
        yield break;
    }
    IEnumerator ShowTrophy(RectTransform limitWon)
    {
        float timeStep = Time.fixedDeltaTime / .5f;

        Image iTrophy = limitWon.GetChild(0).GetComponent<Image>();

        //werde opaque und x1.5 so groß

        Color stepColor = (1 - iTrophy.color.a) * timeStep * Color.black;
        Vector3 diffScale = .5f * Vector3.one;
        for(float count = 0; count < 1; count += timeStep)
        {
            iTrophy.color += stepColor;
            iTrophy.transform.localScale = Vector3.one + diffScale * smoothMove.Evaluate(count);
            yield return new WaitForFixedUpdate();
        }
        for (float count = 0; count < 1; count += timeStep)
        {
            iTrophy.transform.localScale = Vector3.one + diffScale * smoothMove.Evaluate(1 - count);
            yield return new WaitForFixedUpdate();
        }

        yield break;
    }
}
