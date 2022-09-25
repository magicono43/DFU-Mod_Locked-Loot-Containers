using UnityEngine;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Items;

namespace LockedLootContainers
{
    public class LLCObject : MonoBehaviour
    {
        #region Fields

        ulong loadID = 0;

        TextFile.Token[] onClickText;
        TextFile.Token[] regionManufacturedText;
        TextFile.Token[] companyManufacturedText;

        bool isLocked = false;
        bool isTrapped = false;
        bool isEmpty = false;
        bool isLockJammed = false;
        bool hasBeenBashed = false;
        bool hasBeenInspected = false;

        int chestMaterial = 0; // Likely won't be an int, will probably be some custom material type I make that will be filled in later on.
        int lockMaterial = 0;
        int lockComplexity = 0;
        int magicResistance = 0;

        int picksAttempted = 0;
        int bashesAttempted = 0;

        int trapAmount = 0;
        int trapType = 0; // May eventually if possibly to have more than 1 trap on a chest, that this would be better as an array/list of the effects attached kind of like spell-bundle arrays.

        ItemCollection oldLoot;
        ItemCollection attachedLoot;

        #endregion

        #region Properties

        public ulong LoadID
        {
            get { return loadID; }
            set { loadID = value; }
        }

        public TextFile.Token[] OnClickText
        {
            get { return onClickText; }
            set { onClickText = value; }
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

        public bool IsLockJammed
        {
            get { return isLockJammed; }
            set { isLockJammed = value; }
        }

        public int ChestMaterial
        {
            get { return chestMaterial; }
            set { chestMaterial = value; }
        }

        public int LockMaterial
        {
            get { return lockMaterial; }
            set { lockMaterial = value; }
        }

        public int LockComplexity
        {
            get { return lockComplexity; }
            set { lockComplexity = value; }
        }

        public int MagicResistance
        {
            get { return magicResistance; }
            set { magicResistance = value; }
        }

        public int PicksAttempted
        {
            get { return picksAttempted; }
            set { picksAttempted = value; }
        }

        public int BashesAttempted
        {
            get { return bashesAttempted; }
            set { bashesAttempted = value; }
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

        #region Public Methods



        #endregion

        #region Private Methods



        #endregion
    }
}