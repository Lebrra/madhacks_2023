using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // if ball, set ball static, pan camera down, disable last track, enable new track, set ball dynamic
        if (collision.CompareTag("Ball"))
        {
            GameLoader.ProgressTrack?.Invoke(collision.attachedRigidbody);
        }
    }
}
