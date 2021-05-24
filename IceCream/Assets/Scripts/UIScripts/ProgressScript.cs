using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class ProgressScript : MonoBehaviour
{
    public static ProgressScript progressDisplay;

    public static float progressPoints;
    public static float goal_Bronze, goal_Silver, goal_Gold;

    private float x_max;
    private Slider slider;
    private RectTransform bronzeLimit, silverLimit;

    private Image Troph_Bronze, Troph_Silver, Troph_Gold;
    private MenuScript menu;

    // Start is called before the first frame update
    void Start()
    {
        progressDisplay = this;

        menu = transform.parent.parent.GetComponent<MenuScript>();

        //Muss später in Awake eingestellt werden:
        goal_Bronze = 3;
        goal_Silver = 5;
        goal_Gold = 6;

        //Hole alle wichtigen elemente:
        Troph_Bronze = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        Troph_Silver = transform.GetChild(0).GetChild(1).GetComponent<Image>();
        Troph_Gold   = transform.GetChild(0).GetChild(2).GetComponent<Image>();

        slider = transform.GetChild(1).GetComponent<Slider>();
        bronzeLimit = slider.transform.GetChild(1).GetChild(1).GetComponent<RectTransform>();
        silverLimit = slider.transform.GetChild(1).GetChild(2).GetComponent<RectTransform>();
        x_max = silverLimit.anchoredPosition.x;

        //Baue auf:
        slider.value = 0;
        bronzeLimit.anchoredPosition = Vector2.right * x_max * goal_Bronze / goal_Gold;
        silverLimit.anchoredPosition = Vector2.right * x_max * goal_Silver / goal_Gold;

        Color troph_color = new Color(1, 1, 1, 0.25f);
        Troph_Bronze.color = troph_color;
        Troph_Silver.color = troph_color;
        Troph_Gold.color = troph_color;
    }

    public void UpdateProgressDisplay(float progressToAdd)
    {
        progressPoints += progressToAdd;
        slider.value = progressPoints > goal_Gold ? 1 : progressPoints / goal_Gold;
        if (progressPoints >= goal_Bronze) Troph_Bronze.color = Color.white;
        if (progressPoints >= goal_Silver) Troph_Silver.color = Color.white;
        if (progressPoints >= goal_Gold) Troph_Silver.color = Color.white;

        if(progressPoints >= goal_Gold)
            menu.ChangeWinLoseMenu(true);
    }
}
