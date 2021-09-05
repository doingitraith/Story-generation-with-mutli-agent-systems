using System.Collections.Generic;
using Game;

namespace Information_Flow
{
    public class InformationContext
    {
        public int NumOfTimesRecieved { get; set; }
        public float Believability { get; set; }
        public float Heuristic { get; set; }

        public List<Agent> ReceivedFrom { get; set; }

        public InformationContext(int numOfTimesRecieved)
            => (NumOfTimesRecieved, Believability, Heuristic, ReceivedFrom)
                = (numOfTimesRecieved, 1.0f, 1.0f, new List<Agent>());
    }
}
