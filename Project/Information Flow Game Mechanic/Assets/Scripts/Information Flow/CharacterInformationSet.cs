using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInformationSet : InformationSet
{
    public Character Subject { get; set; }
    public List<InformationAdjective> Properties { get; set; }
    public List<Item> Possessions { get; set; }
}
