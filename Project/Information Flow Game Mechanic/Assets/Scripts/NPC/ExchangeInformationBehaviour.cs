using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExchangeInformationBehaviour : AgentBehaviour
{
    private const int EXCHANGE_TIME = 5;

    private bool _isPaused;
    private Agent _reciever;
    
    public ExchangeInformationBehaviour(Agent agent, Agent reciever) : base(agent)
    {
        _reciever = reciever;
        _isPaused = false;
        base.Init();
        
        if(_reciever == null)
            throw new NullReferenceException("Reciever field is null for "+_reciever);
    }

    public override IEnumerator DoBehaviour()
    {
        yield return new WaitForSeconds(EXCHANGE_TIME);
        
        if (_isPaused)
            yield return null;
        
        _reciever.Memory.TryAddNewInformation(Agent.Memory.GetInformationToExchange());
        IsFinished = true;
    }

    public override IEnumerator InterruptBehaviour()
    {
        _isPaused = true;
        return null;
    }

    public override IEnumerator ResumeBehaviour()
    {
        _isPaused = false;
        return null;
    }

    public override bool IsBehaviourFinished()
    {
        return IsFinished;
    }
}
