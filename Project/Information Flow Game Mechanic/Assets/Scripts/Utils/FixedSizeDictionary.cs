using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FixedSizeDictionary<TKey, TValue> : Dictionary<TKey, TValue>
{
    private int _size;

    public FixedSizeDictionary(int size)
        => (_size) = (size);
    
    public new void Add(TKey key, TValue value)
    {
        base.Add(key, value);
        while (base.Count > _size)
            base.Remove(base.Keys.ElementAt(0));
    }
}
