using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BeauRoutine;
using UnityEngine.Serialization;
using static TreeEditor.TreeEditorHelper;

public class GameManager : MonoBehaviour
{
    // static messages
    public static Action<Rigidbody2D> ProgressTrack;
    public static Action EndGame;
    public static Action<int> ScorePoints;  // in single digits, multiply by 100 for visuals

    [SerializeField]
    TrackLoader[] tracks;   // always 5 - don't create them we can reuse them
    int currentTrack = -1;

    [Header("Camera"), SerializeField]
    Transform cameraTransform;
    Routine cameraPan;
    [SerializeField, FormerlySerializedAs("animCurve")]
    AnimationCurve panCurve;
    [SerializeField]
    float nextTrackPanTime = 2F;

    [Header("Ball"), SerializeField]
    Transform ballTransform;
    [SerializeField]
    Transform ballStart;

    private void OnEnable()
    {
        ProgressTrack += NextTrack;
        CreateGame();
    }
    private void OnDisable()
    {
        ProgressTrack -= NextTrack;
    }

    public void CreateGame()
    {
        // determine point variation (always 5 tracks)
        // between 1 and 2:
        int rand = UnityEngine.Random.Range(0, 6);
        tracks[0].Initialize(10 + rand);
        tracks[1].Initialize(10 - rand);

        // between 3, 4, 5:
        rand = UnityEngine.Random.Range(5, 13);
        tracks[4].Initialize(10 + rand);
        var lastRand = UnityEngine.Random.Range(4, 30 - rand - 3);
        tracks[3].Initialize(lastRand);
        tracks[2].Initialize(Mathf.Clamp(30 - (rand + 10) - lastRand, 4, 22));
    }

    // Called from Timeline
    public void StartPan(float panSpeed)
    {
        PanCamera(0, panSpeed);
    }

    // Called from Timeline
    public void StartGame()
    {
        currentTrack = 0;
        tracks[currentTrack].EnableTrack();
        
        ballTransform.position = ballStart.position;
        ballTransform.gameObject.SetActive(true);
        ThrowBallIn(ballTransform.GetComponent<Rigidbody2D>());
    }

    public void PanCamera(int trackIndex, float speed, Action postAction = null)
    {
        if (trackIndex >= tracks.Length || trackIndex < 0) Debug.LogError("Invalid track index");
        else cameraPan.Replace(CameraPan(trackIndex, speed, postAction));
    }

    IEnumerator CameraPan(int trackIndex, float speed, Action postAction)
    {
        Vector3 cameraPos = new Vector3(tracks[trackIndex].transform.position.x, tracks[trackIndex].transform.position.y, -10F);
        yield return cameraTransform.MoveTo(cameraPos, speed).Ease(panCurve);
        postAction?.Invoke();
    }

    public void NextTrack(Rigidbody2D ballRB)
    {
        ballRB.bodyType = RigidbodyType2D.Static;
        // are we done?
        currentTrack++;
        if (currentTrack >= 5)
        {
            Debug.Log("GAME END");
        }
        else
        {
            Debug.Log("NEXT TRACK");
            PanCamera(currentTrack, nextTrackPanTime, () => ThrowBallIn(ballRB));
            tracks[currentTrack - 1].DisableTrack();
            tracks[currentTrack].EnableTrack();
        }
    }

    void ThrowBallIn(Rigidbody2D ball)
    {
        ball.bodyType = RigidbodyType2D.Dynamic;
        ball.AddForce(Vector2.right * UnityEngine.Random.Range(-2F, 2F), ForceMode2D.Impulse);
    }
}
