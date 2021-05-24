using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommentaryScript : MonoBehaviour
{
    public static long commentaryMask;

    public static CommentaryScript commentary;
    private static float volume;
    private AudioSource aSrc;

    // Start is called before the first frame update
    void Start()
    {
        if (commentary != null) Destroy(gameObject);
        commentary = this;
        DontDestroyOnLoad(gameObject);

        commentaryMask = 0;
        aSrc = GetComponent<AudioSource>();
        aSrc.volume = volume;
    }

    public static void SetCommentaryVolume(float commentVolume)
    {
        volume = commentVolume;
        if (commentary != null) commentary.aSrc.volume = volume;
    }

    public void PlayCommentary(string commentName) => StartCoroutine(PlayingCommentary(commentName));

    IEnumerator PlayingCommentary(string commentName)
    {
        AudioClip comment = Resources.Load<AudioClip>("Comments/" + commentName);
        if (aSrc.isPlaying) aSrc.Stop();
        yield return new WaitForFixedUpdate();
        aSrc.clip = comment;
        aSrc.Play();
        yield break;
    }
}
