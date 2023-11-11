using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipperCollision : MonoBehaviour
{
    [SerializeField]
    bool isLeft;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball") && collision.contactCount > 0)
        {
            FlipperInput.FlipperHitBall?.Invoke(isLeft, collision.rigidbody, collision.contacts[0].normal);
        }
    }
}
