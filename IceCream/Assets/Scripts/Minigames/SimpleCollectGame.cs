using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static IceStandScript;
using static TouchAndScreen;

public class SimpleCollectGame : MiniGameScript
{
    public float maxSpeed = .2f;
    int doneCount = 0;


    protected override IEnumerator PlayGame()
    {
        //Bewege Eis nach unten, bis nur noch die Spitze des Eisturms zu sehen ist:
        float endPos_y = -2;//-Camera.main.orthographicSize / 1.5f;//Wenn ort == 5
        if(cone_mini.transform.childCount > 0) endPos_y += cone_mini.transform.position.y - cone_mini.transform.GetChild(cone_mini.transform.childCount - 1).position.y;

        AnimationCurve animCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        float ortDiff = 2.5f - Camera.main.orthographicSize;
        float ortStart = Camera.main.orthographicSize;
        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            cone_mini.transform.localPosition = Vector3.up * endPos_y * animCurve.Evaluate(count);
            Camera.main.orthographicSize = ortStart + ortDiff * animCurve.Evaluate(count);
            yield return new WaitForFixedUpdate();
        }

        //Setze Infofeld:
        txtField.transform.GetChild(0).GetComponent<Text>().text = "Halte die Waffel und ziehe sie zum\nEis um es aufzufangen!";

        //Lasse Infofeld erscheinen:
        CanvasGroup gameUIGroup = inGameUI.GetComponent<CanvasGroup>();
        gameUIGroup.alpha = 0;
        inGameUI.SetActive(true);
        txtField.SetActive(true);
        anim_Success.gameObject.SetActive(false);
        anim_Failure.gameObject.SetActive(false);
        float timeStep = Time.fixedDeltaTime / .5f;
        for(float count = 0; count < 1; count += timeStep)
        {
            inGameUI.transform.localScale = Vector3.one * (.5f + .75f * count);
            gameUIGroup.alpha = count;
            yield return new WaitForFixedUpdate();
        }
        timeStep *= 2;
        for (float count = 0; count < 1; count += timeStep)
        {
            inGameUI.transform.localScale = Vector3.one * (1.25f - .25f * count);
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        timeStep *= .5f;
        for (float count = 1; count > 0; count -= timeStep)
        {
            inGameUI.transform.localScale = Vector3.one * (.5f + .5f * count);
            gameUIGroup.alpha = count;
            yield return new WaitForFixedUpdate();
        }
        inGameUI.SetActive(false);


        //Hier startSequenz einfügen:


        //Ziehe Eis aus der Waffel heraus (besser programmierbar):
        for (int i = 1; i < iceOnCone.Count; i++) iceOnCone[i].transform.parent = transform;


        //Hauptspiel:
        float currentSpeed = 0;
        float diff_x;
        float spawn_countdown = 0;
        float afterTime = .5f;
        float stiffness = .5f;
        int iceCount = 0;
        while (afterTime > 0)
        {
            if (stoppingGame) { afterTime -= Time.fixedDeltaTime; stiffness = .8f; }
            //Spawne Eis:
            else if(spawn_countdown <= 0 && plannedIce.Count > iceCount)
            {
                spawn_countdown = 10f / (10 + ++iceCount);
                GameObject ice = Instantiate(iceStand.MiniIcePrefab, (Vector2)Camera.main.transform.position + new Vector2((.7f - Random.Range(0f, 1.4f)) * Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize + 1), Quaternion.identity);
                ice.GetComponent<MiniIceScript>().fallSpeed = .05f + iceCount * .005f;
                ice.GetComponent<MiniIceScript>().id = startID + iceCount;
                ice.GetComponent<MiniIceScript>().attribute = plannedIce[iceCount - 1];
            }
            spawn_countdown -= Time.fixedDeltaTime;


            //Aktualisiere Eisturm:
            if(iceOnCone.Count > 0)
            {
                //iceOnCone[0].transform.position = new Vector3(cone_mini.transform.position.x, iceOnCone[0].transform.position.y, 0);
                for(int i = 1; i < iceOnCone.Count; i++)
                {
                    diff_x = iceOnCone[i-1].transform.position.x - iceOnCone[i].transform.position.x;
                    iceOnCone[i].transform.position += Vector3.right * diff_x * stiffness;
                }
            }

            //Aktualisiere Position der Waffel:
#if (!(UNITY_STANDALONE || UNITY_EDITOR))
            if(Input.touchCount == 0)
            {
                currentSpeed *= .9f;
                cone_mini.transform.position += Vector3.left * currentSpeed;//Ausgleiten
                yield return new WaitForFixedUpdate();
                continue;
            }
#endif

            diff_x = cone_mini.transform.position.x - PixelToWorld(Input.mousePosition).x;
            currentSpeed = Mathf.Abs(diff_x) > maxSpeed ? maxSpeed * Mathf.Sign(diff_x) : diff_x;
            cone_mini.transform.position -= Vector3.right * currentSpeed;

            yield return new WaitForFixedUpdate();
        }


        if (iceOnCone.Count > 2) cone_mini.transform.position += Vector3.right * (iceOnCone[1].transform.position.x - iceOnCone[0].transform.position.x) * .9f;
        //Abschlussanimation:
        float end_x = -cone_mini.transform.position.x;
        float start_y = transform.position.y;
        float diff_y = iceOnCone.Count * .6f;
        float start_ort = Camera.main.orthographicSize;
        float diff_ort = iceOnCone.Count * .4f + 1 - start_ort;
        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            transform.position = new Vector3(end_x * animCurve.Evaluate(count), start_y + diff_y * animCurve.Evaluate(count));
            Camera.main.orthographicSize = start_ort + diff_ort * animCurve.Evaluate(count);
            yield return new WaitForFixedUpdate();
        }
        runGame = false;
        yield break;
    }

    public override void AddIce(GameObject iceObj)
    {
        Debug.Log("add Ice");
        addedIce.Add(iceObj.GetComponent<MiniIceScript>().attribute);
        Destroy(iceObj.GetComponent<MiniIceScript>());
        iceObj.transform.parent = transform;

        if(iceOnCone.Count == 0)
        {
            iceObj.transform.parent = cone_mini.transform;
            iceObj.transform.localPosition = Vector3.up * 1.1f;
        }
        else
            iceObj.transform.position = new Vector3(iceObj.transform.position.x, iceOnCone[iceOnCone.Count - 1].transform.position.y + .6f);
        iceOnCone.Add(iceObj);

        StartCoroutine(MoveUp());

        stoppingGame = ++doneCount == plannedIce.Count;
    }

    IEnumerator MoveUp()
    {
        Vector3 step = Vector3.down * 1.6f * Time.fixedDeltaTime;
        float ortStep = .6f * Time.fixedDeltaTime;
        float timeStep = Time.fixedDeltaTime * 2;
        for (float count = 0; count < 1; count += timeStep)
        {
            transform.position += step;
            Camera.main.orthographicSize += ortStep;
            yield return new WaitForFixedUpdate();
        }
        yield break;
    }
}
