using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static IceStandScript;
using static TouchAndScreen;

public class SimpleCollectGame : MiniGameScript
{
    public float maxSpeed = .2f;

    protected override IEnumerator PlayGame()
    {
        //Bewege Eis nach unten, bis nur noch die Spitze des Eisturms zu sehen ist:
        float endPos_y = -Camera.main.orthographicSize / 1.5f;
        if(cone_mini.transform.childCount > 0) endPos_y += cone_mini.transform.position.y - cone_mini.transform.GetChild(cone_mini.transform.childCount - 1).position.y;

        AnimationCurve animCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            cone_mini.transform.localPosition = Vector3.up * endPos_y * animCurve.Evaluate(count);
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
        for (int i = 1; i < iceOnCone.Count; i++) iceOnCone[i].transform.parent = null;

        //Hauptspiel:
        float currentSpeed = 0;
        float diff_x;
        float spawn_countdown = 0;
        int iceCount = 0;
        while (runGame)
        {
            //Spawne Eis:
            if(spawn_countdown <= 0 && plannedIce.Count > iceCount)
            {
                spawn_countdown = 1f / (1 + ++iceCount);
                GameObject ice = Instantiate(iceStand.MiniIcePrefab, Camera.main.transform.position + new Vector3((.9f - Random.Range(0f, 1.8f)) * Camera.main.orthographicSize * Camera.main.aspect, Camera.main.orthographicSize + 1), Quaternion.identity);
                ice.GetComponent<MiniIceScript>().id = iceCount;
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
                    iceOnCone[i].transform.position += Vector3.right * diff_x * .8f;
                }
            }

            //Aktualisiere Position der Waffel:
#if (!(UNITY_STANDALONE || UNITY_EDITOR))
            if(Input.touchCount == 0)
            {
                currentSpeed *= .9f;
                cone_mini.transform.position += Vector3.right * currentSpeed;//Ausgleiten
                yield return new WaitForFixedUpdate();
                continue;
            }
#endif

            diff_x = cone_mini.transform.position.x - PixelToWorld(Input.mousePosition).x;
            currentSpeed = Mathf.Abs(diff_x) > maxSpeed ? maxSpeed * Mathf.Sign(diff_x) : diff_x;
            cone_mini.transform.position -= Vector3.right * currentSpeed;

            yield return new WaitForFixedUpdate();
        }
        yield break;
    }
}
