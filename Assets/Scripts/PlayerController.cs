using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpPower = 1;
    public float speed = 1;
    public bool isOnPlatform = false;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space) && isOnPlatform)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpPower);
        }

        float horizInput = Input.GetAxis("Horizontal");
        if (Mathf.Abs(horizInput) > 0)
        {
            rb.velocity = new Vector3(horizInput * speed, rb.velocity.y);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject go = collision.gameObject;
        if (go.CompareTag("Platform") || go.CompareTag("Starter Platform"))
        {
            if (!isOnPlatform)
            {
                isOnPlatform = true;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        GameObject go = collision.gameObject;
        if ((go.CompareTag("Platform") || go.CompareTag("Starter Platform")))
        {
            if (isOnPlatform)
            {
                isOnPlatform = false;
            }
        }
    }
}
