using UnityEngine;

public class DisappearingGameObject : MonoBehaviour
{
    private Subscription<PlayerJumpEvent> jumpSub;

    private void Start()
    {
        jumpSub = EventBus.Subscribe<PlayerJumpEvent>(OnPlayerFirstJump);
    }

    // this text should disappear on first jump
    private void OnPlayerFirstJump(PlayerJumpEvent e)
    {
        if (e.firstJump)
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(jumpSub);
    }
}