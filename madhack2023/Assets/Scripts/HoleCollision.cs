using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BeauRoutine;

public class HoleCollision : MonoBehaviour
{
    float maxCompareDist = 5F;
    [SerializeField]
    float maxBallVelocity = 5F;
    [SerializeField]
    GameObject trap;
    [SerializeField]
    TextMeshPro tmpro;
    
    int pointValue = 0;

    bool caught = false;
    Routine delay;

    public void Initialize(int points)  // range of 3-5
    {
        pointValue = points;
        tmpro.text = (points * 100).ToString(); // numbers are only bigger to the player | bigger numbers = more fun

        // size is dependent on point value -> most points (1000) = size is 3 | least points (100) = size is 5
        float inversePointPercent = 1F - (((float)points - 1F) / 9F);
        float scaler = inversePointPercent * 2F + 3F;
        transform.localScale = Vector2.one * scaler;

        // update max radius for catching
        maxCompareDist = GetComponent<CircleCollider2D>().radius * scaler * 0.9F;
        Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y + maxCompareDist), Color.green, 20F);

        ResetTrap();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!delay.Exists() && !caught && collision.CompareTag("Ball"))
        {
            var dist = Vector2.Distance(transform.position, collision.transform.position);
            //Debug.Log(collision.attachedRigidbody.velocity.magnitude);
            if (dist < maxCompareDist && collision.attachedRigidbody.velocity.magnitude < maxBallVelocity)
            {
                // put up colliders & fade out ball & give points
                trap.SetActive(true);
                caught = true;
                Routine.Start(ConfirmCatch(collision.attachedRigidbody));
            }
        }
    }

    IEnumerator ConfirmCatch(Rigidbody2D ballRB)
    {
        yield return new WaitForEndOfFrame();

        var dist = Vector2.Distance(transform.position, ballRB.transform.position); 
        if (dist < maxCompareDist && !delay.Exists())
        {
            // catch confirmed
            GameManager.ScorePoints?.Invoke(pointValue);
            ballRB.GetComponent<Animator>()?.SetTrigger("Fade");
            yield return 0.68F;
            GameManager.ThrowBall?.Invoke(ballRB, true);
            ResetTrap();
        }
        else
        {
            // false alarm, let go
            ResetTrap();
        }
    }

    public void ResetTrap()
    {
        trap.SetActive(false);
        caught = false;
    }

    public void ForceDelay()
    {
        delay.Replace(Delay());
    }

    IEnumerator Delay()
    {
        yield return 1F;
    }
}
