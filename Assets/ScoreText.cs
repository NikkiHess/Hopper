using UnityEngine;
using TMPro;

public class ScoreText : MonoBehaviour
{
    [SerializeField] int score = 0;
    [SerializeField] string prefix = "Score: ";
    [SerializeField] GameObject player; // GameObject whose height we track for the score

    Subscription<PlayerJumpEvent> jumpSub;
    Subscription<PlatformTouchEvent> starterPlatTouchSub;

    TMP_Text text;
    Vector3 playerStart;

    bool countScore = false;

    private void Start()
    {
        jumpSub = EventBus.Subscribe<PlayerJumpEvent>(OnPlayerFirstJump);
        starterPlatTouchSub = EventBus.Subscribe<PlatformTouchEvent>(OnStarterPlatformTouch);

        // handle text
        text = GetComponent<TMP_Text>();
        text.enabled = false; // disable this text to begin with
    }

    private void Update()
    {
        // only update the score if we've jumped, this prevents score being too high at the start
        if (countScore)
        {
            // get the height of the player and convert it to int
            int height = (int)(player.transform.position.y - playerStart.y);

            // only update the score if the height is higher
            if (height > score)
            {
                score = height;
            }

            text.text = prefix + score;
        }
    }

    private void OnStarterPlatformTouch(PlatformTouchEvent e)
    {
        if(e.isStarterPlatform)
        {
            playerStart = player.transform.position;
        }
    }

    // when the player first jumps, we show this text and start counting
    private void OnPlayerFirstJump(PlayerJumpEvent e)
    {
        if (e.firstJump)
        {
            // should say "Score: 0"
            text.text = prefix + score;
            text.enabled = true;

            countScore = true;
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(jumpSub);
        EventBus.Unsubscribe(starterPlatTouchSub);
    }
}