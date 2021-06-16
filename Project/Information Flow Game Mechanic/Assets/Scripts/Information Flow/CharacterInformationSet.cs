using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInformationSet : InformationSet
{
    public List<Item> Possessions { get; set; }
    public override bool Contains(Information information)
    {
        switch (information.Verb)
        {
            case InformationVerb.NULL:
                return false;
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
                return false;
        }
    }
}
