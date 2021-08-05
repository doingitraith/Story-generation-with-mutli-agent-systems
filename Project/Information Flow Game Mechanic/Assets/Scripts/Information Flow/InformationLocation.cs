using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationLocation
{
    public string Name;
    public Mutation Mutation;
    public Transform Location;

    public InformationLocation(string name, Transform location, Mutation mutation)
        => (Name, Mutation, Location) = (name, mutation, location);
}
