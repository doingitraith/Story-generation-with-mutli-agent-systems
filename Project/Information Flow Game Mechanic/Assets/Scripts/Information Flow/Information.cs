using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InformationVerb
{
    NULL = -1,
    IS = 0,
    HAS,
    AT
}

public class Information
{
    public WorldObject Subject { get; }
    public InformationVerb Verb { get; }
    public Item Object { get; }
    public InformationAdjective Adjective { get; }
    public InformationLocation Location { get; }

    public Information()
        => (Subject, Verb, Object, Adjective, Location) =
            (null, InformationVerb.NULL, null, null, null);

    /// <summary>
    /// Creates an Information of the form "Subject HAS Object" 
    /// </summary>
    /// <param name="character">Subject of the information</param>
    /// <param name="object">Object of the information</param>
    public Information(Character character, Item @object)
        => (Subject, Verb, Object, Adjective, Location) =
            (character, InformationVerb.HAS, @object, null, null);

    /// <summary>
    /// Creates an Information of the form "Subject IS Adjective" 
    /// </summary>
    /// <param name="character">Subject of the information</param>
    /// <param name="informationAdjective">Property of the subject</param>
    public Information(WorldObject subject, InformationAdjective informationAdjective)
        => (Subject, Verb, Object, Adjective, Location) =
            (subject, InformationVerb.IS, null, informationAdjective, null);

    /// <summary>
    /// Creates an Information of the form "Subject is AT Location" 
    /// </summary>
    /// <param name="character">Subject of the information</param>
    /// <param name="informationLocation">Location of the subject</param>
    public Information(WorldObject subject, InformationLocation informationLocation)
        => (Subject, Verb, Object, Adjective, Location) =
            (subject, InformationVerb.AT, null, null, informationLocation);
}
