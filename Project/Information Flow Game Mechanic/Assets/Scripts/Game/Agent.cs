using System;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class Agent : WorldObject
{
    public InformationManager Memory;
    public List<Item> Inventory;
    public List<NPC> Acquaintances;

    protected void Start()
    {
        base.Start();
        Inventory = new List<Item>();
        Acquaintances = new List<NPC>();
    }

    protected void Update()
    {
        base.Update();
    }

    void OnTriggerEnter(Collider other)
    {
        InformationObject infoObject = null;
        bool isInformationAdded = false;
        if (other.gameObject.TryGetComponent<InformationObject>(out infoObject))
            isInformationAdded = Memory.TryAddNewInformation(infoObject.Information);
        if(isInformationAdded)
            Debug.Log(this.Name + "learned: \""+infoObject.Information.ToString()+"\"");
    }
}