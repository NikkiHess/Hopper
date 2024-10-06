using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleLerp : MonoBehaviour
{
    public float targetY = 0;
    public float lerpSpeed = 1f;
    public bool doLerp = false;
    public bool hasLerped = false;
    public GameObject instructionsText;

    private Subscription<PlatformTouchEvent> platTouchSub;
    private float startTime;
    private float journeyLength;
    private Vector3 targetPos;

    // Start is called before the first frame update
    void Start()
    {
        platTouchSub = EventBus.Subscribe<PlatformTouchEvent>(OnPlatformTouch);

        targetPos = new Vector3(transform.position.x, targetY, transform.position.z);
        journeyLength = Vector3.Distance(transform.position, targetPos);
    }

    void OnPlatformTouch(PlatformTouchEvent e)
    {
        Debug.Log(e.ToString());

        if (!hasLerped)
        {
            if (e.platform.CompareTag("Starter Platform"))
            {
                doLerp = true;
                startTime = Time.time;
            }
        }
    }

    private void Update()
    {
        float dist = (Time.time - startTime) * lerpSpeed;
        float progress = dist / journeyLength;

        if(doLerp && !hasLerped)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, progress);

            if(Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                doLerp = false;
                hasLerped = true;

                instructionsText.SetActive(true);
            }
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(platTouchSub);
    }
}
