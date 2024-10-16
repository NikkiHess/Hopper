using UnityEngine;
using System.Collections.Generic;

public class PossiblyOneWayInvertible : MonoBehaviour
{
    public bool isOneWay = true;

    public bool canBeGray = false;
    public bool isGray = false;
    public Material[] gray;
    [SerializeField] [Range(0f, 1f)] float grayChance = 0.5f;

    [SerializeField] List<GameObject> toInvert;
    public bool isVoidCrawler = false;
    public bool isSpikes = false;

    public Material[] translucent, nonTranslucent, invertedTranslucent, invertedNonTranslucent;

    public bool inverted = false;
    bool playerInverted = false;
    
    GameObject player;
    Vector3 pVelocity;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        // if we're spikes, we need to track all the individual spikes
        if(isSpikes)
        {
            foreach(Transform child in transform)
            {
                toInvert.Add(child.transform.GetChild(0).gameObject);
            }
        }
        else if(isVoidCrawler)
        {
            toInvert.Add(gameObject);
            foreach (Transform child in transform)
            {
                if (child.name.Contains("Spike"))
                {
                    toInvert.Add(child.transform.GetChild(0).GetChild(0).gameObject);
                }
            }
        }

        if(isOneWay)
        {
            gameObject.layer = LayerMask.NameToLayer("Platform");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Collidable Platform");
        }
    }

    private void Update()
    {
        if (player == null) return;

        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            pVelocity = playerRb.velocity;
        }

        if (!isGray) {
            playerInverted = player.GetComponent<PlayerController>().inverted;

            // if the player is in the right dimension, go non-translucent
            if (inverted == playerInverted)
            {
                // if we're inverted, we need to use inverted materials
                if (inverted)
                {
                    foreach (GameObject go in toInvert)
                    {
                        if (go.GetComponent<Renderer>().materials != invertedNonTranslucent)
                            go.GetComponent<Renderer>().materials = invertedNonTranslucent;
                    }
                }
                // else use non-inverted
                else
                {
                    foreach (GameObject go in toInvert)
                    {
                        if (go.GetComponent<Renderer>().materials != nonTranslucent)
                            go.GetComponent<Renderer>().materials = nonTranslucent;
                    }
                }
            }
            else
            {
                // if we're inverted, we need to use inverted materials
                if (inverted)
                {
                    foreach (GameObject go in toInvert)
                    {
                        if (go.GetComponent<Renderer>().materials != invertedTranslucent)
                            go.GetComponent<Renderer>().materials = invertedTranslucent;
                    }
                }
                // else use non-inverted
                else
                {
                    foreach (GameObject go in toInvert)
                    {
                        if (go.GetComponent<Renderer>().materials != translucent)
                            go.GetComponent<Renderer>().materials = translucent;
                    }
                }
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

            // check if we're moving downwards (if one way)
            if (isOneWay && player.GetComponent<Rigidbody>().velocity.y < 0)
            {
                // only allow collision if player + platform are in the same dimension
                if (isGray || inverted == player.GetComponent<PlayerController>().inverted)
                {
                    // allow collision
                    gameObject.layer = LayerMask.NameToLayer("Collidable Platform");
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;

            // check if not one way
            if (!isOneWay)
            {
                // only allow collision if player + platform are in the same dimension
                if (inverted != player.GetComponent<PlayerController>().inverted)
                {
                    // don't allow collision
                    gameObject.layer = LayerMask.NameToLayer("Platform");

                    player.GetComponent<Rigidbody>().velocity = pVelocity;
                }
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (isOneWay)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                gameObject.layer = LayerMask.NameToLayer("Platform");
            }
        }
    }

    public void DecideGray()
    {
        if (canBeGray)
        {
            isGray = UnityEngine.Random.Range(0f, 1f) < grayChance;
            if (isGray)
            {
                foreach (GameObject go in toInvert)
                {
                    go.GetComponent<Renderer>().materials = gray;
                }
            }
        }
    }
}
