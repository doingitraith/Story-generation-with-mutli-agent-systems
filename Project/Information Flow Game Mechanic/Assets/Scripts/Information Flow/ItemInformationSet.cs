using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ItemInformationSet : InformationSet
{
    public Agent Owner { get; set; }

    public ItemInformationSet(Information information)
    {
        if (information.Verb.Equals(InformationVerb.HAS))
        { 
            throw new Exception(
                "Should not be reached. A HAS information should only be created through a CharacterInformationSet");
            //Subject = information.Object;
        }
        else
        {
            if (information.Subject ! is Item)
                throw new Exception(
                    "ItemInformationSet that is not HAS should only be created with an Item as Subject");
            
            Subject = (Item) information.Subject;
        }

        Properties = new List<InformationAdjective>();

        UpdateInformationSet(information);
    }
    
    public override void UpdateInformationSet(Information information)
    {
        switch (information.Verb)
        {
            case InformationVerb.IS: {UpdateProperties(information);}
                break;
            case InformationVerb.HAS:
            {
                throw new Exception(
                    "Should not be reached. A HAS information should only be created through a CharacterInformationSet");
                //Owner = (Agent)information.Subject;
            }
                break;
            case InformationVerb.AT: { Location = information.Location;}
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override List<Information> GetInformationList()
    {
        List<Information> infos = new List<Information>();

        if(Location != null)
            infos.Add(new Information(Subject, Location));
        
        if(Owner != null)
            infos.Add(new Information(Owner,(Item)Subject));

            if(Properties.Count > 0)
            foreach (InformationAdjective prop in Properties)
                infos.Add(new Information(Subject, prop));
    
        return infos;
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
