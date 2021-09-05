using UnityEngine;
using UnityEngine.AI;

namespace NPC_Behaviour
{
    public class NPCWalkTest : MonoBehaviour
    {
        public Transform Goal;
       
        // Start is called before the first frame update
        void Start ()
        {
            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            agent.destination = Goal.position;
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
