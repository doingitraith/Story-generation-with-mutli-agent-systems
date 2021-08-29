using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class InformationContext
{
    public int NumOfTimesRecieved { get; set; }
    public float Believability { get; set; }

    public InformationContext(int numOfTimesRecieved)
        => (NumOfTimesRecieved, Believability) = (numOfTimesRecieved, .0f);
}
