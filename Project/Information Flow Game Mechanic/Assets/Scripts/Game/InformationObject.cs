using System;
using System.Diagnostics;
using Information_Flow;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game
{
    public enum InformationPropagationType
    {
        Visual = 0,
        Audio,
        Instant,
        Persistant,
        None
    }

    [Serializable]
    public struct InformationEntry
    {
        [SerializeField]
        private WorldObject _subject;
        [SerializeField]
        private InformationVerb _verb;
        [SerializeField]
        private Item _object;
        [SerializeField]
        private Adjectives _adjective;
        [SerializeField]
        private Location _location;
        [SerializeField]
        private bool _not;

        public InformationEntry(WorldObject subject, InformationVerb verb, Item item, Adjectives adjective,
            Location location, bool isNot)
            => (this._subject, _verb, _object, _adjective, _location, _not) = (subject, verb, item, adjective, location, isNot);
    
        public Information GetInformation()
        {
            switch (_verb)
            {
                case InformationVerb.Is:
                    return new Information(new Information(_subject, GameManager.Instance.WorldAdjectives[_adjective]), _not);
                case InformationVerb.Has:
                    return new Information(new Information((Agent)_subject, _object), _not);
                case InformationVerb.At:
                    return new Information(new Information(_subject, _location), _not);
                case InformationVerb.Null:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public class InformationObject : MonoBehaviour
    {
        public InformationPropagationType PropagationType = InformationPropagationType.None;

        public InformationEntry InformationEntry;
        public Information Information { get; set; }
        public void Start()
        {
            Information ??= InformationEntry.GetInformation();
        }

        public void Update()
        {
        }
    }
}