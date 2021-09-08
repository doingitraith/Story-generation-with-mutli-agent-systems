using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace NPC_Behaviour
{
    public class WalkBehaviour : AgentBehaviour
    {
        public Transform Destination;
    
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        private float _walkingSpeed = .1f;

        public WalkBehaviour(NPC agent, Transform destination) : base(agent)
        {
            Destination = destination;
            Destination.position += Vector3.forward * 2 * Random.value + Vector3.right * 2 * Random.value;
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
            _animator.SetFloat("RunningSpeed", _walkingSpeed);
            yield return null;
        }

        public void UpdateTarget()
            => _navMeshAgent.destination = Destination.position;

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
