using UnityEngine;

namespace Information_Flow
{
    public class InformationLocation
    {
        public string Name;
        public Mutation Mutation;
        public Transform Location;

        public InformationLocation(string name, Transform location, Mutation mutation)
            => (Name, Mutation, Location) = (name, mutation, location);
    
        public override bool Equals(object o)
        {
            if (!(o is InformationLocation other))
                return false;

            return Name.Equals(other.Name) && (Mutation?.Equals(other.Mutation) ?? true) && Location.Equals(other.Location);
        }

        public override int GetHashCode()
            => Name.GetHashCode() * (Mutation != null ? Mutation.GetHashCode() : 1) * Location.GetHashCode();
    }
}
