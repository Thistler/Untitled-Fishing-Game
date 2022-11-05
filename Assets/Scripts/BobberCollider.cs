using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobberCollider : MonoBehaviour
{

    [SerializeField] private Sprite waterSprite;
    [SerializeField] private Sprite groundSprite;

    private Rigidbody BobberRigidbody;
    private SpriteRenderer BobberSprite;

    private void Start()
    {
        BobberRigidbody = GetComponent<Rigidbody>();
        BobberSprite = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Water" || other.gameObject.tag == "Ground")
        {
            // Stop further movement of bobber
            BobberRigidbody.velocity = Vector3.zero;
            BobberRigidbody.useGravity = false;
            // Prevent bobber from detecting a second collision
            BobberRigidbody.detectCollisions = false;

            if (other.gameObject.tag == "Ground")
            {
                BobberSprite.sprite = groundSprite;
                // TODO: No longer fishing
            }
            else if (other.gameObject.tag == "Water")
            {
                BobberSprite.sprite = waterSprite;
                // TODO: Begin the fish
            }
        }
    }
}
