using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InformationSet
{
    public WorldObject Subject { get; set; }
    public InformationLocation Location { get; set; }
    
    public List<InformationAdjective> Properties { get; set; }

    public abstract bool Contains(Information information);
    public abstract void UpdateInformationSet(Information information);
    
    protected void UpdateProperties(Information information)
    {
        // TODO check for contradictions
        throw new NotImplementedException();
        
        Properties.Add(information.Adjective);
    }
}