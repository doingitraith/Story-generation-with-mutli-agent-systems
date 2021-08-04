using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldObject : MonoBehaviour
{
    public string Name;
    public Mutation Mutation;
    [SerializeField]
    private List<string> Mutations;

    public InformationSubject InformationSubject;
    public InformationLocation Location;

    protected virtual void Awake()
    {
        for (int i = Mutations.Count-1; i >= 0; i--)
            Mutation = new Mutation(Mutations[i], Mutation);

        InformationSubject = new InformationSubject(Name, Mutation);
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }
}
