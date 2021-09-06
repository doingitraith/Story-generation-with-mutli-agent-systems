using UnityEngine;

namespace Utils
{
    public class ItemSpawner : MonoBehaviour
    {
        public GameObject Item;

        // Start is called before the first frame update
        void Start()
        {
            Instantiate(Item, transform.position, Quaternion.identity, transform);
        }

        // Update is called once per frame
        void Update()
        {
            if(transform.childCount == 0)
                Instantiate(Item, transform.position, Quaternion.identity, transform);
        }
    }
}
