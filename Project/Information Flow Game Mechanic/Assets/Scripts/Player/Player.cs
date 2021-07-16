using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Agent
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Memory = new InformationManager(this);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
