using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CommentaryScript;

public class OptionScript : MonoBehaviour
{
    Animator anim;

    public void ChangeOptions(bool opening)
    {
        anim = GetComponent<Animator>();

        if (opening)
        {
            gameObject.SetActive(true);
            anim.enabled = true;
            anim.Play("OpenOptions");
        }
        else
        {
            anim.enabled = true;
            anim.Play("CloseOptions");
        }
        StartCoroutine(CloseAnim(opening));
    }

    IEnumerator CloseAnim(bool opening)
    {
        yield return new WaitForSeconds(.5f);
        anim.enabled = false;
        if (!opening) gameObject.SetActive(false);
    }

    public void ChangeAudioComments(bool isOn) => commentaryOff = !isOn;
    public void ChangeAudioComments(float volume) { CommentaryScript.SetCommentaryVolume(volume); }
}
