using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    public bool inverted = false;
    private BoxCollider boxCollider;

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    // if we're moving downwards, allow collision
    // stipulation: must not be (inverted) + (in main dimension)
    //              or (not inverted) + (in inverted dimension)
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject player = other.gameObject;

            // check if we're moving downwards
            if (player.GetComponent<Rigidbody>().velocity.y < 0)
            {
                // only allow collision if player + platform are in the same dimension
                if (inverted == player.GetComponent<PlayerController>().inverted)
                {
                    // allow collision
                    boxCollider.isTrigger = false;
                }
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
