using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneTemplate;
using UnityEngine;

public class ExchangeInformationBehaviour : AgentBehaviour
{
    protected const int EXCHANGE_TIME = 5;

    protected bool _isPaused;
    protected Agent _reciever;
    
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
        Agent.IsOccupied = true;
        yield return new WaitForSeconds(EXCHANGE_TIME);
        
        if (_isPaused)
            yield return null;
        
        
        List<Information> infos = Agent.Memory.GetInformationsToExchange(1);
        if (infos != null)
            GameManager.Instance.CreateConversationInformation(infos[0], Agent.transform.position);
        
        IsFinished = true;
        Agent.IsOccupied = false;
    }

    public override IEnumerator InterruptBehaviour()
    {
        Agent.IsOccupied = false;
        _isPaused = true;
        return null;
    }

    public override IEnumerator ResumeBehaviour()
    {
        Agent.IsOccupied = true;
        _isPaused = false;
        return null;
    }

    public override bool IsBehaviourFinished()
    {
        return IsFinished;
    }
}
