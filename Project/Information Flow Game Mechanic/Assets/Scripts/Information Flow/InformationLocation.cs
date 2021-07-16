using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InformationLocation : MonoBehaviour
{
    public string Name;
    public Vector3 Location { get; private set; }

    void Start()
    {
        Location = gameObject.transform.position;
    }
}
