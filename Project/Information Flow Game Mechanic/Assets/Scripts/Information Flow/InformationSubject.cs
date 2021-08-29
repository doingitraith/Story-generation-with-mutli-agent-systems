using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationSubject
{
    public string Name;
    public Mutation Mutation;

    public InformationSubject(string name, Mutation mutation)
        => (Name, Mutation) = (name, mutation);

    public override bool Equals(object o)
    {
        if (!(o is InformationSubject other))
            return false;

        return Name.Equals(other.Name) && (Mutation?.Equals(other.Mutation) ?? true);
    }

    public override int GetHashCode()
        => Name.GetHashCode() * (Mutation != null ? Mutation.GetHashCode() : 1);
}
