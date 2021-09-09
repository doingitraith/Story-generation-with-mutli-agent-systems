using System;
using Information_Flow;
using UnityEngine;

namespace Game
{
    public class SightCone : MonoBehaviour
    {
        public Agent Owner;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!Owner.IsSeeing)
                return;
            
            if (other.gameObject.TryGetComponent<WorldObject>(out var worldObject))
            {
                worldObject.StateInfos.ForEach(s=>Owner.Memory.TryAddNewInformation(
                    new Information(s.GetInformation()), Owner));
            }
        }
    }
}
