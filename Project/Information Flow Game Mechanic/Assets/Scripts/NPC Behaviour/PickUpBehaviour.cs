using System.Collections;
using Game;
using UnityEngine;

namespace NPC_Behaviour
{
    public class PickUpBehaviour : AgentBehaviour
    {
        private Item _item;
        public PickUpBehaviour(Agent agent, Item item) : base(agent)
        {
            _item = item;
            base.Init();
        }

        public override IEnumerator DoBehaviour()
        {
            Agent.IsOccupied = true;
            IsFinished = false;
            Collider[] hits = Physics.OverlapSphere(Agent.transform.position, 3.0f);
            foreach (Collider c in hits)
            {
                var i = c.gameObject.GetComponent<Item>();
                if(i!= null && i.Equals(_item))
                    Agent.PickUpItem(_item);
            }
            IsFinished = true;
            Agent.IsOccupied = false;
            yield return null;
        }

        public override IEnumerator InterruptBehaviour()
        {
            Agent.IsOccupied = false;
            yield return null;
        }

        public override IEnumerator ResumeBehaviour()
        {
            Agent.IsOccupied = true;
            yield return null;
        }

        public override bool IsBehaviourFinished()
            => IsFinished;
    }
}
