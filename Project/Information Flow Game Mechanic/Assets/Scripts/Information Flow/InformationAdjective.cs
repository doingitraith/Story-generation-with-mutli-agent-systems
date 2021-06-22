using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationAdjective
{
    public string Characteristic { get; }
    public List<InformationAdjective> Contradictions { get; }

    public InformationAdjective(string characteristic, List<InformationAdjective> contradictions)
        => (Characteristic, Contradictions) = (characteristic, contradictions);
    
    public bool Equals(InformationAdjective other)
    {
        return this.GetType() == other.GetType() && this.Characteristic.Equals(other.Characteristic);
    }
}

public class InformationProperty : InformationAdjective
{
    public InformationProperty(string characteristic, List<InformationAdjective> contradictions)
        : base(characteristic, contradictions)
    {
    }
}

public class InformationOpinion : InformationAdjective
{
    public InformationOpinion(string characteristic, List<InformationAdjective> contradictions)
        : base(characteristic, contradictions)
    {
    }
}
