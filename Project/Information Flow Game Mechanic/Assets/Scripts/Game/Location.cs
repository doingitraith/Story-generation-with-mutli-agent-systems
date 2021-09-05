using System.Collections;
using System.Collections.Generic;
using Information_Flow;
using UnityEngine;

public class Location : MonoBehaviour
{
    public string Name;
    public int WorldImportance;
    private Mutation _mutation;
    public Transform LocationTransform;
    public InformationLocation InformationLocation;
    [SerializeField]
    private List<string> _mutations;
    void Start()
    {
        for (int i = _mutations.Count-1; i >= 0; i--)
            _mutation = new Mutation(_mutations[i], _mutation);
        
        LocationTransform = transform;

        InformationLocation = new InformationLocation(Name, LocationTransform, _mutation);
    }
}
