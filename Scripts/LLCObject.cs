using UnityEngine;
using DaggerfallWorkshop;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game;

namespace LockedLootContainers
{
    [RequireComponent(typeof(DaggerfallAudioSource))] // Appears to automatically assign an audiosource to this class gameobject when it is created? Atleast that is what I think it's meant to do?
    public class LLCObject : MonoBehaviour
    {
        #region Fields

        ulong loadID = 0;

        TextFile.Token[] regionManufacturedText;
        TextFile.Token[] companyManufacturedText;
        int[] recentInspectValues = { -1, -1, -1, -1, -1, -1, -1, -1 };

        bool isLocked = false; // Value is kind of pointless right now, since as soon as the chest unlocks it changes into a loot-pile and does not persist in an "unlocked" state or whatever.
        bool isTrapped = false;
        bool isEmpty = false; // Looks like someone was here already, and they decided to relatch the lock after they emptied the chest...
        bool isLockJammed = false; // Presumably will be unable to unlock after it has been jammed, including open spell magic, and will need force at this point, atleast probably.
        bool hasHiddenCompartment = false; // If this chest generated with an extra hidden compartment that can only be detected if perceptive, skilled, and lucky enough, etc.
        bool hasBeenBashed = false;
        bool hasBeenInspected = false;

        ChestMaterials chestMaterial = ChestMaterials.None;
        int chestSturdiness = 1;
        int chestMagicResist = 1;
        int chestStartHP = 0;
        int chestCurrentHP = 0;

        LockMaterials lockMaterial = LockMaterials.None;
        int lockSturdiness = 1;
        int lockMagicResist = 1;
        int lockComplexity = 1;
        int jamResist = 1;
        int lockStartHP = 0;
        int lockCurrentHP = 0;

        int picksAttempted = 0;
        int lockMechStartHP = 0;
        int lockMechCurrentHP = 0;

        int trapAmount = 0;
        int trapType = 0; // May eventually if possibly to have more than 1 trap on a chest, that this would be better as an array/list of the effects attached kind of like spell-bundle arrays.

        ItemCollection oldLoot;
        ItemCollection attachedLoot;

        DaggerfallAudioSource dfAudioSource;

        #endregion

        #region Properties

        public ulong LoadID
        {
            get { return loadID; }
            set { loadID = value; }
        }

        public TextFile.Token[] RegionManufacturedText
        {
            get { return regionManufacturedText; }
            set { regionManufacturedText = value; }
        }

        public TextFile.Token[] CompanyManufacturedText
        {
            get { return companyManufacturedText; }
            set { companyManufacturedText = value; }
        }

        public int[] RecentInspectValues
        {
            get { return recentInspectValues; }
            set { recentInspectValues = value; }
        }

        public bool IsLocked
        {
            get { return isLocked; }
            set { isLocked = value; }
        }

        public bool IsTrapped
        {
            get { return isTrapped; }
            set { isTrapped = value; }
        }

        public bool IsEmpty
        {
            get { return isEmpty; }
            set { isEmpty = value; }
        }

        public bool IsLockJammed
        {
            get { return isLockJammed; }
            set { isLockJammed = value; }
        }

        public bool HasHiddenCompartment
        {
            get { return hasHiddenCompartment; }
            set { hasHiddenCompartment = value; }
        }

        public bool HasBeenBashed
        {
            get { return hasBeenBashed; }
            set { hasBeenBashed = value; }
        }

        public bool HasBeenInspected
        {
            get { return hasBeenInspected; }
            set { hasBeenInspected = value; }
        }

        public ChestMaterials ChestMaterial
        {
            get { return chestMaterial; }
            set { chestMaterial = value; }
        }

        public int ChestSturdiness
        {
            get { return chestSturdiness; }
            set { chestSturdiness = value; }
        }

        public int ChestMagicResist
        {
            get { return chestMagicResist; }
            set { chestMagicResist = value; }
        }

        public int ChestStartHP
        {
            get { return chestStartHP; }
            set { chestStartHP = value; }
        }

        public int ChestCurrentHP
        {
            get { return chestCurrentHP; }
            set { chestCurrentHP = value; }
        }

        public LockMaterials LockMaterial
        {
            get { return lockMaterial; }
            set { lockMaterial = value; }
        }

        public int LockSturdiness
        {
            get { return lockSturdiness; }
            set { lockSturdiness = value; }
        }

        public int LockMagicResist
        {
            get { return lockMagicResist; }
            set { lockMagicResist = value; }
        }

        public int LockComplexity
        {
            get { return lockComplexity; }
            set { lockComplexity = value; }
        }

        public int JamResist
        {
            get { return jamResist; }
            set { jamResist = value; }
        }

        public int LockStartHP
        {
            get { return lockStartHP; }
            set { lockStartHP = value; }
        }

        public int LockCurrentHP
        {
            get { return lockCurrentHP; }
            set { lockCurrentHP = value; }
        }

        public int PicksAttempted
        {
            get { return picksAttempted; }
            set { picksAttempted = value; }
        }

        public int LockMechStartHP
        {
            get { return lockMechStartHP; }
            set { lockMechStartHP = value; }
        }

        public int LockMechCurrentHP
        {
            get { return lockMechCurrentHP; }
            set { lockMechCurrentHP = value; }
        }

        public int TrapAmount
        {
            get { return trapAmount; }
            set { trapAmount = value; }
        }

        public int TrapType
        {
            get { return trapType; }
            set { trapType = value; }
        }

        public ItemCollection Oldloot
        {
            get { return oldLoot; }
            set { oldLoot = value; }
        }

        public ItemCollection AttachedLoot
        {
            get { return attachedLoot; }
            set { attachedLoot = value; }
        }

        #endregion

        void Awake()
        {
            dfAudioSource = GetComponent<DaggerfallAudioSource>();
        }

        #region Collision Handling

        private void OnCollisionEnter(Collision collision)
        {
            DoCollision(collision, null);
        }

        private void OnTriggerEnter(Collider other)
        {
            DoCollision(null, other);
        }

        void DoCollision(Collision collision, Collider other)
        {
            // Get missile based on collision type
            DaggerfallMissile missile = null;
            if (collision != null && other == null)
                missile = collision.gameObject.transform.GetComponent<DaggerfallMissile>();
            else if (collision == null && other != null)
                missile = other.GetComponentInParent<DaggerfallMissile>();
            else
                return;

            if (missile == null)
                return;

            if (missile.Caster != GameManager.Instance.PlayerEntityBehaviour) // Only want these to count for the player's projectiles, but not idea how this might work if enemies are blocked by chests.
                return;

            bool deleteChest = false;

            if (missile.IsArrow)
            {
                LLCObject hitChest = gameObject.GetComponent<LLCObject>();
                if (hitChest)
                {
                    if (LockedLootContainersMain.AttemptChestBashWithArrows(hitChest, missile))
                        deleteChest = true;
                }
            }
            else if (missile.TargetType == DaggerfallWorkshop.Game.MagicAndEffects.TargetTypes.SingleTargetAtRange || missile.TargetType == DaggerfallWorkshop.Game.MagicAndEffects.TargetTypes.AreaAtRange)
            {
                LLCObject hitChest = gameObject.GetComponent<LLCObject>();
                if (hitChest)
                {
                    if (LockedLootContainersMain.AttemptDestructiveMagicChestImpact(hitChest, missile))
                        deleteChest = true;
                }
            }

            if (deleteChest)
                Destroy(gameObject);
        }

        #endregion
    }
}