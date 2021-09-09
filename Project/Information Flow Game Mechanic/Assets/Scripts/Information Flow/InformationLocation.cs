using UnityEngine;

namespace Information_Flow
{
    public class InformationLocation
    {
        public string Name;
        public readonly Mutation Mutation;
        public readonly Transform Location;
        private readonly string _originalName;

        public InformationLocation(string name, Transform location, Mutation mutation)
            => (Name, _originalName, Mutation, Location) = (name, name, mutation, location);
    
        public override bool Equals(object o)
        {
            if (!(o is InformationLocation other))
                return false;
            
            return _originalName.Equals(other._originalName)
                   && (Mutation?.Equals(other.Mutation) ?? true)
                   && Location.Equals(other.Location);
        }

        public override int GetHashCode()
            => _originalName.GetHashCode() * (Mutation != null ? Mutation.GetHashCode() : 1) * Location.GetHashCode();
    }
}
