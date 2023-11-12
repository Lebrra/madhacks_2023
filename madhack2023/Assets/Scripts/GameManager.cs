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
    public static Action<int> ScorePoints;  // in single digits, multiply by 100 for visuals
    public static Action<Rigidbody2D, bool> ThrowBall;
    public static Action<int[]> UpdateScores;

    [SerializeField]
    TrackLoader[] tracks;   // always 5 - don't create them we can reuse them
    int currentTrack = -1;

    [SerializeField]
    TMPro.TextMeshProUGUI scoreText;
    int score;
    Routine scoreAnimated;

    [Header("Camera"), SerializeField]
    Transform cameraTransform;
    Routine cameraPan;
    [SerializeField, FormerlySerializedAs("animCurve")]
    AnimationCurve panCurve;
    [SerializeField]
    float nextTrackPanTime = 2F;

    [Header("Ball"), SerializeField]
    Transform ballTransform;

    [Header("End"), SerializeField]
    Animator endAnim;
    [SerializeField]
    TMPro.TextMeshProUGUI endScore;
    [SerializeField]
    GameObject highScore;
    int[] highScores;
    const string SCOREKEY = "highscore";

    private void OnEnable()
    {
        ProgressTrack += NextTrack;
        ThrowBall += ThrowBallIn;
        ScorePoints += CollectPoints;
        LoadScores();
        CreateGame();
    }
    private void OnDisable()
    {
        ProgressTrack -= NextTrack;
        ThrowBall -= ThrowBallIn;
        ScorePoints -= CollectPoints;
    }

    public void CreateGame()
    {
        ballTransform.gameObject.SetActive(false);
        endAnim.SetTrigger("Reset");

        score = 0;
        CollectPoints(0);
        UpdateScores?.Invoke(highScores);

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

        ballTransform.gameObject.SetActive(true);
        ThrowBallIn(ballTransform.GetComponent<Rigidbody2D>(), true);
        scoreText.GetComponent<Animator>().SetTrigger("In");
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
            EndGame();
        }
        else
        {
            Debug.Log("NEXT TRACK");
            PanCamera(currentTrack, nextTrackPanTime, () => ThrowBallIn(ballRB));
            tracks[currentTrack - 1].DisableTrack();
            tracks[currentTrack].EnableTrack();
        }
    }

    void EndGame()
    {
        Debug.Log("GAME END");

        endScore.text = (score * 100).ToString();
        if (CheckNewHighScore(score))
        {
            highScore.SetActive(true);
            SaveScores(score);
        }
        else highScore.SetActive(false);

        scoreText.GetComponent<Animator>().SetTrigger("Out");
        endAnim.SetTrigger("End");
    }

    void ThrowBallIn(Rigidbody2D ball, bool placeBall = false)
    {
        if (placeBall) ball.transform.position = tracks[currentTrack].BallSpawnpoint.position;
        ball.bodyType = RigidbodyType2D.Dynamic;
        ball.AddForce(Vector2.right * UnityEngine.Random.Range(-2F, 2F), ForceMode2D.Impulse);
    }

    void CollectPoints(int value)
    {
        Debug.LogWarning($"Collected {value} points!");
        //scoreText.text = (score * 100).ToString();
        scoreAnimated.Replace(AnimateScore(score, score + value));
        score += value;
    }

    IEnumerator AnimateScore(int from, int to)
    {
        var time = 0.5F;
        while (time > 0F)
        {
            scoreText.text = Mathf.RoundToInt(Mathf.Lerp(to, from, 0.5F - time) * 100F).ToString();
            time -= Time.deltaTime;
            yield return null;
        }
        scoreText.text = (score * 100).ToString();
    }

    void LoadScores()
    {
        highScores = new int[5] { -1, -1, -1, -1, -1};
        for (int i = 1; i <= 5; i++)
        {
            string key = SCOREKEY + i.ToString();
            if (PlayerPrefs.HasKey(key))
            {
                highScores[i - 1] = PlayerPrefs.GetInt(key);
            }
            else break; // scores are ordered, filling from 1st to 5th
        }
    }

    void SaveScores(int newScore)
    {
        // already confirmed this better 
        int placement = -1;
        for (int i = 4; i >= 0; i--)
        {
            if (newScore > highScores[i]) placement = i;
            else break;
        }
        if (placement < 0)
        {
            Debug.LogError($"Score is not new: {newScore} | Worst high score: {highScores[4]}");
            return;
        }
        for (int i = 4; i > placement; i--)
        {
            highScores[i] = highScores[i - 1];
        }
        highScores[placement] = newScore;

        //save
        for (int i = 1; i <= 5; i++)
        {
            if (highScores[i - 1] > 0)
            {
                string key = SCOREKEY + i.ToString();
                PlayerPrefs.SetInt(key, highScores[i - 1]);
                Debug.Log($"[Saved] {key} => {highScores[i - 1]}");
            }
            else break;
        }
    }

    bool CheckNewHighScore(int score)
    {
        return score > highScores[4] && score > 0;
    }
}
