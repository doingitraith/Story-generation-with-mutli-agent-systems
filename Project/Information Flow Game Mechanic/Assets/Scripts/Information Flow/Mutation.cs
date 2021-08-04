using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mutation
{
    public string Value;
    private Mutation ParentMutation;

    public Mutation(string value, Mutation parent) => (Value, ParentMutation) = (value, parent);
    
    public void Mutate()
        => this.Value = ParentMutation != null ? ParentMutation.Value : Value;
}
