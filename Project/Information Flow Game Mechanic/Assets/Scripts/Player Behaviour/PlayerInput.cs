using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player_Behaviour
{
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
            if (_animator.GetBool("IsAttacking"))
                return;
            
            Vector3 moveVelocity = _moveSpeed * (
                _currentMove.x * (Quaternion.AngleAxis(-45.0f, Vector3.up)*Vector3.right) +
                _currentMove.y * (Quaternion.AngleAxis(-45.0f, Vector3.up)*Vector3.forward)
            );

            Vector3 moveThisFrame = Time.deltaTime * moveVelocity;
            transform.Translate(moveThisFrame, Space.World);
        
            if (moveThisFrame != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(moveVelocity);
        }
    
    
        public void Move(InputAction.CallbackContext context)
        {
            if (_animator.GetBool("IsAttacking"))
                return;
            
            _currentMove = context.ReadValue<Vector2>();

            _animator.SetFloat("RunningSpeed", _currentMove.magnitude);
            if (context.canceled)
                _animator.SetTrigger("DoStop");
        }

        public void Interact(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                //Debug.Log("Interaction pressed");
                gameObject.GetComponent<Interactor>().Interact();
            }
        }

        public void Attack(InputAction.CallbackContext context)
        {
            _animator.SetTrigger("DoStop");
            _animator.SetBool("IsAttacking", true);
            _animator.SetTrigger("DoAttack");
            StartCoroutine(AttackAnimation());
        }

        private IEnumerator AttackAnimation()
        {
            do
            {
                yield return null;

            } while (_animator.GetCurrentAnimatorStateInfo(0).IsName("Melee Attack Down"));
            
            _animator.SetBool("IsAttacking", false);
        }
        
    }
}
