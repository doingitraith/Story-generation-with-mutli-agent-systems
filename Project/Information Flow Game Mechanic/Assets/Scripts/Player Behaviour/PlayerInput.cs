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
        private string _attackStateName;
        private int _attackStateHash;

        private void Start()
        {
            _animator = GetComponentInChildren<Animator>();
            _attackStateName = "Base Layer.Attack.Attack1";
            _attackStateHash = Animator.StringToHash(_attackStateName);
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
            _currentMove = context.ReadValue<Vector2>().normalized;

            _animator.SetFloat("RunningSpeed", _currentMove.magnitude);
            if (context.canceled)
                _animator.SetTrigger("DoStop");
        }

        public void Interact(InputAction.CallbackContext context)
        {
            if(_animator.GetBool("IsAttacking"))
                return;
            
            if (context.started)
            {
                //Debug.Log("Interaction pressed");
                gameObject.GetComponent<Interactor>().Interact();
            }
        }

        public void Attack(InputAction.CallbackContext context)
        {
            if(_animator.GetBool("IsAttacking") || !gameObject.GetComponent<Player>().HasWeapon())
                return;
            
            if (context.started)
            {
                _animator.SetBool("IsAttacking", true);
                gameObject.GetComponent<Interactor>().Attack();
                StartCoroutine(AttackAnimation());
            }
        }

        private IEnumerator AttackAnimation()
        {
            //_animator.SetTrigger("DoAttack");
            _animator.CrossFadeInFixedTime(_attackStateName, .2f);
            while (_animator.GetCurrentAnimatorStateInfo(0).fullPathHash != _attackStateHash)
                yield return null;
            
            float counter = 0;
            float waitTime = _animator.GetCurrentAnimatorStateInfo(0).length;

            //Now, Wait until the current state is done playing
            while (counter < (waitTime))
            {
                counter += Time.deltaTime;
                yield return null;
            }

            _animator.SetBool("IsAttacking", false);
        }
        
    }
}
