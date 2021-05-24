using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CatalogScript : MonoBehaviour
{
    CanvasGroup infoField, buttonField, placeHolder;
    Text title, anecdote, infoText;
    Image iceImage;
    Text life, range, velocity, jumpForce, agility, stability, upForce, fallSpeed;

    Animator anim;

    bool initialized;
    private void Start()
    {
        InitializeCatalog();
    }

    void InitializeCatalog()
    {
        if (initialized) return;
        initialized = true;

        buttonField = transform.GetChild(0).GetChild(0).GetComponent<CanvasGroup>();
        infoField = transform.GetChild(0).GetChild(1).GetComponent<CanvasGroup>();
        placeHolder = transform.GetChild(0).GetChild(2).GetComponent<CanvasGroup>();

        Transform Tinfo = infoField.transform;
        title = Tinfo.GetChild(0).GetComponent<Text>();
        anecdote = Tinfo.GetChild(1).GetComponent<Text>();
        iceImage = Tinfo.GetChild(2).GetComponent<Image>();
        infoText = Tinfo.GetChild(4).GetComponent<Text>();

        Transform Tbutton = Tinfo.GetChild(5);
        life = Tbutton.GetChild(1).GetComponent<Text>();
        range = Tbutton.GetChild(3).GetComponent<Text>();
        velocity = Tbutton.GetChild(5).GetComponent<Text>();
        jumpForce = Tbutton.GetChild(7).GetComponent<Text>();
        agility = Tbutton.GetChild(9).GetComponent<Text>();
        stability = Tbutton.GetChild(11).GetComponent<Text>();
        upForce = Tbutton.GetChild(13).GetComponent<Text>();
        fallSpeed = Tbutton.GetChild(15).GetComponent<Text>();

        anim = GetComponent<Animator>();
    }

    public void ChangeCatalog(bool opening)
    {
        if (opening)
        {
            InitializeCatalog();
            gameObject.SetActive(true);
            anim.enabled = true;
            anim.Play("OpenCatalog");
        }
        else
        {
            anim.enabled = true;
            anim.Play("CloseCatalog");
        }
        StartCoroutine(CloseAnim(opening));
    }

    IEnumerator CloseAnim(bool opening)
    {
        yield return new WaitForSeconds(.5f);
        anim.enabled = false;
        if (!opening) gameObject.SetActive(false);
    }

    public void SetInfo(IceAttribute attribute)
    {
        title.text = attribute.name;
        anecdote.text = attribute.anecdote;
        iceImage.sprite = Resources.Load<Sprite>("Attributes/Sprites/" + attribute.name);
        infoText.text = attribute.description;

        life.text = Mathf.RoundToInt(attribute.life).ToString();
        range.text = Mathf.RoundToInt(attribute.shootPower).ToString();
        velocity.text = Mathf.RoundToInt((attribute.speed - 1) * 100).ToString() + "%";
        jumpForce.text = Mathf.RoundToInt((attribute.jumpForce - 1) * 100).ToString() + "%";
        agility.text = Mathf.RoundToInt((attribute.agility - 1) * 100).ToString() + "%";
        stability.text = Mathf.RoundToInt((attribute.instability - 1) * -100).ToString() + "%";
        upForce.text = Mathf.RoundToInt((attribute.upForce - 1) * 100).ToString() + "%";
        fallSpeed.text = Mathf.RoundToInt((attribute.dropSpeed - 1) * 100).ToString() + "%";

        if (!infoField.gameObject.activeSelf) StartCoroutine(ShowInfoField());
    }

    IEnumerator ShowInfoField()
    {
        infoField.gameObject.SetActive(true);
        for(float count = 0; count < 1; count += Time.fixedDeltaTime)
        {
            infoField.alpha = count;
            placeHolder.alpha = 1 - count;
            yield return new WaitForFixedUpdate();
        }
        placeHolder.gameObject.SetActive(false);
        yield break;
    }
}
