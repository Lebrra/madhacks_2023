using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

    public void Initialize(int points)  // range of 3-5
    {
        pointValue = points;
        tmpro.text = (points * 100).ToString(); // numbers are only bigger to the player | bigger numbers = more fun

        // size is dependent on point value -> most points (1000) = size is 3 | least points (100) = size is 5
        float inversePointPercent = 1F - (((float)points - 1F) / 9F);
        float scaler = inversePointPercent * 2F + 3F;
        transform.localScale = Vector2.one * scaler;

        // update max radius for catching
        maxCompareDist = GetComponent<CircleCollider2D>().radius / 4F;

        ResetTrap();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!caught && collision.CompareTag("Ball"))
        {
            var dist = Vector2.Distance(transform.position, collision.transform.position);
            Debug.Log(collision.attachedRigidbody.velocity.magnitude);
            if (dist < maxCompareDist && collision.attachedRigidbody.velocity.magnitude < maxBallVelocity)
            {
                // put up colliders & fade out ball & give points
                trap.SetActive(true);
                caught = true;
                GameLoader.ScorePoints?.Invoke(pointValue);
            }
        }
    }

    public void ResetTrap()
    {
        trap.SetActive(false);
        caught = false;
    }
}
