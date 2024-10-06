using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AppearOnGameOver : MonoBehaviour
{
    [SerializeField] TMP_Text finalScoreText;

    GameObject gameOverBox;
    Subscription<GameOverEvent> gameOverEvent;

    private void Start()
    {
        gameOverEvent = EventBus.Subscribe<GameOverEvent>(OnGameOver);
        gameOverBox = transform.GetChild(0).gameObject;
    }

    private void OnGameOver(GameOverEvent e)
    {
        gameOverBox.SetActive(true);

        TMP_Text text = gameOverBox.transform.Find("Final Score Text").GetComponent<TMP_Text>();
        text.text = "Final Score: " + e.finalScore;
    }
}
