using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobberCollider : MonoBehaviour
{

    [SerializeField] private Sprite waterSprite;
    [SerializeField] private Sprite groundSprite;
    [SerializeField] private Sprite sunkSprite;

    [SerializeField] private AudioClip SplashingSoundEffect;

    private PlayerController PlayerController;

    private Rigidbody BobberRigidbody;
    private SpriteRenderer BobberSprite;

    private void Start()
    {
        BobberRigidbody = GetComponent<Rigidbody>();
        BobberSprite = GetComponentInChildren<SpriteRenderer>();
        PlayerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
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
                PlayerController.ResetBobberFromGround();
            }
            else if (other.gameObject.tag == "Water")
            {
                BobberSprite.sprite = waterSprite;
                PlayerController.StartWaitingForBite(MapManager.StaticMapManager.GetTileType(other.gameObject));
            }
        }
    }

    public void SinkSprite()
    {
        BobberSprite.sprite = sunkSprite;
        // TODO: Should audio for sinking bobber go here?
    }

    public void PlaySplashSound()
    {
        if (!GetComponent<AudioSource>().isPlaying) GetComponent<AudioSource>().PlayOneShot(SplashingSoundEffect, 0.2F);
    }
}
