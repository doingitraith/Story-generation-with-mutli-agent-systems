using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ItemInformationSet : InformationSet
{
    public Character Owner { get; set; }

    public ItemInformationSet(Information information)
    {
        if (information.Verb.Equals(InformationVerb.HAS))
            Subject = information.Object;
        else
            Subject = information.Subject;
        
        Properties = new List<InformationAdjective>();

        UpdateInformationSet(information);
    }
    
    public override void UpdateInformationSet(Information information)
    {
        switch (information.Verb)
        {
            case InformationVerb.IS: {UpdateProperties(information);}
                break;
            case InformationVerb.HAS: { Owner = (Character)information.Subject;}
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
                return information.Object.Equals(Subject)
                && Owner.Equals(information.Subject);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
