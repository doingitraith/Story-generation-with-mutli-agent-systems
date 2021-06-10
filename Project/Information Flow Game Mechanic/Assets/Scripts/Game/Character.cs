using System;
using System.Collections.Generic;
using UnityEngine;

public class Character : WorldObject
{
    public string Name;
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
}