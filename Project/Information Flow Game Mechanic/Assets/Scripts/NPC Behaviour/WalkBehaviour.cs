using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace NPC_Behaviour
{
    public class WalkBehaviour : AgentBehaviour
    {
        public Transform Destination;
    
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;

        public WalkBehaviour(NPC agent, Transform destination) : base(agent)
        {
            Destination = destination;
            base.Init();
        
            _navMeshAgent = Agent.GetComponentInChildren<NavMeshAgent>();
            if(_navMeshAgent == null)
                throw new NullReferenceException("Agent "+Agent.Name+" does not have a NavMeshAgent component.");
            _navMeshAgent.updateRotation = false;
        
            _animator = Agent.GetComponentInChildren<Animator>();
            if(_animator == null)
                throw new NullReferenceException("Agent "+Agent.Name+" does not have a Animator component.");
        }

        public override IEnumerator DoBehaviour()
        {
            Agent.IsOccupied = true;
            _navMeshAgent.destination = Destination.position;
            _navMeshAgent.isStopped = false;
            _animator.SetFloat("RunningSpeed", .5f);
            yield return null;
        }

        public override IEnumerator InterruptBehaviour()
        {
            _navMeshAgent.destination = Agent.transform.position;
            _navMeshAgent.isStopped = true;
            _animator.SetFloat("RunningSpeed", .0f);
            _animator.SetTrigger("DoStop");
            Agent.IsOccupied = false;
            yield return null;
        }

        public override IEnumerator ResumeBehaviour()
            => DoBehaviour();

        public override bool IsBehaviourFinished()
        {
            if (Vector3.Distance(Agent.transform.position, Destination.position) <= 1.0f)
            {
                _navMeshAgent.destination = Agent.transform.position;
                _navMeshAgent.isStopped = true;
                Agent.IsOccupied = false;
                _animator.SetFloat("RunningSpeed", .0f);
                _animator.SetTrigger("DoStop");
                return true;
            }
            else
                return false;
        }
    }
}
