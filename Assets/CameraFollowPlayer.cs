using UnityEngine;
public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] bool followPlayer = false;

    private Subscription<PlayerJumpEvent> jumpSub;
    GameObject player = null;

    private void Start()
    {
        jumpSub = EventBus.Subscribe<PlayerJumpEvent>(OnPlayerFirstJump);
    }

    private void Update()
    {
        if (followPlayer)
        {
            transform.position = new Vector3(
                transform.position.x,
                player.transform.position.y + 5.35f,
                transform.position.z
            );
        }
    }

    // this text should disappear on first jump
    private void OnPlayerFirstJump(PlayerJumpEvent e)
    {
        if (e.firstJump)
        {
            followPlayer = true;
            player = e.player;
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(jumpSub);
    }
}