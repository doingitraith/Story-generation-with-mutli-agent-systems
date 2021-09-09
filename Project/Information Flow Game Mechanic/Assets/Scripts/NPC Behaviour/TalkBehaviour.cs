using System.Collections;
using Game;

namespace NPC_Behaviour
{
    public class TalkBehaviour : AgentBehaviour
    {
        private readonly Agent _conversationPartner;
    
        public TalkBehaviour(Agent agent, Agent conversationPartner) : base(agent)
        {
            _conversationPartner = conversationPartner;
            base.Init();
        }

        public override IEnumerator DoBehaviour()
        {
            Agent.IsOccupied = true;
            GameManager.Instance.DialogueManager.StartDialogue(Agent, _conversationPartner);
            yield return null;
        }

        public override IEnumerator InterruptBehaviour()
        {
            Agent.IsOccupied = false;
            GameManager.Instance.DialogueManager.Stop();
            yield return null;
        }

        public override IEnumerator ResumeBehaviour()
        {
            GameManager.Instance.DialogueManager.ResetDialogue();
            yield return null;
        }

        public override bool IsBehaviourFinished()
        {
            Agent.IsOccupied = GameManager.Instance.DialogueManager.IsDialogueRunning;
            return !GameManager.Instance.DialogueManager.IsDialogueRunning;
        }
    }
}
