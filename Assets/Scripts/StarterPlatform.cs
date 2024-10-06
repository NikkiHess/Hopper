using UnityEngine;

public class StarterPlatform : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            EventBus.Publish(new PlatformTouchEvent(gameObject));
        }
    }
}

public class PlatformTouchEvent
{
    public GameObject platform;
    public bool isStarterPlatform = false;
    public PlatformTouchEvent(GameObject platform)
    {
        this.platform = platform;
        isStarterPlatform = platform.CompareTag("Starter Platform");
    }

    public override string ToString()
    {
        return "platform touched: " + platform.name;
    }
}
