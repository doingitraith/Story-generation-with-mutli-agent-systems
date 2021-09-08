using UnityEngine;

namespace Information_Flow
{
    public class InformationLocation
    {
        public string Name;
        public readonly Mutation Mutation;
        private readonly Transform _location;
        private readonly string _originalName;

        public InformationLocation(string name, Transform location, Mutation mutation)
            => (Name, _originalName, Mutation, _location) = (name, name, mutation, location);
    
        public override bool Equals(object o)
        {
            if (!(o is InformationLocation other))
                return false;
            
            return _originalName.Equals(other._originalName)
                   && (Mutation?.Equals(other.Mutation) ?? true)
                   && _location.Equals(other._location);
        }

        public override int GetHashCode()
            => _originalName.GetHashCode() * (Mutation != null ? Mutation.GetHashCode() : 1) * _location.GetHashCode();
    }
}
