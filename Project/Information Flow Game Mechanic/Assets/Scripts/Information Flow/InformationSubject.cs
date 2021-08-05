using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationSubject
{
    public string Name;
    public Mutation Mutation;

    public InformationSubject(string name, Mutation mutation)
        => (Name, Mutation) = (name, mutation);
}
