using System.Collections;
using System.Collections.Generic;
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
    public PlatformTouchEvent(GameObject platform)
    {
        this.platform = platform;
    }

    public override string ToString()
    {
        return "platform touched: " + platform.name;
    }
}
