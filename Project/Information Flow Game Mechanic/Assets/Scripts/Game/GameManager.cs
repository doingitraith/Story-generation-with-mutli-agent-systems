using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public  class GameManager : MonoBehaviour
{
    public Dictionary<Adjectives, InformationAdjective> WorldAdjectives;
    public DialogueRunner DialogueRunner;
    public Text NameText;

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
        WorldAdjectives = new Dictionary<Adjectives, InformationAdjective>();
        InitAdjectives();
    }

    public void Start()
    {
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
        DialogueRunner.StartDialogue(conversationStarter.YarnNode);
    }
}
