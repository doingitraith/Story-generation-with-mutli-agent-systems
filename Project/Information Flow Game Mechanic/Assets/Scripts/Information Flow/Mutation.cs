using System;

namespace Information_Flow
{
    public class Mutation
    {
        public string Value;
        private readonly Mutation _parentMutation;

        public Mutation(string value, Mutation parent) => (Value, _parentMutation) = (value, parent);
    
        public void Mutate()
            => this.Value = _parentMutation != null ? _parentMutation.Value : Value;

        public override bool Equals(object o)
        {
            if (!(o is Mutation other))
                return false;

            return Value.Equals(other.Value) && (_parentMutation?.Equals(other._parentMutation) ?? true);
        }

        public override int GetHashCode()
            => Value.GetHashCode() * (_parentMutation != null ? _parentMutation.GetHashCode() : 1);
    }
}
