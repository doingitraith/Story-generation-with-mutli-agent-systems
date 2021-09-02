using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;
using Yarn.Unity;

public class Player : Agent
{
    protected override void Awake()
    {
        base.Awake();
    }
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Memory = new InformationManager(this);
        /*
        Memory.TryAddNewInformation(new Information(FindObjectOfType<NPC>().GetComponent<NPC>(),
            GameManager.Instance.WorldAdjectives[Adjectives.alive]),this);
        Memory.TryAddNewInformation(new Information(FindObjectOfType<NPC>().GetComponent<NPC>(),
            GameManager.Instance.WorldAdjectives[Adjectives.evil]),this);
        */
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
