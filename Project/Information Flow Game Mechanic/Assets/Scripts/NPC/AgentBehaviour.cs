using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BehaviourType
{
    Walk = 0,
    Dialogue = 1,
    Exchange = 2,
    Send = 3,
    Wait = 4,
}

[Serializable]
public struct BehaviourEntry
{
    [SerializeField]
    public BehaviourType Type;
    [SerializeField]
    public GameObject BehaviourObject;
    [SerializeField]
    public int BehaviourTime;
}

public abstract class AgentBehaviour
{
    protected Agent Agent;
    protected bool IsFinished;

    protected AgentBehaviour(Agent agent) => (Agent, IsFinished) = (agent, false);

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
