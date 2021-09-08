using System;
using System.Collections.Generic;
using Game;
using Information_Flow;
using UnityEngine;
using UnityEngine.AI;

namespace NPC_Behaviour
{
    public class NPC : Agent
    {
        public float MemoryMutationInterval = 30.0f;
        [SerializeField]
        private List<BehaviourEntry> Routine;
        private const int MEMORY_SIZE = 5;
        private AgentBehaviour _currentBehaviour;
        private float _currentMutationTime = .0f;
        private int _currentBehaviourIdx = -1;

        protected override void Awake()
        {
            base.Awake();
        }
    
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            Memory = new InformationManager(this);
        
            _currentBehaviour = SelectNextBehaviour();
            StartCoroutine(_currentBehaviour.DoBehaviour());
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
            if (isDead)
                return;
            
            base.Update();
            _currentMutationTime += Time.deltaTime;
            if (_currentMutationTime > MemoryMutationInterval)
            {
                Memory.MutateMemory();
                _currentMutationTime = .0f;
            }

            if (_currentBehaviour.IsBehaviourFinished())
            {
                _currentBehaviour = SelectNextBehaviour();
                StartCoroutine(_currentBehaviour.DoBehaviour());
            }
            else if(_currentBehaviour is WalkBehaviour && !_currentBehaviour.IsPaused)
                ((WalkBehaviour)_currentBehaviour).UpdateTarget();
        }

        private void LateUpdate()
        {
            NavMeshAgent navMeshAgent = GetComponentInChildren<NavMeshAgent>();
            if (navMeshAgent == null)
                return;
            Vector3 dir = navMeshAgent.velocity.normalized;
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
                        this, behaviour.BehaviourObject.transform);
                case BehaviourType.Dialogue:
                    return new TalkBehaviour(
                        this, behaviour.BehaviourObject.GetComponent<Agent>());
                case BehaviourType.Exchange:
                    return new ExchangeInformationBehaviour(
                        this, behaviour.BehaviourObject.GetComponent<Agent>());
                case BehaviourType.Send:
                    return new SendInformationBehaviour(this);
                case BehaviourType.Wait:
                    return new WaitBehaviour(this, behaviour.BehaviourTime);
                case BehaviourType.PickUp:
                    return new PickUpBehaviour(this, behaviour.BehaviourObject.GetComponent<Item>());
                case BehaviourType.Drop:
                    return new DropBehaviour(this, behaviour.BehaviourObject.GetComponent<Item>());
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void InterruptNPC()
            => StartCoroutine(_currentBehaviour.InterruptBehaviour());
    
        public void ResumeNPC()
            => StartCoroutine(_currentBehaviour.ResumeBehaviour());

        private void Die()
        {
            isDead = true;
            GetComponentInChildren<Animator>().SetTrigger("DoDie");
        }

        public void Hit()
        {
            if (isHurt)
            {
                Die();
                GameManager.Instance.CreateVisibleInformation(
                    new Information(this, GameManager.Instance.WorldAdjectives[Adjectives.Dead]),
                    transform.position);
                Debug.Log(Name+" is dead");
            }
            else
            {
                isHurt = true;
                GameManager.Instance.CreateVisibleInformation(
                    new Information(this, GameManager.Instance.WorldAdjectives[Adjectives.Hurt]),
                    transform.position);
                Debug.Log(Name+" is hurt");
            }
        }
    }
}
