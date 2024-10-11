using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public void MarkOffscreen()
    {
        PlatformManager pm = GameObject.Find("Shuffling Platforms").GetComponent<PlatformManager>();
        pm.offscreenPlatforms.Add(gameObject);
    }
}
