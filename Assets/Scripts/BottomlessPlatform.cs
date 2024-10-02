using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomlessPlatform : MonoBehaviour
{
    private BoxCollider boxCollider;
    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    // if we're moving downwards, allow collision
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject player = other.gameObject;

            // check if we're moving downwards
            if (player.GetComponent<Rigidbody>().velocity.y < 0)
            {
                // allow collision
                boxCollider.isTrigger = false;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            boxCollider.isTrigger = true;
        }
    }
}
