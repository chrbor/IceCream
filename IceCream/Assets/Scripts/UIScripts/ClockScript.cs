﻿using System.Collections;
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
    public bool running;

    private RectTransform timePointer;
    private Transform sun;

    private float width;

    // Start is called before the first frame update
    void Start()
    {
        clock = this;
        time = 0;

        timePointer = transform.GetChild(1).GetComponent<RectTransform>();
        width = timePointer.anchoredPosition.x * 2;
        sun = Camera.main.transform.GetChild(0);

        StartTimer();
    }

    public void StartTimer()
    {
        StartCoroutine(RunTimer());
    }

    IEnumerator RunTimer()
    {
        running = true;

        while(running && timeLeft > 0)
        {
            //Sonne ist im Sommer in Italien von 6 bis 20Uhr auf
            //Bei 1min = .5sec führt das zu einer maximalen Spielzeit von 420sec = 7min (~ Zeit in Mario)
            float percent = 1 - timeLeft / 420f;
            float centered = percent - .5f;
            postprocess.colorAdj.colorFilter.value = hdrFactor.Evaluate(percent) * timeColor.Evaluate(percent);
            timePointer.anchoredPosition = Vector2.right * width * percent;
            sun.localScale = Vector3.one * Camera.main.orthographicSize * .25f;
            sun.position = Camera.main.transform.position + new Vector3(centered * 1.4f * Camera.main.aspect * Camera.main.orthographicSize, -Camera.main.orthographicSize * (.7f + centered * centered), 10);

            //yield return new WaitForSeconds(1);
            yield return new WaitForFixedUpdate();
            timeLeft -= Time.fixedDeltaTime;
            time += Time.fixedDeltaTime;
        }

        if(!running)
        {
            yield return new WaitUntil(() => running);
            StartCoroutine(RunTimer());
        }
        yield break;
    }

}