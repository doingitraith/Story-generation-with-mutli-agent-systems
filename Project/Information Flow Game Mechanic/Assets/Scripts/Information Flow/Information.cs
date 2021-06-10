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
    public Character Character { get; }
    public InformationVerb Verb { get; }
    public Character Object { get; }
    public InformationAdjective Adjective { get; }
    public InformationLocation Location { get; }

    public Information()
        => (Character, Verb, Object, Adjective) =
            (null, InformationVerb.NULL, null, null);

    /// <summary>
    /// Creates an Information of the form "Subject HAS Object" 
    /// </summary>
    /// <param name="character">Subject of the information</param>
    /// <param name="object">Object of the information</param>
    public Information(Character character, Character @object)
        => (Character, Verb, Object, Adjective) =
            (character, InformationVerb.HAS, @object, null);

    /// <summary>
    /// Creates an Information of the form "Subject IS Adjective" 
    /// </summary>
    /// <param name="character">Subject of the information</param>
    /// <param name="informationAdjective">Property of the subject</param>
    public Information(Character character, InformationAdjective informationAdjective)
        => (Character, Verb, Object, Adjective) =
            (character, InformationVerb.IS, null, informationAdjective);

    /// <summary>
    /// Creates an Information of the form "Subject is AT Location" 
    /// </summary>
    /// <param name="character">Subject of the information</param>
    /// <param name="informationLocation">Location of the subject</param>
    public Information(Character character, InformationLocation informationLocation)
        => (Character, Verb, Object, Adjective, Location) =
            (character, InformationVerb.AT, null, null, informationLocation);
}
