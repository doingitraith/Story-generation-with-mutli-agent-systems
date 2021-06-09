using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class InformationManager
{
    private FixedSizedQueue _memory;
    // TODO Mapping for information;

    public InformationManager(int memorySize)
    {
        _memory = new FixedSizedQueue(memorySize);
    }
}
