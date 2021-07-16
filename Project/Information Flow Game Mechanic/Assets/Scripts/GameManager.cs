using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class GameManager : MonoBehaviour
{
    public static Dictionary<Adjectives, InformationAdjective> WorldAdjectives;

    private void Awake()
    {
        WorldAdjectives = new Dictionary<Adjectives, InformationAdjective>();
        InitAdjectives();
    }

    public void Start()
    {
    }

    public static void InitAdjectives()
    {
        // Add adjective Properties
        WorldAdjectives.Add(Adjectives.alive, 
            new InformationProperty(Adjectives.alive, new List<InformationAdjective>()));
        WorldAdjectives.Add(Adjectives.dead, 
            new InformationProperty(Adjectives.dead, new List<InformationAdjective>()));
        
        // Add contradictions
        WorldAdjectives[Adjectives.alive].AddContradiction(WorldAdjectives[Adjectives.dead]);
        WorldAdjectives[Adjectives.dead].AddContradiction(WorldAdjectives[Adjectives.alive]);
    }
}
