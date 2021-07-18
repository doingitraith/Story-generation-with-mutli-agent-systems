using System;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class Agent : WorldObject
{
    public InformationManager Memory;
    public List<Item> Inventory;
    public List<NPC> Acquaintances;
    public YarnProgram YarnScript;
    public string YarnNode;
    public bool IsHearing = true;
    public bool IsSeeing = true;

    protected override void Start()
    {
        base.Start();
        if (YarnScript != null)
            GameManager.Instance.DialogueRunner.Add(YarnScript);
        Inventory = new List<Item>();
        Acquaintances = new List<NPC>();
    }

    protected override void Update()
    {
        base.Update();
    }
    
    public void InteractNPC(Agent interactAgent)
    {
        GameManager.Instance.StartDialogue(this, interactAgent);
    }

    public void InteractItem(Item interactItem)
    {
        throw new System.NotImplementedException();
    }

    void OnTriggerEnter(Collider other)
    {
        InformationObject infoObject = null;
        InformationLocation infoLocation = null;
        bool isInformationAdded = false;
        if (other.gameObject.TryGetComponent<InformationObject>(out infoObject))
        {
            switch (infoObject.PropagationType)
            {
                case InformationPropagationType.VISUAL:
                {
                    if (IsSeeing)
                        isInformationAdded = Memory.TryAddNewInformation(infoObject.Information);
                }
                    break;
                case InformationPropagationType.AUDIO:
                {
                    if (IsHearing)
                        isInformationAdded = Memory.TryAddNewInformation(infoObject.Information);
                }
                    break;
                case InformationPropagationType.INSTANT:
                    isInformationAdded = Memory.TryAddNewInformation(infoObject.Information);
                    break;
                case InformationPropagationType.PERSISTANT:
                    isInformationAdded = Memory.TryAddNewInformation(infoObject.Information);
                    break;
                case InformationPropagationType.NONE:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        if (other.gameObject.TryGetComponent<InformationLocation>(out infoLocation))
        {
            Location = infoLocation;
            //TODO: GameManager Create Info
        }

        if(isInformationAdded)
            Debug.Log(this.Name + " learned: \""+infoObject.Information.ToString()+"\"");
    }
}