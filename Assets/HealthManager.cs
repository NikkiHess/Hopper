using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] int maxHealth = 3;
    int health;

    [SerializeField] float damageCooldown;
    bool onCooldown = false;

    [SerializeField] float scaleRate = 0.1f;
    Vector3 baseScale;

    private void Start()
    {
        health = maxHealth;
        baseScale = transform.localScale;
    }

    public void Damage()
    {
        if (onCooldown) return;

        if(health > 0)
        {
            health--;
            StartCoroutine(DamageAnimation());

            if (health == 0)
            {
                StartCoroutine(Kill());
            }
        }

        // short cooldown to prevent continuous damage
        StartCoroutine(DamageCooldown());
    }

    IEnumerator DamageCooldown()
    {
        onCooldown = true;
        yield return new WaitForSeconds(damageCooldown);
        onCooldown = false;
    }

    IEnumerator DamageAnimation()
    {
        float targetScale = baseScale.x * ((float)health / maxHealth);
        // scale down maxHealth times
        while (transform.localScale.x > targetScale)
        {
            transform.localScale -= new Vector3(scaleRate, scaleRate, scaleRate);
            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator Kill()
    {
        while(transform.localScale.x > 0)
        {
            transform.localScale -= new Vector3(scaleRate, scaleRate, scaleRate);
            yield return new WaitForSeconds(0.05f);
        }

        if(transform.localScale.x < 0)
            transform.localScale = new Vector3(0, 0, 0);

        GameEnder ge = GetComponent<GameEnder>();
        ge.EndGame();
    }
}
