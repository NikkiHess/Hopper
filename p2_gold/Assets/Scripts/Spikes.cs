using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField][Range(0, 1)] float invertChance = 0.5f;

    private void Start()
    {
        if (UnityEngine.Random.Range(0, 1f) > invertChance) {
            GetComponent<PossiblyOneWayInvertible>().inverted = true;
        }
    }
}
