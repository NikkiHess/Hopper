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
    [SerializeField] Vector2 invertedRange; // min and max inverted section sizes

    [SerializeField] int generations = 0; // number of times we generated or moved a platform
    bool inverted = false; // invert
    [SerializeField] int invertedSectionTop = 0; // the top of the inverted section, if there is one

    [SerializeField] Material invertedBase, invertedTranslucentBase;
    [SerializeField] Material invertedOutline, invertedTranslucentOutline;
    [SerializeField] Material _base, translucentBase;
    [SerializeField] Material outline, translucentOutline;

    // the list of platforms we're populating
    List<GameObject> platforms = new(); // newer C# syntax?

    // subscription to player's first jump
    private Subscription<PlayerJumpEvent> jumpSub;

    private void Start()
    {
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
            foreach (GameObject platform in platforms) {
                float currentDistanceToPlayer = platform.transform.position.y - e.player.transform.position.y;
                float offscreenDist = -offscreenNumPlatforms * platformSeparation; // ex: -4 * 4 = 16 units before we shuffle out the platform

                // if the player is high enough, we need to rotate this platform
                if (currentDistanceToPlayer < offscreenDist)
                {
                    MovePlatformToTop(platform);

                    // we are planning to invert
                    if (inverted)
                    {
                        // only actually invert if we haven't reached the top of the inverted section
                        if(generations < invertedSectionTop)
                        {
                            InvertPlatform(platform);
                        }
                        // otherwise we can reset
                        else
                        {
                            inverted = false;
                        }
                    }
                    // we're not planning to invert, make sure we're base
                    // also decide if we want to invert
                    else
                    {
                        UninvertPlatform(platform);

                        DecideInvertSection();
                    }

                    generations++;
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

            generations++;

            yield return new WaitForSeconds(0.5f);
        }
    }

    void MovePlatformToTop(GameObject platform)
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
    }

    void DecideInvertSection()
    {
        // 25% chance to start an inverted section of platforms
        if(UnityEngine.Random.Range(0, 1f) < .25f)
        {
            inverted = true;
            invertedSectionTop = 
                generations + 1 +
                UnityEngine.Random.Range(
                    (int)invertedRange.x, 
                    (int)invertedRange.y
                );
        }
    }

    void InvertPlatform(GameObject platform)
    {
        Renderer platRenderer = platform.GetComponent<Renderer>();

        //Material[] invertedMats = new[] { invertedBase, invertedOutline };
        //Material[] invertedTranslucentMats = new[] { invertedTranslucentBase, invertedTranslucentOutline };
        //platRenderer.materials = invertedTranslucentMats;

        OneWayPlatform owp = platform.GetComponent<OneWayPlatform>();
        owp.inverted = true;
        owp.nonTranslucent = new[] { invertedBase, invertedOutline };
        owp.translucent = new[] { invertedTranslucentBase, invertedTranslucentOutline };
    }

    void UninvertPlatform(GameObject platform)
    {
        Renderer platRenderer = platform.GetComponent<Renderer>();

        //Material[] mats = new[] { _base, outline };
        //Material[] translucentMats = new[] { translucentBase, translucentOutline };
        //platRenderer.materials = translucentMats;

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
