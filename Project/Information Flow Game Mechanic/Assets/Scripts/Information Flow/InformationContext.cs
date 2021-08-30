using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class InformationContext
{
    public int NumOfTimesRecieved { get; set; }
    public float Believability { get; set; }
    public float Heuristic { get; set; }

    public List<Agent> ReceivedFrom { get; set; }

    public InformationContext(int numOfTimesRecieved)
        => (NumOfTimesRecieved, Believability, Heuristic, ReceivedFrom)
            = (numOfTimesRecieved, .0f, .0f, new List<Agent>());
}
