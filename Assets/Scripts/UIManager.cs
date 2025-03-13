using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TextMeshProUGUI scoreText;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void UpdateScoreUI(float score)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }
}
