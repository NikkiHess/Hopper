using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool inverted = false;
    public bool hasJumpedForFirstTime = false; // tells us whether the player has jumped for the first time

    [SerializeField]
    private float jumpPower = 1;

    [SerializeField]
    private float speed = 1;

    private bool isOnPlatform = false; // are we able to jump?
    private bool controlsEnabled = false; // have we finished the opening scene?

    private Subscription<PlatformTouchEvent> starterPlatTouchSub;
    private Subscription<GameOverEvent> gameOverSub;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        starterPlatTouchSub = EventBus.Subscribe<PlatformTouchEvent>(OnStarterPlatformTouch);
        gameOverSub = EventBus.Subscribe<GameOverEvent>(OnGameOver);
    }

    private void Update()
    {
        if (controlsEnabled)
        {
            if (Input.GetKey(KeyCode.Space) && isOnPlatform)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpPower);

                // if we jump for the first time...
                // publish a first jump event
                // and spawn our platforms
                if(!hasJumpedForFirstTime)
                {
                    EventBus.Publish(new PlayerJumpEvent(gameObject, true));
                    hasJumpedForFirstTime = true;
                }
                else
                {
                    EventBus.Publish(new PlayerJumpEvent(gameObject, false));
                }
            }

            float horizInput = Input.GetAxis("Horizontal");
            if (Mathf.Abs(horizInput) > 0)
            {
                rb.velocity = new Vector3(horizInput * speed, rb.velocity.y);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject go = collision.gameObject;
        if (go.CompareTag("Platform") || go.CompareTag("Starter Platform"))
        {
            if (!isOnPlatform)
            {
                isOnPlatform = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        GameObject go = collision.gameObject;
        if ((go.CompareTag("Platform") || go.CompareTag("Starter Platform")))
        {
            if (isOnPlatform)
            {
                isOnPlatform = false;
            }
        }
    }

    private void OnStarterPlatformTouch(PlatformTouchEvent e)
    {
        if (e.isStarterPlatform)
        {
            controlsEnabled = true;
        }
    }

    // when the game ends, disable controls (just in case)
    private void OnGameOver(GameOverEvent e)
    {
        controlsEnabled = false;
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(starterPlatTouchSub);
    }
}

public class PlayerJumpEvent
{
    public GameObject player;
    public bool firstJump = false;

    public PlayerJumpEvent(GameObject player, bool firstJump)
    {
        this.player = player;
        this.firstJump = firstJump;
    }

    public override string ToString()
    {
        return player.name;
    }
}