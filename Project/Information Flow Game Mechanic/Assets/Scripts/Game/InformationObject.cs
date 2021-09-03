using System;
using System.Diagnostics;
using UnityEngine;

    public enum InformationPropagationType
    {
        VISUAL = 0,
        AUDIO,
        INSTANT,
        PERSISTANT,
        NONE
    }
    
    public class InformationObject : MonoBehaviour
    {
        public InformationPropagationType PropagationType = InformationPropagationType.NONE;
        public WorldObject Subject;
        public InformationVerb Verb;
        public Item Object;
        
        public Adjectives SetAdjective;

        public InformationAdjective Adjective;
        public Location Location;
        
        public Information Information { get; set; }

        public void Start()
        {
            if (Information != null)
                return;
            switch (Verb)
            {
                case InformationVerb.Null: { Information = new Information(); }
                    break;
                case InformationVerb.Is: { Information = 
                    new Information(Subject, GameManager.Instance.WorldAdjectives[SetAdjective]); }
                    break;
                case InformationVerb.Has:{ Information = new Information((Agent)Subject, Object); }
                    break;
                case InformationVerb.At:{ Information = new Information(Subject, Location); }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Update()
        {
            
        }
    }