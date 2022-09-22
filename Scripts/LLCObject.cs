using UnityEngine;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Items;

namespace LockedLootContainers
{
    public class LLCObject : MonoBehaviour
    {
        #region Fields

        TextFile.Token[] onClickText;

        ulong chestID = 0;

        bool isLocked = false;
        bool isTrapped = false;
        bool isEmpty = false;
        bool hasBeenBashed = false;

        int lockDifficulty = 0;
        int chestMaterial = 0; // Likely won't be an int, will probably be some custom material type I make that will be filled in later on.

        int trapAmount = 0;
        int trapType = 0; // May eventually if possibly to have more than 1 trap on a chest, that this would be better as an array/list of the effects attached kind of like spell-bundle arrays.

        ItemCollection oldLoot;
        ItemCollection attachedLoot;

        #endregion

        #region Properties

        public TextFile.Token[] OnClickText
        {
            get { return onClickText; }
            set { onClickText = value; }
        }

        public ulong ChestID
        {
            get { return chestID; }
            set { chestID = value; }
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

        public int LockDifficulty
        {
            get { return lockDifficulty; }
            set { lockDifficulty = value; }
        }

        public int ChestMaterial
        {
            get { return chestMaterial; }
            set { chestMaterial = value; }
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