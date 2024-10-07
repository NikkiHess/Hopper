using UnityEngine;

public class StarterPlatform : MonoBehaviour
{
    [SerializeField] Material _base, outline;
    [SerializeField] Material invertedBase, invertedOutline;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            EventBus.Publish(new PlatformTouchEvent(gameObject));
        }
    }

    // like magic...
    public void Disappear()
    {
        gameObject.SetActive(false);
    }

    public void Invert(bool inverted)
    {
        Renderer r = GetComponent<Renderer>();
        if (inverted)
        {
            r.materials = new[] { invertedBase, invertedOutline };
        }
        else
        {
            r.materials = new[] { _base, outline };
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
