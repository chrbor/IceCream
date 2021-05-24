using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Coverscript : MonoBehaviour
{
    private Text clickToStart;

    // Start is called before the first frame update
    void Start()
    {
        clickToStart = transform.GetChild(1).GetComponent<Text>();
        StartCoroutine(BlinkText());
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKey || Input.GetMouseButton(0) || Input.touchCount > 0)
        {
            SceneManager.LoadScene("Menu");
            Destroy(this);
        }
    }

    IEnumerator BlinkText()
    {
        float timeStep = Time.fixedDeltaTime / 1;
        Color addedColor = Color.black * -timeStep * .5f;
        while (true)
        {
            for(float count = 0; count < 1; count += timeStep)
            {
                clickToStart.color += addedColor;
                yield return new WaitForFixedUpdate();
            }
            addedColor *= -1;
        }
    }
}
