using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static GameManager;
using static Cone2Script;
using static ClockScript;

public class IceStandScript : MonoBehaviour
{
    public static IceStandScript iceStand;
    public static GameObject canvas;

    public enum IceType { vanilla, schoco, strawberry, pistacie, straciatella, blue, chilli, mango, lemon, cassis, milk}
    public static List<IceAttribute> plannedIce, addedIce;


    public GameObject IceImagePrefab;
    public GameObject IcePrefab;
    public GameObject ConePrefab;

    [Header("Prefabs für die Minispiele:")]
    public GameObject miniGamePrefab;
    private MiniGameScript miniGame;
    public GameObject MiniIcePrefab;
    public GameObject MiniConePrefab;

    [HideInInspector]
    public Transform iceDisplay;
    Transform startField, infoField;
    Text header, anecdote, body, dataText;
    Image iceImage;

    private void Start()
    {
        iceStand = this;
        canvas = transform.parent.parent.gameObject;
        StartCoroutine(UpdateTimer());

        plannedIce = new List<IceAttribute>();
        addedIce = new List<IceAttribute>();
        Canvas.ForceUpdateCanvases();
        Camera.main.transform.position = new Vector3(-4 * Camera.main.orthographicSize * Camera.main.aspect, 1<<15, -10);

        //Ermittle alle benötigten Referenzen:
        iceDisplay = transform.GetChild(1);
        startField = transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0);
        infoField = startField.parent.GetChild(1);

        header = infoField.GetChild(0).GetChild(0).GetComponent<Text>();
        anecdote = infoField.GetChild(0).GetChild(1).GetComponent<Text>();
        body = infoField.GetChild(0).GetChild(2).GetComponent<Text>();
        iceImage = infoField.GetChild(1).GetComponent<Image>();
        dataText = infoField.GetChild(2).GetComponent<Text>();

        //Erstelle Cone in der Plattformer-Scene:
        if(SceneManager.sceneCount > 1 && cone == null)
        {
            GameObject coneObj = Instantiate(ConePrefab, player.transform.position, Quaternion.identity);
            coneObj.GetComponent<ParentObjScript>().target = player.transform.GetChild(0).GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetChild(0).gameObject;
            SceneManager.MoveGameObjectToScene(coneObj, mainScene);
        }

