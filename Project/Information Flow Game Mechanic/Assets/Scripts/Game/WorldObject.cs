using System;
using System.Collections.Generic;
using System.Linq;
using Information_Flow;
using UnityEngine;

namespace Game
{
    public abstract class WorldObject : MonoBehaviour
    {
        public string Name;
        public bool IsPerson;
        public bool IsUniqe;
        public int WorldImportance;
        public Mutation Mutation;
        public List<InformationEntry> StateInfos;
        [SerializeField]
        private List<string> Mutations;

        public InformationSubject InformationSubject;
        public InformationLocation Location;

        protected virtual void Awake()
        {

            for (int i = Mutations.Count-1; i >= 0; i--)
                Mutation = new Mutation(Mutations[i], Mutation);

            InformationSubject = new InformationSubject(Name, IsPerson, IsUniqe, Mutation);
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            Location location = GameObject.Find("Locations").GetComponent<Location>();
            Location = location.InformationLocation;
            StateInfos.Add(new InformationEntry(
                this, InformationVerb.At, null,0,location,false));
        }

        // Update is called once per frame
        protected virtual void Update()
        {
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent<Location>(out var infoLocation))
            {
                GameManager.Instance.CreateArrivalInformation(this, infoLocation);
                Location = infoLocation.InformationLocation;
                InformationEntry atInfo = StateInfos.First(i 
                    => i.GetInformation().Verb.Equals(InformationVerb.At));
                atInfo = new InformationEntry(this, InformationVerb.At, null,
                    0, infoLocation, false);

            }
        }
    }
}
