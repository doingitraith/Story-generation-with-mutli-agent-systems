using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelSpinner : MonoBehaviour
{
    [SerializeField] private float _speedAroundX;
    [SerializeField] private float _speedAroundY;
    [SerializeField] private float _speedAroundZ;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(transform.position,Vector3.right, _speedAroundX*Time.deltaTime);
        transform.RotateAround(transform.position,Vector3.up, _speedAroundY*Time.deltaTime);
        transform.RotateAround(transform.position,Vector3.forward, _speedAroundZ*Time.deltaTime);
    }
}
