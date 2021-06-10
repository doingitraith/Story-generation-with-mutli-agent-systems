using System;
using UnityEngine;
using UnityEngine.AI;

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

    public override void DoBehaviour()
    {
        _navMeshAgent.destination = Destination.position;
        _animator.SetFloat("RunningSpeed", .5f);
    }

    public override void InterruptBehaviour()
    {
        _navMeshAgent.destination = Agent.transform.position;
        _animator.SetTrigger("DoStop");
    }

    public override void ResumeBehaviour()
        => DoBehaviour();

    public override bool IsBehaviourFinished()
        => Agent.transform.position.Equals(Destination.position);
}
