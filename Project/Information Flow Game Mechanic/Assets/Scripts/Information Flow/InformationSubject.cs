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
        if (!(o is InformationSubject))
            return false;
        InformationSubject other = o as InformationSubject;
        return Name.Equals(other.Name);
    }
}
