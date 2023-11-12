using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackLoader : MonoBehaviour
{
    [SerializeField]
    FlipperInput myFlippers;
    [SerializeField]
    GameObject ceilingCol;
    [SerializeField]
    GameObject floorCol;


    [SerializeField]
    HoleCollision holePref;
    [SerializeField]
    List<Transform> holeSpawnpoints;
    [SerializeField]
    Transform ballSpawnpoint;
    public Transform BallSpawnpoint { get => ballSpawnpoint; }

    List<HoleCollision> holes = null;

    public void Initialize(int totalPoints)
    {
        // generate holes, points
        ResetTrack();

        // skewing hole count to favor 3 holes
        int holeCount = Random.Range(2, 10);
        if (holeCount > 5) holeCount = 3;

        // generate points
        Debug.Log($"{totalPoints} chosen for this track", gameObject);
        int rawPointValue = Mathf.RoundToInt((float)totalPoints);
        int[] points = new int[holeCount];
        for (int i = 0; i < holeCount; i++) points[i] = -1;

        while (points[holeCount - 1] < 0)
        {
            for (int i = 0; i < holeCount - 1; i++)
            {
                points[i] = Random.Range(1, Mathf.Min(rawPointValue, 11));
                rawPointValue = Mathf.Clamp(rawPointValue - points[i], 1, rawPointValue);
            }
            if (rawPointValue <= 10) points[holeCount - 1] = rawPointValue;
            else Debug.LogWarning($"Bad point distribution for {totalPoints} points, trying again", gameObject);
        }

        // generate holes - favor pools
        if (holes == null) holes = new List<HoleCollision>();

        // generate temp list to pull spawns from randomly
        var holeSpawns = new List<Transform>();
        foreach (var hole in holeSpawnpoints) holeSpawns.Add(hole);

        for (int i = 0; i < holeCount; i++)
        {
            // can we reuse a hole?
            if (holes.Count == i)
            {
                var hole = Instantiate(holePref, transform);
                holes.Add(hole);
            }
            else
            {
                holes[i].gameObject.SetActive(true);
            }

            // choose location
            int spawn = Random.Range(0, holeSpawns.Count);
            holes[i].transform.position = holeSpawns[spawn].position;
            holeSpawns.RemoveAt(spawn);

            // give points
            holes[i].Initialize(points[i]);
        }
        DisableTrack();
    }

    public void EnableTrack()
    {
        GameManager.ThrowBall += DelayHoles;
        myFlippers.enabled = true;
        ceilingCol.SetActive(true);
        floorCol.SetActive(true);
        DelayHoles(null, false);
    }

    public void DisableTrack()
    {
        GameManager.ThrowBall -= DelayHoles;
        myFlippers.enabled = false;
        ceilingCol.SetActive(false);
        floorCol.SetActive(false);
    }

    public void ResetTrack()
    {
        if (holes != null)
        {
            foreach (var hole in holes)
            {
                hole.gameObject.SetActive(false);
            }
        }
    }

    void DelayHoles(Rigidbody2D rb, bool b)
    {
        foreach (var hole in holes)
        {
            if (hole.gameObject.activeInHierarchy)
            {
                hole.ForceDelay();
            }
        }
    }
}
