using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleCollision : MonoBehaviour
{
    float maxCompareDist = 5F;
    [SerializeField]
    float maxBallVelocity = 5F;
    [SerializeField]
    GameObject trap;

    bool caught = false;

    private void Start()
    {
        maxCompareDist = GetComponent<CircleCollider2D>().radius / 4F;
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
            }
        }
    }

    public void ResetTrap()
    {
        trap.SetActive(false);
        caught = false;
    }
}
