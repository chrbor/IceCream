using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ClockScript;
using static CameraScript;
public class GameManager : MonoBehaviour
{
    public static GameManager manager;

    public static bool staticCam;
    public static bool pauseMove;
    public static bool runGame;

    public static bool pauseGame;
    public static bool pauseMiniGame;

    public static bool commentaryON;

    public static GameObject player;
    public static PlayerAttribute pAttribute;

    public static Scene mainScene;
    public static AudioListener mainListener;
    public static GameObject mainCamera;
    public static GameObject globalLight;
    public static GameObject mainCanvas;

    // Start is called before the first frame update
    void Start()
    {
        commentaryON = true;

        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;

        manager = this;
        mainScene = SceneManager.GetActiveScene();
        mainListener = Camera.main.GetComponent<AudioListener>();
        mainCamera = Camera.main.gameObject;
        globalLight = GameObject.FindGameObjectWithTag("light");
    }

    public static void PauseTheGame(bool pausing)
    {
        pauseGame = pausing;
        pauseMove = pausing;
        Physics2D.autoSimulation = !pausing;
        if (clock != null) clock.running = !pausing;
    }

    [System.Serializable]
    public class PlayerAttribute
    {
        public float velocity;
        public float real_vel { get; private set; }
        public float jumpPower;

        public void Set_vel(float vel) { velocity = vel; real_vel = velocity * Time.fixedDeltaTime; }
        public void Set_vel() => real_vel = velocity * Time.fixedDeltaTime;
    }
}
