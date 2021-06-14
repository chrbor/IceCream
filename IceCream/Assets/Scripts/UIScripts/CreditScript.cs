using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditScript : MonoBehaviour
{
    public float normalScroll;
    public float speedScroll;

    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(transform.childCount - 1).GetChild(0).GetChild(0).GetComponent<Animator>().Play("phIdle");

        StartCoroutine(RollCredits());
    }

    IEnumerator RollCredits()
    {
        RectTransform rTransform = GetComponent<RectTransform>();
        float goalPos = rTransform.anchoredPosition.y;
        rTransform.anchoredPosition = new Vector2(0, -rTransform.anchoredPosition.y);


        while(rTransform.anchoredPosition.y < goalPos)
        {
#if(UNITY_STANDALONE || UNITY_EDITOR)
            if(Input.GetKey(KeyCode.Escape)) SceneManager.LoadScene("Menu");
#endif
            rTransform.anchoredPosition += Vector2.up * ((Input.anyKey || Input.GetMouseButton(0) || Input.touchCount > 0) ? speedScroll : normalScroll);
            yield return new WaitForFixedUpdate();
        }
        yield return new WaitForSeconds(1);

        yield return new WaitUntil(() => Input.anyKey || Input.GetMouseButton(0) || Input.touchCount > 0);
        SceneManager.LoadScene("Menu");
        yield break;
    }
}
