using System.Collections;
using Game;

namespace NPC_Behaviour
{
    public class DropBehaviour : AgentBehaviour
    {
        private Item _item;
        public DropBehaviour(Agent agent, Item item) : base(agent)
        {
            _item = item;
            base.Init();
        }

        public override IEnumerator DoBehaviour()
        {
            Agent.IsOccupied = true;
            IsFinished = false;
            if(Agent.Inventory.Contains(_item))
                Agent.DropItem(_item);
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
