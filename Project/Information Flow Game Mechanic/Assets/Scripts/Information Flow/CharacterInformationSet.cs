using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInformationSet : InformationSet
{
    public List<Item> Possessions { get; set; }

    public CharacterInformationSet(Information information)
    {
        Subject = information.Subject;
        Possessions = new List<Item>();
        Properties = new List<InformationAdjective>();

        UpdateInformationSet(information);
    }
    
    public override void UpdateInformationSet(Information information)
    {
        switch (information.Verb)
        {
            case InformationVerb.IS: {UpdateProperties(information);}
                break;
            case InformationVerb.HAS: {Possessions.Add(information.Object);}
                break;
            case InformationVerb.AT: { Location = information.Location;}
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public override bool Contains(Information information)
    {
        switch (information.Verb)
        {
            case InformationVerb.AT:
                return information.Subject.Equals(Subject)
                       && information.Location.Equals(Location);
            case InformationVerb.IS:
                return information.Subject.Equals(Subject)
                       && Properties.Contains(information.Adjective);
            case InformationVerb.HAS:
                return information.Subject.Equals(Subject)
                       && Possessions.Contains(information.Object);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

}
