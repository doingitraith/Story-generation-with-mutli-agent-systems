using System.Collections;
using Game;
using UnityEngine;

namespace NPC_Behaviour
{
    public class WaitBehaviour : AgentBehaviour
    {
        private readonly IEnumerator _waitCoroutine;
        public WaitBehaviour(Agent agent, int duration) : base(agent)
        {
            _waitCoroutine = Wait(duration);
            IsPaused = false;
            base.Init();
        }

        public override IEnumerator DoBehaviour()
        {
            Agent.IsOccupied = true;
            NPC n = GameManager.FindObjectOfType<NPC>();
            IsFinished = false;
            yield return Agent.StartCoroutine(_waitCoroutine);
            IsFinished = true;
            Agent.IsOccupied = false;
            yield return null;
        }

        private IEnumerator Wait(int duration)
        {
            yield return new WaitForSeconds(duration);
            while(IsPaused)
                yield return null;
        }

        public override IEnumerator InterruptBehaviour()
        {
            IsPaused = true;
            Agent.IsOccupied = false;
            yield return null;
        }

        public override IEnumerator ResumeBehaviour()
        {
            IsPaused = false;
            Agent.IsOccupied = true;
            yield return null;
        }

        public override bool IsBehaviourFinished()
        {
            return IsFinished;
        }
    }
}
