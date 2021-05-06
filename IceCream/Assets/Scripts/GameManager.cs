using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager manager;

    public static bool staticCam;
    public static bool pauseMove;
    public static bool runGame;

    public static GameObject player;
    public static PlayerAttribute pAttribute;

    // Start is called before the first frame update
    void Start()
    {
        manager = this;
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
