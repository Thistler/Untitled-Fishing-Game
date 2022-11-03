using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCharacterMover : MonoBehaviour
{
    private InputHandler _input;
    private float _startingYPos;

    // This may have to be tweaked when we animate the sprites
    public Sprite sideSprite;
    public Sprite frontSprite;
    public Sprite backSprite;

    [SerializeField]
    private float moveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        _input = GetComponent<InputHandler>();
        _startingYPos = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        var targetVector = new Vector3(_input.InputVector.x, 0, _input.InputVector.y);

        // Move
        MoveTowardTarget(targetVector);

        // Update sprite
        // TODO: Might refactor this to only update values when needed
        if(targetVector.z > 0.1)
        {
            GetComponent<SpriteRenderer>().sprite = backSprite;
            GetComponent<SpriteRenderer>().flipX = false;
        }
        if(targetVector.z < -0.1)
        {
            GetComponent<SpriteRenderer>().sprite = frontSprite;
            GetComponent<SpriteRenderer>().flipX = false;
        }
        if(targetVector.x > 0.2)
        {
            GetComponent<SpriteRenderer>().sprite = sideSprite;
            GetComponent<SpriteRenderer>().flipX = false;
        }
        if (targetVector.x < -0.2)
        {
            GetComponent<SpriteRenderer>().sprite = sideSprite;
            GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    private void MoveTowardTarget(Vector3 targetVector)
    {
        var speed = moveSpeed * Time.deltaTime;
        transform.Translate(targetVector * moveSpeed);
        // TODO: There is probably a better way to do this, ie, preventing y value from being adjsuted at all
        transform.position = new Vector3(transform.position.x, _startingYPos, transform.position.z);
    }
}
