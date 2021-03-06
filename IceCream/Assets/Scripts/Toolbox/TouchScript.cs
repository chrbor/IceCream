using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using static TouchAndScreen;

using UnityEngine.UI;

public class TouchScript : MonoBehaviour
{
    public GameObject touchPrefab;

    InputControl control;
    private int mask_id = 0;
    public static bool portraitMode;

    private GameObject mousePointer;

    private void Start()
    {
        //Aktiviere Inputsystem:
        EnhancedTouchSupport.Enable();
    }

    private void Update()
    {
        //if (pauseGame || !runGame) return;
        foreach (var touch in Touch.activeFingers) StartCoroutine(KeepTouchpoint(touch));

        //Mouse-Support:
        if (mousePointer != null)
        {
            if (!Input.GetMouseButton(0)) { Destroy(mousePointer); return; }

            mousePointer.transform.position = PixelToWorld(Input.mousePosition);
        }
        else if(Input.GetMouseButton(0))
        {
            mousePointer = Instantiate(touchPrefab, PixelToWorld(Input.mousePosition), Quaternion.identity);
            mousePointer.GetComponent<TouchSensor>().Initialize(999);
        }
    }

    IEnumerator KeepTouchpoint(Finger finger)
    {
        //checke id:
        if ((1 << finger.index & mask_id) > 0) yield break;
        mask_id |= 1 << finger.index;

        //Positioniere Touchpoint:
        GameObject touchSensor = Instantiate(touchPrefab, PixelToWorld(finger.screenPosition), Quaternion.identity);
        touchSensor.GetComponent<TouchSensor>().Initialize(finger.index);
        while (finger.isActive)
        {
            touchSensor.transform.position = PixelToWorld(finger.screenPosition);
            yield return new WaitForEndOfFrame();
        }

        Destroy(touchSensor);
        mask_id ^= 1 << finger.index;

        yield break;
    }
}
