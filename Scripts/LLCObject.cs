using UnityEngine;
using DaggerfallConnect.Arena2;

namespace LockedLootContainers
{
    public class LLCObject : MonoBehaviour
    {
        #region Fields

        TextFile.Token[] greetingText;

        bool hasGreeting = false;
        ulong linkedAlliesID = 0;
        bool inventoryPopulated = false;

        #endregion

        #region Properties

        public TextFile.Token[] GreetingText
        {
            get { return greetingText; }
            set { greetingText = value; }
        }

        public bool HasGreeting
        {
            get { return hasGreeting; }
            set { hasGreeting = value; }
        }

        public ulong LinkedAlliesID
        {
            get { return linkedAlliesID; }
            set { linkedAlliesID = value; }
        }

        public bool InventoryPopulated
        {
            get { return inventoryPopulated; }
            set { inventoryPopulated = value; }
        }

        #endregion

        #region Public Methods



        #endregion

        #region Private Methods



        #endregion
    }
}