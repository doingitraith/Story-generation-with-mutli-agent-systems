using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    public string _NPCName;
    private AgentBehaviour _currentBehaviour;
    private LinkedList<AgentBehaviour> _routine;
    
    // Start is called before the first frame update
    void Start()
    {
        _currentBehaviour = new WalkBehaviour(this, GameObject.Find("Stable").transform);
        _currentBehaviour.DoBehaviour();
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
