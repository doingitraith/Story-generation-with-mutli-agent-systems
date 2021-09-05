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
        
            while(_isPaused)
                yield return null;
        
        
            List<Information> infos = Agent.Memory.
                GetInformationToExchange(1, _reciever.InformationSubject);
            if (infos != null)
                GameManager.Instance.CreateConversationInformation(infos[0], Agent.transform.position);
        
            IsFinished = true;
            Agent.IsOccupied = false;
            yield return null;
        }

        public override IEnumerator InterruptBehaviour()
        {
            Agent.IsOccupied = false;
            _isPaused = true;
            yield return null;
        }

        public override IEnumerator ResumeBehaviour()
        {
            Agent.IsOccupied = true;
            _isPaused = false;
            yield return null;
        }

        public override bool IsBehaviourFinished()
        {
            return IsFinished;
        }
    }
}
