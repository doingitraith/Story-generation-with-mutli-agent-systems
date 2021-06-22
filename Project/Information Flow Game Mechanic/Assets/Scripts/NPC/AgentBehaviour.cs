using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgentBehaviour
{
    public Agent Agent;
    public bool IsFinished;

    public AgentBehaviour(Agent agent) => (Agent, IsFinished) = (agent, false);

    protected void Init()
    {
        IsFinished = false;
        
        if (Agent == null)
            throw new NullReferenceException("Agent field is null for "+this);
    }

    public abstract IEnumerator DoBehaviour();

    public abstract IEnumerator InterruptBehaviour();

    public abstract IEnumerator ResumeBehaviour();

    public abstract bool IsBehaviourFinished();
}
