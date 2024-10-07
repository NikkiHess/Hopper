using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementDirectionLooker : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public float maxRotationAngle = 45f;

    Rigidbody rb;
    Quaternion initialRotation;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        initialRotation = transform.rotation;
    }

    private void Update()
    {
        Vector3 velocity = rb.velocity;

        // rotate to looking direction
        if (velocity != Vector3.zero)
        {
            // get the movement direction
            Vector3 movementDir = -velocity.normalized;

            // get the rotation we want to reach
            Quaternion targetRotation = Quaternion.LookRotation(-velocity);

            // limit rotation to max allowed
            if (Quaternion.Angle(initialRotation, targetRotation) > maxRotationAngle)
            {
                targetRotation = Quaternion.RotateTowards(initialRotation, targetRotation, maxRotationAngle);
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        // rotate back to initial rotation
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, initialRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
