using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static CommentaryScript;

public class AwakeCommentary : MonoBehaviour
{
    public string comment;
    public int id;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PlayingComment());
    }

    public void PlayComment()
    {
        StartCoroutine(PlayingComment());
    }

    IEnumerator PlayingComment()
    {
        yield return new WaitForSeconds(.2f);
        if (commentary == null) yield break;

        commentary.PlayCommentary(comment, id);
        yield break;
    }
}
