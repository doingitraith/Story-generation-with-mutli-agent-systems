using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class NPC : Character
{
    public Routine Routine;
    
    private const int MEMORY_SIZE = 5;
    private AgentBehaviour _currentBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        Memory = new InformationManager(this, MEMORY_SIZE);
        _currentBehaviour = new WalkBehaviour(this, GameObject.Find("Stable").transform);
        _currentBehaviour.DoBehaviour();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(GetComponentInChildren<NavMeshAgent>().velocity.normalized);
    }
}
