using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectRatioEnforcer : MonoBehaviour
{
    [SerializeField] private int targetWidth = 1080;
    [SerializeField] private int targetHeight = 1920;
    [SerializeField] private float scalingFactor = 0.8f; // how much of the screen should we take up

    private void Start()
    {
        EnforceAspectRatio();
    }

    private void EnforceAspectRatio()
    {
        float targetAspect = (float) targetWidth / targetHeight;
        float windowAspect = Screen.width / Screen.height;

        int displayHeight = Display.main.systemHeight;
        int displayWidth = Display.main.systemWidth;

        int scaledHeight = Mathf.RoundToInt(displayHeight * scalingFactor);
        int scaledWidth = Mathf.RoundToInt(scaledHeight * targetAspect);

        Screen.SetResolution(scaledWidth, scaledHeight, false);
    }
}
