using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisappearingText : MonoBehaviour
{

    private Subscription<PlayerFirstJumpEvent> firstJumpSub;

    private void Start()
    {
        firstJumpSub = EventBus.Subscribe<PlayerFirstJumpEvent>(OnFirstJump);
    }

    // this text should disappear on first jump, hopefully
    private void OnFirstJump(PlayerFirstJumpEvent e)
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(firstJumpSub);
    }
}