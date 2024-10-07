using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class MovingArrow : MonoBehaviour
{
    public bool pointingLeft = false;
    [SerializeField] float speed = 2f;
    [SerializeField] float magnitude = 0.3f;

    private void Start()
    {
        // make the arrow point left if we need to move left
        if(pointingLeft)
        {
            Vector3 scale = transform.localScale;
            transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
        }
    }

    private void Update()
    {
        Vector3 pos = transform.position;

        // bounce between -magnitude and magnitude
        float x = Mathf.PingPong(Time.time * speed, magnitude * 2) - magnitude;
        transform.position = new Vector3(x, pos.y, pos.z);
    }
}
