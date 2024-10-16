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
    private bool hasInverted = false;

    private Subscription<PlatformTouchEvent> starterPlatTouchSub;
    private Subscription<GameOverEvent> gameOverSub;

    private Rigidbody rb;
    private Renderer _renderer;
    private GameObject starterPlat;

    float leftCameraEdgeX, rightCameraEdgeX;

    [SerializeField] private Material wavyOutline;
    [SerializeField] private Material wavyOutlineInverted;
    [SerializeField] private Material normalOutline;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        starterPlatTouchSub = EventBus.Subscribe<PlatformTouchEvent>(OnStarterPlatformTouch);
        gameOverSub = EventBus.Subscribe<GameOverEvent>(OnGameOver);

        _renderer = GetComponent<Renderer>();

        // since the camera only moves up, we can get the camera edges right off the start
        CalculateEdgePositions();
    }

    private void Update()
    {
        if (controlsEnabled)
        {
            // handle first time jumping
            if (Input.GetKey(KeyCode.Space) && isOnPlatform)
            {
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

            if(isOnPlatform && hasJumpedForFirstTime)
            {
                rb.velocity = new Vector3(rb.velocity.x, jumpPower);
            }

            // handle horizontal movement
            float horizInput = Input.GetAxis("Horizontal");
            if (Mathf.Abs(horizInput) > 0)
            {
                rb.velocity = new Vector3(horizInput * speed, rb.velocity.y);
            }

            // handle moving off screen
            Bounds rendererBounds = _renderer.bounds;

            Vector3 renderLeft = new Vector3(rendererBounds.min.x, rendererBounds.center.y, rendererBounds.center.z);
            Vector3 renderRight = new Vector3(rendererBounds.max.x, rendererBounds.center.y, rendererBounds.center.z);

            Vector3 viewportLeft = Camera.main.WorldToViewportPoint(renderLeft);
            Vector3 viewportRight = Camera.main.WorldToViewportPoint(renderRight);

            bool doInvert = false;

            // off the left side of the screen?
            if (viewportRight.x < 0)
            {
                transform.position = new Vector3(rightCameraEdgeX, transform.position.y, transform.position.z);
                doInvert = true;
            }
            // off the right side of the screen?
            else if (viewportLeft.x > 1)
            {
                transform.position = new Vector3(leftCameraEdgeX, transform.position.y, transform.position.z);
                doInvert = true;
            }

            float threshold = 0.15f;
            Debug.Log(viewportLeft.x + " " + viewportRight.x);
            if (viewportLeft.x < threshold || viewportRight.x > 1 - threshold)
            {
                List<Material> materials = new();
                materials.Add(_renderer.materials[0]);
                materials.Add(inverted ? wavyOutlineInverted : wavyOutline);
                _renderer.SetMaterials(materials);
            }
            else
            {
                List<Material> materials = new();
                materials.Add(_renderer.materials[0]);
                materials.Add(normalOutline);
                _renderer.SetMaterials(materials);
            }

            if(doInvert)
            {
                inverted = !inverted; // invert, change dimensions
                starterPlat.GetComponent<StarterPlatform>().Invert(inverted);

                // we should publish a player invert event here
                EventBus.Publish(new PlayerInvertEvent(gameObject, !hasInverted));
                if(!hasInverted) hasInverted = true; // make sure this only happens the first time
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
            starterPlat = e.platform;
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
        EventBus.Unsubscribe(gameOverSub);
    }

    private void CalculateEdgePositions()
    {
        // Use the player's initial position to determine depth
        Vector3 playerPosition = transform.position;
        float cameraZ = Camera.main.WorldToViewportPoint(playerPosition).z;

        // Left edge in world space
        Vector3 leftEdgeWorldPosition = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, cameraZ));
        leftCameraEdgeX = leftEdgeWorldPosition.x;

        // Right edge in world space
        Vector3 rightEdgeWorldPosition = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, cameraZ));
        rightCameraEdgeX = rightEdgeWorldPosition.x;
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
        return "Jumped: " + player.name + ", First Jump: " + firstJump;
    }
}

public class PlayerInvertEvent
{
    public GameObject player;
    public bool firstInvert = false;

    public PlayerInvertEvent(GameObject player, bool firstInvert)
    {
        this.player = player;
        this.firstInvert = firstInvert;
    }

    public override string ToString()
    {
        return "Inverted: " + player.name + ", First Invert: " + firstInvert;
    }
}