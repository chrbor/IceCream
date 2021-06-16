using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CommentaryScript;

public class Tutorial_2 : TutorialScript
{
    protected override IEnumerator ChangeTutorial(bool active)
    {
        group.alpha = active ? 0 : 1;
        IceStandScript.stopTimer = active;
        if (active) HoldButton.LockAll();
        yield return new WaitForSeconds(.1f);

        for (float count = 0; count < 1; count += timeStep)
        {
            group.alpha = active ? count : 1 - count;
            yield return new WaitForFixedUpdate();
        }
        if (!active)
        {
            commentary.PlayCommentary(comment, commentID);
            HoldButton.UnlockAll();
            Destroy(gameObject);
            yield break;
        }


        //Eingangstext ist Voreinstellung:
        //text.text = "Möchtest du das Tutorial\nzu dem Eisstand sehen?";

        textField.gameObject.SetActive(true);
        buttons.gameObject.SetActive(true);
        textFieldGroup.alpha = 0;
        buttons.alpha = 0;
        for (float count = 0; count < 1; count += timeStep)
        {
            buttons.alpha = count;
            textFieldGroup.alpha = count;
            yield return new WaitForFixedUpdate();
        }
        textFieldGroup.alpha = 1;
        yield break;
    }


    protected override IEnumerator RunTutorial()
    {
        for (float count = 1; count > 0; count -= timeStep)
        {
            buttons.alpha = count;
            yield return new WaitForFixedUpdate();
        }
        buttons.gameObject.SetActive(false);

        text.text = "Keine Sorge: Es ist schlimmer, als es aussieht!";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Der Eisstand funktioniert in 2 Schritten:";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Zuerst muss du die Eiskugeln auswählen, die du haben möchtest";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        for (float count = 1; count > 0; count -= Time.fixedDeltaTime * 5) { textFieldGroup.alpha = count; yield return new WaitForFixedUpdate(); }
        textField.anchoredPosition = new Vector2(-160, -200);
        text.text = "Tippe dazu die jeweilige Eissorte an";
        for (float count = 0; count < 1; count += Time.fixedDeltaTime * 5) { textFieldGroup.alpha = count; yield return new WaitForFixedUpdate(); }
        textFieldGroup.alpha = 1;

        StartCoroutine(ShowView(new Vector2(.65f, .55f), new Vector2(0.05f, .35f)));
        yield return new WaitForSeconds(.2f);

        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Jedes Eis hat spezielle Eigenschaften, die sich auf den Spieler,";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "den Turm aus Eiskugeln und die Umgebung auswirken";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Wenn man eine Eissorte gedrückt hält, öffnet sich ein InfoFeld zu dem Eis";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        StartCoroutine(HideView());
        for(float count = 1; count > 0; count -= Time.fixedDeltaTime * 5) { textFieldGroup.alpha = count; yield return new WaitForFixedUpdate(); }
        textFieldGroup.alpha = 0;

        //Zeige Infofeld:
        Transform infoField = transform.parent.GetChild(0).GetChild(1).GetChild(0).GetChild(1).GetChild(0);
        CanvasGroup startButton = infoField.GetChild(0).GetComponent<CanvasGroup>();
        CanvasGroup infoBox = infoField.GetChild(1).GetComponent<CanvasGroup>();
        infoBox.gameObject.SetActive(true);
        startButton.gameObject.SetActive(true);

        for(float count = 0; count < 1; count += timeStep)
        {
            infoBox.alpha = count;
            startButton.alpha = 1 - count;
            yield return new WaitForFixedUpdate();
        }

        textField.anchoredPosition = new Vector2(-160, -25);
        text.text = "In ihr stehen die wichtigsten Eigenschaften zusammengefasst";    
        for (float count = 0; count < 1; count += Time.fixedDeltaTime * 5) { textFieldGroup.alpha = count; yield return new WaitForFixedUpdate(); }
        textFieldGroup.alpha = 1;

        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        StartCoroutine(ShowView(new Vector2(.1f, .175f), new Vector2(0.575f, 0.05f)));
        yield return new WaitForSeconds(.2f);

        text.text = "Neben der Beschreibung sind diese drei Werte wichtig (von oben nach unten):";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "1. Leben\n2. Reichweite\n3. Größe";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Leben gibt an wie viele Sekunden das Eis existiert, bevor es schmilzt (verschwindet)";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Ist das Eis auf der Waffel, dann hat es immer volle Leben";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Berührt das Eis den Boden, dann verliert es 1 Leben";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Auch äußere Effekte wie Explosionen können die Leben verringern";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Ist das Eis kurz davor zu verschwinden, dann fängt die Kugel an zu blinken.";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Es sollte dann wieder aufgefangen werden, damit es nicht verschwindet";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Die Reichweite gibt an, wie weit das Eis";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);
#if (UNITY_ANDROID)
        text.text = "von der Waffel weggeschossen werden kann (Touchscreen tippen)";
#elif (UNITY_STANDALONE)
        text.text = "von der Waffel weggeschossen werden kann (LEERTASTE)";
#endif
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Die Größe gibt an, wie groß die Eiskugel ist";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Sie bestimmt, wie stark das Eis-Meter ansteigt, wenn es ins Ziel gebracht wird";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        StartCoroutine(HideView());
        yield return new WaitForSeconds(.2f);

        text.text = "Über das Menü kann auf den Eiskatalog zugegriffen werden";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "In ihm stehen alle Eigenschaften der jeweiligen Eissorte";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        for (float count = 1; count > 0; count -= Time.fixedDeltaTime * 5) { textFieldGroup.alpha = count; yield return new WaitForFixedUpdate(); }
        textFieldGroup.alpha = 0;

        for (float count = 0; count < 1; count += timeStep)
        {
            infoBox.alpha = 1 - count;
            startButton.alpha = count;
            yield return new WaitForFixedUpdate();
        }
        infoBox.gameObject.SetActive(false);
        startButton.alpha = 1;

        textField.anchoredPosition = new Vector2(-160, -200);
        text.text = "Das ausgewählte Eis wird auf der Waffel rechts neben dem Eisstand gezeigt";
        StartCoroutine(ShowView(new Vector2(.25f, 1f), new Vector2(0.75f, 0f)));
        for (float count = 0; count < 1; count += Time.fixedDeltaTime * 5) { textFieldGroup.alpha = count; yield return new WaitForFixedUpdate(); }
        textFieldGroup.alpha = 1;

        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Kugeln, die schon auf der Waffel sind, sind ausgegraut und können nicht mehr geändert werden";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Mit dem roten Button wird die gesamte Auswahl gelöscht.";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Mit dem gelben Button wird die oberste Kugel gelöscht.";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Mit dem blauen Button verlässt man den Eisstand";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        StartCoroutine(HideView());
        for (float count = 1; count > 0; count -= Time.fixedDeltaTime * 5) { textFieldGroup.alpha = count; yield return new WaitForFixedUpdate(); }
        textField.anchoredPosition = new Vector2(-160, -25);
        text.text = "Wenn man alle Eiskugeln ausgewählt hat,";
        StartCoroutine(ShowView(new Vector2(.25f, .225f), new Vector2(0.25f, .05f)));
        for (float count = 0; count < 1; count += Time.fixedDeltaTime * 5) { textFieldGroup.alpha = count; yield return new WaitForFixedUpdate(); }
        textFieldGroup.alpha = 1;

        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Dann muss man das Minispiel mit dem grünen Button starten";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Scheitert man mit dem Minispiel, so gibt es keine weiteren Strafen";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Man kann also das Minispiel beliebig oft spielen";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Jedoch läuft, während man am Eisstand ist, weiterhin die Zeit ab";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        StartCoroutine(HideView());
        for (float count = 1; count > 0; count -= Time.fixedDeltaTime * 5) { textFieldGroup.alpha = count; yield return new WaitForFixedUpdate(); }
        textField.anchoredPosition = new Vector2(-160, -200);
        text.text = "Und dies ist das Ende von diesem Tutorial";
        for (float count = 0; count < 1; count += Time.fixedDeltaTime * 5) { textFieldGroup.alpha = count; yield return new WaitForFixedUpdate(); }
        textFieldGroup.alpha = 1;

        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        text.text = "Viel Spaß beim Erstellen des Eisturms!";
        yield return new WaitWhile(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);

        mat_disp.SetTexture("_View", empty);
        StartCoroutine(ChangeTutorial(false));
        yield break;
    }
}