        //Erstelle bestehendes Eis:
        UpdateIceDisplay();
    }

    public void UpdateIceDisplay()
    {
        if (cone == null) return;
        GameObject obj;
        Color light_gray = new Color(.5f, .5f, .5f, 1);
        for(int i = iceDisplay.childCount; i < cone.iceTower.Count; i++)
        {
            obj = Instantiate(IceImagePrefab, iceDisplay.transform);
            obj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Attributes/Sprites/" + cone.iceTower[i].Get_attribute().name);
            obj.GetComponent<Image>().color = light_gray;
        }
        for (int i = iceDisplay.childCount - cone.iceTower.Count; i < addedIce.Count; i++)
        {
            obj = Instantiate(IceImagePrefab, iceDisplay.transform);
            obj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Attributes/Sprites/" + addedIce[i].name);
            obj.GetComponent<Image>().color = light_gray;
        }        
    }

    public void ExitMiniGame()
    {
        if (mainScene == null) return;
        if (addedIce.Count > 0) GelateriaScript.gelateria.waitForIce = true;
        AddToTower(addedIce);

        Scene oldScene = SceneManager.GetActiveScene();
        SceneManager.SetActiveScene(mainScene);
        mainCamera.SetActive(true);
        SceneManager.UnloadSceneAsync(oldScene);
    }

    public void StartMiniGame()
    {
        if (plannedIce.Count == 0) return;

        if(miniGamePrefab == null)
        {
            addedIce.AddRange(plannedIce);
            AddToTower(addedIce);
            ExitMiniGame();
            return;
        }
        EventSystem.current.SetSelectedGameObject(null);

        //Starte das minigame:
        miniGame = Instantiate(miniGamePrefab).GetComponent<MiniGameScript>();
        miniGame.StartGame();
    }

    public void AddToTower(List<IceAttribute> addedAttributes, int numberOfIce = 999)
    {
        if (cone == null) return;

        //Ordne das Eis richtig an:
        int index = 1;
        if (cone.iceTower.Count > 1)
        {
            cone.transform.rotation = Quaternion.identity;
            float posInCone_height = (cone.iceTower[1].Get_transform().localScale.x + cone.iceTower[0].Get_transform().localScale.x) * .65f;
            for(; index < cone.iceTower.Count-1; index++)
            {
                cone.iceTower[index].Set_posInCone(Vector2.up * posInCone_height);
                posInCone_height += (cone.iceTower[index].Get_transform().localScale.x + cone.iceTower[index + 1].Get_transform().localScale.x) * .3f;
            }
            cone.iceTower[cone.iceTower.Count - 1].Set_posInCone(Vector2.up * posInCone_height);
        }

        if (addedAttributes.Count == 0) return;

        //Füge das Eis hinzu:
        numberOfIce = Mathf.Clamp(numberOfIce, 0, addedAttributes.Count);
        if (numberOfIce < addedAttributes.Count) addedAttributes.RemoveRange(numberOfIce, addedAttributes.Count - numberOfIce);
        int lastIce = cone.iceTower.Count - 1;
        Vector3 icePos = cone.transform.position + 
            (cone.iceTower.Count == 1 ? Vector3.up : (Vector3)cone.iceTower[lastIce].Get_posInCone() + Vector3.up * (cone.iceTower[lastIce].Get_attribute().scale + addedAttributes[0].scale) * .75f);

        GameObject obj = Instantiate(IcePrefab);
        SceneManager.MoveGameObjectToScene(obj, mainScene);
        obj.GetComponent<IceScript>().attribute = addedAttributes[0];
        obj.transform.position = icePos;

        for (int i = 1; i < addedAttributes.Count; i++)
        {
            obj = Instantiate(IcePrefab);
            SceneManager.MoveGameObjectToScene(obj, mainScene);

            obj.GetComponent<IceScript>().attribute = addedAttributes[i];

            icePos += Vector3.up * (addedAttributes[i-1].scale + addedAttributes[i].scale) * .75f;
            obj.transform.position = icePos;
        }
        addedAttributes.Clear();
    }

    public void ResetIce()
    {
        //Erstes Element ist die Waffel, alle Kugeln in iceTower können nicht geändert werden, da sie schon auf der Waffel sind:
        int min = cone != null ? cone.iceTower.Count + addedIce.Count - 1 : 0;
        Debug.Log("min: " + min + ", childcount: " + iceDisplay.childCount);
        for(int i = iceDisplay.childCount - 1; i > min; i--)
        {
            Destroy(iceDisplay.GetChild(i).gameObject);
        }
        plannedIce.Clear();
    }

    public void DeleteLastIce()
    {
        if (plannedIce.Count == 0) return;
        Destroy(iceDisplay.GetChild(iceDisplay.childCount - 1).gameObject);
        plannedIce.RemoveAt(plannedIce.Count - 1);
    }

    public void AddIce(IceAttribute attribute)
    {
        if (plannedIce.Count >= 45) return;
        plannedIce.Add(attribute);
        GameObject obj = Instantiate(IceImagePrefab, iceDisplay.transform);
        obj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Attributes/Sprites/" + attribute.name);
    }

    public void ShowInfo(IceAttribute attribute)
    {
        header.text = attribute.name;
        anecdote.text = attribute.anecdote;
        body.text = attribute.description;
        iceImage.sprite = Resources.Load<Sprite>("Attributes/Sprites/" + attribute.name);
        dataText.text = attribute.life.ToString() + "\n" + attribute.shootPower + "\n" + Mathf.RoundToInt(attribute.scale * 10);

        StartCoroutine(ShowingInfo());
    }

    IEnumerator ShowingInfo()
    {
        StartCoroutine(ChangingInfo(false));
        yield return new WaitUntil(() => !(Input.GetMouseButton(0) || Input.touchCount > 0));
        yield return new WaitForSeconds(.5f);
        yield return new WaitUntil(() => Input.GetMouseButton(0) || Input.touchCount > 0);
        StartCoroutine(ChangingInfo(true));
        yield break;
    }
    IEnumerator ChangingInfo(bool hide)
    {
        //Animation zur Anzeige der Info:
        startField.gameObject.SetActive(true);
        infoField.gameObject.SetActive(true);
        CanvasGroup startGroup = startField.GetComponent<CanvasGroup>();
        CanvasGroup infoGroup = infoField.GetComponent<CanvasGroup>();

        startGroup.alpha = hide ? 0 : 1;
        infoGroup.alpha = 1 - startGroup.alpha;

        float timeStep = 2 * Time.fixedDeltaTime;
        float startStep = hide ? timeStep : -timeStep;
        float infoStep = -startStep;
        for (float count = 0; count < 1; count += timeStep)
        {
            startGroup.alpha += startStep;
            infoGroup.alpha += infoStep;
            yield return new WaitForFixedUpdate();
        }

        startField.gameObject.SetActive(hide);
        infoField.gameObject.SetActive(!hide);
        if (hide) HoldButton.UnlockAll();
    }

    public static bool stopTimer;
    IEnumerator UpdateTimer()
    {
        if (clock == null) yield break;

        while(clock.timeLeft > 0)
        {
            if (stopTimer) yield return new WaitWhile(() => stopTimer);
            yield return new WaitForFixedUpdate();
            clock.timeLeft -= Time.fixedDeltaTime;
            clock.time += Time.fixedDeltaTime;
        }
        yield break;
    }
}
