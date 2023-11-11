using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeauRoutine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class FlipperInput : MonoBehaviour
{
    List<string> activeInputs;

    [SerializeField]
    float flipperSpeed = 0.2F;

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

    private void Start()
    {
        activeInputs = new List<string>();
    }

    private void Update()
    {
        // todo: support many inputs
        if (Input.GetKeyDown(KeyCode.LeftArrow)) leftRoutine.Replace(LeftFlipperUp());
        if (Input.GetKeyUp(KeyCode.LeftArrow)) leftRoutine.Replace(LeftFlipperDown());
        if (Input.GetKeyDown(KeyCode.RightArrow)) rightRoutine.Replace(RightFlipperUp());
        if (Input.GetKeyUp(KeyCode.RightArrow)) rightRoutine.Replace(RightFlipperDown());
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
}
