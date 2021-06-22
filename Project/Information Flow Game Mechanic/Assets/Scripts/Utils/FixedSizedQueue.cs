using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class FixedSizedQueue : Queue<InformationSet>
{
    public int Size { get; private set; }

    public FixedSizedQueue(int size)
    {
        Size = size;
    }

    public new void Enqueue(InformationSet obj)
    {
        Enqueue(obj);
        while (Count > Size)
            Dequeue();
    }
}
