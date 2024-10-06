using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffscreenDetector : MonoBehaviour
{
    [SerializeField] bool hasDoneGameOver = false;

    private void Update()
    {
        if (!hasDoneGameOver)
        {
            Vector3 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);

            // if we're off the bottom of the screen, game over
            if (viewportPosition.y < 0)
            {
                hasDoneGameOver = true;

                // trigger game over
                ScoreTracker scoreTracker = GetComponent<ScoreTracker>();
                EventBus.Publish(new GameOverEvent(scoreTracker.score));
            }
        }
    }
}

public class GameOverEvent
{
    public int finalScore;

    public GameOverEvent(int finalScore)
    {
        this.finalScore = finalScore;
    }

    public override string ToString()
    {
        return "Game over! Final score: " + finalScore;
    }
}