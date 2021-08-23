using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeculativeInformationManager : InformationManager
{
    public SpeculativeInformationManager(Agent owner) : base(owner) { }

    public SpeculativeInformationManager(Agent owner, int memorySize) : base(owner, memorySize) { }
}
