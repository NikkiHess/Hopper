using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvertingBackground : MonoBehaviour
{
    [SerializeField] Material background, invertedBackground;
    PlayerController playerController; // the player's PlayerController script
    Renderer _renderer;
    bool previousInversionState;

    private void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _renderer = GetComponent<Renderer>();

        previousInversionState = !playerController.inverted;
    }

    private void Update()
    {
        // only worry about updating when we need to
        if (playerController.inverted != previousInversionState)
        {
            // if we must invert, we shall do it
            if (playerController.inverted)
            {
                _renderer.material = invertedBackground;
            }
            // otherwise let's make sure the material is normal
            else
            {
                _renderer.material = background;
            }
        }

        previousInversionState = playerController.inverted;
    }
}
