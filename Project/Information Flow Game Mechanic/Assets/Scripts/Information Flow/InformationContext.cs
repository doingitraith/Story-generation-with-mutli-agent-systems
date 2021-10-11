using System.Collections.Generic;
using Game;
using UnityEngine;

namespace Information_Flow
{
    public class InformationContext
    {
        public Information Information { get; set; }
        public int NumOfTimesRecieved { get; set; }
        public float Believability { get; set; }
        public float Heuristic { get; set; }

        public List<Agent> ReceivedFrom { get; }

        public double Age { get; set; }
        
        public InformationContext(Information information, int numOfTimesRecieved)
            => (Information, NumOfTimesRecieved, Believability, Heuristic, ReceivedFrom, Age)
                = (information, numOfTimesRecieved, 1.0f, 1.0f, new List<Agent>(), Time.realtimeSinceStartupAsDouble);
        
        protected bool Equals(InformationContext other)
        {
            return Equals(Information, other.Information)
                   && NumOfTimesRecieved == other.NumOfTimesRecieved
                   && Believability.Equals(other.Believability)
                   && Heuristic.Equals(other.Heuristic)
                   && Equals(ReceivedFrom, other.ReceivedFrom)
                   && Age.Equals(other.Age)
                   && ReceivedFrom.TrueForAll(c=>c.Equals(other.ReceivedFrom[ReceivedFrom.IndexOf(c)]));
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Information != null ? Information.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ NumOfTimesRecieved;
                hashCode = (hashCode * 397) ^ Believability.GetHashCode();
                hashCode = (hashCode * 397) ^ Heuristic.GetHashCode();
                hashCode = (hashCode * 397) ^ (ReceivedFrom != null ? ReceivedFrom.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Age.GetHashCode();
                return hashCode;
            }
        }
    }
}
