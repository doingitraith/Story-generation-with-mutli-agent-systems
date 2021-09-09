using System;
using System.Collections.Generic;
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
            Location = GameObject.Find("Locations").GetComponent<Location>().InformationLocation;
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
                this.Location = infoLocation.InformationLocation;
            }
        }
    }
}
