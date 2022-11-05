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
            BobberRigidbody.velocity = Vector3.zero;
            BobberRigidbody.useGravity = false;

            if (other.gameObject.tag == "Water")
            {
                BobberSprite.sprite = waterSprite;
                // TODO: Begin the fish
            }
            else if (other.gameObject.tag == "Ground")
            {
                BobberSprite.sprite = groundSprite;
                // TODO: No longer fishing
            }
        }
        
    }
}
