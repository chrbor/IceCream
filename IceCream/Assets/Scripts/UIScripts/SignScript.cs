using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;
using static CameraScript;

public class SignScript : MonoBehaviour
{
    [TextArea(5, 7)]
    public string text;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.GetComponent<TouchSensor>().tipped || runGame || pauseGame) return;
        PlayerScript.blockShoot = true;

        mainCanvas.transform.GetChild(mainCanvas.transform.childCount - 2).GetChild(1).GetComponent<Text>().text = text;

        StartCoroutine(cScript.SetRotation());
        mainCanvas.GetComponent<MenuScript>().ChangeSign(true);
        StartCoroutine(ReadSign());
    }

    IEnumerator ReadSign()
    {
        yield return new WaitUntil(() => !Input.GetMouseButton(0) && Input.touchCount == 0);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount != 0);
        yield return new WaitUntil(() => !Input.GetMouseButton(0) && Input.touchCount == 0);

        mainCanvas.GetComponent<MenuScript>().ChangeSign(false);
        PlayerScript.blockShoot = false;
        yield break;
    }

}
