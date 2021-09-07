using Game;
using NPC_Behaviour;
using UnityEngine;

namespace Player_Behaviour
{
    public class Interactor : MonoBehaviour
    {
        private bool _isInteractNPC, _isInteractItem;
        private NPC _interactNPC;
        private Item _interactItem;
        private Agent _interactor;
    
        // Start is called before the first frame update
        void Start()
        {
            _isInteractNPC = _isInteractItem = false;
            _interactNPC = null; 
            _interactItem = null;
            _interactor = gameObject.GetComponent<Agent>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void Interact()
        {
            if (_isInteractNPC)
                _interactor.InteractNPC(_interactNPC);
            else if(_isInteractItem)
                _interactor.PickUpItem(_interactItem);
        }

        public void Attack()
        {
            if (_interactNPC)
                _interactor.AttackNPC(_interactNPC);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Interactor"))
            {
                if (other.gameObject.transform.parent.gameObject.TryGetComponent<NPC>(out _interactNPC))
                {
                    _isInteractNPC = true;
                    _isInteractItem = false;
                    //Debug.Log("Can interact with NPC "+_interactNPC.Name);
                }
                else if (other.gameObject.transform.parent.gameObject.TryGetComponent<Item>(out _interactItem))
                {
                    _isInteractItem = true;
                    _isInteractNPC = false;
                    //Debug.Log("Can interact with Item "+_interactItem.Name);
                }
            }
        }
    
        public void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Interactor"))
            {
                _isInteractNPC = _isInteractItem = false;
                _interactNPC = null;
                _interactItem = null;
                //Debug.Log("Can no longer interact");
            }
        }
    }
}
