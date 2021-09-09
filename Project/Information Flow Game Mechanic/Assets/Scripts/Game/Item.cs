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
            gameObject.GetComponentInChildren<MeshRenderer>().enabled = false;
            transform.parent = agent.transform.Find("Inventory");
            transform.position = agent.transform.position;
            GetComponentInChildren<SphereCollider>().enabled = false;
            GetComponentInChildren<Renderer>().enabled = false;
            
            if (tag.Equals("Weapon"))
                agent.EquipItem(this);
        }

        public void Drop()
        {
            SetInventory(false);
            gameObject.GetComponentInChildren<MeshRenderer>().enabled = true;
            transform.parent = GameObject.Find("Items").transform;
            transform.Translate(Vector3.right);
            GetComponentInChildren<SphereCollider>().enabled = true;
            GetComponentInChildren<Renderer>().enabled = true;
        }
    }
}
