using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Routine
{
    private List<AgentBehaviour> _routineSteps;

    public Routine()
        => _routineSteps = new List<AgentBehaviour>();

    public Routine(List<AgentBehaviour> initialRoutine)
        => _routineSteps = initialRoutine;

    public void AddBehaviour(AgentBehaviour behaviour)
        => AddBehaviour(behaviour, 0);

    public void AddBehaviour(AgentBehaviour behaviour, int idx)
        => _routineSteps.Insert(idx, behaviour);

    public void RemoveBehaviour(int idx)
        => _routineSteps.RemoveAt(idx);

    public AgentBehaviour GetBehaviour(int idx)
        => _routineSteps.ElementAt(idx);
}
