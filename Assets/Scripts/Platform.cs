using TMPro;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public GameObject voidCrawlerInstance; // the instance we have spawned
    public GameObject spikeInstance; // the instance we have spawned

    public void DespawnEnemy()
    {
        if (voidCrawlerInstance != null)
        {
            Destroy(voidCrawlerInstance);
            voidCrawlerInstance = null;
        }
    }

    public void DespawnSpikes()
    {
        if (spikeInstance != null)
        {
            Destroy(spikeInstance);
            spikeInstance = null;
        }
    }

    public void MarkOffscreen()
    {
        PlatformManager pm = GameObject.Find("Platform Manager").GetComponent<PlatformManager>();
        pm.offscreenPlatforms.Add(gameObject);
        DespawnEnemy(); // no
        DespawnSpikes();
    }
}
