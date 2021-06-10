using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        Memory = new InformationManager();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
    }
}
