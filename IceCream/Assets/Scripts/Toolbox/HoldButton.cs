using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent OnTipped, OnHold;

    public static event UnityAction<bool> lockButtons;
    public static event UnityAction unlockButtons;

    protected Sprite normal;
    public Sprite selectedSprite;
    public Sprite clickedSprite;
    public Sprite lockedSprite;

    Image image;

    protected bool pointerDown;
    protected bool locked, selected;
    
    void Awake()
    {
        lockButtons += LockButton;
        unlockButtons += UnLockButton;

        image = GetComponent<Image>();
        normal = image.sprite;
        ResetButton();
    }
    private void OnDestroy()
    {
        lockButtons -= LockButton;
        unlockButtons -= UnLockButton;
    }

    public static void UnlockAll() => unlockButtons.Invoke();
    void UnLockButton() { locked = false; selected = false; }
    void LockButton(bool locking)
    {
        locked = locking;
        selected &= locked;
        image.sprite = !locked ? normal : selected ? selectedSprite : lockedSprite;
    }


    public void ResetButton()
    {
        pointerDown = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //switch(button_type)
        if (!(pointerDown || locked))
        {
            pointerDown = true;

            //Do stuff on press:
            //image.color = new Color(1, 1, 1, 0.75f);
            image.sprite = clickedSprite;
            StartCoroutine(HoldCounter());
        }
    }
    public virtual void OnPointerUp(PointerEventData eventData)
    {
        pointerDown = false;
        if (!locked)
        {
            //pointerDown = false;

            //Do stuff on release:
            image.sprite = normal;
            OnTipped.Invoke();
        }
    }

    bool counting;
    IEnumerator HoldCounter()
    {
        if (counting) yield break;
        counting = true;

        for(float count = 0; count < 1 && pointerDown; count += Time.fixedDeltaTime) yield return new WaitForFixedUpdate();
        if (!pointerDown) { counting = false; yield break; }

        selected = true;
        lockButtons.Invoke(true);
        OnHold.Invoke();

        counting = false;
        yield break;
    }
}
