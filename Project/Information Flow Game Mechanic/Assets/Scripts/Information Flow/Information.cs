using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Random = UnityEngine.Random;

public enum InformationVerb
{
    NULL = -1,
    IS = 0,
    HAS,
    AT
}

public class Information : IMutatable
{
    public InformationSubject Subject { get; }
    public InformationVerb Verb { get; }
    public InformationSubject Object { get; }
    public InformationAdjective Adjective { get; }
    public InformationLocation Location { get; }

    public Information()
        => (Subject, Verb, Object, Adjective, Location) =
            (null, InformationVerb.NULL, null, null, null);

    /// <summary>
    /// Creates an Information of the form "Subject HAS Object" 
    /// </summary>
    /// <param name="agent">Subject of the information</param>
    /// <param name="object">Object of the information</param>
    public Information(Agent agent, Item @object)
        => (Subject, Verb, Object, Adjective, Location) =
            (agent.InformationSubject, InformationVerb.HAS, @object.InformationSubject, null, null);

    /// <summary>
    /// Creates an Information of the form "Subject IS Adjective" 
    /// </summary>
    /// <param name="character">Subject of the information</param>
    /// <param name="informationAdjective">Property of the subject</param>
    public Information(WorldObject subject, InformationAdjective informationAdjective)
        => (Subject, Verb, Object, Adjective, Location) =
            (subject.InformationSubject, InformationVerb.IS, null, informationAdjective, null);

    /// <summary>
    /// Creates an Information of the form "Subject is AT Location" 
    /// </summary>
    /// <param name="character">Subject of the information</param>
    /// <param name="informationLocation">Location of the subject</param>
    public Information(WorldObject subject, InformationLocation informationLocation)
        => (Subject, Verb, Object, Adjective, Location) =
            (subject.InformationSubject, InformationVerb.AT, null, null, informationLocation);

    /// <summary>
    /// Creates a copy of an Information
    /// </summary>
    /// <param name="information">Information to copy</param>
    public Information(Information information) =>
        (Subject, Verb, Object, Adjective, Location) = (information.Subject, information.Verb, information.Object,
            information.Adjective, information.Location);

    public bool Equals(Information other)
    {
        if (other.Verb != Verb && Subject.Equals(other.Subject))
            return false;

        switch (Verb)
        {
            case InformationVerb.AT: { return Location.Equals(other.Location); }
            case InformationVerb.IS: { return Adjective.Equals(other.Adjective); }
            case InformationVerb.HAS: { return Object.Equals(other.Object); }
            default: { return false; }
        }
    }

    public override string ToString()
    {
        switch (Verb)
        {
            case InformationVerb.NULL:
                return "NULL Information";
            case InformationVerb.IS:
                return Subject.Name + " is " + Adjective.Characteristic;
            case InformationVerb.HAS:
                return Subject.Name + " has " + Object.Name;
            case InformationVerb.AT:
                return Subject.Name + " is at " + Location.Name;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Mutate()
    {
        if (Verb == InformationVerb.AT)
        {
            if (Subject is Item)
            {
               
            }
            else
            {
                Location.Name = Location.Mutation.Value;
                Location.Mutation.Mutate();
            }
        }
        else if(Verb == InformationVerb.HAS)
        {
            
        }


    }
}
