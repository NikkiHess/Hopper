using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{
    [SerializeField] GameObject platformPrefab;
    [SerializeField] float maxXMagnitude;
    [SerializeField] float platformSeparation;
    [SerializeField] int offscreenNumPlatforms = 4; // # of platforms before we start moving things up
    [SerializeField] int numPlatforms = 6;

    // the list of platforms we're populating
    List<GameObject> platforms = new List<GameObject>();

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
                }
            }
        }
    }

    IEnumerator SpawnPlatforms()
    {
        // create numPlatforms platforms for our pool
        for (int i = 0; i < numPlatforms; i++)
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
    private float GetRandomX()
    {
        return UnityEngine.Random.Range(-maxXMagnitude, maxXMagnitude);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(jumpSub);
    }
}
