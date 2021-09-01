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
    
    private Dictionary<Information, InformationContext> _stableMemory;
    private Dictionary<Information, InformationContext> _speculativeMemory;
    private Agent _owner;

    public InformationManager(Agent owner)
        => (_owner, _stableMemory, _speculativeMemory) = 
            (owner, new Dictionary<Information, InformationContext>(),
                new Dictionary<Information, InformationContext>());

    // Create FixedSizeList
    public InformationManager(Agent owner, int memorySize)
        => (_owner, _stableMemory, _speculativeMemory) =
            (owner, new FixedSizeDictionary<Information, InformationContext>(memorySize),
                new Dictionary<Information, InformationContext>());

    public bool ContainsStableInformation(Information information)
        =>_stableMemory.ContainsKey(information);
    
    public bool ContainsSpeculativeInformation(Information information)
        =>_speculativeMemory.ContainsKey(information);

    public bool TryAddNewInformation(Information information, Agent sender)
    {
        if (_owner.InformationSubject.Equals(information.Subject))
            return false;
        
        if (!_owner.Acquaintances.ContainsKey(sender))
            _owner.Acquaintances.Add(sender, _owner.Equals(sender) ? 1.0f : .5f);

        if (ContainsStableInformation(information) && !_stableMemory[information].ReceivedFrom.Contains(sender))
            _stableMemory[information].NumOfTimesRecieved++;

        _stableMemory[information].ReceivedFrom.Add(sender);
        float believability = _stableMemory[information].Heuristic;
        _owner.Acquaintances[sender] = _owner.Equals(sender) ? 
            1.0f : _owner.Acquaintances[sender] + believability / 10.0f;

            List<Information> filteredInfos = 
            _stableMemory.Keys.ToList().FindAll(i=>i.Verb.Equals(information.Verb));

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
                
                _stableMemory.Add(new Information(information), new InformationContext(1));
            }
                break;
            case InformationVerb.HAS:
            {
                filteredInfos = filteredInfos.FindAll(i => i.Object.Equals(information.Object));
                switch (filteredInfos.Count)
                {
                    case 0: _stableMemory.Add(new Information(information), new InformationContext(1));
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
                    case 0: _stableMemory.Add(new Information(information), new InformationContext(1));
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
    
    public bool TryAddNewSpeculativeInformation(Information information, Agent sender)
    {
        // don't add information about self
        if (_owner.InformationSubject.Equals(information.Subject))
            return false;
        
        // add acquaintance with initial trust
        if (!_owner.Acquaintances.ContainsKey(sender))
            _owner.Acquaintances.Add(sender, _owner.Equals(sender) ? 1.0f : .5f);

        // increase timesReceived if received from new agent
        if (ContainsSpeculativeInformation(information) 
            && !_speculativeMemory[information].ReceivedFrom.Contains(sender))
            _speculativeMemory[information].NumOfTimesRecieved++;
        
        // Update information context and trust
        float believability = _speculativeMemory[information].Believability;
        _speculativeMemory[information].ReceivedFrom.Add(sender);
        _owner.Acquaintances[sender] = _owner.Equals(sender) ? 
            1.0f : _owner.Acquaintances[sender]+believability / 10.0f;

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
                
                _speculativeMemory.Add(new Information(information), new InformationContext(1));
            }
                break;
            case InformationVerb.HAS:
            {
                filteredInfos = filteredInfos.FindAll(i => i.Object.Equals(information.Object));
                switch (filteredInfos.Count)
                {
                    case 0: _speculativeMemory.Add(new Information(information), new InformationContext(1));
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
                    case 0: _speculativeMemory.Add(new Information(information), new InformationContext(1));
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

        if (believability >= _owner.BelievabilityThreshold)
        {
            if (TryAddNewInformation(information, sender))
                _speculativeMemory.Remove(information);
        }
        
        return true;
    }

    public void UpdateBelievability()
    {
        List<Information> data = new List<Information>(_speculativeMemory.Keys.ToList());
        data.AddRange(_stableMemory.Keys.ToList());

        // Map
        Dictionary<Information, Dictionary<Information, float>> distances = 
            new Dictionary<Information, Dictionary<Information, float>>();
        
        data.ForEach(d => distances.Add(d, new Dictionary<Information, float>()));
        
        data.ForEach(i => data.Where(j => !i.Equals(j)).ToList()
            .ForEach(k => distances[i].Add(k, GetInformationDistance(i,k))));
        

        // Reduce
        foreach (var distance in distances)
        {
            if (_stableMemory.ContainsKey(distance.Key) && distance.Value.Values.Any())
                _stableMemory[distance.Key].Believability = distance.Value.Values.Average();
            else if(_speculativeMemory.ContainsKey(distance.Key) && distance.Value.Values.Any())
                _speculativeMemory[distance.Key].Believability = distance.Value.Values.Average();
        }
    }

    private float GetInformationDistance(Information i1, Information i2)
    {
        InformationContext c1 = _stableMemory.ContainsKey(i1) ? _stableMemory[i1] : _speculativeMemory[i1];
        InformationContext c2 = _stableMemory.ContainsKey(i2) ? _stableMemory[i2] : _speculativeMemory[i2];
        if (c1 == null || c2 == null)
            throw new NullReferenceException(
                "Information not found in either memory. This should be impossible");

        CalcInformationHeuristic(i1, c1);
        CalcInformationHeuristic(i2, c2);
        
        return Mathf.Abs(c1.Heuristic-c2.Heuristic);
    }
    
     private void CalcInformationHeuristic(Information information, InformationContext context)
    {
        if (information.Subject == null)
            throw new NullReferenceException(
                "InformationSubject of " + information.Subject + " should not be empty");
        
        float b = context.Believability;
        int n = context.NumOfTimesRecieved;
        float h = context.Heuristic;
        float infosAboutSubject = 
            (float)(_stableMemory.Keys.Count(i => i.Subject.Name.Equals(information.Subject.Name))+
            _speculativeMemory.Keys.Count(i => i.Subject.Name.Equals(information.Subject.Name)))
            /NumberOfMemories;
        
        switch (information.Verb)
        {
            case InformationVerb.IS:
            {
                h = Mathf.Pow(
                    Convert.ToSingle(information.Adjective.Characteristic)*
                    (information.Subject.IsPerson ? 
                        CalcRelationDistance(information.Subject) : 
                        CalcItemHeuristic(information.Subject))+
                    1.0f, n );
            }
                break;
            case InformationVerb.HAS:
            {
                h = Mathf.Pow(CalcRelationDistance(information.Subject)*
                              CalcItemHeuristic(information.Object)+
                    1.0f, n);
            }
                break;
            case InformationVerb.AT:
            {
                Location worldLocation = GameManager.FindObjectsOfType<Location>().
                        First(i => i.Name.Equals(information.Location.Name));
                float worldImportance = 1.0f;
                if (worldLocation != null)
                    worldImportance = worldLocation.WorldImportance;
                h = Mathf.Pow(
                    worldImportance*
                    (information.Subject.IsPerson ? 
                        CalcRelationDistance(information.Subject) : 
                        CalcItemHeuristic(information.Subject))+
                    1.0f,n);
            }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        h *= b * infosAboutSubject * 
             (context.ReceivedFrom.Where(r=> _owner.Acquaintances.ContainsKey(r))
                 .Sum(a => _owner.Acquaintances[a]));
        context.Heuristic = h;
    }

    private float CalcRelationDistance(InformationSubject informationSubject)
    {
        float h = .0f;
        if (!informationSubject.IsPerson)
            return h;
        
        List<Agent> knownAssociates = GameManager.FindObjectsOfType<Agent>().
            Where(a => a.Name.Equals(informationSubject.Name)).ToList();
        if (knownAssociates.Any())
        {
            Agent goal = new List<Agent>(knownAssociates).OrderBy(x => Random.value).ElementAt(0);
            Tuple<int, Agent> NextToTarget = BreadthFirstShortestPath.ShortestPath(_owner, goal);
            if (NextToTarget != null)
                h = _owner.Acquaintances[NextToTarget.Item2] / NextToTarget.Item1;
        }
        else
            h = .5f;

        Agent worldAgent = GameManager.FindObjectsOfType<Agent>().
            First(i => i.Name.Equals(informationSubject.Name));
        float worldImportance = 1.0f;
        if (worldAgent != null)
            worldImportance = worldAgent.WorldImportance;
        
        return h*worldImportance;
    }
    
    private float CalcItemHeuristic(InformationSubject informationSubject)
    {
        float h = .0f;
        if (informationSubject.IsPerson)
            return h;

        List<Information> owners = new List<Information>(_stableMemory.Keys);
        owners.AddRange(_speculativeMemory.Keys);
        owners = owners.
            Where(i => i.Verb == InformationVerb.HAS && i.Object.Equals(informationSubject)).ToList();
        if (owners.Count > 1)
            throw new Exception("Should only be one owner");

        Item worldItem = GameManager.FindObjectsOfType<Item>().First(i => i.Name.Equals(informationSubject.Name));
        float worldImportance = 1.0f;
        if (worldItem != null)
            worldImportance = worldItem.WorldImportance;
        
        h += .5f + _owner.Inventory.Count(i => i.Name.Equals(informationSubject.Name))
                 * worldImportance
                 * .5f
                 + owners.Count*.5f;

        return h;
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
