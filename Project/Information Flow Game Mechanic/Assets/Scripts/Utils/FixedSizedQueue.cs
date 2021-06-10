using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class FixedSizedQueue : ConcurrentQueue<InformationSet>
{
    private readonly InformationSet syncObject = new InformationSet();

    public int Size { get; private set; }

    public FixedSizedQueue(int size)
    {
        Size = size;
    }

    public new void Enqueue(InformationSet obj)
    {
        base.Enqueue(obj);
        lock (syncObject)
        {
            while (base.Count > Size)
            {
                InformationSet outObj;
                base.TryDequeue(out outObj);
            }
        }
    }
}
