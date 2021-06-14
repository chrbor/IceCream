using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressScript : MonoBehaviour
{
    public static ProgressScript progressDisplay;

    public static List<IceAttribute> iceDelivered;
    public static float progressPoints;

    public static float goal_Bronze, goal_Silver, goal_Gold;

    private float x_max;
    private RectTransform slider;
    private Rect rect_slider;
    private RectTransform bronzeLimit, silverLimit;

    private Image Troph_Bronze, Troph_Silver, Troph_Gold;
    private bool bronzeWin, silverWin, goldWin;
    private MenuScript menu;

    private AnimationCurve smoothMove;

    // Start is called before the first frame update
    void Start()
    {
        progressDisplay = this;
        iceDelivered = new List<IceAttribute>();

        menu = transform.parent.parent.GetComponent<MenuScript>();
        smoothMove = AnimationCurve.EaseInOut(0, 0, 1, 1);

        //Muss später in Awake eingestellt werden:
        goal_Bronze = 3;
        goal_Silver = 4;
        goal_Gold = 5;
        progressPoints = 0;

        //Hole alle wichtigen elemente:
        Troph_Bronze = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        Troph_Silver = transform.GetChild(0).GetChild(1).GetComponent<Image>();
        Troph_Gold   = transform.GetChild(0).GetChild(2).GetComponent<Image>();

        slider = transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<RectTransform>();
        bronzeLimit = transform.GetChild(1).GetChild(1).GetComponent<RectTransform>();
        silverLimit = transform.GetChild(1).GetChild(2).GetComponent<RectTransform>();
        x_max = slider.rect.width;//silverLimit.anchoredPosition.x;

        //Baue auf:
        slider.sizeDelta *= Vector2.up;
        slider.anchoredPosition = Vector2.zero;
        bronzeLimit.anchoredPosition = Vector2.right * x_max * goal_Bronze / goal_Gold;
        silverLimit.anchoredPosition = Vector2.right * x_max * goal_Silver / goal_Gold;

        Color troph_color = new Color(1, 1, 1, 0.25f);
        Troph_Bronze.color = troph_color;
        Troph_Silver.color = troph_color;
        Troph_Gold.color = troph_color;
    }

    public void UpdateProgressDisplay(IceAttribute attribute)
    {
        iceDelivered.Add(attribute);
        if(progressPoints > goal_Gold) { progressPoints += attribute.scale; return; }

        progressPoints += attribute.scale;
        slider.sizeDelta = new Vector2(x_max * (progressPoints > goal_Gold ? 1 : progressPoints / goal_Gold), slider.sizeDelta.y);
        slider.anchoredPosition = new Vector2(slider.sizeDelta.x/2, 0);
        if (!bronzeWin && progressPoints >= goal_Bronze) { bronzeWin = true; StartCoroutine(ShowTrophy(Troph_Bronze)); }
        if (!silverWin && progressPoints >= goal_Silver) { silverWin = true; StartCoroutine(ShowTrophy(Troph_Silver)); }
        if (!goldWin && progressPoints >= goal_Gold) { goldWin = true; StartCoroutine(ShowTrophy(Troph_Gold)); }


        if(progressPoints >= goal_Gold)
            menu.ChangeWinLoseMenu(true);
    }

    IEnumerator ShowTrophy(Image iTrophy)
    {
        float timeStep = Time.fixedDeltaTime / .5f;

        //werde opaque und x1.5 so groß
        Color stepColor = (1 - iTrophy.color.a) * timeStep * Color.black;
        Vector3 diffScale = .5f * Vector3.one;
        for (float count = 0; count < 1; count += timeStep)
        {
            iTrophy.color += stepColor;
            iTrophy.transform.localScale = Vector3.one + diffScale * smoothMove.Evaluate(count);
            yield return new WaitForFixedUpdate();
        }
        for (float count = 0; count < 1; count += timeStep)
        {
            iTrophy.transform.localScale = Vector3.one + diffScale * smoothMove.Evaluate(1 - count);
            yield return new WaitForFixedUpdate();
        }

        yield break;
    }
}
