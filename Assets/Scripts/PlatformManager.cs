using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [SerializeField] GameObject platformPrefab;
    [SerializeField] float maxXMagnitude; // how far to the side can we go?
    [SerializeField] float platformSeparation; // how far apart should platforms be?
    [SerializeField] int offscreenNumPlatforms = 4; // # of platforms before we start moving things up
    [SerializeField] int numPlatformsToGenerate = 6; // # of platforms to generate
    [SerializeField] Vector2Int invertedRange; // min and max inverted section sizes

    [SerializeField] Material invertedBase, invertedTranslucentBase;
    [SerializeField] Material invertedOutline, invertedTranslucentOutline;
    [SerializeField] Material _base, translucentBase;
    [SerializeField] Material outline, translucentOutline;

    int currentGeneration = 0; // number of times we generated or moved a platform
    bool inverted = false; // invert
    bool hasInverted = false; // have we reached the inverted section for the first time?
    int invertedSectionTop = 0; // the top of the inverted section, if there is one

    // the list of platforms we're populating
    List<GameObject> platforms = new(); // newer C# syntax?

    // subscription to player's first jump
    private Subscription<PlayerJumpEvent> jumpSub;

    private void Start()
    {
        // make sure that our inverted range is an actual RANGE
        if (invertedRange.x >= invertedRange.y)
        {
            Debug.LogError("Invalid invertedRange values: Min should be less than Max.");
        }

        jumpSub = EventBus.Subscribe<PlayerJumpEvent>(OnPlayerJump);
    }

    void OnPlayerJump(PlayerJumpEvent e)
    {
        // on first jump, spawn platforms
        if (e.firstJump)
        {
            StartCoroutine(SpawnPlatforms());
        }
        // after that, currentPlatform gets moved up after a couple jumps
        else
        {
            // decide whether to invert section if we haven't yet
            if(!inverted)
            {
                DecideInvertSection();
            }

            bool firstPlatformMoved = false;

            foreach (GameObject platform in platforms) {
                float currentDistanceToPlayer = platform.transform.position.y - e.player.transform.position.y;
                float offscreenDist = -offscreenNumPlatforms * platformSeparation; // ex: -4 * 4 = 16 units before we shuffle out the platform

                // if the player is high enough, we need to rotate this platform
                if (currentDistanceToPlayer < offscreenDist)
                {
                    Vector3 newPlatPos = MovePlatformToTop(platform);

                    // we are planning to invert
                    if (inverted && !firstPlatformMoved)
                    {
                        // publish an invert event telling us whether we're  on our first invert
                        EventBus.Publish(
                            new SectionInvertEvent(
                                currentGeneration,
                                !hasInverted,
                                platformSeparation,
                                newPlatPos.x >= 0 // move left if platform is on right side of screen
                            )
                        );

                        // if we invert for the first time, we need to put out an event to
                        // get ready to do a tutorial
                        if (!hasInverted)
                        {
                            hasInverted = true;
                        }

                        firstPlatformMoved = true;
                    }

                    if(inverted)
                    {
                        // only actually invert if we haven't reached the top of the inverted section
                        if (currentGeneration < invertedSectionTop)
                        {
                            InvertPlatform(platform);
                        }
                        // otherwise we can reset
                        else
                        {
                            // explicitly uninvert to avoid bugs
                            UninvertPlatform(platform);
                            inverted = false;
                        }
                    }
                    // we're not planning to invert, make sure we're base
                    // also decide if we want to invert
                    else
                    {
                        UninvertPlatform(platform);
                    }

                    currentGeneration++;
                }
            }
        }
    }

    IEnumerator SpawnPlatforms()
    {
        // create numPlatforms platforms for our pool
        for (int i = 0; i < numPlatformsToGenerate; i++)
        {
            Vector3 pos = Vector3.zero;

            // x pos is random for all platforms except the first two
            if (i == 0) pos.x = 0;
            else if (i == 1) pos.x = maxXMagnitude;
            else pos.x = GetRandomX();
            
            // platform y position starts at platformSeparation and goes up from there
            pos.y = platformSeparation + i * platformSeparation;

            // create an instance
            GameObject instance = Instantiate(platformPrefab, pos, Quaternion.identity, transform);

            platforms.Add(instance);

            currentGeneration++;

            yield return new WaitForSeconds(0.5f);
        }
    }

    Vector3 MovePlatformToTop(GameObject platform)
    {
        float highestY = float.MinValue;

        foreach (GameObject plat in platforms)
        {
            if(plat.transform.position.y > highestY)
            {
                highestY = plat.transform.position.y;
            }
        }

        // move current platform platformSeparation above highest
        Vector3 newPos = platform.transform.position;
        newPos.x = GetRandomX();
        newPos.y = highestY + platformSeparation;

        platform.transform.position = newPos;

        return newPos;
    }

    void DecideInvertSection()
    {
        // 25% chance to start an inverted section of platforms
        if(UnityEngine.Random.Range(0, 1f) < .25f)
        {
            inverted = true;

            // Ensure min is less than max
            int minRange = Mathf.Min(invertedRange.x, invertedRange.y);
            int maxRange = Mathf.Max(invertedRange.x, invertedRange.y);

            int invertedSectionLength = UnityEngine.Random.Range(minRange, maxRange + 1);
            invertedSectionTop = currentGeneration + 1 + invertedSectionLength;
        }
    }

    void InvertPlatform(GameObject platform)
    {
        Renderer platRenderer = platform.GetComponent<Renderer>();

        OneWayPlatform owp = platform.GetComponent<OneWayPlatform>();
        owp.inverted = true;
        owp.nonTranslucent = new[] { invertedBase, invertedOutline };
        owp.translucent = new[] { invertedTranslucentBase, invertedTranslucentOutline };
    }

    void UninvertPlatform(GameObject platform)
    {
        Renderer platRenderer = platform.GetComponent<Renderer>();

        OneWayPlatform owp = platform.GetComponent<OneWayPlatform>();
        owp.inverted = false;
        owp.nonTranslucent = new[] { _base, outline };
        owp.translucent = new[] { translucentBase, translucentOutline };
    }

    private float GetRandomX()
    {
        return UnityEngine.Random.Range(-maxXMagnitude, maxXMagnitude);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(jumpSub);
    }
}

public class SectionInvertEvent
{
    public int generation;
    public float platformSeparation;
    public bool firstInvert;
    public bool pointingLeft;

    public SectionInvertEvent(int generation, bool firstInvert, float platformSeparation, bool pointingLeft)
    {
        this.generation = generation;
        this.firstInvert = firstInvert;
        this.platformSeparation = platformSeparation;
        this.pointingLeft = pointingLeft;
    }
}
