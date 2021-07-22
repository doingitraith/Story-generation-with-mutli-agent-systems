using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class InformationManager
{
    public int NumberOfMemories
    {
        get { return GetInformations().Count; }
    }
    
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

    /*
    public Information GetInformationToExchange()
    {
        return DistillInformation();
    }
    */

    public List<Information> GetInformationsToExchange(int numberOfInfos)
    {
        if (NumberOfMemories < numberOfInfos)
            return null;
        
        List<Information> shuffleList = new List<Information>(GetInformations());
        //Shuffle List
        shuffleList.OrderBy(x => Guid.NewGuid()).ToList();

        return shuffleList.Take(numberOfInfos).ToList();
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

    private List<Information> GetInformations()
    {
        if (_memory.Count == 0)
            return new List<Information>();

        List<Information> informations = new List<Information>();
        
        List<InformationSet> infoSetList = _memory.ToList();
        foreach (InformationSet infoSet in infoSetList)
            informations.AddRange(infoSet.GetInformationList());

        return informations;
    }
}
