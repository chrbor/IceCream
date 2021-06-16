using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static PPAccess;

public class ClockScript : MonoBehaviour
{
    public static ClockScript clock;
    [HideInInspector]
    public float time;
    public float timeLeft;

    public Gradient timeColor;
    public AnimationCurve hdrFactor;

    [HideInInspector]
    public bool running
    {
        get { return run; }
        set
        {
            run = value;
            if (run && clock != null) clock.StartTimer();
        }
    }
    private bool run;

    private RectTransform timePointer;
    private Transform sun;
    private RectTransform sunImage;

    private float width;

    // Start is called before the first frame update
    void Awake()
    {
        clock = this;
        time = 0;

        timePointer = transform.GetChild(2).GetComponent<RectTransform>();
        width = timePointer.anchoredPosition.x * 2;
        sun = Camera.main.transform.GetChild(0);

#if (UNITY_ANDROID)
        sun.gameObject.SetActive(false);
        sunImage = transform.GetChild(0).GetComponent<RectTransform>();
        sunImage.gameObject.SetActive(true);
#endif

        running = true;
    }

    private void StartTimer()
    {
        StartCoroutine(RunTimer());
    }

    public void SetStartEnd(float startpoint, float endpoint)
    {
        transform.GetChild(3).GetComponent<RectTransform>().anchoredPosition = new Vector2(width * (1 - startpoint / 420f), 37.5f);
        transform.GetChild(4).GetComponent<RectTransform>().anchoredPosition = new Vector2(width * (1 - endpoint / 420f), 37.5f);

        transform.GetChild(3).gameObject.SetActive(true);
        transform.GetChild(4).gameObject.SetActive(true);
    }

    private bool fastForwarding;
    public void FastForward(bool isOn)
    {
        fastForwarding = isOn;
    }

    IEnumerator RunTimer()
    {
        while(run && timeLeft > 0)
        {
            //Sonne ist im Sommer in Italien von 6 bis 20Uhr auf
            //Bei 1min = .5sec führt das zu einer maximalen Spielzeit von 420sec = 7min (~ Zeit in Mario)
            float percent = 1 - timeLeft / 420f;
            float centered = percent - .5f;
            postprocess.colorAdj.colorFilter.value = hdrFactor.Evaluate(percent) * timeColor.Evaluate(percent);
            timePointer.anchoredPosition = Vector2.right * width * percent;
            
#if (UNITY_STANDALONE || UNITY_EDITOR)
            sun.localScale = Vector3.one * Camera.main.orthographicSize * .25f;
            sun.position = Camera.main.transform.position + new Vector3(centered * 3f * Camera.main.orthographicSize, -Camera.main.orthographicSize * (.5f + centered * centered), 10);
#endif

#if (UNITY_ANDROID)
            sunImage.anchoredPosition = timePointer.anchoredPosition + Vector2.up * 50 * (-Camera.main.orthographicSize * (-0.25f + centered * centered));
#endif
            /*
            float rot_current = Camera.main.transform.eulerAngles.z * Mathf.Deg2Rad;
            float height = -Camera.main.orthographicSize * (.5f + centered * centered) + 5;
            sun.position = timePointer.position + new Vector3(-Mathf.Sin(rot_current), Mathf.Cos(rot_current)) * height;
            */

            //yield return new WaitForSeconds(1);
            yield return new WaitForFixedUpdate();
            timeLeft -= fastForwarding ? 1 : Time.fixedDeltaTime;
            time += Time.fixedDeltaTime;

            if (timeLeft <= 0) transform.parent.parent.GetComponent<MenuScript>().ChangeWinLoseMenu(true);
        }

        yield break;
    }

}
