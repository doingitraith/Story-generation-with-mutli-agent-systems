using UnityEngine;

namespace Game
{
    public class Item : WorldObject
    {
        public bool IsVisible;
        private bool _isInInventory;

        protected override void Awake()
        {
            base.Awake();
        }
    
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }

        public void SetInventory(bool isInInventory)
            => _isInInventory = isInInventory;

        public void PickUp(Agent agent)
        {
            SetInventory(true);
            transform.parent = agent.transform.Find("Inventory");
            transform.position = agent.transform.position;
            GetComponentInChildren<SphereCollider>().enabled = false;
            GetComponentInChildren<Renderer>().enabled = false;
        }

        public void Drop()
        {
            SetInventory(false);
            transform.parent = GameObject.Find("Items").transform;
            transform.Translate(Vector3.right);
            GetComponentInChildren<SphereCollider>().enabled = true;
            GetComponentInChildren<Renderer>().enabled = true;
        }
    }
}
