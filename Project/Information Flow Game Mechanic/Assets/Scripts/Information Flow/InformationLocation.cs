using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationLocation
{
    public string Name { get; }
    public Transform Position { get; }

    public InformationLocation(string name, Transform position)
        => (Name, Position) = (name, position);
}
