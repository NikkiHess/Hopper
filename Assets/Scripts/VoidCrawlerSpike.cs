using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidCrawlerSpike : MonoBehaviour
{
    [SerializeField] float scaleLength = 0.2f;
    Vector3 initialScale; // euler
    Vector3 randomScale; // euler

    [SerializeField] Vector2 scaleRange = new Vector2(0.7f, 1.5f);
    bool needsNewScale = true; // tells us whether to start a lerp

    private void Start()
    {
        initialScale = transform.localScale;
    }

    private void Update()
    {
        if(needsNewScale)
        {
            StartCoroutine(SpikeScale());
        }
    }

    IEnumerator SpikeScale()
    {
        float scaleTime = 0f; // tells us how long we've been scaling
        float randomVal = UnityEngine.Random.Range(scaleRange.x, scaleRange.y);
        randomScale = new Vector3(randomVal, randomVal, randomVal);
        needsNewScale = false;
        
        float scaleProgress = scaleTime / scaleLength;
        while (scaleProgress < 1f) {
            scaleTime += Time.deltaTime;
            scaleProgress = scaleTime / scaleLength;

            transform.localScale = Vector3.Lerp(initialScale, randomScale, scaleProgress);

            yield return null;
        }

        initialScale = transform.localScale;
        needsNewScale = true; // allow process to start again
    }
}
