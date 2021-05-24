using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;
using static PPAccess;

public class MenuScript : MonoBehaviour
{
    public void ExitGame()
    {
        Application.Quit(0);
    }
    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void StartTestLevel()
    {
        SceneManager.LoadScene("TestLevel");
    }
    public void LoadCredits()
    {
        SceneManager.LoadScene("Credits");
    }
    public void LoadMainMenu()
    {
        ResetPostProcess();
        SceneManager.LoadScene("Menu");
    }

    public void ReloadScene()
    {
        ResetPostProcess();
        PauseTheGame(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ChangePauseMenu(bool open)
    {
        StartCoroutine(ChangingMenu(open, transform.GetChild(1).GetComponent<CanvasGroup>()));
    }

    public void ChangeWinLoseMenu(bool open)
    {
        StartCoroutine(ChangingMenu(open, transform.GetChild(transform.childCount - 1).GetComponent<CanvasGroup>()));
    }

    IEnumerator ChangingMenu(bool openingMenu, CanvasGroup menuGroup)
    {
        CanvasGroup ingameGroup = transform.GetChild(0).GetComponent<CanvasGroup>();
        menuGroup.gameObject.SetActive(true);
        SpriteRenderer sun = Camera.main.transform.GetChild(0).GetComponent<SpriteRenderer>();

        Color sunColor = sun.color;

        PauseTheGame(openingMenu);
        float percent;
        for (float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            percent = (openingMenu ? count : 1 - count);
            postprocess.doF.focalLength.value = 1 + 100 * percent;
            ingameGroup.alpha = 1 - percent;
            menuGroup.alpha = percent;
            sun.color = new Color(sun.color.r, sun.color.g, sun.color.b, 1 - percent);
            yield return new WaitForFixedUpdate();
        }

        menuGroup.gameObject.SetActive(openingMenu);
        yield break;
    }
}
