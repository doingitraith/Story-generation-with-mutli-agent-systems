using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Random = UnityEngine.Random;

public enum InformationVerb
{
    Null = -1,
    Is = 0,
    Has = 1,
    At = 2
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
            (null, InformationVerb.Null, null, null, null);

    /// <summary>
    /// Creates an Information of the form "Subject HAS Object" 
    /// </summary>
    /// <param name="agent">Subject of the information</param>
    /// <param name="object">Object of the information</param>
    public Information(Agent agent, Item @object)
        => (Subject, Verb, Object, Adjective, Location) =
            (agent.InformationSubject, InformationVerb.Has, @object.InformationSubject, null, null);

    /// <summary>
    /// Creates an Information of the form "Subject IS Adjective" 
    /// </summary>
    /// <param name="character">Subject of the information</param>
    /// <param name="informationAdjective">Property of the subject</param>
    public Information(WorldObject subject, InformationAdjective informationAdjective)
        => (Subject, Verb, Object, Adjective, Location) =
            (subject.InformationSubject, InformationVerb.Is, null, informationAdjective, null);

    /// <summary>
    /// Creates an Information of the form "Subject is AT Location" 
    /// </summary>
    /// <param name="character">Subject of the information</param>
    /// <param name="informationLocation">Location of the subject</param>
    public Information(WorldObject subject, Location informationLocation)
        => (Subject, Verb, Object, Adjective, Location) =
            (subject.InformationSubject, InformationVerb.At, null, null, informationLocation.InformationLocation);

    /// <summary>
    /// Creates a copy of an Information
    /// </summary>
    /// <param name="information">Information to copy</param>
    public Information(Information information) =>
        (Subject, Verb, Object, Adjective, Location) = (information.Subject, information.Verb, information.Object,
            information.Adjective, information.Location);

    /// <summary>
    /// Changes the Subject of an Information
    /// </summary>
    /// <param name="newSubject">the new Subject of the Information</param>
    /// <param name="information">rest of the Information</param>
    public Information(InformationSubject newSubject, Information information)
    {
        (Subject, Verb, Object, Adjective, Location) = (newSubject, information.Verb, information.Object,
            information.Adjective, information.Location);
    }

    public override bool Equals(object o)
    {
        if (!(o is Information other))
            return false;
        
        if (other.Verb != Verb || (other.Verb == Verb && !Subject.Equals(other.Subject)))
            return false;

        switch (Verb)
        {
            case InformationVerb.At: { return Location.Equals(other.Location); }
            case InformationVerb.Is: { return Adjective.Equals(other.Adjective); }
            case InformationVerb.Has: { return Object.Equals(other.Object); }
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
            case InformationVerb.Null:
                return "NULL Information";
            case InformationVerb.Is:
                return Subject.Name + " is " + Adjective.Characteristic.ToString().ToLower();
            case InformationVerb.Has:
                return Subject.Name + " has " + Object.Name;
            case InformationVerb.At:
                return Subject.Name + " is at " + Location.Name;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Mutate()
    {
        switch (Verb)
        {
            case InformationVerb.Has:
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
            case InformationVerb.At:
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
