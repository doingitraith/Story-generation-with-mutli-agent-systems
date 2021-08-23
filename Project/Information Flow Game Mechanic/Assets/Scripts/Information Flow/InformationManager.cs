using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class InformationManager
{
    public int NumberOfMemories => _memory.Count;
    
    private List<Information> _memory;
    private Agent _owner;

    public InformationManager(Agent owner)
        => (_owner, _memory) = (owner, new List<Information>());

    // Create FixedSizeList
    public InformationManager(Agent owner, int memorySize)
        => (_owner, _memory) = (owner, new FixedSizeList<Information>(memorySize));

    public bool ContainsInformation(Information information)
        =>_memory.Contains(information);

    public bool TryAddNewInformation(Information information, Agent sender)
    {
        if (sender != null)
        {
            if (!_owner.Acquaintances.ContainsKey(sender))
            {
                if(_owner.Equals(sender))
                    _owner.Acquaintances.Add(sender, 100.0f);
                else
                    _owner.Acquaintances.Add(sender, 1.0f);
            }

            EvaluateInformation(information, sender);
        }

        if (ContainsInformation(information) || _owner.InformationSubject.Equals(information.Subject))
            return false;

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
    }

    private void EvaluateInformation(Information information, Agent sender)
    {
        float infoValue = GetInformationHeuristic(information);
        
        float trust = _owner.Acquaintances[sender];
        _owner.Acquaintances[sender] = CalculateNewTrust(infoValue, trust);
    }

    private float CalculateNewTrust(float infoValue, float trust)
    {
        //TODO: Calculate new trust with infoValue 
        //throw new NotImplementedException();
        return trust;
    }

    private float GetInformationHeuristic(Information information)
    {
        //TODO: Get Information value based on context
        //throw new NotImplementedException();
        return .0f;
    }

    public List<Information> GetInformationsToExchange(int numberOfInfos)
    {
        if (NumberOfMemories < numberOfInfos)
            return null;
        
        List<Information> shuffleList = new List<Information>(GetInformations());
        //Shuffle List
        shuffleList = shuffleList.OrderBy(x => Random.value).ToList();

        return shuffleList.Take(numberOfInfos).ToList();
    }

    private List<Information> GetInformations() 
        => _memory;

    public void MutateMemory()
    {
        if (NumberOfMemories > 0)
        {
            var idx = Random.Range(0, NumberOfMemories);
            GetInformations()[idx].Mutate();
        }
    }
}
