using System;
using System.Diagnostics;
using UnityEngine;

namespace Game
{
    public class InformationObject : MonoBehaviour
    {
        public WorldObject Subject;
        public InformationVerb Verb;
        public Item Object;
        public InformationAdjective Adjective;
        public InformationLocation Location;
        
        public Information Information { get; set; }

        public void Start()
        {
            switch (Verb)
            {
                case InformationVerb.NULL: { Information = new Information(); }
                    break;
                case InformationVerb.IS: { Information = new Information(Subject, Adjective); }
                    break;
                case InformationVerb.HAS:{ Information = new Information((Agent)Subject, Object); }
                    break;
                case InformationVerb.AT:{ Information = new Information(Subject, Location); }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Update()
        {
            
        }
    }
}