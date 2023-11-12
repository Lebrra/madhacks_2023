using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighScoreLoader : MonoBehaviour
{
    [SerializeField]
    RectTransform scoresObj;
    [SerializeField]
    TextMeshProUGUI[] highscores;
    [SerializeField]
    GameObject[] breaks;

    string[] scorePrefix = { "1st - ", "2nd - ", "3rd - ", "4th - ", "5th - " };

    private void OnEnable()
    {
        GameManager.UpdateScores += LoadScores;
    }

    private void OnDisable()
    {
        GameManager.UpdateScores -= LoadScores;
    }

    public void LoadScores(int[] scores)
    {
        if (scores[0] < 0) scoresObj.gameObject.SetActive(false);
        else
        {
            scoresObj.gameObject.SetActive(true);

            for (int i = 0; i < 5; i++)
            {
                if (scores[i] > 0)
                {
                    highscores[i].text = scorePrefix[i] + (scores[i] * 100).ToString();
                    highscores[i].gameObject.SetActive(true);
                    if (i > 0) breaks[i - 1].SetActive(true);
                }
                else highscores[i].gameObject.SetActive(false);
            }
        }
    }

    // Called on StartButton OnClick
    public void HideScores()
    {
        scoresObj.gameObject.SetActive(false);
    }
}
