using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkBehaviour : AgentBehaviour
{
    public Agent ConversationPartner;
    
    public TalkBehaviour(Agent agent, Agent conversationPartner) : base(agent)
    {
        ConversationPartner = conversationPartner;
        base.Init();
    }

    public override IEnumerator DoBehaviour()
    {
        Agent.IsOccupied = true;
        GameManager.Instance.StartDialogue(Agent, ConversationPartner);
        return null;
    }

    public override IEnumerator InterruptBehaviour()
    {
        Agent.IsOccupied = false;
        GameManager.Instance.DialogueRunner.Stop();
        return null;
    }

    public override IEnumerator ResumeBehaviour()
    {
        GameManager.Instance.DialogueRunner.ResetDialogue();
        return null;
    }

    public override bool IsBehaviourFinished()
    {
        Agent.IsOccupied = GameManager.Instance.DialogueRunner.IsDialogueRunning;
        return !GameManager.Instance.DialogueRunner.IsDialogueRunning;
    }
}
