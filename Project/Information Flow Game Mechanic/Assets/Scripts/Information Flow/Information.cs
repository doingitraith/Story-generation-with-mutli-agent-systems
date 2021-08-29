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
    public Information(WorldObject subject, Location informationLocation)
        => (Subject, Verb, Object, Adjective, Location) =
            (subject.InformationSubject, InformationVerb.AT, null, null, informationLocation.InformationLocation);

    /// <summary>
    /// Creates a copy of an Information
    /// </summary>
    /// <param name="information">Information to copy</param>
    public Information(Information information) =>
        (Subject, Verb, Object, Adjective, Location) = (information.Subject, information.Verb, information.Object,
            information.Adjective, information.Location);

    public override bool Equals(object o)
    {
        if (!(o is Information other))
            return false;
        
        if (other.Verb != Verb || (other.Verb == Verb && !Subject.Equals(other.Subject)))
            return false;

        switch (Verb)
        {
            case InformationVerb.AT: { return Location.Equals(other.Location); }
            case InformationVerb.IS: { return Adjective.Equals(other.Adjective); }
            case InformationVerb.HAS: { return Object.Equals(other.Object); }
            default: { return false; }
        }
    }

    public override int GetHashCode()
    {
        return Verb.GetHashCode() * (Subject != null ? Subject.GetHashCode() : 1)
                                  * (Object != null ? Object.GetHashCode() : 1)
                                  * (Adjective != null ? Adjective.GetHashCode() : 1)
                                  * (Location != null ? Location.GetHashCode() : 1);
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
        switch (Verb)
        {
            case InformationVerb.HAS:
            {
                if (Random.value > .5f)
                {
                    if (Subject.Mutation != null)
                    {
                        Subject.Name = Subject.Mutation.Value;
                        Subject.Mutation.Mutate();
                    }
                }
                else
                {
                    if (Object.Mutation != null)
                    {
                        Object.Name = Object.Mutation.Value;
                        Object.Mutation.Mutate();
                    }
                }
            }
                break;
            case InformationVerb.AT:
            {
                /*
                Subject.Name = Subject.Mutation.Value;
                Subject.Mutation.Mutate();
                */
                if (Location.Mutation != null)
                {
                    Location.Name = Location.Mutation.Value;
                    Location.Mutation.Mutate();
                }
            }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

    }
}
