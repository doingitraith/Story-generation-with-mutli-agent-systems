using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Information_Flow
{
    public enum Adjectives
    {
        Alive = 1,
        Dead = -10,
        Hurt = -7,
        Dangerous = -8,
        Armed = -3,
        Enemy = -9
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
        {
            switch (Characteristic)
            {
                case Adjectives.Alive:
                case Adjectives.Dead:
                case Adjectives.Hurt:
                case Adjectives.Dangerous:
                case Adjectives.Armed:
                    return Characteristic.ToString();
                case Adjectives.Enemy:
                    return "an Enemy of mine";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

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
}