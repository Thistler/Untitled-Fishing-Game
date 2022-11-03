using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCharacterMover : MonoBehaviour
{
    private InputHandler _input;
    private float _startingYPos;

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
    }

    private void MoveTowardTarget(Vector3 targetVector)
    {
        var speed = moveSpeed * Time.deltaTime;
        transform.Translate(targetVector * moveSpeed);
        // TODO: There is probably a better way to do this, ie, preventing y value from being adjsuted at all
        transform.position = new Vector3(transform.position.x, _startingYPos, transform.position.z);
    }
}
