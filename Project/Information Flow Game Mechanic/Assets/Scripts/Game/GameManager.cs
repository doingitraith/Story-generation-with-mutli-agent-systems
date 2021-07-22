using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using Random = UnityEngine.Random;

public  class GameManager : MonoBehaviour
{
    public Dictionary<Adjectives, InformationAdjective> WorldAdjectives;
    public DialogueRunner DialogueRunner;
    public Text NameText;
    private Agent _currentConversationStarter;
    private Agent _currentConversationPartner;

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
        NameText.text = conversationStarter is Player ? conversationPartner.Name : conversationStarter.Name;
        DialogueRunner.variableStorage.SetValue("$NPCName", NameText.text);

        List<Information> npcInfos = conversationPartner.Memory.GetInformationsToExchange(1);
        
        DialogueRunner.variableStorage.SetValue("$HasNPCReplies", npcInfos!=null);
        if(npcInfos!=null)
            DialogueRunner.variableStorage.SetValue("$NPCReplyText", npcInfos[0].ToString());

        int numOfReplies = conversationStarter.Memory.NumberOfMemories;
        DialogueRunner.variableStorage.SetValue("$NumOfReplies", numOfReplies);
        if (numOfReplies > 0)
        {
            List<Information> playerInfos =
                conversationStarter.Memory.GetInformationsToExchange(numOfReplies > 2 ? 3 : numOfReplies > 1 ? 2 : 1); 
            
            DialogueRunner.variableStorage.SetValue("$ReplyText1", playerInfos[0].ToString());
            if(numOfReplies > 1)
                DialogueRunner.variableStorage.SetValue("$ReplyText2", playerInfos[1].ToString());
            if(numOfReplies > 2)
                DialogueRunner.variableStorage.SetValue("$ReplyText3", playerInfos[2].ToString());
        }
        
        DialogueRunner.StartDialogue(conversationStarter.YarnNode);
    }

    public void ReceiveReply(string[] parameters)
    {
        if(parameters[0]=="Player")
            Debug.Log("Player learns \""+parameters[1]+"\"");
        else
            Debug.Log(parameters[0]+" learns \""+parameters[1]+"\"");
    }
}
