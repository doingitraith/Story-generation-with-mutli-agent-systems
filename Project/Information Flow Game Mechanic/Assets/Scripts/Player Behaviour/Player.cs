using Game;
using Information_Flow;

namespace Player_Behaviour
{
    public class Player : Agent
    {
        protected override void Awake()
        {
            base.Awake();
        }
    
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            Memory = new InformationManager(this);
            Quests.Add(GameManager.Instance.Quest);
            /*
        Memory.TryAddNewInformation(new Information(FindObjectOfType<NPC>().GetComponent<NPC>(),
            GameManager.Instance.WorldAdjectives[Adjectives.ALIVE]),this);
        Memory.TryAddNewInformation(new Information(FindObjectOfType<NPC>().GetComponent<NPC>(),
            GameManager.Instance.WorldAdjectives[Adjectives.evil]),this);
        */
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
        }

        public bool HasWeapon()
        {
            return EquippedItem != null && EquippedItem.gameObject.CompareTag("Weapon");
        }
    }
}
