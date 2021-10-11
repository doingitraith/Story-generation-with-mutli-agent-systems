using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using Information_Flow;
using UnityEngine;

namespace NPC_Behaviour
{
    public class ExchangeInformationBehaviour : AgentBehaviour
    {
        protected const int EXCHANGE_TIME = 5;
        
        protected Agent _reciever;
    
        public ExchangeInformationBehaviour(Agent agent, Agent reciever) : base(agent)
        {
            _reciever = reciever;
            IsPaused = false;
            base.Init();
        
            if(_reciever == null)
                throw new NullReferenceException("Reciever field is null for "+_reciever);
        }

        public override IEnumerator DoBehaviour()
        {
            Agent.IsOccupied = true;
            _reciever.IsOccupied = true;
            
            (_reciever as NPC)?.InterruptNPC();
            
            yield return new WaitForSeconds(EXCHANGE_TIME);
        
            while(IsPaused)
                yield return null;
        
            Agent.Memory.TryAddNewInformation(
                new Information(_reciever, _reciever.Location),
                Agent);
            
            _reciever.Memory.TryAddNewInformation(
                new Information(Agent, Agent.Location),
                _reciever);
        
            List<Information> infos = Agent.Memory.
                GetInformationToExchange(1, _reciever.InformationSubject);
            if (infos != null)
                GameManager.Instance.CreateConversationInformation(infos[0], Agent.transform.position, _reciever);
            
            infos = _reciever.Memory.
                GetInformationToExchange(1, Agent.InformationSubject);
            if (infos != null)
                GameManager.Instance.CreateConversationInformation(infos[0], _reciever.transform.position, Agent);
        
            IsFinished = true;
            Agent.IsOccupied = false;
            _reciever.IsOccupied = false;
            (_reciever as NPC)?.ResumeNPC();
            yield return null;
        }

        public override IEnumerator InterruptBehaviour()
        {
            Agent.IsOccupied = false;
            IsPaused = true;
            yield return null;
        }

        public override IEnumerator ResumeBehaviour()
        {
            Agent.IsOccupied = true;
            IsPaused = false;
            yield return null;
        }

        public override bool IsBehaviourFinished()
        {
            return IsFinished;
        }
    }
}
