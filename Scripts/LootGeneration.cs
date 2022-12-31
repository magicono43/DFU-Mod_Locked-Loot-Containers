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
using DaggerfallWorkshop.Game.Formulas;

namespace LockedLootContainers
{
    public partial class LockedLootContainersMain
    {
        public static void PopulateChestLoot(LLCObject llcObj, int totalRoomValueMod, int[] itemGroupOdds)
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

            float lowOddsMod = 0f;
            float midOddsMod = 0f;
            float highOddsMod = 0f;

            switch (llcObj.ChestMaterial) // For now, these are intended to be the modifier each "lootOdds" value is going to be multipled by the end before item generation rolls happen, still placeholder.
            {
                default:
                case ChestMaterials.Wood:
                    lowOddsMod = 0.35f; midOddsMod = 0.60f; highOddsMod = 0.80f; break;
                case ChestMaterials.Iron:
                    lowOddsMod = 0.60f; midOddsMod = 0.85f; highOddsMod = 1.0f; break;
                case ChestMaterials.Steel:
                    lowOddsMod = 1.1f; midOddsMod = 1.3f; highOddsMod = 1.5f; break;
                case ChestMaterials.Orcish:
                    lowOddsMod = 2.2f; midOddsMod = 2.6f; highOddsMod = 3.0f; break;
                case ChestMaterials.Mithril:
                    lowOddsMod = 1.4f; midOddsMod = 1.1f; highOddsMod = 1.1f; break;
                case ChestMaterials.Dwarven:
                    lowOddsMod = 2.5f; midOddsMod = 1.3f; highOddsMod = 1.5f; break;
                case ChestMaterials.Adamantium:
                    lowOddsMod = 2.9f; midOddsMod = 1.7f; highOddsMod = 2.0f; break;
                case ChestMaterials.Daedric:
                    lowOddsMod = 4.5f; midOddsMod = 3.2f; highOddsMod = 2.5f; break;
            }

            for (int i = 0; i < itemGroupOdds.Length; i++) // Odds are modified in this loop for each item group within the "itemGroupOdds" array.
            {
                if (itemGroupOdds[i] <= 0)
                    continue;
                else if (itemGroupOdds[i] > 0 && itemGroupOdds[i] <= 15)
                    itemGroupOdds[i] = (int)Mathf.Round(itemGroupOdds[i] * lowOddsMod);
                else if (itemGroupOdds[i] > 15 && itemGroupOdds[i] <= 50)
                    itemGroupOdds[i] = (int)Mathf.Round(itemGroupOdds[i] * midOddsMod);
                else
                    itemGroupOdds[i] = (int)Mathf.Round(itemGroupOdds[i] * highOddsMod);
            }

            for (int i = 0; i < itemGroupOdds.Length; i++) // Item groups within "itemGroupOdds" are actually looped through and rolled to determine what items in those groups should be populated in chest.
            {
                int itemChance = itemGroupOdds[i];

                if (itemChance <= 0)
                    continue;

                while (Dice100.SuccessRoll(itemChance))
                {
                    DaggerfallUnityItem item = DetermineLootItem(i);
                    chestItems.AddItem(item);
                    itemChance -= 100;
                }
            }

            // So presumably after the above for-loop, there will possibly be some left over "room value" mods applied somehow and then items will start being rolled based on the itemGroupOdds array values.
            // Will continue working on this loot generation stuff tomorrow. For now, keep using "Jewelry Additions" loot generation code and methods as a primary example to copy/pull from.
            // Next maybe work on actually generating items and resources in the chest based on various factors such as dungeon type, totalRoomValueMod, and the various chest attributes possibly?
            // Now that I have the items groups sort of proposed for the chest loot system, will probably want to actually start on some coding for the generation with this "complete" for now?
        }

        public static DaggerfallUnityItem DetermineLootItem(int itemGroupLLC)
        {
            Genders gender = GameManager.Instance.PlayerEntity.Gender;
            Races race = GameManager.Instance.PlayerEntity.Race;
            Array enumArray;
            int enumIndex = -1;

            switch (itemGroupLLC)
            {
                default:
                    return null;
                case (int)ChestLootItemGroups.Drugs:
                    return ItemBuilder.CreateRandomDrug(); // Just vanilla selection for now.
                case (int)ChestLootItemGroups.LightArmor: // Will take modded/custom items into consideration later, after this works for the vanilla stuff atleast, for all item groups here.
                    enumArray = Enum.GetValues(typeof(HeavyArmor));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    return ItemBuilder.CreateArmor(gender, race, (Armor)enumArray.GetValue(enumIndex), ArmorMaterialTypes.Leather); // Will need testing to see if casting to vanilla enum works here.
                case (int)ChestLootItemGroups.MediumArmor:
                    enumArray = Enum.GetValues(typeof(HeavyArmor));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    return ItemBuilder.CreateArmor(gender, race, (Armor)enumArray.GetValue(enumIndex), ArmorMaterialTypes.Chain); // Will need testing to see if casting to vanilla enum works here.
                case (int)ChestLootItemGroups.HeavyArmor:
                    enumArray = Enum.GetValues(typeof(HeavyArmor));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    ArmorMaterialTypes plateType = (ArmorMaterialTypes)0x0200 + UnityEngine.Random.Range(0, 10); // Will have materials be determined based on other factors later, placeholder for now.
                    return ItemBuilder.CreateArmor(gender, race, (Armor)enumArray.GetValue(enumIndex), plateType); // Will need testing to see if casting to vanilla enum works here.
                case (int)ChestLootItemGroups.Shields:
                    enumArray = Enum.GetValues(typeof(Shields));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    return ItemBuilder.CreateArmor(gender, race, (Armor)enumArray.GetValue(enumIndex), FormulaHelper.RandomArmorMaterial(UnityEngine.Random.Range(1, 21))); // Random level for now.
                case (int)ChestLootItemGroups.SmallWeapons:
                    return null; // Start here tomorrow with "small weapons" will have to do some weird stuff due to having some modded values inside this enum and also the "wand" item, etc will see.
            }
        }
    }
}