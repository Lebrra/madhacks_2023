using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeauRoutine;
using System;

public class FlipperInput : MonoBehaviour
{
    public static Action<bool, Rigidbody2D, Vector2> FlipperHitBall;

    List<string> activeInputs;

    [SerializeField]
    float flipperSpeed = 0.2F;
    [SerializeField]
    float flipperForce = 1F;

    [Header("LEFT"), SerializeField]
    Transform leftAnchor;
    [SerializeField]
    float minLeftAngle = 0F;
    [SerializeField]
    float maxLeftAngle = 70F;
    Routine leftRoutine;

    [Header("RIGHT"), SerializeField]
    Transform rightAnchor;
    [SerializeField]
    float minRightAngle = 0F;
    [SerializeField]
    float maxRightAngle = -70F;
    Routine rightRoutine;

    private void OnEnable()
    {
        activeInputs = new List<string>();
        FlipperHitBall += ApplyFlipperForce;
    }
    private void OnDisable()
    {
        FlipperHitBall -= ApplyFlipperForce;
        if (leftRoutine.Exists()) leftRoutine.Stop();
        if (rightRoutine.Exists()) rightRoutine.Stop();
    }

    private void Update()
    {
        // todo: support many inputs
        if (Input.GetKeyDown(KeyCode.LeftArrow)) OnInputDown("left");
        if (Input.GetKeyUp(KeyCode.LeftArrow)) OnInputUp("left");
        if (Input.GetKeyDown(KeyCode.RightArrow)) OnInputDown("right");
        if (Input.GetKeyUp(KeyCode.RightArrow)) OnInputUp("right");
    }

    void OnInputDown(string input)
    {
        if (activeInputs.Contains(input))
        {
            Debug.Log($"Input {input} already registered. Ignoring OnInputDown");
        }
        else
        {
            activeInputs.Add(input);
            // do oninputenter
            switch (input)
            {
                case "left":
                    leftRoutine.Replace(LeftFlipperUp());
                    break;
                case "right":
                    rightRoutine.Replace(RightFlipperUp());
                    break;
            }
        }
    }

    void OnInput(string input)
    {
        // do oninput
    }

    void OnInputUp(string input)
    {
        if (activeInputs.Contains(input))
        {
            activeInputs.Remove(input);
            // do oninputup
            switch (input)
            {
                case "left":
                    leftRoutine.Replace(LeftFlipperDown());
                    break;
                case "right":
                    rightRoutine.Replace(RightFlipperDown());
                    break;
            }
        }
        else
        {
            activeInputs.Add(input);
            Debug.Log($"Input {input} not found. Ignoring OnInputUp");
        }
    }

    IEnumerator LeftFlipperUp()
    {
        float currAngle = leftAnchor.rotation.eulerAngles.z % 360F;
        float timeLeft = flipperSpeed - (Mathf.Abs(currAngle) / Mathf.Abs(maxLeftAngle - minLeftAngle) * flipperSpeed);
        yield return leftAnchor.RotateTo(maxLeftAngle, Mathf.Clamp(timeLeft, 0F, flipperSpeed), Axis.Z);
    }

    IEnumerator LeftFlipperDown()
    {
        float currAngle = leftAnchor.rotation.eulerAngles.z % 360F;
        float timeLeft = Mathf.Abs(currAngle) / Mathf.Abs(maxLeftAngle - minLeftAngle) * flipperSpeed;
        yield return leftAnchor.RotateTo(minLeftAngle, Mathf.Clamp(timeLeft, 0F, flipperSpeed), Axis.Z);
    }

    IEnumerator RightFlipperUp()
    {
        float currAngle = (rightAnchor.rotation.eulerAngles.z - 360F) % 360F;
        float timeLeft = flipperSpeed - (Mathf.Abs(currAngle) / Mathf.Abs(minRightAngle - maxRightAngle) * flipperSpeed);
        yield return rightAnchor.RotateTo(maxRightAngle, Mathf.Clamp(timeLeft, 0F, flipperSpeed), Axis.Z);
    }

    IEnumerator RightFlipperDown()
    {
        float currAngle = (rightAnchor.rotation.eulerAngles.z - 360F) % 360F;
        float timeLeft = Mathf.Abs(currAngle) / Mathf.Abs(minRightAngle - maxRightAngle) * flipperSpeed;
        yield return rightAnchor.RotateTo(minRightAngle, Mathf.Clamp(timeLeft, 0F, flipperSpeed), Axis.Z);
    }

    public void ApplyFlipperForce(bool isLeft, Rigidbody2D ball, Vector2 contactNorm)
    {
        if (ApproveBallForce(isLeft))
        {
            Debug.Log("Applying force on ball");
            Vector2 forceNorm = contactNorm * -1F;
            ball.AddForce(forceNorm * flipperForce, ForceMode2D.Impulse);
            Debug.DrawRay(ball.transform.position, contactNorm * 1F, Color.red, 8F);
            Debug.DrawRay(ball.transform.position, forceNorm * 1F, Color.blue, 8F);
        }
    }

    bool ApproveBallForce(bool isLeft)
    {
        return (isLeft && leftRoutine.Exists() && activeInputs.Contains("left")) ||
            (!isLeft && rightRoutine.Exists() && activeInputs.Contains("right"));
    }
}
