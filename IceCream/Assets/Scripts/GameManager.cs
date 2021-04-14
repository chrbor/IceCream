using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;

    public static bool staticCam;
    public static bool pauseMove;
    public static bool runGame;

    // Start is called before the first frame update
    void Start()
    {
        manager = this;
    }
}
