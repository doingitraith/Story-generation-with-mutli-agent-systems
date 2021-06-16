using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InformationManager
{
    private Queue<InformationSet> _memory;
    // TODO Mapping for information;

    public InformationManager()
    {
        _memory = new Queue<InformationSet>();
    }
    
    public InformationManager(int memorySize)
    {
        _memory = new FixedSizedQueue(memorySize);
    }

    public bool Contains(Information information)
    {
        return _memory.Any(s => s.Contains(information));
    }
}
