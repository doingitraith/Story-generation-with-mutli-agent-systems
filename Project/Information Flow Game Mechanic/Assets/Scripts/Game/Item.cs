using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : WorldObject
{
    private bool _isInInventory;

    protected override void Awake()
    {
        base.Awake();
    }
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    public void SetInventory(bool isInInventory)
        => _isInInventory = isInInventory;
}
