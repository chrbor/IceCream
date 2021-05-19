using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;

public class GelateriaScript : MonoBehaviour
{
    public static GelateriaScript gelateria;

    bool hasPlayer;
    bool loadingScene;
    public string sceneName;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.layer == 9) { hasPlayer = true; return; }
        if (!(hasPlayer && other.GetComponent<TouchSensor>().tipped)) return;

        //Was passiert, wenn angetippt wird:
        loadingScene = true;
        globalLight.SetActive(false);
        mainListener.enabled = false;
        pauseGame = true;
        pauseMove = true;
        Physics2D.autoSimulation = false;
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
        globalLight.SetActive(true);
        mainListener.enabled = true;
        pauseGame = false;
        pauseMove = false;
        Physics2D.autoSimulation = true;
    }
}
