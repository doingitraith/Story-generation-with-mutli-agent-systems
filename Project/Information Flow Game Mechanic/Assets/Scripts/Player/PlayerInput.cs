using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    private Vector2 _currentMove;
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        Vector3 moveVelocity = _moveSpeed * (
            _currentMove.x * Vector3.right +
            _currentMove.y * Vector3.forward
        );

        Vector3 moveThisFrame = Time.deltaTime * moveVelocity;
        transform.Translate(moveThisFrame, Space.World);
        
        if (moveThisFrame != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(moveVelocity);
    }
    
    
    public void Move(InputAction.CallbackContext context)
    {
        _currentMove = context.ReadValue<Vector2>();
        
        _animator.SetFloat("RunningSpeed", _currentMove.magnitude);
        if (context.canceled)
            _animator.SetTrigger("DoStop");


    }

    public void Interact(InputAction.CallbackContext context)
    {
        if (context.started)
            Debug.Log("Interaction pressed");    
    }
    
}
