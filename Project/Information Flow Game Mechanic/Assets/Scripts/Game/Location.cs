using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour
{
    public string Name;
    public int WorldImportance;
    public Mutation Mutation;
    [SerializeField]
    private List<string> Mutations;
    public Transform LocationTransform;
    public InformationLocation InformationLocation;
    void Start()
    {
        for (int i = Mutations.Count-1; i >= 0; i--)
            Mutation = new Mutation(Mutations[i], Mutation);
        
        LocationTransform = transform;

        InformationLocation = new InformationLocation(Name, LocationTransform, Mutation);
    }
}
