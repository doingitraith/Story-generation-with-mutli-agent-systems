using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class InformationManager
{
    public int NumberOfMemories => NumberOfStableMemories + NumberOfSpeculativeMemories;
    private int NumberOfStableMemories => _stableMemory.Count;
    private int NumberOfSpeculativeMemories => _speculativeMemory.Count;
    
    private Dictionary<Information, int> _stableMemory;
    private Dictionary<Information, int> _speculativeMemory;
    private Agent _owner;

    public InformationManager(Agent owner)
        => (_owner, _stableMemory, _speculativeMemory) = (owner, new Dictionary<Information, int>(), new Dictionary<Information, int>());

    // Create FixedSizeList
    public InformationManager(Agent owner, int memorySize)
        => (_owner, _stableMemory, _speculativeMemory) = (owner, new FixedSizeDictionary<Information, int>(memorySize), new Dictionary<Information, int>());

    public bool ContainsStableInformation(Information information)
        =>_stableMemory.ContainsKey(information);
    
    public bool ContainsSpeculativeInformation(Information information)
        =>_speculativeMemory.ContainsKey(information);

    public bool TryAddNewInformation(Information information, Agent sender)
    {
        if (!_owner.Acquaintances.ContainsKey(sender))
            _owner.Acquaintances.Add(sender, _owner.Equals(sender) ? 100.0f : 1.0f);

        if (_owner.InformationSubject.Equals(information.Subject))
            return false;

        if (ContainsStableInformation(information))
            _stableMemory[information]++;

        List<Information> filteredInfos = _stableMemory.Keys.ToList().FindAll(i=>i.Verb.Equals(information.Verb));

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
                
                foreach (Information i in infosToRemove)
                    _stableMemory.Remove(i);
                
                _stableMemory.Add(new Information(information), 1);
            }
                break;
            case InformationVerb.HAS:
            {
                filteredInfos = filteredInfos.FindAll(i => i.Object.Equals(information.Object));
                switch (filteredInfos.Count)
                {
                    case 0: _stableMemory.Add(new Information(information), 1);
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
                    case 0: _stableMemory.Add(new Information(information), 1);
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
        
        float infoHeuristic = GetInformationHeuristic(information, _stableMemory[information]);
        EvaluateInformation(information, sender, infoHeuristic);

        return true;
    }
    
    public bool TryAddNewSpeculativeInformation(Information information, Agent sender)
    {
        if (!_owner.Acquaintances.ContainsKey(sender))
            _owner.Acquaintances.Add(sender, _owner.Equals(sender) ? 100.0f : 1.0f);

        if (_owner.InformationSubject.Equals(information.Subject))
            return false;

        if (ContainsSpeculativeInformation(information))
            _speculativeMemory[information]++;

        List<Information> filteredInfos = _speculativeMemory.Keys.ToList().
            FindAll(i=>i.Verb.Equals(information.Verb));

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

                foreach (Information i in infosToRemove)
                    _speculativeMemory.Remove(i);
                
                _speculativeMemory.Add(new Information(information), 1);
            }
                break;
            case InformationVerb.HAS:
            {
                filteredInfos = filteredInfos.FindAll(i => i.Object.Equals(information.Object));
                switch (filteredInfos.Count)
                {
                    case 0: _speculativeMemory.Add(new Information(information), 1);
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
                    case 0: _speculativeMemory.Add(new Information(information), 1);
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
        
        float infoHeuristic = GetInformationHeuristic(information, _speculativeMemory[information]);
        EvaluateInformation(information, sender, infoHeuristic);

        if (infoHeuristic >= _owner.BelievabilityThreshold)
        {
            if (TryAddNewInformation(information, sender))
                _speculativeMemory.Remove(information);
        }

        return true;
    }

    private void EvaluateInformation(Information information, Agent sender, float infoHeuristic)
    {
        float trust = _owner.Acquaintances[sender];
        _owner.Acquaintances[sender] = CalculateNewTrust(infoHeuristic, trust);
    }

    private float CalculateNewTrust(float infoValue, float trust)
    {
        //TODO: Calculate new trust with infoValue 
        //throw new NotImplementedException();
        return trust;
    }

    private float GetInformationHeuristic(Information information, int numOfInformationLearned)
    {
        //TODO: Get Information value based on context
        //throw new NotImplementedException();
        return .0f;
    }
    
    public List<Information> GetInformationsToExchange(int numberOfInfos)
    {
        List<Information> allMemories = new List<Information>(_stableMemory.Keys.ToList());
        allMemories.AddRange(_speculativeMemory.Keys.ToList());
        
        if (NumberOfMemories < numberOfInfos)
            return null;
        
        List<Information> shuffleList = new List<Information>(allMemories);
        //Shuffle List
        shuffleList = shuffleList.OrderBy(x => Random.value).ToList();

        return shuffleList.Take(numberOfInfos).ToList();
    }

    public void MutateMemory()
    {
        if (NumberOfMemories > 0)
        {
            var idx = Random.Range(0, NumberOfMemories);
            if (idx < NumberOfStableMemories)
                _stableMemory.Keys.ToList()[idx].Mutate();
            else
                _speculativeMemory.Keys.ToList()[idx-NumberOfStableMemories].Mutate();
        }
    }
}
