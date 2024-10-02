using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleLerp : MonoBehaviour
{
    public Vector3 targetPos; // to slerp to
    public float slerpSpeed = 1f;
    public bool doLerp = false;
    public bool hasLerped = false;

    private Subscription<PlatformTouchEvent> platTouchSub;
    private float startTime;
    private float journeyLength;

    // Start is called before the first frame update
    void Start()
    {
        platTouchSub = EventBus.Subscribe<PlatformTouchEvent>(OnPlatformTouch);

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
        float dist = (Time.time - startTime) * slerpSpeed;
        float progress = dist / journeyLength;

        if(doLerp)
        {
            transform.position = Vector3.Slerp(transform.position, targetPos, progress);

            if(Vector3.Distance(transform.position, targetPos) < 0.01f)
            {
                doLerp = false;
                hasLerped = true;
            }
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(platTouchSub);
    }
}
