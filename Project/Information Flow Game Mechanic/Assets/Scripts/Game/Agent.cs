using System;
using System.Collections.Generic;
using System.Linq;
using Information_Flow;
using NPC_Behaviour;
using UnityEngine;

namespace Game
{
    public class Agent : WorldObject
    {
        public InformationManager Memory;
        public List<Item> Inventory;
        public Dictionary<Agent, float> Acquaintances;
        [SerializeField]
        public List<Agent> ImportantPeople;
        public List<Quest> Quests;
        public YarnProgram YarnScript;
        public string YarnNode;
        public bool IsHearing = true;
        public bool IsSeeing = true;
        public bool isHurt = false;
        public bool isDead = false;
        public float BelievabilityThreshold;
        public List<Information> CurrentReplies;
        public bool IsOccupied = false;
        public Item EquippedItem;
        public Transform EquipPosition;

        protected override void Awake()
        {
            base.Awake();
        }
    
        protected override void Start()
        {
            base.Start();
            if (YarnScript != null)
                GameManager.Instance.DialogueManager.AddYarnFile(YarnScript);
            Acquaintances = new Dictionary<Agent, float>();
        
            ImportantPeople.ForEach(p => Acquaintances.Add(p, 1.0f));

            Quests = new List<Quest>();
            CurrentReplies = new List<Information>();

            if(Inventory == null)
                Inventory = new List<Item>();
            
            if (StateInfos == null)
                StateInfos = new List<InformationEntry>();
            
            transform.Find("Inventory").GetComponentsInChildren<Item>().ToList().ForEach(i => PickUpItem(i));
        
            Inventory.Where(i=>i.IsVisible).ToList().ForEach(i =>
            {
                StateInfos.Add(new InformationEntry(
                    this, InformationVerb.Has, i, 0, null, false));
                if(i.gameObject.CompareTag("Weapon"))
                    StateInfos.Add(new InformationEntry(
                            this, InformationVerb.Is, i, Adjectives.Armed, null, false));
            });
        }

        protected override void Update()
        {
            base.Update();
        }
    
        public void InteractNPC(Agent interactAgent)
        {
            IsOccupied = true;
            interactAgent.IsOccupied = true;
            GameManager.Instance.DialogueManager.StartDialogue(this, interactAgent);
        }
        
        public void AttackNPC(NPC interactNpc)
        {
            interactNpc.Hit();
        }

        public void PickUpItem(Item interactItem)
        {
            IsOccupied = true;
            Inventory.Add(interactItem);
            interactItem.PickUp(this);
            GameManager.Instance.CreateVisibleInformation(new Information(this, interactItem), transform.position);
            IsOccupied = false;
        }

        public void DropItem(Item interactItem)
        {
            IsOccupied = true;
            Inventory.Remove(interactItem);
            interactItem.Drop();
            GameManager.Instance.CreateVisibleInformation(
                new Information(new Information(this, interactItem),true), transform.position);
            IsOccupied = false;
        }

        void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            if (other.gameObject.TryGetComponent<InformationObject>(out var infoObject))
            {
                switch (infoObject.PropagationType)
                {
                    case InformationPropagationType.Visual:
                    {
                        if (IsSeeing)
                            Memory.TryAddNewInformation(infoObject.Information, this);
                    }
                        break;
                    case InformationPropagationType.Audio:
                    {
                        if (IsHearing)
                            Memory.TryAddNewInformation(infoObject.Information, this);
                    }
                        break;
                    case InformationPropagationType.Conversation:
                    {
                        // TODO change to speculative
                        if (IsHearing)
                            Memory.TryAddNewInformation(infoObject.Information, infoObject.Sender);
                    }
                        break;
                    case InformationPropagationType.Instant:
                        Memory.TryAddNewInformation(infoObject.Information, this);
                        break;
                    case InformationPropagationType.Persistant:
                        Memory.TryAddNewInformation(infoObject.Information, null);
                        break;
                    case InformationPropagationType.None:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            /*
            if(isInformationAdded)
                Debug.Log(this.Name + " learned: \""+infoObject.Information.ToString()+"\"");
            */
        }

        public override bool Equals(object o)
        {
            if (!(o is Agent other))
                return false;

            return gameObject.Equals(other.gameObject);
        }

        public override int GetHashCode()
            => gameObject.GetHashCode();

        public void EquipItem(Item item)
        {
            if (EquipPosition != null)
            {
                EquippedItem = item;
                item.GetComponentInChildren<MeshRenderer>().enabled = true;
                item.transform.position = EquipPosition.position;
                item.transform.rotation = Quaternion.identity;
                item.transform.parent = EquipPosition;
                GetComponentInChildren<SphereCollider>().enabled = false;
                item.GetComponentInChildren<Renderer>().enabled = true;
            }
        }
    }
}