using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTest : MonoBehaviour
{
    [SerializeField] private bool simpleMove;
    [SerializeField] private float moveSpeed;
    private CharacterController _characterController;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space) && !simpleMove)
        {
            _characterController.Move(Vector3.back * (moveSpeed * Time.deltaTime));
        }
        else if(Input.GetKey(KeyCode.Space) && simpleMove)
        {
            _characterController.SimpleMove(Vector3.back * (moveSpeed * Time.deltaTime));
        }
    }
}
