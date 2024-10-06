using UnityEngine;

public class GameEnder : MonoBehaviour
{
    [SerializeField] bool hasDoneGameOver = false;
    public void EndGame()
    {
        if (!hasDoneGameOver)
        {
            // trigger game over
            ScoreTracker scoreTracker = GetComponent<ScoreTracker>();
            EventBus.Publish(new GameOverEvent(scoreTracker.score));
        }
    }
}

public class GameOverEvent
{
    public int score;

    public GameOverEvent(int score)
    {
        this.score = score;
    }

    public override string ToString()
    {
        return "Score: " + score;
    }
}
