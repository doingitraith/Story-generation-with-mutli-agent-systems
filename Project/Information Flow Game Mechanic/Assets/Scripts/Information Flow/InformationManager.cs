using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Game;
using UnityEngine;
using Utils;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Information_Flow
{
    public class InformationManager
    {
        public readonly Agent Owner;
        public InferenceEngine InferenceEngine;
        public int NumberOfMemories => NumberOfStableMemories + NumberOfSpeculativeMemories;
        private readonly List<InformationContext> _stableMemory;
        private readonly List<InformationContext> _speculativeMemory;
        private int NumberOfStableMemories => _stableMemory.Count;
        private int NumberOfSpeculativeMemories => _speculativeMemory.Count;

        public InformationManager(Agent owner)
            => (Owner, _stableMemory, _speculativeMemory, InferenceEngine) = 
                (owner, new List<InformationContext>(),
                    new List<InformationContext>(),
                    new InferenceEngine(this, GameManager.Instance.WorldRules));

        // Create FixedSizeList
        /*
        public InformationManager(Agent owner, int memorySize)
            => (Owner, _stableMemory, _speculativeMemory, InferenceEngine) =
                (owner, new FixedSizeDictionary<Information, InformationContext>(memorySize, new InformationComparer()),
                    new Dictionary<Information, InformationContext>(),
                    new InferenceEngine(this, GameManager.Instance.WorldRules));
                    */

        public bool ContainsStableInformation(Information information)
            =>_stableMemory.Any(c => c.Information.Equals(information));
    
        public bool ContainsSpeculativeInformation(Information information)
            =>_speculativeMemory.Any(c => c.Information.Equals(information));

        private void RemoveStableInformation(Information information)
        {
            int idx = _stableMemory.FindIndex(c => c.Information.Equals(information));
            if(idx!=-1)
                _stableMemory.RemoveAt(idx);
        }

        private void RemoveSpeculativeInformation(Information information)
        {
            int idx = _speculativeMemory.FindIndex(c => c.Information.Equals(information));
            if(idx!=-1)
                _speculativeMemory.RemoveAt(idx);
        }

        private InformationContext GetStableMemory(Information information)
            => _stableMemory.Find(c => c.Information.Equals(information));
        
        private InformationContext GetSpeculativeMemory(Information information)
            => _speculativeMemory.Find(c => c.Information.Equals(information));

        public bool TryAddNewInformation(Information information, Agent sender)
        {
            if (Owner.InformationSubject.Equals(information.Subject))
                return false;
            if (information.Not)
            {
                if (ContainsStableInformation(information))
                    RemoveStableInformation(information);
            }
            if (!Owner.Acquaintances.ContainsKey(sender))
                Owner.Acquaintances.Add(sender, Owner.Equals(sender) ? 1.0f : .5f);

            List<InformationContext> filteredInfos = 
                _stableMemory.FindAll(c=>c.Information.Verb.Equals(information.Verb));

            if (!ContainsStableInformation(information))
            {
                switch (information.Verb)
                {
                    case InformationVerb.Is:
                    {
                        filteredInfos = filteredInfos.FindAll(c => 
                            c.Information.Subject.Equals(information.Subject));
                        InformationAdjective adjective = information.Adjective;

                        List<Information> infosToRemove = new List<Information>();

                        foreach (InformationContext context in filteredInfos)
                        {
                            Information adjInfo = context.Information;
                            List<InformationAdjective> cons = adjInfo.Adjective.Contradictions;
                            if (cons.Contains(adjective))
                                infosToRemove.Add(adjInfo);
                        }

                        infosToRemove.ForEach(RemoveStableInformation);

                        _stableMemory.Add(new InformationContext(new Information(information),1));
                    }
                        break;
                    case InformationVerb.Has:
                    {
                        filteredInfos = filteredInfos.FindAll(c =>
                            c.Information.Object.Equals(information.Object));
                        if (information.Object.IsUnique)
                        {
                            switch (filteredInfos.Count)
                            {
                                case 0:
                                    _stableMemory.Add(
                                        new InformationContext(new Information(information),1));
                                    break;
                                case 1:
                                    filteredInfos[0].Information = new Information(information);
                                    break;
                                default:
                                    throw new Exception("An unique Item cannot have more than one Owner: "
                                                        + information.Object.ToString());
                            }
                        }
                        else
                            _stableMemory.Add(new InformationContext(new Information(information),
                                1));
                    }
                        break;
                    case InformationVerb.At:
                    {
                        filteredInfos = filteredInfos.FindAll(c =>
                            c.Information.Subject.Equals(information.Subject));
                        switch (filteredInfos.Count)
                            {
                                case 0:
                                    _stableMemory.Add(new InformationContext(new Information(information),
                                        1));
                                    break;
                                case 1:
                                    filteredInfos[0].Information = new Information(information);
                                    break;
                                default:
                                    throw new Exception("An Agent cannot be in more than one places: "
                                                        + information.Subject.ToString());
                            }
                        }
                        break;
                    case InformationVerb.Null:
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            if (ContainsStableInformation(information))
            {
                int idx = _stableMemory.FindIndex(c => c.Information.Equals(information));
                
                if (!_stableMemory[idx].ReceivedFrom.Contains(sender))
                {
                    _stableMemory[idx].NumOfTimesRecieved++;
                    _stableMemory[idx].ReceivedFrom.Add(sender);
                }

                float believability = _stableMemory[idx].Heuristic;
                Owner.Acquaintances[sender] = Owner.Equals(sender) ? 
                    1.0f : Owner.Acquaintances[sender] + believability / 10.0f;
            }

            sender.StateInfos.ForEach(s=>TryAddNewInformation(new Information(s.GetInformation()), Owner));
            
            return true;
        }
    
        public bool TryAddNewSpeculativeInformation(Information information, Agent sender)
        {
            // don't add information about self
            if (Owner.InformationSubject.Equals(information.Subject) || ContainsStableInformation(information))
                return false;
            if (information.Not)
            {
                if (ContainsSpeculativeInformation(information))
                    RemoveSpeculativeInformation(information);
            }
            else
            {
                // add acquaintance with initial trust
                if (!Owner.Acquaintances.ContainsKey(sender))
                    Owner.Acquaintances.Add(sender, Owner.Equals(sender) ? 1.0f : .5f);

                List<InformationContext> filteredInfos =
                    _speculativeMemory.FindAll(c => c.Information.Verb.Equals(information.Verb));

                if (!ContainsSpeculativeInformation(information))
                {
                    switch (information.Verb)
                    {
                        case InformationVerb.Is:
                        {
                            filteredInfos = filteredInfos.FindAll(
                                c => c.Information.Subject.Equals(information.Subject));
                            InformationAdjective adjective = information.Adjective;

                            List<Information> infosToRemove = new List<Information>();

                            foreach (InformationContext context in filteredInfos)
                            {
                                Information adjInfo = context.Information;
                                List<InformationAdjective> cons = adjInfo.Adjective.Contradictions;
                                if (cons.Contains(adjective))
                                    infosToRemove.Add(adjInfo);
                            }

                            infosToRemove.ForEach(RemoveSpeculativeInformation);

                            _speculativeMemory.Add(new InformationContext(new Information(information),
                                1));
                        }
                            break;
                        case InformationVerb.Has:
                        {
                            filteredInfos = filteredInfos.FindAll(
                                c => c.Information.Object.Equals(information.Object));
                            if (information.Object.IsUnique)
                            {
                                switch (filteredInfos.Count)
                                {
                                    case 0:
                                        _speculativeMemory.Add(new InformationContext(new Information(information),
                                            1));
                                        break;
                                    case 1:
                                        filteredInfos[0].Information = new Information(information);
                                        break;
                                    default:
                                        throw new Exception("An Item cannot have more than one Owner: "
                                                            + information.Object.ToString());
                                }
                            }
                            else
                                _speculativeMemory.Add(new InformationContext(new Information(information),
                                    1));
                        }
                            break;
                        case InformationVerb.At:
                        {
                            filteredInfos = filteredInfos.FindAll(
                                c => c.Information.Subject.Equals(information.Subject));
                            switch (filteredInfos.Count)
                                {
                                    case 0:
                                    {
                                        _speculativeMemory.Add(new InformationContext(new Information(information),
                                            1));

                                        Owner.Acquaintances.Keys
                                            .Where(n => n.InformationSubject.Equals(information.Subject)).ToList()
                                            .ForEach(a =>
                                                a.StateInfos.ForEach(i =>
                                                    TryAddNewInformation(i.GetInformation(), Owner)));
                                    }
                                        break;
                                    case 1:
                                        filteredInfos[0].Information = new Information(information);
                                        break;
                                    default:
                                        throw new Exception("An Agent cannot be in more than one places: "
                                                            + information.Subject.ToString());
                                }
                        }
                            break;
                        case InformationVerb.Null:
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                // increase timesReceived if received from new agent
                if (ContainsSpeculativeInformation(information))
                {
                    int idx = _speculativeMemory.FindIndex(c => c.Information.Equals(information));
                    if (!_speculativeMemory[idx].ReceivedFrom.Contains(sender))
                    {
                        _speculativeMemory[idx].NumOfTimesRecieved++;
                        _speculativeMemory[idx].ReceivedFrom.Add(sender);
                    }
                    // Update information context and trust
                    float believability = _speculativeMemory[idx].Believability;
                    Owner.Acquaintances[sender] =
                        Owner.Equals(sender) ? 1.0f : Owner.Acquaintances[sender] + believability / 10.0f;

                    if (believability >= Owner.BelievabilityThreshold)
                    {
                        if (TryAddNewInformation(information, sender))
                            RemoveSpeculativeInformation(information);
                    }
                }
            }
        
            sender.StateInfos.ForEach(s=>TryAddNewInformation(new Information(s.GetInformation()), Owner));
        
            return true;
        }

        public void UpdateBelievability()
        {
            List<InformationContext> data = new List<InformationContext>(_speculativeMemory);
            data.AddRange(_stableMemory);

            // Map
            var distances = new List<List<float>>();

            for (int i = 0; i < data.Count; i++)
            {
                distances.Add(new List<float>());
                for (int k = 0; k < data.Count; k++)
                {
                    Information i1 = data[i].Information;
                    Information i2 = data[k].Information;
                    if (!i1.Equals(i2))
                        distances[i].Add(GetInformationDistance(i1, i2));
                }
            }
            
            // Reduce
            for (int i = 0; i < distances.Count; i++)
            {
                if (ContainsStableInformation(data[i].Information) && distances[i].Any())
                    GetStableMemory(data[i].Information).Believability = distances[i].Average();
                else if (ContainsSpeculativeInformation(data[i].Information) && distances[i].Any())
                    GetSpeculativeMemory(data[i].Information).Believability = distances[i].Average();
            }

            /*
            data.Where(i => _stableMemory.ContainsKey(i)).
                Where(i=> _stableMemory[i].Believability < Owner.BelievabilityThreshold).ToList().
                ForEach(i=> _stableMemory.Remove(i));
            */
        }

        private float GetInformationDistance(Information i1, Information i2)
        {
            InformationContext c1 = ContainsStableInformation(i1) ? GetStableMemory(i1) : GetSpeculativeMemory(i2);
            InformationContext c2 = ContainsStableInformation(i2) ? GetStableMemory(i1) : GetSpeculativeMemory(i2);

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
            float infosAboutSubject = (float)(_stableMemory.Count(c => 
                                                  c.Information.Subject.Name.Equals(information.Subject.Name))+
                                              _speculativeMemory.Count(c => 
                                                  c.Information.Subject.Name.Equals(information.Subject.Name)))
                                      /NumberOfMemories;
        
            switch (information.Verb)
            {
                case InformationVerb.Is:
                {
                    h = Mathf.Pow(
                        Convert.ToSingle(information.Adjective.Characteristic)/10.0f*
                        (information.Subject.IsPerson ? 
                            CalcRelationDistance(information.Subject) : 
                            CalcItemHeuristic(information.Subject))+
                        1.0f, n );
                }
                    break;
                case InformationVerb.Has:
                {
                    h = Mathf.Pow(CalcRelationDistance(information.Subject)*
                                  CalcItemHeuristic(information.Object)+
                                  1.0f, n);
                }
                    break;
                case InformationVerb.At:
                {
                    Location worldLocation = GameObject.FindObjectsOfType<Location>().
                        First(i => i.InformationLocation.Equals(information.Location));
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
                 (context.ReceivedFrom.Where(r=> Owner.Acquaintances.ContainsKey(r))
                     .Sum(a => Owner.Acquaintances[a]));
            context.Heuristic = h;
        }
     
        private float CalcRelationDistance(InformationSubject informationSubject)
        {
            float h = .0f;
            if (!informationSubject.IsPerson)
                return h;
        
            List<Agent> knownAssociates = GameObject.FindObjectsOfType<Agent>().
                Where(a => a.InformationSubject.Equals(informationSubject)).ToList();
        
            if (knownAssociates.Count > 1)
                throw new Exception("There should only be one Agent with the name " + knownAssociates[0].Name);
        
            Agent worldAgent = knownAssociates[0];
            if(knownAssociates.Count == 0)
                h = .5f;
            else if (Owner.ImportantPeople.Contains(knownAssociates[0]))
                h = 1.0f;
            else
            {
                Agent goal = new List<Agent>(knownAssociates).OrderBy(x => Random.value).ElementAt(0);
                Tuple<int, Agent> NextToTarget = BreadthFirstShortestPath.ShortestPath(Owner, goal);
                if (NextToTarget != null)
                    h = Owner.Acquaintances[NextToTarget.Item2] / NextToTarget.Item1;
            }

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

            List<InformationContext> owners = new List<InformationContext>(_stableMemory);
            owners.AddRange(_speculativeMemory);
            owners = owners.Where(c => c.Information.Verb == InformationVerb.Has && 
                                       c.Information.Object.Equals(informationSubject)).ToList();
            
            if (informationSubject.IsUnique && owners.Count > 1)
                throw new Exception("Unique Item should only be one owner");

            Item[] worldItems = GameObject.FindObjectsOfType<Item>();
            Item worldItem = worldItems.First(i => i.InformationSubject.Equals(informationSubject));
            float worldImportance = 1.0f;
            if (worldItem != null)
                worldImportance = worldItem.WorldImportance;
        
            h += .5f + Owner.Inventory.Count(i => i.Name.Equals(informationSubject.Name))
                     * worldImportance
                     * .5f
                     + owners.Count*.5f;

            return h;
        }

        public List<Information> GetInformationToExchange(int numberOfInfos, InformationSubject target)
        {
            var allMemories = new List<InformationContext>(_stableMemory);
                allMemories.AddRange(_speculativeMemory);

                if (NumberOfMemories < numberOfInfos)
                    return null;
                
                var shuffleList = new List<InformationContext>(allMemories);
                
                //Shuffle List
                shuffleList = shuffleList.Where(c => !c.Information.Subject.Equals(target) && 
                                                     !c.Information.Verb.Equals(InformationVerb.At))
                    .OrderByDescending(c => Mathf.Abs(c.Believability)).ToList();

                if (shuffleList.Count == 0)
                    return null;

                return shuffleList.Take(numberOfInfos).Select(c=>c.Information).ToList();
            }

        public void MutateMemory()
        {
            if (NumberOfMemories > 0)
            {
                var idx = Random.Range(0, NumberOfMemories);
                if (idx < NumberOfStableMemories)
                    _stableMemory[idx].Information.Mutate();
                else
                    _speculativeMemory[idx-NumberOfStableMemories].Information.Mutate();
            }
        }

        public List<InformationContext> GetStableMemory()
            => _stableMemory;
    }
}
