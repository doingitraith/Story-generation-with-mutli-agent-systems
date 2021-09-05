using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using Information_Flow;
using UnityEngine;

namespace NPC_Behaviour
{
    public class SendInformationBehaviour : ExchangeInformationBehaviour
    {
        public SendInformationBehaviour(Agent agent) 
            : base(agent, agent.Acquaintances.Keys.OrderBy(x=>Random.value).First())
        {
        }
    
        public override IEnumerator DoBehaviour()
        {
            Agent.IsOccupied = true;
            yield return new WaitForSeconds(EXCHANGE_TIME);
        
            if (_isPaused)
                yield return null;
        
        
            List<Information> infos = Agent.Memory.
                GetInformationToExchange(1, _reciever.InformationSubject);
            if (infos != null)
                _reciever.Memory.TryAddNewInformation(infos[0], Agent);
        
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
