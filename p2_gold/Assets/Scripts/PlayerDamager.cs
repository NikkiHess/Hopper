using UnityEngine;

public class PlayerDamager : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            bool inverted = GetComponent<PossiblyOneWayInvertible>().inverted;
            bool playerInverted = collision.gameObject.GetComponent<PlayerController>().inverted;
            if (inverted == playerInverted)
            {
                GameObject player = collision.gameObject;
                player.GetComponent<HealthManager>().Damage();
            }
        }
    }
}
