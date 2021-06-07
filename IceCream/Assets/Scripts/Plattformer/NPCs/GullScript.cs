using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GullScript : BirdScript
{
    protected override void Start()
    {
        base.Start();
        transform.GetChild(1).GetComponent<TriggerScript>().On_T_Enter += TriggerEntered;
    }

    private void OnDestroy()
    {
        transform.GetChild(1).GetComponent<TriggerScript>().On_T_Enter -= TriggerEntered;
    }

    public void TriggerEntered(Collider2D other)
    {
        if (other.gameObject.layer != 8 || pauseMovement || invincible) return;
        if (other.GetComponent<IceScript>().id < 0) return;

        StartCoroutine(Attack(other.gameObject));
    }

    IEnumerator Attack(GameObject other)
    {
        //Notice-Animation:
        anim.Play("Notice");
        pauseMovement = true;

        Vector2 startPos = transform.position;
        Vector2 currentPos = startPos;
        Vector2 diff = (Vector2)other.transform.position - currentPos;
        yield return new WaitForSeconds(1);

        if (invincible) { pauseMovement = false; yield break; }

        //greife an:
        invincible = true;

        float timeStep = Time.fixedDeltaTime;
        float sqrCount;
        Vector3 startRot = Vector3.forward * 30 * Mathf.Sign(diff.y) * Mathf.Sign(transform.localScale.x);
        transform.eulerAngles = startRot;

        for(float count = 0; count < 1; count += timeStep)
        {
            sqrCount = count * count;
            transform.position = currentPos + new Vector2(sqrCount, 1 - (1-count) * (1-count)) * diff;
            transform.eulerAngles = startRot * (1 - sqrCount);
            yield return new WaitForFixedUpdate();
        }
        currentPos = transform.position;
        diff *= new Vector2(.5f, -1);
        startRot *= -1;
        for (float count = 0; count < 1; count += timeStep)
        {
            sqrCount = count * count;
            transform.position = currentPos + new Vector2(1 - (1 - count) * (1 - count), sqrCount) * diff;
            transform.eulerAngles = startRot * sqrCount;
            yield return new WaitForFixedUpdate();
        }

        transform.rotation = Quaternion.identity;
        pauseMovement = false;
        invincible = false;

        timeStep *= 2;
        for(float count = 1; count > 0; count -= timeStep)
        {
            transform.eulerAngles = startRot * count;
            yield return new WaitForFixedUpdate();
        }
        transform.rotation = Quaternion.identity;
        anim.Play("Fly");
        yield break;
    }
}
