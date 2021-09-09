using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Information_Flow;
using NPC_Behaviour;
using Player_Behaviour;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using Random = UnityEngine.Random;

namespace Game
{
    public  class GameManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject InformationPrefab;
        [SerializeField]
        private float MapReduceWaitSeconds;
        [SerializeField]
        private float InferenceWaitSeconds;
        [SerializeField] private bool IsMouseLocked;
        public Dictionary<Adjectives, InformationAdjective> WorldAdjectives;
        public List<InferenceRule> WorldRules;
        public Quest Quest;
        public DialogueManager DialogueManager;
        private IEnumerator _updateBelievability;
        private IEnumerator _inference;
        private Player _player;

        private static GameManager instance;

        public static GameManager Instance
        {
            get
            {
                if (instance == null)
                    instance = FindObjectOfType<GameManager>();

                return instance;
            }
        }
    
        private void Awake()
        {
            Random.InitState(15102021);
            
            if (IsMouseLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            _player = FindObjectOfType<Player>();
            WorldAdjectives = new Dictionary<Adjectives, InformationAdjective>();
            InitAdjectives();
            WorldRules = new List<InferenceRule>();
            InitRules();
            InitQuest();
        }

        public void Start()
        {
            _updateBelievability = UpdateBelievability();
            _inference = Inference();
            StartCoroutine(_updateBelievability);
            StartCoroutine(_inference);
        }

        public void Update()
        {
            if(_player.Quests.Any(q=>q.IsQuestFinished()))
                Debug.Log("Quest finished");
        }

        private void OnApplicationQuit()
        {
            StopCoroutine(_updateBelievability);
            StopCoroutine(_inference);
        }

        private IEnumerator UpdateBelievability()
        {
            while (true)
            {
                List<Agent> agents = FindObjectsOfType<Agent>().ToList();
                foreach (var agent in agents)
                {
                    agent.Memory.UpdateBelievability();   
                
                    yield return new WaitForSeconds(MapReduceWaitSeconds);
                }
            }
        }

        private IEnumerator Inference()
        {
            while (true)
            {
                List<Agent> agents = FindObjectsOfType<Agent>().ToList();
                foreach (var agent in agents)
                {
                    agent.Memory.InferenceEngine.Evaluate();
                    yield return new WaitForSeconds(InferenceWaitSeconds);
                }
            }
        }

        private void InitAdjectives()
        {
            // Add adjective Properties
            WorldAdjectives.Add(Adjectives.Alive, 
                new InformationProperty(Adjectives.Alive, new List<InformationAdjective>()));
            WorldAdjectives.Add(Adjectives.Dead, 
                new InformationProperty(Adjectives.Dead, new List<InformationAdjective>()));
            WorldAdjectives.Add(Adjectives.Hurt, 
                new InformationProperty(Adjectives.Hurt, new List<InformationAdjective>()));
            WorldAdjectives.Add(Adjectives.Armed, 
                new InformationProperty(Adjectives.Armed, new List<InformationAdjective>()));
            WorldAdjectives.Add(Adjectives.King, 
                new InformationProperty(Adjectives.King, new List<InformationAdjective>()));
        
            // Add adjective Opinions
            WorldAdjectives.Add(Adjectives.Enemy, 
                new InformationOpinion(Adjectives.Enemy, new List<InformationAdjective>()));
            WorldAdjectives.Add(Adjectives.Dangerous, 
                new InformationOpinion(Adjectives.Dangerous, new List<InformationAdjective>()));
        
            // Add contradictions
            WorldAdjectives[Adjectives.Alive].AddContradiction(WorldAdjectives[Adjectives.Dead]);
            WorldAdjectives[Adjectives.Dead].AddContradiction(WorldAdjectives[Adjectives.Alive]);
            WorldAdjectives[Adjectives.Hurt].AddContradiction(WorldAdjectives[Adjectives.Dead]);
        }

        private void InitRules()
        {
            var rule = new InferenceRule(new BoolExpression(new Information(_player, WorldAdjectives[Adjectives.Armed])))
                .And(new BoolExpression(new Information(_player, WorldAdjectives[Adjectives.Dangerous])));
            rule.Consequences = new List<Information> { new Information(_player, WorldAdjectives[Adjectives.Enemy]) };
            rule.AppliesToSelf = false;
            
            rule = new InferenceRule(new BoolExpression(new Information(_player, WorldAdjectives[Adjectives.Enemy])));
            rule.Consequences = new List<Information> { new Information(_player, WorldAdjectives[Adjectives.Dangerous]) };
            rule.AppliesToSelf = false;

            WorldRules.Add(rule);
        }

        private void InitQuest()
        {
            var rule = new InferenceRule(new BoolExpression(new Information(_player, WorldAdjectives[Adjectives.King])))
                .And(new BoolExpression(new Information(_player, GameObject.Find("Castle").GetComponent<Location>())));
            rule.AppliesToSelf = true;
            var info = new Information(GameObject.Find("Dragon").GetComponent<NPC>(), WorldAdjectives[Adjectives.Dead]);

            Quest q = new Quest(GameObject.Find("Queen").GetComponent<NPC>());
            q.GoalRules.Add(rule);
            q.GoalInfos.Add(info);

            Quest = q;
        }

        public void CreateArrivalInformation(Agent agent, Location location)
        {
            InformationObject informationObject = 
                Instantiate(InformationPrefab, agent.transform.position, Quaternion.identity).
                    GetComponent<InformationObject>();
            
            informationObject.InformationEntry = new InformationEntry(
                agent, InformationVerb.At, null, 0, location, false);
            informationObject.PropagationType = InformationPropagationType.Visual;
        }

        public void CreateConversationInformation(Information information, Vector3 position, Agent sender)
        {
            InformationObject informationObject = Instantiate(InformationPrefab, position, Quaternion.identity).
                GetComponent<InformationObject>();
            informationObject.Information = information;
            informationObject.PropagationType = InformationPropagationType.Conversation;
            informationObject.Sender = sender;
        }
    
        public void CreateVisibleInformation(Information information, Vector3 position)
        {
            InformationObject informationObject = Instantiate(InformationPrefab, position, Quaternion.identity).
                GetComponent<InformationObject>();
            informationObject.Information = information;
            informationObject.PropagationType = InformationPropagationType.Visual;
        }
    }
}
