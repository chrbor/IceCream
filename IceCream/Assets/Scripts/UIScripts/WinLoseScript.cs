using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ProgressScript;

public class WinLoseScript : MonoBehaviour
{
    CanvasGroup buttonsGroup;

    // Start is called before the first frame update
    void Start()
    {
        buttonsGroup = transform.GetChild(0).GetComponent<CanvasGroup>();

        Canvas.ForceUpdateCanvases();
        //Wenn nicht geschafft, dann verstecke "weiter"-Button
        if(progressPoints < goal_Bronze)
        {
            buttonsGroup.transform.GetChild(1).gameObject.SetActive(true);
            buttonsGroup.transform.GetChild(3).gameObject.SetActive(false);
        }

        StartCoroutine(ShowLevelResult());
    }

    IEnumerator ShowLevelResult()
    {
        //Staple alle Eiskugeln, um das Endergebnis anzuzeigen:


        //Zeige Buttons:
        buttonsGroup.gameObject.SetActive(true);
        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            buttonsGroup.alpha = count;
            yield return new WaitForFixedUpdate();
        }
        yield break;
    }
}
