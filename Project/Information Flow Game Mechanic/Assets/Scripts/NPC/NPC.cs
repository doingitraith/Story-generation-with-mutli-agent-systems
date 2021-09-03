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
    [SerializeField]
    private List<BehaviourEntry> Routine;
    private const int MEMORY_SIZE = 5;
    private AgentBehaviour _currentBehaviour;
    private float _currentMutationTime = .0f;
    private int _currentBehaviourIdx = 0;

    protected override void Awake()
    {
        base.Awake();
    }
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Memory = new InformationManager(this, MEMORY_SIZE);
        
        _currentBehaviour = SelectNextBehaviour();
        _currentBehaviour.DoBehaviour();
        /*
        Memory.TryAddNewInformation(new Information(FindObjectOfType<Player>().GetComponent<Player>(),
            GameManager.Instance.WorldAdjectives[Adjectives.ALIVE]),this);
        Memory.TryAddNewInformation(new Information(FindObjectOfType<Player>().GetComponent<Player>(),
            GameManager.Instance.WorldAdjectives[Adjectives.evil]),this);
        */
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

        if (_currentBehaviour.IsFinished)
        {
            _currentBehaviour = SelectNextBehaviour();
            _currentBehaviour.DoBehaviour();
        }
    }

    private void LateUpdate()
    {
        Vector3 dir = GetComponentInChildren<NavMeshAgent>().velocity.normalized;
        if(dir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(dir);
    }

    private AgentBehaviour SelectNextBehaviour()
    {
        BehaviourEntry behaviour = Routine[++_currentBehaviourIdx % Routine.Count];
        switch (behaviour.Type)
            {
                case BehaviourType.Walk:
                    return new WalkBehaviour(
                        this, behaviour.BehaviourObject.GetComponent<Location>().LocationTransform);
                case BehaviourType.Dialogue:
                    return new TalkBehaviour(
                        this, behaviour.BehaviourObject.GetComponent<Agent>());
                case BehaviourType.Exchange:
                    return new ExchangeInformationBehaviour(
                        this, behaviour.BehaviourObject.GetComponent<Agent>());
                case BehaviourType.Send:
                    return new SendInformationBehaviour(this);
                default:
                    throw new ArgumentOutOfRangeException();
            }
    }

    public void InterruptNPC()
        => _currentBehaviour.InterruptBehaviour();
    
    public void ResumeNPC()
        => _currentBehaviour.ResumeBehaviour();
}
