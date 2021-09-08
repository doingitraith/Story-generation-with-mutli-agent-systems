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
        public DialogueRunner DialogueRunner;
        public Text NameText;
        private Agent _currentConversationStarter;
        private Agent _currentConversationPartner;
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
            DialogueRunner.AddCommandHandler("ReceiveReply", ReceiveReply);
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

        public void StartDialogue(Agent conversationStarter, Agent conversationPartner)
        {
            _currentConversationStarter = conversationStarter;
            _currentConversationPartner = conversationPartner;
            
            _currentConversationStarter.Memory.TryAddNewInformation(
                new Information(_currentConversationPartner, _currentConversationPartner.Location),
                _currentConversationStarter);
            
            _currentConversationPartner.Memory.TryAddNewInformation(
                new Information(_currentConversationStarter, _currentConversationStarter.Location),
                _currentConversationPartner);
            
            
            if (_currentConversationStarter is Player)
            {
                NameText.text = _currentConversationPartner.Name;
                (_currentConversationPartner as NPC)?.InterruptNPC();
            }
            else
            {
                NameText.text = _currentConversationStarter.name;
                (_currentConversationStarter as NPC)?.InterruptNPC();
            }
            
            _currentConversationPartner.IsOccupied = true;
            _currentConversationStarter.IsOccupied = true;
            
            DialogueRunner.variableStorage.SetValue("$NPCName", NameText.text);
        
            _currentConversationPartner.CurrentReplies = _currentConversationPartner.Memory.
                    GetInformationToExchange(1, _currentConversationStarter.InformationSubject).ToList();

            DialogueRunner.variableStorage.SetValue("$HasNPCReplies", 
                _currentConversationPartner.CurrentReplies.Count!=0);
        
            if(_currentConversationPartner.CurrentReplies.Count!=0)
                DialogueRunner.variableStorage.SetValue("$NPCReplyText",
                    _currentConversationPartner.CurrentReplies[0].ToString());

            int numOfReplies = _currentConversationStarter.Memory.NumberOfMemories;
            _currentConversationStarter.CurrentReplies = _currentConversationStarter.Memory.
                    GetInformationToExchange(numOfReplies > 2 ? 3 : numOfReplies > 1 ? 2 : 1,
                        _currentConversationPartner.InformationSubject).ToList();
            
            numOfReplies = _currentConversationStarter.CurrentReplies.Count;
            DialogueRunner.variableStorage.SetValue("$NumOfReplies", numOfReplies);
            if (numOfReplies > 0)
            {
                DialogueRunner.variableStorage.SetValue("$ReplyText1", 
                    _currentConversationStarter.CurrentReplies[0].ToString());
                if(numOfReplies > 1)
                    DialogueRunner.variableStorage.SetValue("$ReplyText2", 
                        _currentConversationStarter.CurrentReplies[1].ToString());
                if(numOfReplies > 2)
                    DialogueRunner.variableStorage.SetValue("$ReplyText3", 
                        _currentConversationStarter.CurrentReplies[2].ToString());
            }
        
            DialogueRunner.StartDialogue(_currentConversationStarter.YarnNode);
        }

        public void EndDialogue()
        {
            NPC npc = null;
            if (_currentConversationStarter is Player)
                npc = _currentConversationPartner as NPC;
            else
                npc = _currentConversationStarter as NPC;
            
            _currentConversationStarter.IsOccupied = false;
            _currentConversationPartner.IsOccupied = false;

            npc.ResumeNPC();
        }

        public void ReceiveReply(string[] parameters)
        {
            var replyIdx = Int32.Parse(parameters[parameters.Length-1]);

            Agent player = null;
            Agent npc = null;
            if (_currentConversationStarter is Player)
            {
                player = _currentConversationStarter;
                npc = _currentConversationPartner;
            }
            else
            {
                player = _currentConversationPartner;
                npc = _currentConversationStarter;
            }

            if (parameters[0] == "Player")
            {
                player.Memory.TryAddNewSpeculativeInformation(npc.CurrentReplies[replyIdx], npc);
                //Debug.Log("Player learns \"" + npc.CurrentReplies[replyIdx].ToString() + "\"");
            }
            else
            {
                npc.Memory.TryAddNewSpeculativeInformation(player.CurrentReplies[replyIdx], player);
                //Debug.Log(parameters[0] + " learns \"" + player.CurrentReplies[replyIdx].ToString() + "\"");
            }
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
