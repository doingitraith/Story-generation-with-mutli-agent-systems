using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldObject : MonoBehaviour
{
    public string Name;
    public Mutation Mutation;
    [SerializeField]
    private List<string> Mutations;
    public InformationLocation Location;
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        for (int i = Mutations.Count-1; i >= 0; i--)
            Mutation = new Mutation(Mutations[i], Mutation);   
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }
}
