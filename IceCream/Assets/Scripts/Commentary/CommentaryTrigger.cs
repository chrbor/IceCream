using UnityEngine;
using static CommentaryScript;
using static GameManager;

public class CommentaryTrigger : MonoBehaviour
{
    public string commentName;
    public int id = -1;

    private void Start()
    {
        if (id < 0) return;
        if ((commentaryMask & ((long)1 << id)) != 0) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (commentary == null) return;
        commentary.PlayCommentary(commentName, id);
        Destroy(gameObject);
    }
}
