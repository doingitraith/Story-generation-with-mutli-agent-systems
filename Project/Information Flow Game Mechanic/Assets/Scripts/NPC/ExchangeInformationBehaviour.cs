using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneTemplate;
using UnityEngine;

public class ExchangeInformationBehaviour : AgentBehaviour
{
    public GameObject InformationPrefab;
    
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
        
        // TODO: Change to CreateNewInformationPrefab
        
        List<Information> infos = Agent.Memory.GetInformationsToExchange(1);
        if (infos == null)
        {
            // TODO: Handle empty information case
        }
        else
        {
            InformationPrefab.GetComponent<InformationObject>().Information = infos[0];
            Agent.Instantiate(InformationPrefab,Agent.transform.position, Quaternion.identity);
        }
        
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
