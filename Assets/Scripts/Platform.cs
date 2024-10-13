using TMPro;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField][Range(0f, 1f)] float enemySpawnChance = 0.5f;
    [SerializeField][Range(0f, 1f)] float obstacleSpawnChance = 0.5f;

    [SerializeField] GameObject voidCrawler;
    [SerializeField] GameObject spikes;

    GameObject voidCrawlerInstance; // the instance we have spawned
    
    public void TrySpawnEnemyOrObstacle()
    {
        Renderer r = GetComponent<Renderer>();
        Bounds e = r.bounds;
        Vector3 spawnPos = new Vector3(
            e.center.x,
            e.center.y + e.extents.y, // just above the platform?
            e.center.z
        );

        if (GetComponent<OneWayPlatform>().isGray)
        {
            float enemySpawnFloat = UnityEngine.Random.Range(0, 1f);
            float obstacleSpawnFloat = UnityEngine.Random.Range(0, 1f);

            if (enemySpawnFloat < enemySpawnChance)
            {
                spawnPos += new Vector3(0, voidCrawler.GetComponent<Renderer>().bounds.extents.y);
                GameObject instance = Instantiate(voidCrawler, spawnPos, Quaternion.identity, transform);
                instance.transform.localScale = new Vector3(.18f, 1.62f, .18f);
            }
            else if(obstacleSpawnFloat < obstacleSpawnChance)
            {
                // TODO
            }
        }
    }

    public void DespawnEnemy()
    {
        if (voidCrawlerInstance != null)
        {
            GameObject.Destroy(voidCrawlerInstance);
        }
    }

    public void MarkOffscreen()
    {
        PlatformManager pm = GameObject.Find("Platform Manager").GetComponent<PlatformManager>();
        pm.offscreenPlatforms.Add(gameObject);
        DespawnEnemy(); // no
    }
}
