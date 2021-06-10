using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class InformationManager
{
    private ConcurrentQueue<InformationSet> _memory;
    // TODO Mapping for information;

    public InformationManager()
    {
        _memory = new ConcurrentQueue<InformationSet>();
    }
    
    public InformationManager(int memorySize)
    {
        _memory = new FixedSizedQueue(memorySize);
    }
}
