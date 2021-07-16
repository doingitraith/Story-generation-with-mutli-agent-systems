using System;
using System.Collections.Generic;
using UnityEngine;

public class Agent : WorldObject
{
    public InformationManager Memory;
    public List<Item> Inventory;
    public List<NPC> Acquaintances;
    public bool IsHearing = true;
    public bool IsSeeing = true;

    protected override void Start()
    {
        base.Start();
        Inventory = new List<Item>();
        Acquaintances = new List<NPC>();
    }

    protected override void Update()
    {
        base.Update();
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
            Debug.Log(this.Name + "learned: \""+infoObject.Information.ToString()+"\"");
    }
}