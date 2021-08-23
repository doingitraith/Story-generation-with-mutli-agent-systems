using System.Collections;
using System.Collections.Generic;
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
        LongTermMemory = new InformationManager(this);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
