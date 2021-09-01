using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yarn.Unity;
using Random = UnityEngine.Random;

public class Agent : WorldObject
{
    public InformationManager Memory;
    public List<Item> Inventory;
    public Dictionary<Agent, float> Acquaintances;
    [SerializeField]
    private List<Agent> ImportantPeople;
    public List<Quest> Quests;
    public YarnProgram YarnScript;
    public string YarnNode;
    public bool IsHearing = true;
    public bool IsSeeing = true;
    public float BelievabilityThreshold;
    public List<Information> CurrentReplies;
    public bool IsOccupied = false;

    protected override void Awake()
    {
        base.Awake();
    }
    
    protected override void Start()
    {
        base.Start();
        if (YarnScript != null)
            GameManager.Instance.DialogueRunner.Add(YarnScript);
        Inventory = new List<Item>();
        Acquaintances = new Dictionary<Agent, float>();
        
        foreach (Agent agent in ImportantPeople)
            Acquaintances.Add(agent, 1.0f);

        Quests = new List<Quest>();
        CurrentReplies = new List<Information>();
    }

    protected override void Update()
    {
        base.Update();
    }
    
    public void InteractNPC(Agent interactAgent)
    {
        IsOccupied = true;
        GameManager.Instance.StartDialogue(this, interactAgent);
    }

    public void InteractItem(Item interactItem)
    {
        IsOccupied = true;
        throw new System.NotImplementedException();
    }

    void OnTriggerEnter(Collider other)
    {
        InformationObject infoObject = null;
        Location infoLocation = null;
        bool isInformationAdded = false;
        if (other.gameObject.TryGetComponent<InformationObject>(out infoObject))
        {
            switch (infoObject.PropagationType)
            {
                case InformationPropagationType.VISUAL:
                {
                    if (IsSeeing)
                        isInformationAdded = Memory.TryAddNewInformation(infoObject.Information, this);
                }
                    break;
                case InformationPropagationType.AUDIO:
                {
                    if (IsHearing)
                        isInformationAdded = Memory.TryAddNewInformation(infoObject.Information, this);
                }
                    break;
                case InformationPropagationType.INSTANT:
                    isInformationAdded = Memory.TryAddNewInformation(infoObject.Information, this);
                    break;
                case InformationPropagationType.PERSISTANT:
                    isInformationAdded = Memory.TryAddNewInformation(infoObject.Information, null);
                    break;
                case InformationPropagationType.NONE:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (other.gameObject.TryGetComponent<Location>(out infoLocation))
        {
            GameManager.Instance.CreateInformation(this, infoLocation);
        }

        if(isInformationAdded)
            Debug.Log(this.Name + " learned: \""+infoObject.Information.ToString()+"\"");
    }

    public override bool Equals(object o)
    {
        if (!(o is Agent other))
            return false;

        return gameObject.Equals(other.gameObject);
    }

    public override int GetHashCode()
        => gameObject.GetHashCode();
}