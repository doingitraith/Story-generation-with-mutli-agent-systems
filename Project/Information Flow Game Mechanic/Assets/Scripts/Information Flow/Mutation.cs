using System;

namespace Information_Flow
{
    public class Mutation
    {
        public string Value;
        public readonly Mutation ParentMutation;

        public Mutation(string value, Mutation parent) => (Value, ParentMutation) = (value, parent);
    
        public void Mutate()
            => this.Value = ParentMutation != null ? ParentMutation.Value : Value;

        public override bool Equals(object o)
        {
            if (!(o is Mutation other))
                return false;

            return Value.Equals(other.Value) && (ParentMutation?.Equals(other.ParentMutation) ?? true);
        }

        public override int GetHashCode()
            => Value.GetHashCode() * (ParentMutation != null ? ParentMutation.GetHashCode() : 1);
    }
}
