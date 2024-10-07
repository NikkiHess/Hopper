using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTracker : MonoBehaviour
{
    public int score = 0;

    Subscription<PlayerJumpEvent> jumpSub;
    Subscription<PlatformTouchEvent> starterPlatTouchSub;

    float startHeight; // starting y pos
    bool countScore = false;

    private void Start()
    {
        jumpSub = EventBus.Subscribe<PlayerJumpEvent>(OnPlayerFirstJump);
        starterPlatTouchSub = EventBus.Subscribe<PlatformTouchEvent>(OnStarterPlatformTouch);
    }

    private void Update()
    {
        // only update the score if we've jumped, this prevents score being too high at the start
        if (countScore)
        {
            // get the height offset of the player and convert it to int
            int height = (int)(transform.position.y - startHeight);

            // only update the score if the height is higher
            if (height > score)
            {
                score = height;
                EventBus.Publish(new ScoreUpdateEvent(score));
            }
        }
    }

    private void OnStarterPlatformTouch(PlatformTouchEvent e)
    {
        if (e.isStarterPlatform)
        {
            startHeight = transform.position.y;
        }
    }

    // when the player first jumps, we show this text and start counting
    private void OnPlayerFirstJump(PlayerJumpEvent e)
    {
        if (e.firstJump)
        {
            countScore = true;
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(jumpSub);
        EventBus.Unsubscribe(starterPlatTouchSub);
    }
}

public class ScoreUpdateEvent
{
    public int score;

    public ScoreUpdateEvent(int score)
    {
        this.score = score;
    }
}