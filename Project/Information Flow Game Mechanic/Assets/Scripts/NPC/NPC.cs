using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class NPC : MonoBehaviour
{
    public string NPCName;
    public Routine Routine;
    public InformationManager Memory;

    private const int MEMORY_SIZE = 5;
    private AgentBehaviour _currentBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        _currentBehaviour = new WalkBehaviour(this, GameObject.Find("Stable").transform);
        _currentBehaviour.DoBehaviour();
        Memory = new InformationManager(MEMORY_SIZE);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(GetComponentInChildren<NavMeshAgent>().velocity.normalized);
    }
}
