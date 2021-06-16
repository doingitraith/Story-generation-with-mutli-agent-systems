using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InformationManager
{
    private Queue<InformationSet> _memory;
    private Character _owner;

    public InformationManager(Character owner)
    {
        _owner = owner;
        _memory = new Queue<InformationSet>();
    }
    
    public InformationManager(Character owner, int memorySize)
    {
        _owner = owner;
        _memory = new FixedSizedQueue(memorySize);
    }

    private bool Contains(Information information)
    {
        return _memory.Any(s => s.Contains(information));
    }

    private InformationSet GetInformationSet(WorldObject subject)
    {
        foreach (InformationSet infoSet in _memory)
        {
            if (infoSet.Subject.Equals(subject))
                return infoSet;
        }

        return null;
    }

    public bool TryAddNewInformation(Information information)
    {
        // TODO If subject is owner, add both character and item
        if (Contains(information))
            return false;

        InformationSet infoSet = GetInformationSet(information.Subject);
        if (infoSet != null)
        {
            infoSet.UpdateInformationSet(information);
            return true;
        }
        else
        {
            if(information.Subject is Character)
                _memory.Enqueue(new CharacterInformationSet(information));
            else
                _memory.Enqueue(new ItemInformationSet(information));
            
            return true;
        }
            
    }
}
