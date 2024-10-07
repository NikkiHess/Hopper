using UnityEngine;
using TMPro;

public class TutorialInvertHandler : MonoBehaviour
{
    [SerializeField] float verticalOffset;

    TMP_Text text;
    GameObject whichWay;

    Subscription<SectionInvertEvent> firstInvertSub;
    Subscription<PlayerInvertEvent> playerInvertSub;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMP_Text>();
        firstInvertSub = EventBus.Subscribe<SectionInvertEvent>(ShowTutorial);
        playerInvertSub = EventBus.Subscribe<PlayerInvertEvent>(HideTutorial);

        whichWay = transform.GetChild(0).gameObject;
    }

    void ShowTutorial(SectionInvertEvent e)
    {
        // only do the tutorial on our first invert
        if(e.firstInvert)
        {
            text.enabled = true; // enable the text so we can see it

            float y = (e.generation + 1) * e.platformSeparation + e.platformSeparation;
            transform.position = new(transform.position.x, y + verticalOffset, transform.position.z);

            if (whichWay != null)
            {
                whichWay.SetActive(true);
                whichWay.GetComponent<MovingArrow>().pointingLeft = e.pointingLeft;
            }
        }
    }

    void HideTutorial(PlayerInvertEvent e)
    {
        if (e.firstInvert)
        {
            text.enabled = false;

            if (whichWay != null)
            {
                whichWay.SetActive(false);
            }
        }
    }

    private void OnDestroy()
    {
        EventBus.Unsubscribe(firstInvertSub);
    }
}
