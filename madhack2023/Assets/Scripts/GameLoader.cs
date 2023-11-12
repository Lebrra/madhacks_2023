using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using BeauRoutine;

public class GameLoader : MonoBehaviour
{
    // static messages
    public static Action<Rigidbody2D> ProgressTrack;
    public static Action EndGame;
    public static Action<int> ScorePoints;  // in single digits, multiply by 100 for visuals

    [SerializeField]
    TrackLoader[] tracks;   // always 5 - don't create them we can reuse them
    int currentTrack = -1;

    [SerializeField]
    Transform cameraTransform;
    Routine cameraPan;

    [SerializeField]
    float nextTrackPanTime = 2F;

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

    public void PanCamera(int trackIndex, float speed, Action postAction = null)
    {
        if (trackIndex >= tracks.Length || trackIndex < 0) Debug.LogError("Invalid track index");
        else cameraPan.Replace(CameraPan(trackIndex, speed, postAction));
    }

    IEnumerator CameraPan(int trackIndex, float speed, Action postAction)
    {
        yield return cameraTransform.MoveTo(tracks[trackIndex].transform.position, speed);
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
            PanCamera(currentTrack, nextTrackPanTime, () => ballRB.bodyType = RigidbodyType2D.Dynamic); // TODO: add a little force left or right on the ball so we don't plunge again
            tracks[currentTrack - 1].DisableTrack();
            tracks[currentTrack].EnableTrack();
        }
    }
}
