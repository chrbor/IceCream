using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonGroupScript : MonoBehaviour
{
    public enum ButtonGroupType { AND, XOR}
    public ButtonGroupType type;
    public int inputNumber;
    private int count;
    private bool active, changed;

    public UnityEvent OnRisingEdge, OnFallingEdge;

    // Start is called before the first frame update
    void Start()
    {
        count = 0;
    }

    public void AddOne()
    {
        count++;
        CheckChange();
    }

    public void SubstractOne()
    {
        count--;
        CheckChange();
    }

    private void CheckChange( )
    {
        changed = false;
        switch (type)
        {
            case ButtonGroupType.AND: changed = count == inputNumber ^ active; break;
            case ButtonGroupType.XOR: changed = count == 1 ^ active; break;
        }

        if(changed)
        {
            active = !active;
            if (active) OnRisingEdge.Invoke();
            else OnFallingEdge.Invoke();
        } 
    }
}
