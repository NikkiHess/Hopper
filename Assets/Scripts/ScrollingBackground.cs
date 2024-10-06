using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    public Vector2 scrollSpeed = new Vector2(-0.1f, 0);
    private Renderer backgroundRenderer;

    void Start()
    {
        backgroundRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        // offset based on time and speed
        Vector2 offset = scrollSpeed * Time.time;

        // set offset
        backgroundRenderer.material.mainTextureOffset = new Vector2(offset.x, offset.y);
    }
}
