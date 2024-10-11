using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    public Material[] translucent, nonTranslucent; // the non-translucent materials

    public bool inverted = false;
    bool playerInverted = false;

    //BoxCollider boxCollider;
    GameObject player;
    Renderer _renderer; // the renderer attached to this GameObject

    private void Start()
    {
        //boxCollider = GetComponent<BoxCollider>();
        player = GameObject.FindGameObjectWithTag("Player");
        _renderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        playerInverted = player.GetComponent<PlayerController>().inverted;

        // if the player is in the right dimension, go non-translucent
        if(inverted == playerInverted)
        {
            if(_renderer.materials != nonTranslucent)
                _renderer.materials = nonTranslucent;
        }
        else
        {
            if (_renderer.materials != translucent)
                _renderer.materials = translucent;
        }
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
                    gameObject.layer = LayerMask.NameToLayer("Collidable Platform");
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            gameObject.layer = LayerMask.NameToLayer("Platform");
        }
    }
}
