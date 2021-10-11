using System;
using System.Collections;
using Game;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace NPC_Behaviour
{
    public class WalkBehaviour : AgentBehaviour
    {
        public Vector3 DestinationPosition;
        public bool isMovingTarget;
        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        private float _walkingSpeed = .1f;
        private Transform _destinationTransform;

        public WalkBehaviour(NPC agent, Transform destination) : base(agent)
        {
            _destinationTransform = destination;
            var position = _destinationTransform.position;
            DestinationPosition = new Vector3(position.x, 0.0f, position.z);
            DestinationPosition += Vector3.forward * 2 * Random.value + Vector3.right * 2 * Random.value;

            isMovingTarget = destination.gameObject.GetComponent<Agent>() != null;

            if (NavMesh.SamplePosition(DestinationPosition, out var hit, 4, NavMesh.AllAreas))
                DestinationPosition = hit.position;
            
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
            while (IsPaused)
                yield return null;
            
            Agent.IsOccupied = true;
            _navMeshAgent.destination = DestinationPosition;
            _navMeshAgent.isStopped = false;
            _animator.SetFloat("RunningSpeed", _walkingSpeed);
            yield return null;
        }

        public void UpdateTarget()
            => _navMeshAgent.destination = DestinationPosition = _destinationTransform.position;

        public override IEnumerator InterruptBehaviour()
        {
            _navMeshAgent.destination = Agent.transform.position;
            _navMeshAgent.isStopped = true;
            _animator.SetFloat("RunningSpeed", .0f);
            _animator.SetTrigger("DoStop");
            Agent.IsOccupied = false;
            IsPaused = true;
            yield return null;
        }

        public override IEnumerator ResumeBehaviour()
        {
            IsPaused = false;
            yield return DoBehaviour();
        }

        public override bool IsBehaviourFinished()
        {
            if (Vector3.Distance(Agent.transform.position, DestinationPosition) <= 2.0f)
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
