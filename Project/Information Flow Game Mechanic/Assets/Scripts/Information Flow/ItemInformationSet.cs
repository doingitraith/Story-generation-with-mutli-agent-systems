using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInformationSet : InformationSet
{
    public Item Subject { get; set; }
    public Character Owner { get; set; }
    public List<InformationProperty> Properties { get; set; }
}
