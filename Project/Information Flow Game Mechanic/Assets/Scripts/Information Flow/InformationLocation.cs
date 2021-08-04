using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationLocation : MonoBehaviour
{
    public string Name;
    public Mutation Mutation;
    [SerializeField]
    private List<string> Mutations;
    public Vector3 Location { get; private set; }

    void Start()
    {
        Location = gameObject.transform.position;
        for (int i = Mutations.Count-1; i >= 0; i--)
            Mutation = new Mutation(Mutations[i], Mutation);
    }
}
