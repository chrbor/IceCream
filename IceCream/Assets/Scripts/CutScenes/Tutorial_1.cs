using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CameraScript;
using static GameManager;

public class Tutorial_1 : TutorialScript
{
    protected override IEnumerator RunTutorial()
    {
        for(float count = 1; count > 0; count -= timeStep)
        {
            buttons.alpha = count;
            yield return new WaitForFixedUpdate();
        }
        buttons.gameObject.SetActive(false);

        text.text = "OK, dann lass uns beginnen!";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Ziel des Spiels ist es verschiedenen Leuten Eis zu bringen";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);


        AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        cScript.target = null;
        Vector3 start = cScript.transform.position;
        Vector3 diff =  new Vector3(-87.5f, -2f, -10) - start;
        StartCoroutine(cScript.SetZoom(7));
        for (float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            cScript.transform.position = start + diff * curve.Evaluate(count);
            yield return new WaitForFixedUpdate();
        }

        for(float count = 1; count > 0; count -= Time.fixedDeltaTime * 5){ textFieldGroup.alpha = count; yield return new WaitForFixedUpdate(); }
        textField.anchoredPosition = Vector2.up * 200;
        StartCoroutine(ShowView(new Vector2(.5f ,.7f), new Vector2(.25f, .1f)));
        text.text = "Der Bereich, in den du das Eis bringen musst ist durch ein grünes Rechteck markiert";
        for (float count = 0; count < 1; count += Time.fixedDeltaTime * 5) { textFieldGroup.alpha = count; yield return new WaitForFixedUpdate(); }
        textFieldGroup.alpha = 1;


        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Das Eis muss dabei nicht auf der Waffel sein. Wichtig ist, dass die Eiskugel in den grünen Bereich fällt";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        StartCoroutine(HideView());
        for (float count = 1; count > 0; count -= Time.fixedDeltaTime * 5) { textFieldGroup.alpha = count; yield return new WaitForFixedUpdate(); }
        textField.anchoredPosition = Vector2.down * 200;
        StartCoroutine(ShowView(new Vector2(.35f, .2f), new Vector2(0f, .8f)));
        text.text = "Mit jeder Eiskugel, die du den Leuten bringst steigt das Eis-Meter";
        for (float count = 0; count < 1; count += Time.fixedDeltaTime * 5) { textFieldGroup.alpha = count; yield return new WaitForFixedUpdate(); }
        textFieldGroup.alpha = 1;

        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Wenn das Eis-Meter voll ist, dann hast du das Level erfolgreich abgeschlossen";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        StartCoroutine(HideView());
        for (float count = 1; count > 0; count -= Time.fixedDeltaTime * 5) { textFieldGroup.alpha = count; yield return new WaitForFixedUpdate(); }
        textFieldGroup.alpha = 0;

        start = cScript.transform.position;
        diff = new Vector3(-36.3f, .5f, -10) - start;
        for (float count = 0; count < 1; count += Time.fixedDeltaTime/2)
        {
            cScript.transform.position = start + diff * curve.Evaluate(count);
            yield return new WaitForFixedUpdate();
        }
        StartCoroutine(ShowView(new Vector2(.5f, .7f), new Vector2(.25f, .1f)));

        text.text = "Das Eis bekommst du vom Eisstand";
        textField.anchoredPosition = Vector2.up * 200;
        for (float count = 0; count < 1; count += Time.fixedDeltaTime * 5) { textFieldGroup.alpha = count; yield return new WaitForFixedUpdate(); }
        textFieldGroup.alpha = 1;

        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Laufe zu dem Eisstand und tippe ihn dann an, um Eis zu kaufen\nZu dem Eisstand selbst später mehr";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Der Eisstand selbst öffnet und schließt zu bestimmten Zeiten";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        StartCoroutine(HideView());
        for (float count = 1; count > 0; count -= Time.fixedDeltaTime * 5) { textFieldGroup.alpha = count; yield return new WaitForFixedUpdate(); }
        textField.anchoredPosition = Vector2.down * 100;
        StartCoroutine(ShowView(new Vector2(.7f, .25f), new Vector2(.15f, 0f)));

        text.text = "An der Zeitleiste kannst du erkennen, wie spät es ist";
        for (float count = 0; count < 1; count += Time.fixedDeltaTime * 5) { textFieldGroup.alpha = count; yield return new WaitForFixedUpdate(); }
        textFieldGroup.alpha = 1;

        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "In ihr ist ist die Öffnungs- und Schließzeit des Eisladens vermerkt.";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Beachte, dass das Level nicht abgeschlossen ist, sobald der Laden schließt,";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "sondern erst dann, wenn der Tag zu Ende geht oder den Leuten genug Eis gebracht wurde.";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        StartCoroutine(HideView());
        for (float count = 1; count > 0; count -= Time.fixedDeltaTime * 5) { textFieldGroup.alpha = count; yield return new WaitForFixedUpdate(); }
        textField.anchoredPosition = Vector2.down * 200;
        StartCoroutine(ShowView(new Vector2(.25f, .25f), new Vector2(.75f, .75f)));

        text.text = "Mit dem roten Button kann man die Zeit Vorspulen um Events zu triggern oder das Level zu beenden";
        for (float count = 0; count < 1; count += Time.fixedDeltaTime * 5) { textFieldGroup.alpha = count; yield return new WaitForFixedUpdate(); }
        textFieldGroup.alpha = 1;

        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Mit dem blauen Button pausierst du das Spiel";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        StartCoroutine(HideView());
        for (float count = 1; count > 0; count -= Time.fixedDeltaTime * 5) { textFieldGroup.alpha = count; yield return new WaitForFixedUpdate(); }
        textFieldGroup.alpha = 0;

        start = cScript.transform.position;
        diff = new Vector3(-83.2f, -3.8f, -10) - start;
        for (float count = 0; count < 1; count += Time.fixedDeltaTime / 2)
        {
            cScript.transform.position = start + diff * curve.Evaluate(count);
            yield return new WaitForFixedUpdate();
        }

        text.text = "Die Steuerung erfolgt über die PFEILTASTEN";
        for (float count = 0; count < 1; count += Time.fixedDeltaTime * 5) { textFieldGroup.alpha = count; yield return new WaitForFixedUpdate(); }
        textFieldGroup.alpha = 1;

        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Sobald man Eiskugeln hat, kann man sie mit der LEERTASTE verschießen";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Und damit kommen wir zum Ende des Tutorials.";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Probiere dich am besten hier ein bisschen aus um ein Gefühl für das Spiel zu bekommen";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        cScript.target = player;
        mat_disp.SetTexture("_View", empty);
        StartCoroutine(ChangeTutorial(false));
        yield break;
    }
}
