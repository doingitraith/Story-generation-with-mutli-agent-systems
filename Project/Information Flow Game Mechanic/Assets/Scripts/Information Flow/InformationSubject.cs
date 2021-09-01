using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationSubject
{
    public string Name;
    public bool IsPerson;
    public Mutation Mutation;

    public InformationSubject(string name, bool isPerson, Mutation mutation)
        => (Name, IsPerson, Mutation) = (name, isPerson, mutation);

    public override bool Equals(object o)
    {
        if (!(o is InformationSubject other))
            return false;

        return Name.Equals(other.Name) && (Mutation?.Equals(other.Mutation) ?? true);
    }

    public override int GetHashCode()
        => Name.GetHashCode() * (Mutation != null ? Mutation.GetHashCode() : 1);
}
