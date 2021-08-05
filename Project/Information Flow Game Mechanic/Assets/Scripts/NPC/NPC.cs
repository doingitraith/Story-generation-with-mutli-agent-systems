using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class NPC : Agent
{
    public float MemoryMutationInterval = 30.0f;
    public Routine Routine;

    private const int MEMORY_SIZE = 5;
    private AgentBehaviour _currentBehaviour;
    private float _currentMutationTime = .0f;

    protected override void Awake()
    {
        base.Awake();
    }
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Memory = new InformationManager(this, MEMORY_SIZE);
        /*
        _currentBehaviour = new WalkBehaviour(this, GameObject.Find("Stable").transform);
        _currentBehaviour.DoBehaviour();
        */
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        _currentMutationTime += Time.deltaTime;
        if (_currentMutationTime > MemoryMutationInterval)
        {
            Memory.MutateMemory();
            _currentMutationTime = .0f;
        }
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(GetComponentInChildren<NavMeshAgent>().velocity.normalized);
    }

}
