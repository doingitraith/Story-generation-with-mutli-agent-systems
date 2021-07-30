using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMutatable
{
    IMutatable Mutation { get; set; }
    public void Mutate();
}
