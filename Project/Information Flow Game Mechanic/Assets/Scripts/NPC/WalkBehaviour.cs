using System;
using UnityEngine;
using UnityEngine.AI;

public class WalkBehaviour : AgentBehaviour
{
    public Transform _destination;
    private NavMeshAgent _navMeshAgent;
    private Animator _animator;

    public WalkBehaviour(NPC agent, Transform destination) : base(agent)
    {
        _destination = destination;
        base.Init();
        
        _navMeshAgent = _agent.GetComponentInChildren<NavMeshAgent>();
        if(_navMeshAgent == null)
            throw new NullReferenceException("Agent "+_agent._NPCName+" does not have a NavMeshAgent component.");
        _navMeshAgent.updateRotation = false;
        
        _animator = _agent.GetComponentInChildren<Animator>();
        if(_animator == null)
            throw new NullReferenceException("Agent "+_agent._NPCName+" does not have a Animator component.");
    }

    public override void DoBehaviour()
    {
        _navMeshAgent.destination = _destination.position;
        _animator.SetFloat("RunningSpeed", .5f);
    }

    public override void InterruptBehaviour()
    {
        _navMeshAgent.destination = _agent.transform.position;
        _animator.SetTrigger("DoStop");
    }

    public override void ResumeBehaviour()
    {
        DoBehaviour();
    }

    public override bool IsBehaviourFinished()
    {
        return _agent.transform.position.Equals(_destination.position);
    }
}
