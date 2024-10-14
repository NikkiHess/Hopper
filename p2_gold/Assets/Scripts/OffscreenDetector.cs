using UnityEngine;
using UnityEngine.Events;

public class OffscreenDetector : MonoBehaviour
{
    public UnityEvent onOffscreen;

    Renderer goRenderer;
    Vector3 renderTop;
    Material outline;

    private void Start()
    {
        goRenderer = GetComponent<Renderer>();

        // get the outline material to be used in calculating top
        foreach (Material material in goRenderer.materials)
        {
            if (material.name.Contains("Outline"))
            {
                outline = material;
                break;
            }
        }
    }

    private void Update()
    {
        Bounds rendererBounds = goRenderer.bounds; // bounds for the platform to make it easy to get the "top" edge
        // max y and max z ensure we're at the edge of the viewport
        renderTop = new Vector3(rendererBounds.center.x, rendererBounds.max.y + outline.GetFloat("_Outline_Thickness") * 2, rendererBounds.max.z);

        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(renderTop);

        // do our callback if we're off the bottom of the screen
        if (viewportPosition.y < 0 && onOffscreen != null)
        {
            onOffscreen.Invoke();
        }
        if(onOffscreen == null)
        {
            Debug.Log("onOffscreen is null");
        }
    }
}