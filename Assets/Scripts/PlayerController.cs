using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 PlayerMovementInput;
    private Vector3 Velocity;

    private SpriteRenderer PlayerSprite;

    // 0 - Rod is not out, can move around and interact with objects
    // 1 - Casting
    // 2 - Waiting - Bobber is in water, waiting for fish to bite
    // 3 - Reeling - Bringing in the fish, obviously
    private int PlayerState;

    [SerializeField] private CharacterController Controller;
    [SerializeField] private float Speed;

    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite backSprite;
    [SerializeField] private Sprite sideSprite;

    [SerializeField] private Transform BobberStartPoint;
    [SerializeField] private GameObject Bobber;

    private GameObject newBobber;

    void Start()
    {
        PlayerSprite = GetComponentInChildren<SpriteRenderer>();    
    }

    void Update()
    {
        PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        switch(PlayerState)
        {
            // While rod is put away
            case 0:
                MovePlayer();

                if(Input.GetKey(KeyCode.Mouse0))
                {
                    // Begin our cast
                    PlayerState = 1;

                    // Get our direction
                    if (PlayerSprite.sprite == frontSprite)
                    {
                        BobberStartPoint.eulerAngles = new Vector3(0f, 180f, 0f);
                    }
                    else if (PlayerSprite.sprite == backSprite)
                    {
                        BobberStartPoint.eulerAngles = new Vector3(0f, 0f, 0f);
                    }
                    else if (PlayerSprite.sprite == sideSprite && PlayerSprite.flipX)
                    {
                        BobberStartPoint.eulerAngles = new Vector3(0f, 270f, 0f);
                    }
                    else if (PlayerSprite.sprite == sideSprite && !PlayerSprite.flipX) {
                        BobberStartPoint.eulerAngles = new Vector3(0f, 90f, 0f);
                    }

                    newBobber = (GameObject)Instantiate(Bobber, BobberStartPoint.position, new Quaternion(0f, 0f, 0f, 0f));
                    newBobber.GetComponent<Rigidbody>().AddForce(BobberStartPoint.forward * 100f);
                }
                break;
            
            // Casting, bobber is in the air
            case 1:
                PrepToCancelCast();
                break;

            // Waiting, bobber is in the water
            case 2:
                PrepToCancelCast();
                break;

            // Reeling, fish is comin' in hot
            case 3:
                break;
        }
    }

    // Controls player movement and sprite animation
    private void MovePlayer()
    {
        Vector3 moveVector = transform.TransformDirection(PlayerMovementInput);
        Controller.Move(moveVector * Speed * Time.deltaTime);

        if (PlayerMovementInput.z < -0.1)
        {
            PlayerSprite.sprite = frontSprite;
            PlayerSprite.flipX = false;
        }
        if (PlayerMovementInput.z > 0.1)
        {
            PlayerSprite.sprite = backSprite;
            PlayerSprite.flipX = false;
        }
        if (PlayerMovementInput.x < -0.3)
        {
            PlayerSprite.sprite = sideSprite;
            PlayerSprite.flipX = true;
        }
        if (PlayerMovementInput.x > 0.3)
        {
            PlayerSprite.sprite = sideSprite;
            PlayerSprite.flipX = false;
        }
    }

    // When casting or waiting, we can cancel the cast by pressing q
    private void PrepToCancelCast()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            PlayerState = 0;
            if (newBobber) Destroy(newBobber, 0);
        }
    }

    // Resets the bobber after it falls on the ground
    public void ResetBobberFromGround()
    {
        StartCoroutine("BobberDelay");
    }

    // Resets bobber and player state after a delay
    private IEnumerator BobberDelay()
    {
        yield return new WaitForSecondsRealtime(1f);
        PlayerState = 0;
        if (newBobber)
        {
            Destroy(newBobber, 0);
        }
    }
}
