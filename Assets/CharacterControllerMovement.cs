using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerMovement : MonoBehaviour
{
    private Vector3 PlayerMovementInput;
    private Vector3 Velocity;

    private SpriteRenderer PlayerSprite;

    private bool Fishing;

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

        if(!Fishing)
        {
            MovePlayer();

            if(Input.GetKey(KeyCode.Mouse0))
            {
                Fishing = true;

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
                newBobber.GetComponent<Rigidbody>().AddForce(BobberStartPoint.forward * 700f);
            }
        }
        if (Fishing)
        {
            // Hit Q to cancel cast
            // TODO: This should not be doable when a fish is on the line
            if(Input.GetKey(KeyCode.Q))
            {
                Fishing = false;
                if (newBobber) Destroy(newBobber, 0);
            }
        }
    }

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
}
