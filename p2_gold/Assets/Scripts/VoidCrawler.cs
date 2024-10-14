using UnityEngine;

public class VoidCrawler : MonoBehaviour
{
    [SerializeField] float speed = 2f;

    GameObject platform;
    bool movingRight = true;

    float leftBoundX;
    float rightBoundX;

    PossiblyOneWayInvertible powi;

    void Start()
    {
        platform = transform.parent.gameObject; // the parent platform

        movingRight = UnityEngine.Random.Range(0, 1f) > 0.5f;

        Renderer renderer = platform.GetComponent<Renderer>(); // get platform renderer
        leftBoundX = renderer.bounds.min.x; // platform left bound
        rightBoundX = renderer.bounds.max.x; // platform right bound

        powi = GetComponent<PossiblyOneWayInvertible>();
    }

    void Update()
    {
        Vector3 currentPos = transform.position;

        if(movingRight)
        {
            if(currentPos.x < rightBoundX)
            {
                transform.position = currentPos + (speed * Time.deltaTime * Vector3.right);
            }
            else
            {
                movingRight = false;
            }
        }
        else
        {
            if (currentPos.x > leftBoundX)
            {
                transform.position = currentPos + (speed * Time.deltaTime * Vector3.left);
            }
            else
            {
                movingRight = true;
            }
        }
    }
}
