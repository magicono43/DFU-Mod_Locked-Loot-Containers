using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallConnect;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using System.Collections.Generic;
using System;
using DaggerfallWorkshop.Game.Utility;

namespace LockedLootContainers
{
    public partial class LockedLootContainersMain
    {
        public static void PopulateChestLoot(LLCObject llcObj, int totalRoomValueMod)
        {
            llcObj.AttachedLoot.TransferAll(llcObj.Oldloot);
            ItemCollection chestItems = llcObj.AttachedLoot;
            int initialItemCount = chestItems.Count;

            // Loop through all the items in the "old" loot-pile and delete them from there based on their vanilla IDs, the idea being to leave most modded items alone.
            for (int i = 0; i < initialItemCount; i++)
            {
                DaggerfallUnityItem item = chestItems.GetItem(i); // Hmm, this will almost definitely cause an "Index out of range" error, so I'll have to consider how to solve that issue.

                if (!item.IsQuestItem && item.TemplateIndex >= 0 && item.TemplateIndex <= 511) // Check items for vanilla template index, and remove if they match.
                {
                    chestItems.RemoveItem(item);
                    i--; // Will have to see if this keeps "Index out of range" error from happening, but kind of doubt it, will see.
                }
            }

            // Will continue working on this loot generation stuff tomorrow. For now, keep using "Jewelry Additions" loot generation code and methods as a primary example to copy/pull from.
            // Next maybe work on actually generating items and resources in the chest based on various factors such as dungeon type, totalRoomValueMod, and the various chest attributes possibly?
        }
    }
}