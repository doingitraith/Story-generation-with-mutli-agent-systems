using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class FixedSizedQueue : ConcurrentQueue<Information>
{
    private readonly Information syncObject = new Information();

    public int Size { get; private set; }

    public FixedSizedQueue(int size)
    {
        Size = size;
    }

    public new void Enqueue(Information obj)
    {
        base.Enqueue(obj);
        lock (syncObject)
        {
            while (base.Count > Size)
            {
                Information outObj;
                base.TryDequeue(out outObj);
            }
        }
    }
}
