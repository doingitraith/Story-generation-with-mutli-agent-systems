using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitBehaviour : AgentBehaviour
{
    public WaitBehaviour(Agent agent) : base(agent)
    {
    }

    public override IEnumerator DoBehaviour()
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator InterruptBehaviour()
    {
        throw new System.NotImplementedException();
    }

    public override IEnumerator ResumeBehaviour()
    {
        throw new System.NotImplementedException();
    }

    public override bool IsBehaviourFinished()
    {
        throw new System.NotImplementedException();
    }
}
