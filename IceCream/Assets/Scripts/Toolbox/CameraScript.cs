using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public static CameraScript cScript;
    public static Vector2 camWindow;

    public GameObject target;
    [Range(0, 0.1f)]
    public float strength;
    public Vector2 offset;
    [HideInInspector]
    public AudioSource aSrc;

    private AnimationCurve curve;

    private bool shaking;
    int shakeCount;
    float shakeImpact;

    // Start is called before the first frame update
    void Awake()
    {
        cScript = this;
        aSrc = GetComponent<AudioSource>();
        curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        camWindow = new Vector2(Camera.main.aspect, 1) * Camera.main.orthographicSize;

        if (target == null) return;

        transform.position += (Vector3)(strength * ((Vector2)(target.transform.position - transform.position) + offset)); 
    }

    public void DoShake(float reduction = 1000)
    {
        if (shaking)
        {
            shakeCount += 10;
            shakeImpact = (shakeImpact + Camera.main.orthographicSize / reduction) / 2;
            return;
        }
        shaking = true;
        shakeCount = 40;
        shakeImpact = Camera.main.orthographicSize / reduction;
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        for (; shakeCount > 0; shakeCount--)
        {
            transform.position += (Vector3)(Random.insideUnitCircle * shakeCount * shakeImpact);
            yield return new WaitForEndOfFrame();
        }
        shaking = false;
        yield break;
    }

    public IEnumerator SetRotation(float rotation = 0, float rotTime = 1)
    {
        float start = transform.rotation.eulerAngles.z;
        float diff = rotation - start;
        while (Mathf.Abs(diff) > 180) diff -= Mathf.Sign(diff) * 360;
        float timeStep = Time.fixedDeltaTime / rotTime;
        for (float count = 0; count < 1; count += timeStep)
        {
            transform.eulerAngles = new Vector3(0, 0, start + diff * curve.Evaluate(count));
            yield return new WaitForFixedUpdate();
        }
        yield break;
    }

    public IEnumerator SetZoom(float zoom, float zoomTime = 1)
    {
        float start = Camera.main.orthographicSize;
        float diff = zoom - start;

        float timeStep = Time.fixedDeltaTime / zoomTime;
        for(float count = 0; count < 1; count += timeStep)
        {
            Camera.main.orthographicSize = start + diff * curve.Evaluate(count);
            yield return new WaitForFixedUpdate();
        }

        yield break;
    }

    public IEnumerator SetBGM(AudioClip nextBGM = null, float changeTime = 2)
    {
        bool is_aSrc1_playing = aSrc.isPlaying;

        float timeStep = Time.fixedDeltaTime / changeTime;
        for (float count = 1; count > 0; count -= timeStep)
        {
            aSrc.volume = count;
            yield return new WaitForFixedUpdate();
        }
        aSrc.Stop();
        aSrc.volume = 1;
        yield return new WaitForSeconds(.1f);
        aSrc.clip = nextBGM;
        aSrc.Play();
        yield break;
    }
}
