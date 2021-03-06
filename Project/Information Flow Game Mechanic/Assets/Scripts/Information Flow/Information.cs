using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Game;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Information_Flow
{
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
        
        public readonly bool Not = false;

        public Information()
            => (Subject, Verb, Object, Adjective, Location, Not) =
                (null, InformationVerb.Null, null, null, null, false);

        /// <summary>
        /// Creates an Information of the form "Subject HAS Object" 
        /// </summary>
        /// <param name="agent">Subject of the information</param>
        /// <param name="object">Object of the information</param>
        public Information(Agent agent, Item @object)
            => (Subject, Verb, Object, Adjective, Location, Not) =
                (agent.InformationSubject, InformationVerb.Has, @object.InformationSubject, null, null, false);

        /// <summary>
        /// Creates an Information of the form "Subject IS Adjective" 
        /// </summary>
        /// <param name="subject">Subject of the information</param>
        /// <param name="informationAdjective">Property of the subject</param>
        public Information(WorldObject subject, InformationAdjective informationAdjective)
            => (Subject, Verb, Object, Adjective, Location, Not) =
                (subject.InformationSubject, InformationVerb.Is, null, informationAdjective, null, false);

        /// <summary>
        /// Creates an Information of the form "Subject is AT Location" 
        /// </summary>
        /// <param name="subject">Subject of the information</param>
        /// <param name="informationLocation">Location of the subject</param>
        public Information(WorldObject subject, Location informationLocation)
            => (Subject, Verb, Object, Adjective, Location, Not) =
                (subject.InformationSubject, InformationVerb.At, null, null, informationLocation.InformationLocation, false);
        
        /// <summary>
        /// Creates an Information of the form "Subject is AT Location"
        /// </summary>
        /// <param name="subject">Subject of the information</param>
        /// <param name="informationLocation">InformationLocation of the subject</param>
        public Information(WorldObject subject, InformationLocation informationLocation)
            => (Subject, Verb, Object, Adjective, Location, Not) =
                (subject.InformationSubject, InformationVerb.At, null, null, informationLocation, false);

        /// <summary>
        /// Creates a copy of an Information
        /// </summary>
        /// <param name="information">Information to copy</param>
        public Information(Information information)
        {

            Subject = new InformationSubject(information.Subject.Name, information.Subject.IsPerson,
                information.Subject.IsUnique, information.Subject.Mutation);
            Verb = information.Verb;
            if (information.Object != null)
            {
                Object = new InformationSubject(information.Object.Name, information.Object.IsPerson,
                    information.Object.IsUnique, information.Object.Mutation);
            }
            if (information.Adjective != null)
            {
                if (information.Adjective is InformationProperty)
                    Adjective = new InformationProperty(information.Adjective.Characteristic,
                        information.Adjective.Contradictions);
                else
                    Adjective = new InformationOpinion(information.Adjective.Characteristic,
                        information.Adjective.Contradictions);
            }
            if (information.Location != null)
            {
                Location = new InformationLocation(information.Location.Name, information.Location.Location,
                    information.Location.Mutation);
            }
        }

        /// <summary>
        /// Creates the negation of an Information
        /// </summary>
        /// <param name="information">Information to create</param>
        /// <param name="isNot">negation term, set true to negate</param>
        public Information(Information information, bool isNot)
            => (Subject, Verb, Object, Adjective, Location, Not) =
                (information.Subject, information.Verb, information.Object, information.Adjective, information.Location,
                    isNot);

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

            if (other.Verb != Verb || other.Not != Not || (other.Verb == Verb && !Subject.Equals(other.Subject)))
                return false;
            switch (Verb)
            {
                case InformationVerb.At:
                {
                    return Location.Equals(other.Location);
                }
                case InformationVerb.Is:
                {
                    return Adjective.Equals(other.Adjective);
                }
                case InformationVerb.Has:
                {
                    return Object.Equals(other.Object);
                }
                default:
                {
                    return false;
                }
            }
        }

        public bool EqualsExceptForSubject(Information other)
        {
            if (other.Verb != Verb || other.Not != Not)
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
            //return ToString().GetHashCode();
            
            return Verb.GetHashCode() * (Subject != null ? Subject.GetHashCode() : 1)
                                      * (Object != null ? Object.GetHashCode() : 1)
                                      * (Adjective != null ? Adjective.GetHashCode() : 1)
                                      * (Location != null ? Location.GetHashCode() : 1)
                                      * (Not.GetHashCode());
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
                case InformationVerb.Is:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
    }
}