using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InformationSet
{
    public WorldObject Subject { get; set; }
    public InformationLocation Location { get; set; }
    
    public List<InformationAdjective> Properties { get; set; }

    public abstract bool Contains(Information information);
}
