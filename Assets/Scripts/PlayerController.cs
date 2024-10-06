using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float jumpPower = 1;

    [SerializeField]
    private float speed = 1;

    private bool isOnPlatform = false; // are we able to jump?
    private bool controlsEnabled = false; // have we finished the opening scene?

    // tells us whether the player has jumped for the first time, useful for spawning platforms
    private bool hasJumped = false;

    private Subscription<PlatformTouchEvent> starterPlatTouchSub;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        starterPlatTouchSub = EventBus.Subscribe<PlatformTouchEvent>(OnStarterPlatformTouch);
    }

    private void OnStarterPlatformTouch(PlatformTouchEvent e)
    {
        if(e.platform.CompareTag("Starter Platform"))
        {
            controlsEnabled = true;
        }
    }

    private void Update()
    {
        if (controlsEnabled)
        {
            if (Input.GetKeyDown(KeyCode.Space) && isOnPlatform)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpPower);

                // if we haven't jumped for the first time, spawn our platforms
                if(!hasJumped)
                {

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

            // starter platform separation (first jump)
            if(go.CompareTag("Starter Platform"))
            {
                EventBus.Publish(new PlayerFirstJumpEvent(gameObject));
            }
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(starterPlatTouchSub);
    }
}

public class PlayerFirstJumpEvent
{
    GameObject player;

    public PlayerFirstJumpEvent(GameObject player)
    {
        this.player = player;
    }

    public override string ToString()
    {
        return player.name;
    }
}