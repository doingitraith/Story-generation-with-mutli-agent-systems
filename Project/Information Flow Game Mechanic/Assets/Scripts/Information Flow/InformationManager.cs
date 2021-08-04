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
    
    // TODO: Convert to List<Information>
    //private Queue<InformationSet> _memory;
    private List<Information> _memory;
    private Agent _owner;

    public InformationManager(Agent owner)
    {
        _owner = owner;
        _memory = new List<Information>();
    }
    
    public InformationManager(Agent owner, int memorySize)
    {
        _owner = owner;
        _memory = new List<Information>(memorySize);
    }

    private bool Contains(Information information)
    {
        return _memory.Contains(information);
    }

    /*
    private InformationSet GetInformationSet(WorldObject subject)
    {
        foreach (InformationSet infoSet in _memory)
        {
            if (infoSet.Subject.Equals(subject))
                return infoSet;
        }

        return null;
    }
    */
    public bool TryAddNewInformation(Information information)
    {
        if (Contains(information))
            return false;

        //InformationSet infoSet = GetInformationSet(information.Subject);

        //bool isPosession = information.Verb == InformationVerb.HAS;
        
        List<Information> filteredInfos = _memory.FindAll(i=>i.Verb.Equals(information.Verb));

        switch (information.Verb)
        {
            case InformationVerb.IS:
            {
                filteredInfos = filteredInfos.FindAll(i => i.Subject.Equals(information.Subject));
                InformationAdjective adjective = information.Adjective;
                
                List<Information> infosToRemove = new List<Information>();

                foreach (Information adjInfo in filteredInfos)
                {
                    List<InformationAdjective> cons = adjInfo.Adjective.Contradictions;
                    if (cons.Contains(adjective))
                        infosToRemove.Add(adjInfo);
                }
                _memory = _memory.Except(infosToRemove).ToList();
                _memory.Add(new Information(information));
            }
                break;
            case InformationVerb.HAS:
            {
                filteredInfos = filteredInfos.FindAll(i => i.Object.Equals(information.Object));
                switch (filteredInfos.Count)
                {
                    case 0: _memory.Add(new Information(information));
                        break;
                    case 1: filteredInfos[0]=new Information(information);
                        break;
                    default: throw new Exception("An Item cannot have more than one Owner: "
                                                 +information.Object.ToString());
                }
            }
                break;
            case InformationVerb.AT:
            {
                filteredInfos = filteredInfos.FindAll(i => i.Subject.Equals(information.Subject));
                switch (filteredInfos.Count)
                {
                    case 0: _memory.Add(new Information(information));
                        break;
                    case 1: filteredInfos[0]=new Information(information);
                        break;
                    default: throw new Exception("An Agent cannot be in more than one places: "
                                                 +information.Subject.ToString());
                }
            }
                break;
            case InformationVerb.NULL:
            default:
                throw new ArgumentOutOfRangeException();
        }

        return true;
        /*
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
        */

    }

    public List<Information> GetInformationsToExchange(int numberOfInfos)
    {
        if (NumberOfMemories < numberOfInfos)
            return null;
        
        List<Information> shuffleList = new List<Information>(GetInformations());
        //Shuffle List
        shuffleList.OrderBy(x => Random.value).ToList();

        return shuffleList.Take(numberOfInfos).ToList();
    }

    /*
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
    */
    
    private List<Information> GetInformations()
    {
        /*
        if (_memory.Count == 0)
            return new List<Information>();

        List<Information> informations = new List<Information>();
        
        List<InformationSet> infoSetList = _memory.ToList();
        foreach (InformationSet infoSet in infoSetList)
            informations.AddRange(infoSet.GetInformationList());

        return informations;
        */
        return _memory;
    }
}
