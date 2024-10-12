using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] [Range(0f, 1f)] float enemySpawnChance = 0.5f;

    public void TrySpawnEnemy()
    {
        if (GetComponent<OneWayPlatform>().isGray) {
            float enemySpawnFloat = UnityEngine.Random.Range(0, 1f);
            GameObject vc = transform.Find("Void Crawler").gameObject;

            if (enemySpawnFloat < enemySpawnChance)
            {
                vc.SetActive(true);
            }
        }
    }

    public void DespawnEnemy()
    {
        GameObject vc = transform.Find("Void Crawler").gameObject;
        vc = transform.GetChild(0).gameObject;
        vc.SetActive(false);
    }

    public void MarkOffscreen()
    {
        PlatformManager pm = GameObject.Find("Platform Manager").GetComponent<PlatformManager>();
        pm.offscreenPlatforms.Add(gameObject);
        DespawnEnemy(); // no
    }
}
