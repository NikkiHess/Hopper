using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{

    [SerializeField] GameObject platformPrefab;
    [SerializeField] float maxXMagnitude; // how far to the side can we go?
    [SerializeField] float platformSeparation; // how far apart should platforms be?
    [SerializeField] int numPlatformsToGenerate = 6; // # of platforms to generate
    [SerializeField] Vector2Int invertedRange; // min and max inverted section sizes
    [SerializeField] [Range(1, 100)] int percentInvertChance = 1;

    [SerializeField] int numSafePlatforms = 2;
    [SerializeField][Range(0f, 1f)] float enemySpawnChance = 0.5f;
    [SerializeField][Range(0f, 1f)] float spikeSpawnChance = 0.5f;
    [SerializeField] GameObject voidCrawlerPrefab;
    [SerializeField] GameObject spikePrefab;

    int currentGeneration = 0; // number of times we generated or moved a platform
    bool inverted = false; // invert
    bool hasInverted = false; // have we reached the inverted section for the first time?
    int invertedSectionTop = 0; // the top of the inverted section, if there is one

    List<GameObject> platforms = new(); // the list of platforms we're populating
    public List<GameObject> offscreenPlatforms = new();

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

    private void Update()
    {
        if (offscreenPlatforms.Count > 0)
        {
            // decide whether to invert section if we haven't yet
            if (!inverted)
            {
                DecideInvertSection();
            }
        }

        bool firstPlatformMoved = false;

        foreach (GameObject platform in offscreenPlatforms)
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
                    // first inverted platform needs to not be gray
                    platform.GetComponent<PossiblyOneWayInvertible>().isGray = false;
                    hasInverted = true;
                }

                firstPlatformMoved = true;
            }

            if (inverted)
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

        offscreenPlatforms.Clear();
    }

    void OnPlayerJump(PlayerJumpEvent e)
    {
        // on first jump, spawn platforms
        if (e.firstJump)
        {
            StartCoroutine(SpawnPlatforms());
        }
    }

    public void TrySpawnEnemyOrObstacle(Platform platform)
    {
        Renderer r = platform.GetComponent<Renderer>();
        Bounds e = r.bounds;
        Vector3 spawnPos = new Vector3(
            e.center.x,
            e.center.y + e.extents.y, // just above the platform?
            e.center.z
        );

        PossiblyOneWayInvertible invertible = platform.GetComponent<PossiblyOneWayInvertible>();

        if (invertible.isGray)
        {
            float enemySpawnFloat = UnityEngine.Random.Range(0, 1f);
            float obstacleSpawnFloat = UnityEngine.Random.Range(0, 1f);

            if (enemySpawnFloat <= enemySpawnChance)
            {
                Renderer vcRenderer = voidCrawlerPrefab.GetComponent<Renderer>();

                spawnPos += new Vector3(0, vcRenderer.bounds.extents.y);
                GameObject instance = Instantiate(voidCrawlerPrefab, spawnPos, Quaternion.identity, platform.transform);
                instance.transform.localScale = new Vector3(.18f, 1.62f, .18f);

                platform.voidCrawlerInstance = instance;

                // since spikes didn't spawn, increase chance slightly
                spikeSpawnChance += 0.05f;
                // since enemy spawned, decrease chance slightly
                enemySpawnChance -= 0.1f;
            }
            else if (obstacleSpawnFloat <= spikeSpawnChance)
            {
                // no adjustments needed here
                platform.spikeInstance = Instantiate(spikePrefab, spawnPos, Quaternion.identity, platform.transform);

                // since enemy didn't spawn, increase chance slightly
                enemySpawnChance += 0.05f;
                // since spikes spawned, decrease chance slightly
                spikeSpawnChance -= 0.1f;
            }
            else
            {
                // since enemy didn't spawn, increase chance slightly
                enemySpawnChance += 0.05f;

                // since spikes didn't spawn, increase chance slightly
                spikeSpawnChance += 0.05f;
            }
        }

        Mathf.Clamp(enemySpawnChance, 0, 1);
        Mathf.Clamp(spikeSpawnChance, 0, 1);
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
            GameObject platform = Instantiate(platformPrefab, pos, Quaternion.identity, transform);
            platforms.Add(platform);

            // if we've passed the number of safe platforms, start gray platforms and enemy/obstacle spawns
            if (i > numSafePlatforms)
            {
                platform.GetComponent<PossiblyOneWayInvertible>().DecideGray();
                TrySpawnEnemyOrObstacle(platform.GetComponent<Platform>());
            }

            currentGeneration++;

            yield return new WaitForSeconds(3 / numPlatformsToGenerate);
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
        platform.GetComponent<PossiblyOneWayInvertible>().DecideGray();

        // spawn after moving
        TrySpawnEnemyOrObstacle(platform.GetComponent<Platform>());

        return newPos;
    }

    void DecideInvertSection()
    {
        // percentInvertChance% chance to start an inverted section of platforms
        if (UnityEngine.Random.Range(0, 1f) < (percentInvertChance / 100f))
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
        PossiblyOneWayInvertible invertible = platform.GetComponent<PossiblyOneWayInvertible>();
        if (!invertible.isGray)
            invertible.inverted = true;
    }

    void UninvertPlatform(GameObject platform)
    {
        PossiblyOneWayInvertible invertible = platform.GetComponent<PossiblyOneWayInvertible>();
        if (!invertible.isGray)
            invertible.inverted = false;
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
