using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class InformationSet
{
    /*
    public WorldObject Subject { get; set; }
    public InformationLocation Location { get; set; }
    public List<InformationAdjective> Properties { get; set; }

    public abstract bool Contains(Information information);
    public abstract void UpdateInformationSet(Information information);
    public abstract List<Information> GetInformationList();
    
    protected void UpdateProperties(Information information)
    {
        InformationAdjective adj = information.Adjective;

        bool isReplaced = false;

        List<InformationAdjective> propsToRemove = new List<InformationAdjective>();
        
        // Check if property to be added is a contradiction of any existing property
        foreach (var prop in Properties)
        {
            List<InformationAdjective> contradictions = prop.Contradictions;
            foreach (var con in contradictions)
            {
                if (adj.Equals(con))
                {
                    // if it is the first contradiction, replace the property
                    if (!isReplaced)
                    {
                        Properties[Properties.IndexOf(prop)] = con;
                        isReplaced = true;
                    }
                    // if already replaced, mark property for removal
                    else
                    {
                        propsToRemove.Add(prop);
                    }
                }
            }
        }

        // remove marked properties
        Properties = Properties.Except(propsToRemove).ToList();
        
        // if now contradictions found, just add property
        if(!isReplaced)
            Properties.Add(information.Adjective);
    }
    */
}