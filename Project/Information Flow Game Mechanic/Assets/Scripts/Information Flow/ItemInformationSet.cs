using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInformationSet : InformationSet
{
    public Character Owner { get; set; }

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
                return information.Object.Equals(Subject)
                && Owner.Equals(information.Subject);
            default:
                return false;
        }
    }
}
