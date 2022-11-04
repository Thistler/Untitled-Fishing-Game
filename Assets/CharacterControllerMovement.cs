using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControllerMovement : MonoBehaviour
{
    private Vector3 PlayerMovementInput;
    private Vector3 Velocity;

    private SpriteRenderer PlayerSprite;

    [SerializeField] private CharacterController Controller;
    [SerializeField] private float Speed;

    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite backSprite;
    [SerializeField] private Sprite sideSprite;

    void Start()
    {
        PlayerSprite = GetComponentInChildren<SpriteRenderer>();    
    }

    void Update()
    {
        PlayerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector3 moveVector = transform.TransformDirection(PlayerMovementInput);
        Controller.Move(moveVector * Speed * Time.deltaTime);

        if(PlayerMovementInput.x < -0.3)
        {
            PlayerSprite.sprite = sideSprite;
            PlayerSprite.flipX = true;
        }
        if(PlayerMovementInput.x > 0.3)
        {
            PlayerSprite.sprite = sideSprite;
            PlayerSprite.flipX = false;
        }
        if(PlayerMovementInput.z < -0.1)
        {
            PlayerSprite.sprite = frontSprite;
            PlayerSprite.flipX = false;
        }
        if(PlayerMovementInput.z > 0.1)
        {
            PlayerSprite.sprite = backSprite;
            PlayerSprite.flipX = false;
        }
    }
}
