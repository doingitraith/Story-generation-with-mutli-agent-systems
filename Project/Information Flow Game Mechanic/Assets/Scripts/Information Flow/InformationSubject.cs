namespace Information_Flow
{
    public class InformationSubject
    {
        public string Name;
        public readonly bool IsPerson;
        public readonly bool IsUnique;
        public readonly Mutation Mutation;
        private readonly string _originalName;

        public InformationSubject(string name, bool isPerson, bool isUnique, Mutation mutation)
            => (Name, _originalName, IsPerson, IsUnique, Mutation) = (name, name, isPerson, isUnique, mutation);

        public override bool Equals(object o)
        {
            if (!(o is InformationSubject other))
                return false;

            return _originalName.Equals(other._originalName) && (Mutation?.Equals(other.Mutation) ?? true) 
                                                             && IsPerson.Equals(other.IsPerson) 
                                                             && IsUnique.Equals(other.IsUnique);
        }

        public override int GetHashCode()
            => _originalName.GetHashCode() * (Mutation != null ? Mutation.GetHashCode() : 1) 
                                           * IsPerson.GetHashCode()
                                           * IsUnique.GetHashCode();
    }
}
