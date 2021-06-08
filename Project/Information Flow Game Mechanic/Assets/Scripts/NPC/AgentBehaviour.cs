using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgentBehaviour
{
    public NPC _agent;
    public bool _isFinished;

    public AgentBehaviour(NPC agent) => (_agent, _isFinished) = (agent, false);

    protected void Init()
    {
        _isFinished = false;
        
        if (_agent == null)
            throw new NullReferenceException("Agent field is null for "+this);
    }

    public abstract void DoBehaviour();

    public abstract void InterruptBehaviour();

    public abstract void ResumeBehaviour();

    public abstract bool IsBehaviourFinished();
}
