using UnityEngine;

public class PlayerDamager : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject player = collision.gameObject;
            player.GetComponent<HealthManager>().Damage();
        }
    }
}
