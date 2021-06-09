using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class NPCWalkTest : MonoBehaviour
{
    public Transform Goal;
       
    // Start is called before the first frame update
    void Start ()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.destination = Goal.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
