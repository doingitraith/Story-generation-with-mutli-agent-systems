using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    private Vector2 _currentMove;

    private void Update()
    {
        Vector3 moveVelocity = _moveSpeed * (
            _currentMove.x * Vector3.right +
            _currentMove.y * Vector3.forward
        );
        
        Vector3 moveThisFrame = Time.deltaTime * moveVelocity;
        transform.position += moveThisFrame;
        
        if (moveThisFrame != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(moveVelocity);
    }
    
    
    public void Move(InputAction.CallbackContext context)
    {
        _currentMove = context.ReadValue<Vector2>();
    }

    public void Interact(InputAction.CallbackContext context)
    {
        Debug.Log("Interaction pressed");
    }
    
}
