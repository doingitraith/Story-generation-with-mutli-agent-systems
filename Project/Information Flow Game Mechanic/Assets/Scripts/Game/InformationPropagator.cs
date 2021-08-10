using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class InformationPropagator : MonoBehaviour
{
    private SphereCollider _collider;
    private InformationObject _informationObject;
    private float _propagationSpeed = 1.0f;
    private float _timeToLife = 5.0f;
    private float _maxRadius = 10.0f;
    private float _timeElapsed = .0f;

    // Start is called before the first frame update
    void Start()
    {
        _collider = GetComponent<SphereCollider>();
        _informationObject = GetComponent<InformationObject>();
        
        switch (_informationObject.PropagationType)
        {
            case InformationPropagationType.VISUAL:
            {
                _propagationSpeed = 2.0f;
                _timeToLife = 10.0f;
                _maxRadius = 20.0f;
            }
                break;
            case InformationPropagationType.AUDIO:
            {
                _propagationSpeed = 5.0f;
                _timeToLife = 5.0f;
                _maxRadius = 10.0f;
            }
            
                break;
            case InformationPropagationType.INSTANT:
            {
                _propagationSpeed = 1000.0f;
                _timeToLife = 2.0f;
                _maxRadius = 1000.0f;
            }
                break;
            case InformationPropagationType.PERSISTANT:
            {
                _propagationSpeed = 10.0f;
                _timeToLife = -1.0f;
                _maxRadius = 2.0f;
            }
                break;
            case InformationPropagationType.NONE:
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    // Update is called once per frame
    void Update()
    {
        while (_timeElapsed < _timeToLife)
        {
            _collider.radius = Mathf.Clamp(_collider.radius + _propagationSpeed * Time.deltaTime, .0f, _maxRadius);

            _timeElapsed += Time.deltaTime;
            return;
        }
        
        if(_informationObject.PropagationType != InformationPropagationType.PERSISTANT)
            Destroy(gameObject);
    }
}
