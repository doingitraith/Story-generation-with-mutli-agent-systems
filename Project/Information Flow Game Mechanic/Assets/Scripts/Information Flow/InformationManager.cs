using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class InformationManager
{
    private Queue<InformationSet> _memory;
    private Agent _owner;

    public InformationManager(Agent owner)
    {
        _owner = owner;
        _memory = new Queue<InformationSet>();
    }
    
    public InformationManager(Agent owner, int memorySize)
    {
        _owner = owner;
        _memory = new FixedSizedQueue(memorySize);
    }

    private bool Contains(Information information)
    {
        return _memory.Any(s => s.Contains(information));
    }

    private InformationSet GetInformationSet(WorldObject subject)
    {
        foreach (InformationSet infoSet in _memory)
        {
            if (infoSet.Subject.Equals(subject))
                return infoSet;
        }

        return null;
    }

    public bool TryAddNewInformation(Information information)
    {
        if (Contains(information))
            return false;

        InformationSet infoSet = GetInformationSet(information.Subject);

        bool isPosession = information.Verb == InformationVerb.HAS;
        
        if (infoSet != null)
        {
            infoSet.UpdateInformationSet(information);
            if (isPosession)
            {
                ItemInformationSet itemInfoSet = (ItemInformationSet)GetInformationSet(information.Object);
                if (itemInfoSet != null)
                {
                    itemInfoSet.UpdateInformationSet(information);
                }
                else
                {
                    _memory.Enqueue(new ItemInformationSet(information));
                }
            }
            return true;
        }

        if (information.Subject is Agent)
        {
            _memory.Enqueue(new CharacterInformationSet(information));
            if (isPosession)
            {
                ItemInformationSet itemInfoSet = (ItemInformationSet)GetInformationSet(information.Object);
                if (itemInfoSet != null)
                {
                    itemInfoSet.UpdateInformationSet(information);
                }
                else
                {
                    _memory.Enqueue(new ItemInformationSet(information));
                }
            }
        }
        else
            _memory.Enqueue(new ItemInformationSet(information));
            
        return true;

    }

    public Information GetInformationToExchange()
    {
        return DistillInformation();
    }

    private Information DistillInformation()
    {
        if (_memory.Count == 0)
            return null;
        
        List<InformationSet> infoSetList = _memory.ToList();

        InformationSet infoSet = infoSetList[Random.Range(0, infoSetList.Count)];

        List<Information> infoList = infoSet.GetInformationList();
        
        if (infoList.Count == 0)
            return null;

        return infoList[Random.Range(0, infoList.Count)];
    }
}
