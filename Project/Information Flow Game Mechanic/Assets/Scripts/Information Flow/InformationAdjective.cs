using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationAdjective
{
    public string Characteristic { get; private set; }

    public InformationAdjective(string characteristic)
        => Characteristic = characteristic;
    
    public bool Equals(InformationAdjective other)
    {
        return this.GetType().Equals(other.GetType()) && this.Characteristic.Equals(other.Characteristic);
    }
}

public class InformationProperty : InformationAdjective
{
    public InformationProperty(string characteristic) : base(characteristic)
    {
    }
}

public class InformationOpinion : InformationAdjective
{
    public InformationOpinion(string characteristic) : base(characteristic)
    {
    }
}
