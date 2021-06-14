using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class TutorialScript : MonoBehaviour
{
    protected CanvasGroup group, buttons, textFieldGroup;

    protected RectTransform textField;
    protected Text text;

    protected Image disp_image;
    protected Material mat_disp;
    public Texture2D circleMask, rectMask, empty;

    protected float timeStep;

    // Start is called before the first frame update
    void Start()
    {
        group = GetComponent<CanvasGroup>();
        buttons = transform.GetChild(2).GetComponent<CanvasGroup>();

        textField = transform.GetChild(1).GetComponent<RectTransform>();
        textFieldGroup = textField.GetComponent<CanvasGroup>();
        text = textField.GetChild(0).GetComponent<Text>();

        disp_image = transform.GetChild(0).GetComponent<Image>();
        mat_disp = disp_image.material;
        mat_disp.SetTexture("_View", empty);

        timeStep = Time.fixedDeltaTime / .5f;
        StartCoroutine(ChangeTutorial(true));
    }

    protected virtual IEnumerator ChangeTutorial(bool active)
    {
        group.alpha = active ? 0 : 1;
        yield return new WaitForSeconds(.1f);
        if (active) PauseTheGame(true);

        for (float count = 0; count < 1; count += timeStep)
        {
            group.alpha = active ? count : 1 - count;
            yield return new WaitForFixedUpdate();
        }
        if (!active) { PauseTheGame(false); Destroy(gameObject); yield break; }


        //Eingangstext ist Voreinstellung:
        //text.text = "Möchtest du das Tutorial\nzu dem Spiel sehen?";

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

    virtual protected IEnumerator RunTutorial()
    {
        StartCoroutine(ChangeTutorial(false));
        yield break;
    }

    public void EndTutorial()
    {
        StartCoroutine(ChangeTutorial(false));
    }
    public void StartTutorial()
    {
        StartCoroutine(RunTutorial());
    }

    protected IEnumerator ShowView(Vector2 scale, Vector2 offset, bool circle = false)
    {
        float step = Time.fixedDeltaTime / .2f;

        mat_disp.SetTexture("_View", circle ? circleMask : rectMask);
        mat_disp.SetVector("_scale", scale);
        mat_disp.SetVector("_offset", -offset);

        for (float count = 0; count < 1; count += step)
        {
            mat_disp.SetFloat("_strength", count);
            yield return new WaitForFixedUpdate();
        }
        yield break;
    }
    protected IEnumerator HideView()
    {
        float step = Time.fixedDeltaTime / .2f;
        for (float count = 1; count > 0; count -= step)
        {
            mat_disp.SetFloat("_strength", count);
            yield return new WaitForFixedUpdate();
        }
        mat_disp.SetFloat("_strength", 0);
        yield break;
    }
}
