using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommentaryScript : MonoBehaviour
{
    public static int commentaryMask;


    public static CommentaryScript commentary;
    public static bool commentaryOff
    {
        get { return _commentaryOff; }
        set
        {
            _commentaryOff = value;
            if (!_commentaryOff) { commentaryMask = 0; if(commentary.GetComponent<AwakeCommentary>() != null) commentary.GetComponent<AwakeCommentary>().PlayComment(); }
            else if (commentary != null) commentary.aSrc.Stop();
        }
    }
    static bool _commentaryOff; 

    private static float volume;


    private AudioSource aSrc;
    private Animator anim;

    // Start is called before the first frame update
    void Awake()
    {
        //if (commentary != null) Destroy(commentary.gameObject);
        commentary = this;

        //commentaryMask = 0;
        aSrc = GetComponent<AudioSource>();
        aSrc.volume = 1-volume;

        anim = GetComponent<Animator>();
        GetComponent<CanvasGroup>().alpha = 0;
    }

    public static void SetCommentaryVolume(float commentVolume)
    {
        volume = 1-commentVolume;
        if (commentary != null) commentary.aSrc.volume = 1-volume;
    }

    public void PlayCommentary(string commentName, int id)
    {
        if (!commentaryOff && (commentaryMask & (1 << id)) == 0)
        {
            commentaryMask |= 1 << id;
            StartCoroutine(PlayingCommentary(commentName));
        }
    }

    IEnumerator PlayingCommentary(string commentName)
    {
        anim.Play("open");

        AudioClip comment = Resources.Load<AudioClip>("Comments/" + commentName);
        if (aSrc.isPlaying) aSrc.Stop();
        yield return new WaitForFixedUpdate();
        aSrc.clip = comment;
        aSrc.Play();
        yield return new WaitWhile(() => aSrc.isPlaying);
        anim.SetTrigger("close");
        yield break;
    }
}
