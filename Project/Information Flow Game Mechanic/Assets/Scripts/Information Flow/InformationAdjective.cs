using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Adjectives
{
    alive = 0,
    dead = 1,
    good,
    evil
}

public abstract class InformationAdjective
{
    public List<InformationAdjective> Contradictions;
    public readonly Adjectives Characteristic;
    
    public InformationAdjective(Adjectives characteristic, List<InformationAdjective> contradictions)
        => (Characteristic, Contradictions) = (characteristic, contradictions);
    
    public bool Equals(InformationProperty other)
    {
        return this.GetType() == other.GetType() && this.Characteristic.Equals(other.Characteristic);
    }

    public void AddContradiction(InformationAdjective adjective)
    {
        Contradictions.Add(adjective);
    }

    public override string ToString()
        => Characteristic.ToString();
    
    public override bool Equals(object o)
    {
        if (!(o is InformationAdjective other))
            return false;

        return Characteristic.Equals(other.Characteristic);
    }

    public override int GetHashCode()
        => Characteristic.GetHashCode()+1;
}

public class InformationProperty : InformationAdjective
{
    public InformationProperty(Adjectives characteristic, List<InformationAdjective> contradictions)
        : base(characteristic, contradictions)
    {
    }

}

public class InformationOpinion : InformationAdjective
{
    public InformationOpinion(Adjectives characteristic, List<InformationAdjective> contradictions)
        : base(characteristic, contradictions)
    {
    }
}
