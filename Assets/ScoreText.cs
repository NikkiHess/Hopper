using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreText : MonoBehaviour
{
    [SerializeField] string prefix = "Score: ";

    Subscription<ScoreUpdateEvent> scoreUpdateSub;

    TMP_Text text;

    private void Start()
    {
        // handle text
        text = GetComponent<TMP_Text>();
        text.enabled = false; // disable this text to begin with

        scoreUpdateSub = EventBus.Subscribe<ScoreUpdateEvent>(OnScoreUpdate);
    }

    private void OnScoreUpdate(ScoreUpdateEvent e)
    {
        // should say "Score: 0" initially
        text.text = prefix + e.score;

        // enable text if needed
        text.enabled = true;
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(scoreUpdateSub);
    }
}