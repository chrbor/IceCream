using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;
using static ClockScript;
using static HoldButton;

public class GelateriaScript : MonoBehaviour
{
    public static GelateriaScript gelateria;

    bool hasPlayer;
    bool loadingScene;
    public string sceneName;

    private Animator anim_vendor, anim_wagen;
    public float openTime, closeTime;
    public bool isOpen;
    public bool createConeRightSide;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;

        anim_wagen = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
        anim_vendor = transform.GetChild(1).GetChild(0).GetComponent<Animator>();

        if(clock == null)
        {
            Set_isOpen(true);
            return;
        }

        clock.SetStartEnd(openTime, closeTime);
        StartCoroutine(WaitForOpenClose());
    }

    IEnumerator WaitForOpenClose()
    {
        Set_isOpen(isOpen || openTime < 0 || openTime > clock.timeLeft);

        if (!isOpen)
        {
            yield return new WaitUntil(() => clock.timeLeft < openTime);
            Set_isOpen(true);
        }

        yield return new WaitUntil(() => clock.timeLeft < closeTime);
        Set_isOpen(false);
    } 

    void Set_isOpen(bool _isOpen)
    {
        isOpen = _isOpen;
        GetComponent<Collider2D>().enabled = isOpen;
        anim_vendor.SetBool("open", isOpen);
        anim_wagen.SetBool("open", isOpen);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == 9) { hasPlayer = true; anim_vendor.SetTrigger("wink"); return; }
        if (!(hasPlayer && other.GetComponent<TouchSensor>().tipped && isOpen) || buttonPressed) return;

        //Was passiert, wenn angetippt wird:
        loadingScene = true;
        //globalLight.SetActive(false);
        mainCanvas.SetActive(false);
        mainListener.enabled = false;
        PauseTheGame(true);
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 9) hasPlayer = false;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!loadingScene || mode == LoadSceneMode.Single) return;
        loadingScene = false;

        gelateria = this;
        mainCamera.SetActive(false);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
    }
    void OnSceneUnloaded(Scene scene)
    {
        //globalLight.SetActive(true);
        mainCanvas.SetActive(true);
        mainListener.enabled = true;
        PauseTheGame(false);
        
    }
}
