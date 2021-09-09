using Game;
using UnityEngine;

namespace NPC_Behaviour
{
    public class King : MonoBehaviour
    {
        public GameObject Queen;
        private bool _isDone = false;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (GameManager.Instance.DialogueManager.GetVariable("$IsDragonDead").AsBool && !_isDone)
            {
                _isDone = true;
                NPC npc = GetComponent<NPC>();
                npc.AddBehaviour(new BehaviourEntry{BehaviourObject = Queen, Type = BehaviourType.Walk});
                npc.AddBehaviour(new BehaviourEntry{BehaviourObject = Queen, Type = BehaviourType.Exchange});
            }
        }
    }
}
