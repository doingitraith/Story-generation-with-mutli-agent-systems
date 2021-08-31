using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using Random = UnityEngine.Random;

public  class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject InformationPrefab;
    [SerializeField]
    private float MapReduceWaitSeconds;
    public Dictionary<Adjectives, InformationAdjective> WorldAdjectives;
    public DialogueRunner DialogueRunner;
    public Text NameText;
    private Agent _currentConversationStarter;
    private Agent _currentConversationPartner;
    private IEnumerator updateBelievability;

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
        WorldAdjectives = new Dictionary<Adjectives, InformationAdjective>();
        InitAdjectives();
    }

    public void Start()
    {
        DialogueRunner.AddCommandHandler("ReceiveReply", ReceiveReply);
        updateBelievability = UpdateBelievability();
        StartCoroutine(updateBelievability);
    }
    
    private void OnApplicationQuit()
    {
        StopCoroutine(updateBelievability);
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

    public void InitAdjectives()
    {
        // Add adjective Properties
        WorldAdjectives.Add(Adjectives.alive, 
            new InformationProperty(Adjectives.alive, new List<InformationAdjective>()));
        WorldAdjectives.Add(Adjectives.dead, 
            new InformationProperty(Adjectives.dead, new List<InformationAdjective>()));
        
        // Add contradictions
        WorldAdjectives[Adjectives.alive].AddContradiction(WorldAdjectives[Adjectives.dead]);
        WorldAdjectives[Adjectives.dead].AddContradiction(WorldAdjectives[Adjectives.alive]);
    }

    public void StartDialogue(Agent conversationStarter, Agent conversationPartner)
    {
        _currentConversationStarter = conversationStarter;
        _currentConversationPartner = conversationPartner;
        NameText.text = 
            _currentConversationStarter is Player ? _currentConversationPartner.Name : _currentConversationStarter.Name;
        DialogueRunner.variableStorage.SetValue("$NPCName", NameText.text);

        _currentConversationPartner.CurrentReplies = 
            _currentConversationPartner.Memory.GetInformationsToExchange(1);
        
        DialogueRunner.variableStorage.SetValue("$HasNPCReplies", 
            _currentConversationPartner.CurrentReplies.Count!=0);
        
        if(_currentConversationPartner.CurrentReplies.Count!=0)
            DialogueRunner.variableStorage.SetValue("$NPCReplyText",
                _currentConversationPartner.CurrentReplies[0].ToString());

        int numOfReplies = _currentConversationStarter.Memory.NumberOfMemories;
        DialogueRunner.variableStorage.SetValue("$NumOfReplies", numOfReplies);
        if (numOfReplies > 0)
        {
            _currentConversationStarter.CurrentReplies =
                _currentConversationStarter.Memory.
                    GetInformationsToExchange(numOfReplies > 2 ? 3 : numOfReplies > 1 ? 2 : 1); 
            
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

    public void ReceiveReply(string[] parameters)
    {
        var replyIdx = Int32.Parse(parameters[1]);

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
            Debug.Log("Player learns \"" + npc.CurrentReplies[replyIdx].ToString() + "\"");
        }
        else
        {
            npc.Memory.TryAddNewSpeculativeInformation(player.CurrentReplies[replyIdx], player);
            Debug.Log(parameters[0] + " learns \"" + player.CurrentReplies[replyIdx].ToString() + "\"");
        }
    }

    public void CreateInformation(Agent agent, Location location)
    {
        InformationObject informationObject = InformationPrefab.GetComponent<InformationObject>();
            
        informationObject.Subject = agent;
        informationObject.Location = location;
        informationObject.Verb = InformationVerb.AT;
        informationObject.PropagationType = InformationPropagationType.VISUAL;
        Instantiate(InformationPrefab, agent.transform.position, Quaternion.identity);
    }

    public void CreateInformation(Information information, Vector3 position)
    {
        InformationObject informationObject = InformationPrefab.GetComponent<InformationObject>();
        informationObject.Information = information;
        informationObject.PropagationType = InformationPropagationType.AUDIO;

        Instantiate(InformationPrefab, position, Quaternion.identity);
    }
}
