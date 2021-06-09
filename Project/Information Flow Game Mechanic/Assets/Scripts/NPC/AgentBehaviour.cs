using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgentBehaviour
{
    public NPC Agent;
    public bool IsFinished;

    public AgentBehaviour(NPC agent) => (Agent, IsFinished) = (agent, false);

    protected void Init()
    {
        IsFinished = false;
        
        if (Agent == null)
            throw new NullReferenceException("Agent field is null for "+this);
    }

    public abstract void DoBehaviour();

    public abstract void InterruptBehaviour();

    public abstract void ResumeBehaviour();

    public abstract bool IsBehaviourFinished();
}
