using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    public bool isGray = false;
    public Material[] gray;
    [SerializeField] [Range(0f, 1f)] float grayChance = 0.5f;

    public Material[] translucent, nonTranslucent; // the non-translucent materials

    public bool inverted = false;
    bool playerInverted = false;
    
    GameObject player;
    Renderer _renderer; // the renderer attached to this GameObject

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        _renderer = GetComponent<Renderer>();

        isGray = UnityEngine.Random.Range(0f, 1f) < grayChance;
        if (isGray) _renderer.materials = gray;
    }

    private void Update()
    {
        if (!isGray) {
            playerInverted = player.GetComponent<PlayerController>().inverted;

            // if the player is in the right dimension, go non-translucent
            if (inverted == playerInverted)
            {
                if (_renderer.materials != nonTranslucent)
                    _renderer.materials = nonTranslucent;
            }
            else
            {
                if (_renderer.materials != translucent)
                    _renderer.materials = translucent;
            }
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
