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
        private readonly Dictionary<Information, InformationContext> _stableMemory;
        private readonly Dictionary<Information, InformationContext> _speculativeMemory;
        private int NumberOfStableMemories => _stableMemory.Count;
        private int NumberOfSpeculativeMemories => _speculativeMemory.Count;

        public InformationManager(Agent owner)
            => (Owner, _stableMemory, _speculativeMemory, InferenceEngine) = 
                (owner, new Dictionary<Information, InformationContext>(new InformationComparer()),
                    new Dictionary<Information, InformationContext>(),
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
            =>_stableMemory.ContainsKey(information);
    
        public bool ContainsSpeculativeInformation(Information information)
            =>_speculativeMemory.ContainsKey(information);

        public bool TryAddNewInformation(Information information, Agent sender)
        {
            if (Owner.InformationSubject.Equals(information.Subject))
                return false;
            if (information.Not)
            {
                if (ContainsStableInformation(information))
                    _stableMemory.Remove(information);
            }
            if (!Owner.Acquaintances.ContainsKey(sender))
                Owner.Acquaintances.Add(sender, Owner.Equals(sender) ? 1.0f : .5f);

            List<Information> filteredInfos = 
                _stableMemory.Keys.ToList().FindAll(i=>i.Verb.Equals(information.Verb));

            if (!ContainsStableInformation(information))
            {
                switch (information.Verb)
                {
                    case InformationVerb.Is:
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
                    case InformationVerb.Has:
                    {
                        filteredInfos = filteredInfos.FindAll(i => i.Object.Equals(information.Object));
                        switch (filteredInfos.Count)
                        {
                            case 0:
                                _stableMemory.Add(new Information(information), new InformationContext(1));
                                break;
                            case 1:
                                filteredInfos[0] = new Information(information);
                                break;
                            default:
                                throw new Exception("An Item cannot have more than one Owner: "
                                                    + information.Object.ToString());
                        }
                    }
                        break;
                    case InformationVerb.At:
                    {
                        filteredInfos = filteredInfos.FindAll(i => i.Subject.Equals(information.Subject));
                        switch (filteredInfos.Count)
                        {
                            case 0:
                                _stableMemory.Add(new Information(information), new InformationContext(1));
                                break;
                            case 1:
                                filteredInfos[0] = new Information(information);
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
                if (!_stableMemory[information].ReceivedFrom.Contains(sender))
                {
                    _stableMemory[information].NumOfTimesRecieved++;
                    _stableMemory[information].ReceivedFrom.Add(sender);
                }

                float believability = _stableMemory[information].Heuristic;
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
                    _speculativeMemory.Remove(information);
            }
            else
            {
                // add acquaintance with initial trust
                if (!Owner.Acquaintances.ContainsKey(sender))
                    Owner.Acquaintances.Add(sender, Owner.Equals(sender) ? 1.0f : .5f);

                List<Information> filteredInfos =
                    _speculativeMemory.Keys.ToList().FindAll(i => i.Verb.Equals(information.Verb));

                if (!ContainsSpeculativeInformation(information))
                {
                    switch (information.Verb)
                    {
                        case InformationVerb.Is:
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
                        case InformationVerb.Has:
                        {
                            filteredInfos = filteredInfos.FindAll(i => i.Object.Equals(information.Object));
                            switch (filteredInfos.Count)
                            {
                                case 0:
                                    _speculativeMemory.Add(new Information(information), new InformationContext(1));
                                    break;
                                case 1:
                                    filteredInfos[0] = new Information(information);
                                    break;
                                default:
                                    throw new Exception("An Item cannot have more than one Owner: "
                                                        + information.Object.ToString());
                            }
                        }
                            break;
                        case InformationVerb.At:
                        {
                            filteredInfos = filteredInfos.FindAll(i => i.Subject.Equals(information.Subject));
                            switch (filteredInfos.Count)
                            {
                                case 0:
                                {
                                    _speculativeMemory.Add(new Information(information), new InformationContext(1));

                                    Owner.Acquaintances.Keys
                                        .Where(n => n.InformationSubject.Equals(information.Subject)).ToList()
                                        .ForEach(a =>
                                            a.StateInfos.ForEach(i => TryAddNewInformation(i.GetInformation(), Owner)));
                                }
                                    break;
                                case 1:
                                    filteredInfos[0] = new Information(information);
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
                    if (!_speculativeMemory[information].ReceivedFrom.Contains(sender))
                    {
                        _speculativeMemory[information].NumOfTimesRecieved++;
                        _speculativeMemory[information].ReceivedFrom.Add(sender);
                    }
                    // Update information context and trust
                    float believability = _speculativeMemory[information].Believability;
                    Owner.Acquaintances[sender] =
                        Owner.Equals(sender) ? 1.0f : Owner.Acquaintances[sender] + believability / 10.0f;

                    if (believability >= Owner.BelievabilityThreshold)
                    {
                        if (TryAddNewInformation(information, sender))
                            _speculativeMemory.Remove(information);
                    }
                }
            }
        
            sender.StateInfos.ForEach(s=>TryAddNewInformation(new Information(s.GetInformation()), Owner));
        
            return true;
        }

        public void UpdateBelievability()
        {
            List<Information> data = new List<Information>(_speculativeMemory.Keys.ToList());
            data.AddRange(_stableMemory.Keys.ToList());

            // Map
            Dictionary<Information, Dictionary<Information, float>> distances = 
                new Dictionary<Information, Dictionary<Information, float>>();

            data.ForEach(d =>
            {
                if(!distances.ContainsKey(d))
                    distances.Add(d, new Dictionary<Information, float>());
            });
        
            data.ForEach(i => data.Where(j => !i.Equals(j)).ToList()
                .ForEach(k =>
                {
                    if(!distances[i].ContainsKey(k))
                        distances[i].Add(k, GetInformationDistance(i, k));
                }));
        

            // Reduce
            foreach (var distance in distances)
            {
                if (_stableMemory.ContainsKey(distance.Key) && distance.Value.Values.Any())
                    _stableMemory[distance.Key].Believability = distance.Value.Values.Average();
                else if(_speculativeMemory.ContainsKey(distance.Key) && distance.Value.Values.Any())
                    _speculativeMemory[distance.Key].Believability = distance.Value.Values.Average();
            }
            
            /*
            data.Where(i => _stableMemory.ContainsKey(i)).
                Where(i=> _stableMemory[i].Believability < Owner.BelievabilityThreshold).ToList().
                ForEach(i=> _stableMemory.Remove(i));
            */
        }

        private float GetInformationDistance(Information i1, Information i2)
        {
            InformationContext c1 = null;
            InformationContext c2 = null;
            try
            {
                if (_stableMemory.ContainsKey(i1))
                    c1 = _stableMemory[i1];
                else
                {
                    bool test = _stableMemory.ContainsKey(i1);
                    c1 = _speculativeMemory[i1];
                }

                //c1 = _stableMemory.ContainsKey(i1) ? _stableMemory[i1] : _speculativeMemory[i1];
                c2 = _stableMemory.ContainsKey(i2) ? _stableMemory[i2] : _speculativeMemory[i2];
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

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

            List<Information> owners = new List<Information>(_stableMemory.Keys);
            owners.AddRange(_speculativeMemory.Keys);
            owners = owners.
                Where(i => i.Verb == InformationVerb.Has && i.Object.Equals(informationSubject)).ToList();
            if (owners.Count > 1)
                throw new Exception("Should only be one owner");

            Item[] worldItems = GameObject.FindObjectsOfType<Item>();
            Item worldItem = worldItems.First(i => i.Name.Equals(informationSubject.Name));
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
            var allMemories = new Dictionary<Information, InformationContext>(_stableMemory);
            foreach (var m in _speculativeMemory)
                allMemories.Add(m.Key, m.Value);
        
            if (NumberOfMemories < numberOfInfos)
                return null;

            List<Information> shuffleList = new List<Information>(allMemories.Keys);
            //Shuffle List
            shuffleList = shuffleList.Where(i=>!i.Subject.Equals(target) &&
                                               !i.Verb.Equals(InformationVerb.At)).
                OrderByDescending(x => Mathf.Abs(allMemories[x].Believability)).ToList();

            if (shuffleList.Count == 0)
                return null;
            
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

        public Dictionary<Information, InformationContext> GetStableMemory()
            => _stableMemory;
    }
}
