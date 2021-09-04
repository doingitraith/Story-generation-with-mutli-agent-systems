using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitBehaviour : AgentBehaviour
{
    private readonly IEnumerator _waitCoroutine;
    private bool _isPaused;
    public WaitBehaviour(Agent agent, int duration) : base(agent)
    {
        _waitCoroutine = Wait(duration);
        _isPaused = false;
        base.Init();
    }

    public override IEnumerator DoBehaviour()
    {
        Agent.IsOccupied = true;
        NPC n = GameManager.FindObjectOfType<NPC>();
        IsFinished = false;
        yield return Agent.StartCoroutine(_waitCoroutine);
        IsFinished = true;
        Agent.IsOccupied = false;
        yield return null;
    }

    private IEnumerator Wait(int duration)
    {
        yield return new WaitForSeconds(duration);
        while(_isPaused)
            yield return null;
    }

    public override IEnumerator InterruptBehaviour()
    {
        _isPaused = true;
        Agent.IsOccupied = false;
        yield return null;
    }

    public override IEnumerator ResumeBehaviour()
    {
        _isPaused = false;
        Agent.IsOccupied = true;
        yield return null;
    }

    public override bool IsBehaviourFinished()
    {
        return IsFinished;
    }
}
