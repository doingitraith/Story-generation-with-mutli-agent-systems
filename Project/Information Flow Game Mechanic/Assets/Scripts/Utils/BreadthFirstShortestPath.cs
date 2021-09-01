using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BreadthFirstShortestPath
{
    public static Tuple<int, Agent> ShortestPath(Agent start, Agent goal)
    {
        //int dist = 0;
        
        Dictionary<Agent, Agent> predecessor = new Dictionary<Agent, Agent>();
        Dictionary<Agent, bool> visited = new Dictionary<Agent, bool>();
        Dictionary<Agent, int> distance = new Dictionary<Agent, int>();

        Queue<Agent> queue = new Queue<Agent>();
        visited.Add(start, true);
        distance.Add(start, 0);
        
        queue.Enqueue(start);
        while (queue.Count > 0)
        {
            //dist++;
            Agent node = queue.Dequeue();
            foreach (Agent neighbour in node.Acquaintances.Keys.ToList())
            {
                if (!visited.ContainsKey(neighbour))
                    visited.Add(neighbour, false);
                    
                if (visited[neighbour] == false)
                {
                    visited[neighbour] = true;
                    if (!distance.ContainsKey(neighbour))
                        distance.Add(neighbour, distance[node] + 1);
                    else
                        distance[neighbour] = distance[node] + 1;

                    if (!predecessor.ContainsKey(neighbour))
                        predecessor.Add(neighbour, node);
                    else
                        predecessor[neighbour] = node;
                    
                    queue.Enqueue(neighbour);
                    if (neighbour.Equals(goal))
                    {
                        Agent a = goal;
                        while (true)
                        {
                            if (predecessor[a].Equals(start))
                                return new Tuple<int, Agent>(distance[goal], a);
                            a = predecessor[a];
                        }
                    }
                }
            }
        }

        return null;
    }
}
