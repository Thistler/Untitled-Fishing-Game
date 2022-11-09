using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableHandler : MonoBehaviour
{
    [SerializeField] private Sprite NormalSprite;
    [SerializeField] private Sprite GlowSprite;

    public void StartGlow()
    {
        transform.GetComponentInChildren<SpriteRenderer>().sprite = GlowSprite;
    }

    public void StopGlow()
    {
        transform.GetComponentInChildren<SpriteRenderer>().sprite = NormalSprite;
    }
}
