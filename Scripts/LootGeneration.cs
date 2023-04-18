using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Entity;
using System.Collections.Generic;
using System;
using DaggerfallWorkshop.Game.Utility;
using System.Linq;

namespace LockedLootContainers
{
    public partial class LockedLootContainersMain
    {
        public static void PopulateChestLoot(LLCObject llcObj, int totalRoomValueMod, int[] dungTypeMiscOdds, int[] dungTypeItemOdds, int[] dungTypeItemBlacklist)
        {
            int[] miscGroupOdds = (int[])dungTypeMiscOdds.Clone(); // Note to self, make sure to clone an array like this if you plan on having different "instances" of changing the values inside.
            int[] itemGroupOdds = (int[])dungTypeItemOdds.Clone();
            int[] itemBlacklist = (int[])dungTypeItemBlacklist.Clone();
            llcObj.AttachedLoot.ReplaceAll(llcObj.Oldloot);
            ItemCollection chestItems = llcObj.AttachedLoot;
            int initialItemCount = chestItems.Count;

            // Loop through all the items in the "old" loot-pile and delete them from there based on their vanilla IDs, the idea being to leave most modded items alone.
            for (int i = 0; i < initialItemCount; i++)
            {
                DaggerfallUnityItem item = chestItems.GetItem(i);

                if (!item.IsQuestItem && item.TemplateIndex >= 0 && item.TemplateIndex <= 511) // Check items for vanilla template index, and remove if they match.
                {
                    chestItems.RemoveItem(item);
                    i--;
                }
            }

            int[] modifItemGroupOdds = (int[])itemGroupOdds.Clone();
            int[] modifMiscGroupOdds = (int[])miscGroupOdds.Clone();
            float oddsAverage = 0f;
            int chestModCommon = 0;
            int chestModUncommon = 0;
            int chestModRare = 0;

            int lockModCommon = 0;
            int lockModUncommon = 0;
            int lockModRare = 0;

            int compModCommon = 0;
            int compModUncommon = 0;
            int compModRare = 0;

            int roomModCommon = Mathf.FloorToInt(totalRoomValueMod / 10f);
            int roomModUncommon = Mathf.FloorToInt(totalRoomValueMod / 20f);
            int roomModRare = Mathf.FloorToInt(totalRoomValueMod / 50f);

            for (int i = 0; i < 2; i++)
            {
                int matIndex = 0;
                if (i == 0) { matIndex = (int)llcObj.ChestMaterial; }
                else { matIndex = (int)llcObj.LockMaterial; }

                switch (matIndex)
                {
                    default:
                    case (int)ChestMaterials.Wood:
                        if (i == 0) { chestModCommon = UnityEngine.Random.Range(-30, 1); chestModUncommon = UnityEngine.Random.Range(-18, 1); chestModRare = UnityEngine.Random.Range(-7, 1); }
                        else { lockModCommon = UnityEngine.Random.Range(-30, 1); lockModUncommon = UnityEngine.Random.Range(-18, 1); lockModRare = UnityEngine.Random.Range(-7, 1); }
                        oddsAverage += 0.5f; break;
                    case (int)ChestMaterials.Iron:
                        if (i == 0) { chestModCommon = UnityEngine.Random.Range(-10, 6); chestModUncommon = UnityEngine.Random.Range(-5, 3); chestModRare = UnityEngine.Random.Range(-2, 2); }
                        else { lockModCommon = UnityEngine.Random.Range(-10, 6); lockModUncommon = UnityEngine.Random.Range(-5, 3); lockModRare = UnityEngine.Random.Range(-2, 2); }
                        oddsAverage += 0.75f; break;
                    case (int)ChestMaterials.Steel:
                        if (i == 0) { chestModCommon = UnityEngine.Random.Range(8, 15); chestModUncommon = UnityEngine.Random.Range(4, 11); chestModRare = UnityEngine.Random.Range(2, 5); }
                        else { lockModCommon = UnityEngine.Random.Range(8, 15); lockModUncommon = UnityEngine.Random.Range(4, 11); lockModRare = UnityEngine.Random.Range(2, 5); }
                        oddsAverage += 1.5f; break;
                    case (int)ChestMaterials.Orcish:
                        if (i == 0) { chestModCommon = UnityEngine.Random.Range(17, 26); chestModUncommon = UnityEngine.Random.Range(9, 17); chestModRare = UnityEngine.Random.Range(8, 11); }
                        else { lockModCommon = UnityEngine.Random.Range(17, 26); lockModUncommon = UnityEngine.Random.Range(9, 17); lockModRare = UnityEngine.Random.Range(8, 11); }
                        oddsAverage += 3.0f; break;
                    case (int)ChestMaterials.Mithril:
                        if (i == 0) { chestModCommon = UnityEngine.Random.Range(11, 17); chestModUncommon = UnityEngine.Random.Range(5, 13); chestModRare = UnityEngine.Random.Range(3, 6); }
                        else { lockModCommon = UnityEngine.Random.Range(11, 17); lockModUncommon = UnityEngine.Random.Range(5, 13); lockModRare = UnityEngine.Random.Range(3, 6); }
                        oddsAverage += 1.75f; break;
                    case (int)ChestMaterials.Dwarven:
                        if (i == 0) { chestModCommon = UnityEngine.Random.Range(12, 20); chestModUncommon = UnityEngine.Random.Range(6, 14); chestModRare = UnityEngine.Random.Range(5, 8); }
                        else { lockModCommon = UnityEngine.Random.Range(12, 20); lockModUncommon = UnityEngine.Random.Range(6, 14); lockModRare = UnityEngine.Random.Range(5, 8); }
                        oddsAverage += 2.25f; break;
                    case (int)ChestMaterials.Adamantium:
                        if (i == 0) { chestModCommon = UnityEngine.Random.Range(18, 27); chestModUncommon = UnityEngine.Random.Range(12, 20); chestModRare = UnityEngine.Random.Range(11, 14); }
                        else { lockModCommon = UnityEngine.Random.Range(18, 27); lockModUncommon = UnityEngine.Random.Range(12, 20); lockModRare = UnityEngine.Random.Range(11, 14); }
                        oddsAverage += 3.25f; break;
                    case (int)ChestMaterials.Daedric:
                        if (i == 0) { chestModCommon = UnityEngine.Random.Range(28, 36); chestModUncommon = UnityEngine.Random.Range(20, 34); chestModRare = UnityEngine.Random.Range(17, 22); }
                        else { lockModCommon = UnityEngine.Random.Range(28, 36); lockModUncommon = UnityEngine.Random.Range(20, 34); lockModRare = UnityEngine.Random.Range(17, 22); }
                        oddsAverage += 4.5f; break;
                }
            }

            if (llcObj.LockComplexity < 0 || (llcObj.LockComplexity >= 0 && llcObj.LockComplexity <= 19)) { compModCommon = UnityEngine.Random.Range(-5, 1); compModUncommon = UnityEngine.Random.Range(-3, 1); compModRare = UnityEngine.Random.Range(-1, 1); }
            else if (llcObj.LockComplexity >= 20 && llcObj.LockComplexity <= 39) { compModCommon = UnityEngine.Random.Range(2, 8); compModUncommon = UnityEngine.Random.Range(0, 3); compModRare = UnityEngine.Random.Range(0, 2); }
            else if (llcObj.LockComplexity >= 40 && llcObj.LockComplexity <= 59) { compModCommon = UnityEngine.Random.Range(5, 13); compModUncommon = UnityEngine.Random.Range(2, 6); compModRare = UnityEngine.Random.Range(1, 4); }
            else if (llcObj.LockComplexity >= 60 && llcObj.LockComplexity <= 79) { compModCommon = UnityEngine.Random.Range(8, 16); compModUncommon = UnityEngine.Random.Range(3, 8); compModRare = UnityEngine.Random.Range(2, 6); }
            else { compModCommon = UnityEngine.Random.Range(12, 20); compModUncommon = UnityEngine.Random.Range(7, 12); compModRare = UnityEngine.Random.Range(4, 9); }

            int totalModsCommon = chestModCommon + lockModCommon + compModCommon + roomModCommon;
            int totalModsUncommon = chestModUncommon + lockModUncommon + compModUncommon + roomModUncommon;
            int totalModsRare = chestModRare + lockModRare + compModRare + roomModRare;

            // Apply the chest and lock material odds modifiers to the "modifItemGroupOdds" values based on the base itemGroupOdds rarity values.
            for (int i = 0; i < modifItemGroupOdds.Length; i++)
            {
                if (itemGroupOdds[i] <= 0)
                    continue;

                if (itemGroupOdds[i] >= 1 && itemGroupOdds[i] <= 15) { modifItemGroupOdds[i] += totalModsRare; } // Rare
                else if (itemGroupOdds[i] >= 16 && itemGroupOdds[i] <= 41) { modifItemGroupOdds[i] += totalModsUncommon; } // Uncommon
                else { modifItemGroupOdds[i] += totalModsCommon; } // Common

                if (modifItemGroupOdds[i] < 0) // Any values that were reduced below 0 will be brought back up to 1 to atleast give them some chance of spawning.
                    modifItemGroupOdds[i] = 1;
            }

            // Apply the chest and lock material odds modifiers to the "modifMiscGroupOdds" values based on the base miscGroupOdds rarity values.
            for (int i = 0; i < modifMiscGroupOdds.Length; i++)
            {
                if (miscGroupOdds[i] <= 0 || (i >= 2 && i <= 5) || i == 12 || i == 13)
                    continue;

                if (i == 0) { modifMiscGroupOdds[i] += totalModsCommon; }
                else if (i == 1) { modifMiscGroupOdds[i] += Mathf.RoundToInt(totalModsRare / 4f); }
                else if (i == 6) { modifMiscGroupOdds[i] += Mathf.RoundToInt(totalModsUncommon / 2f); }
                else if (i >= 7 && i <= 11) { modifMiscGroupOdds[i] += Mathf.RoundToInt(totalModsRare / 3f); }

                if (modifMiscGroupOdds[i] < 0) // Any values that were reduced below 0 will be brought back up to 1 to atleast give them some chance of spawning.
                    modifMiscGroupOdds[i] = 1;
            }

            /*
            // Debug log string creator, for testing purposes only.
            string baseString = "";
            for (int i = 0; i < modifItemGroupOdds.Length; i++)
            {
                string valueString = "[" + modifItemGroupOdds[i].ToString() + "]";
                baseString = baseString + ", " + valueString;
            }
            Debug.LogFormat("Item Group Odds For Chest: {0}", baseString);

            // Debug log string creator, for testing purposes only.
            baseString = "";
            for (int i = 0; i < modifMiscGroupOdds.Length; i++)
            {
                string valueString = "[" + modifMiscGroupOdds[i].ToString() + "]";
                baseString = baseString + ", " + valueString;
            }
            Debug.LogFormat("Misc Group Odds For Chest: {0}", baseString);
            */

            for (int i = 0; i < modifMiscGroupOdds.Length; i++) // Groups within "miscGroupOdds" are actually looped through and rolled to determine what items in those groups should be populated in chest.
            {
                int itemChance = modifMiscGroupOdds[i];
                float conditionMod = (float)UnityEngine.Random.Range(modifMiscGroupOdds[13], modifMiscGroupOdds[12] + 1) / 100f;
                DaggerfallUnityItem item = null;
                int amount = 0;

                switch (i)
                {
                    default:
                        break;
                    case 0: // Add Gold
                        int goldBonus = (modifMiscGroupOdds[0] - 70) > 0 ? (modifMiscGroupOdds[0] - 70) * UnityEngine.Random.Range(3, 21) : 0;
                        amount = (Dice100.SuccessRoll(itemChance)) ? UnityEngine.Random.Range(modifMiscGroupOdds[2], modifMiscGroupOdds[3] + 1) + goldBonus : 0;
                        if (amount > 0)
                            chestItems.AddItem(ItemBuilder.CreateGoldPieces(RolePlayRealismLootRebalanceCheck ? Mathf.CeilToInt(amount / 3f) : amount));
                        break;
                    case 1: // Add Letter of Credit
                        int locBonus = (modifMiscGroupOdds[0] - 9) > 0 ? (modifMiscGroupOdds[0] - 9) * UnityEngine.Random.Range(12, 42) : 0;
                        amount = (Dice100.SuccessRoll(itemChance)) ? UnityEngine.Random.Range(modifMiscGroupOdds[4], modifMiscGroupOdds[5] + 1) + locBonus : 0;
                        if (amount > 0)
                        {
                            item = ItemBuilder.CreateItem(ItemGroups.MiscItems, (int)MiscItems.Letter_of_credit);
                            item.value = RolePlayRealismLootRebalanceCheck ? Mathf.CeilToInt(amount / 3f) : amount;
                            chestItems.AddItem(item);
                        }
                        break;
                    case 6: // Add Potions
                        if (Dice100.SuccessRoll(itemChance))
                        {
                            int bonus = Dice100.SuccessRoll(15) ? 1 : 0;
                            for (int s = 0; s < 1 + bonus; s++)
                            {
                                amount = UnityEngine.Random.Range(1, 5);
                                for (int p = 0; p < amount; p++)
                                {
                                    chestItems.AddItem(ItemBuilder.CreateRandomPotion());
                                }
                            }
                        }
                        break;
                    case 7: // Add Map
                        if (Dice100.SuccessRoll(itemChance))
                        {
                            int bonus = Dice100.SuccessRoll(15) ? 1 : 0;
                            for (int s = 0; s < 1 + bonus; s++)
                            {
                                chestItems.AddItem(new DaggerfallUnityItem(ItemGroups.MiscItems, 8));
                            }
                        }
                        break;
                    case 8: // Add Potion Recipe
                        if (Dice100.SuccessRoll(itemChance))
                        {
                            int bonus = Dice100.SuccessRoll(15) ? 1 : 0;
                            for (int s = 0; s < 1 + bonus; s++)
                            {
                                int recipeIdx = UnityEngine.Random.Range(0, DaggerfallWorkshop.Game.MagicAndEffects.PotionRecipe.classicRecipeKeys.Length);
                                int recipeKey = DaggerfallWorkshop.Game.MagicAndEffects.PotionRecipe.classicRecipeKeys[recipeIdx];
                                item = new DaggerfallUnityItem(ItemGroups.MiscItems, 4) { PotionRecipeKey = recipeKey };
                                chestItems.AddItem(item);
                            }
                        }
                        break;
                    case 9: // Add Painting, no idea if this CreateItem will work, nor if it is properly given a random painting value, etc. Fix this at some point.
                        if (Dice100.SuccessRoll(itemChance))
                        {
                            int bonus = Dice100.SuccessRoll(15) ? 1 : 0;
                            for (int s = 0; s < 1 + bonus; s++)
                            {
                                item = ItemBuilder.CreateItem(ItemGroups.Paintings, (int)Paintings.Painting);
                                item.currentCondition = (int)(item.maxCondition * conditionMod);
                                chestItems.AddItem(item);
                            }
                        }
                        break;
                    case 10: // Add Soul-gem
                        if (Dice100.SuccessRoll(itemChance))
                        {
                            int bonus = Dice100.SuccessRoll(15) ? 1 : 0;
                            for (int s = 0; s < 1 + bonus; s++)
                            {
                                if (Dice100.FailedRoll(25))
                                {
                                    // Empty soul trap
                                    item = ItemBuilder.CreateItem(ItemGroups.MiscItems, (int)MiscItems.Soul_trap);
                                    item.value = 5000;
                                    item.TrappedSoulType = MobileTypes.None;
                                }
                                else
                                {
                                    // Filled soul trap
                                    item = ItemBuilder.CreateRandomlyFilledSoulTrap();
                                }
                                chestItems.AddItem(item);
                            }
                        }
                        break;
                    case 11: // Add Magic Item, it is unidentified when created.
                        if (Dice100.SuccessRoll(itemChance))
                        {
                            int bonus = Dice100.SuccessRoll(15) ? 1 : 0;
                            for (int s = 0; s < 1 + bonus; s++)
                            {
                                item = ItemBuilder.CreateRandomMagicItem(UnityEngine.Random.Range(1, 22), GameManager.Instance.PlayerEntity.Gender, GameManager.Instance.PlayerEntity.Race);
                                item.currentCondition = (int)(item.maxCondition * conditionMod);
                                chestItems.AddItem(item);
                            }
                        }
                        break;
                }
            }

            // This is to check a bit later on in this method if a "set" of armor and weapons should be created instead of just randomly selected armor/weapons.
            int armorSetOdds = 0;
            bool armorWepsCanBeTogether = false;
            int armorSum = 0;
            int wepSum = 0;
            for (int p = 0; p < 4; p++) // Make sure this is only counting the 7 values related to armor and weapon odds, may need to change loop limit value.
            {
                armorSum += modifItemGroupOdds[p + 1];
            }
            for (int n = 0; n < 3; n++)
            {
                wepSum += modifItemGroupOdds[n + 5];
            }
            armorSetOdds = armorSum + wepSum;
            armorWepsCanBeTogether = (armorSum * wepSum) == 0 ? false : true; // If either armor or weapon odds sum to 0, don't allow sets to be generated in this chest.

            // Each loop randomly pick an armor type, add type to list if it succeeds roll, each cycle reduce the values of all types until none are left basically.
            List<int> chosenArmorTypes = new List<int>();
            List<int> armTypes = new List<int>();
            if (modifItemGroupOdds[1] > 0) { armTypes.Add(1); }
            if (modifItemGroupOdds[2] > 0) { armTypes.Add(2); }
            if (modifItemGroupOdds[3] > 0) { armTypes.Add(3); }
            while ((modifItemGroupOdds[1] + modifItemGroupOdds[2] + modifItemGroupOdds[3]) >= 20)
            {
                int randType = armTypes[UnityEngine.Random.Range(0, armTypes.Count)];
                if (randType == 1) { if (Dice100.SuccessRoll(modifItemGroupOdds[1])) { chosenArmorTypes.Add(1); } }
                else if (randType == 2) { if (Dice100.SuccessRoll(modifItemGroupOdds[2])) { chosenArmorTypes.Add(2); } }
                else if (randType == 3) { if (Dice100.SuccessRoll(modifItemGroupOdds[3])) { chosenArmorTypes.Add(3); } }

                modifItemGroupOdds[1] /= 2; modifItemGroupOdds[2] /= 2; modifItemGroupOdds[3] /= 2;
            }

            // Each loop randomly pick a weapon type, add type to list if it succeeds roll, each cycle reduce the values of all types until none are left basically.
            List<int> chosenWeaponTypes = new List<int>();
            List<int> wepTypes = new List<int>();
            if (modifItemGroupOdds[5] > 0) { wepTypes.Add(1); }
            if (modifItemGroupOdds[6] > 0) { wepTypes.Add(2); }
            if (modifItemGroupOdds[7] > 0) { wepTypes.Add(3); }
            while ((modifItemGroupOdds[5] + modifItemGroupOdds[6] + modifItemGroupOdds[7]) >= 20)
            {
                int randType = wepTypes[UnityEngine.Random.Range(0, wepTypes.Count)];
                if (randType == 1) { if (Dice100.SuccessRoll(modifItemGroupOdds[5])) { chosenWeaponTypes.Add(1); } }
                else if (randType == 2) { if (Dice100.SuccessRoll(modifItemGroupOdds[6])) { chosenWeaponTypes.Add(2); } }
                else if (randType == 3) { if (Dice100.SuccessRoll(modifItemGroupOdds[7])) { chosenWeaponTypes.Add(3); } }

                modifItemGroupOdds[5] /= 2; modifItemGroupOdds[6] /= 2; modifItemGroupOdds[7] /= 2;
            }
            int armWepTotCount = chosenArmorTypes.Count + chosenWeaponTypes.Count;

            // Item groups within "itemGroupOdds" are actually looped through and rolled to determine what items in those groups should be populated in chest.
            DaggerfallUnityItem previousItem = null;
            List<int> prevArmorMats = new List<int>();
            List<int> prevWepMats = new List<int>();
            for (int i = 0; i < modifItemGroupOdds.Length; i++)
            {
                int itemChance = modifItemGroupOdds[i];
                float conditionMod = (float)UnityEngine.Random.Range(modifMiscGroupOdds[13], modifMiscGroupOdds[12] + 1) / 100f;

                if (i == 1)
                {
                    if (armorWepsCanBeTogether)
                    {
                        if ((armorSetOdds >= 100 && Dice100.SuccessRoll(armorSetOdds / 5)) || (armWepTotCount >= 5 && Dice100.SuccessRoll((armWepTotCount - 4) * 20)))
                        {
                            DaggerfallUnityItem[] equipmentSet = RetrieveEquipmentSet(oddsAverage, totalRoomValueMod, conditionMod);
                            for (int g = 0; g < equipmentSet.Length; g++) // No blacklist checking/filtering for now, but whatever, not a big deal.
                            {
                                if (equipmentSet[g] != null)
                                    chestItems.AddItem(equipmentSet[g]);
                            }

                            for (int h = 0; h < 7; h++) // Make sure this is only counting the 7 values related to armor and weapon odds, may need to change loop limit value.
                            {
                                modifItemGroupOdds[h + 1] = 0; // When equipment set is generated, set associated group value odds to 0, so no more can generate in this chest.
                            }

                            // Clear these lists so later in method armor/weapons are not created from them, since we already created a "set" of equipment.
                            chosenArmorTypes.Clear();
                            chosenWeaponTypes.Clear();
                            continue;
                        }
                    }
                }

                if (i >= 1 && i <= 3)
                {
                    if (chosenArmorTypes.Count > 0)
                    {
                        for (int k = 0; k < chosenArmorTypes.Count; k++)
                        {
                            DaggerfallUnityItem item = null;
                            item = DetermineLootItem(chosenArmorTypes[k], conditionMod, oddsAverage, totalRoomValueMod, prevArmorMats);
                            if (item != null)
                            {
                                if (previousItem != null) // Here to try and prevent back-to-back duplicate armor pieces.
                                {
                                    if (previousItem.NativeMaterialValue == item.NativeMaterialValue && previousItem.GetEquipSlot() == item.GetEquipSlot()) { continue; }
                                    else if (previousItem.TemplateIndex == item.TemplateIndex) { if (Dice100.SuccessRoll(85)) { continue; } }
                                    else if (previousItem.GetEquipSlot() == item.GetEquipSlot()) { if (Dice100.SuccessRoll(60)) { continue; } }
                                }

                                if (CheckIfBlacklistedItem(item, itemBlacklist)) { continue; } // For now just remove blacklisted items, instead of trying to reroll here.
                                previousItem = item.Clone();
                                prevArmorMats.Add(item.NativeMaterialValue);
                                chestItems.AddItem(item);
                            }
                        }
                    }
                    previousItem = null;
                    continue;
                }

                if (i >= 5 && i <= 7)
                {
                    if (chosenWeaponTypes.Count > 0)
                    {
                        for (int k = 0; k < chosenWeaponTypes.Count; k++)
                        {
                            DaggerfallUnityItem item = null;
                            item = DetermineLootItem(chosenWeaponTypes[k] + 4, conditionMod, oddsAverage, totalRoomValueMod, prevArmorMats, prevWepMats);
                            if (item != null)
                            {
                                if (previousItem != null) // Here to try and prevent back-to-back duplicate weapons.
                                {
                                    if (previousItem.NativeMaterialValue == item.NativeMaterialValue && previousItem.GetItemHands() == item.GetItemHands() && previousItem.TemplateIndex == item.TemplateIndex) { continue; }
                                    else if (previousItem.TemplateIndex == item.TemplateIndex) { if (Dice100.SuccessRoll(85)) { continue; } }
                                    else if (previousItem.GetItemHands() == item.GetItemHands()) { if (Dice100.SuccessRoll(75)) { continue; } }
                                }

                                if (CheckIfBlacklistedItem(item, itemBlacklist)) { continue; } // For now just remove blacklisted items, instead of trying to reroll here.
                                previousItem = item.Clone();
                                prevWepMats.Add(item.NativeMaterialValue);
                                chestItems.AddItem(item);
                            }
                        }
                    }
                    previousItem = null;
                    continue;
                }

                int failedRegenAttempts = 0;
                while (Dice100.SuccessRoll(itemChance))
                {
                    if (failedRegenAttempts >= 8) // Allow item-groups that failed to create an item due to being blacklisted, have 8 tries before forcing their generation loop to stop.
                        break;

                    if (itemChance >= 60) // This is to try and reduce certain items basically always appearing, attempting to make it feel more "random."
                    {
                        if (Dice100.SuccessRoll(35))
                        {
                            itemChance /= 2;
                            continue;
                        }
                    }

                    DaggerfallUnityItem item = null;
                    item = DetermineLootItem(i, conditionMod, oddsAverage, totalRoomValueMod);

                    if (item != null) // To prevent null object reference errors if item could not be created for whatever reason by DetermineLootItem method.
                    {
                        if (CheckIfBlacklistedItem(item, itemBlacklist)) // Prevent blacklisted items from being created.
                        {
                            failedRegenAttempts++;
                            continue;
                        }

                        if (i >= 8 && i <= 9) // Here to try and prevent back-to-back duplicate clothing pieces.
                        {
                            if (previousItem != null)
                            {
                                if (previousItem.TemplateIndex == item.TemplateIndex && previousItem.dyeColor == item.dyeColor) { continue; }
                                else if (previousItem.GetEquipSlot() == item.GetEquipSlot()) { if (Dice100.SuccessRoll(20)) { continue; } }
                            }
                        }

                        previousItem = item.Clone();
                        chestItems.AddItem(item);
                    }

                    itemChance /= 2;
                }
            }
        }

        public static DaggerfallUnityItem DetermineLootItem(int itemGroupLLC, float conditionMod, float oddsAverage, int totalRoomValueMod, List<int> pArmMats = null, List<int> pWepMats = null)
        {
            Genders gender = GameManager.Instance.PlayerEntity.Gender;
            Races race = GameManager.Instance.PlayerEntity.Race;
            DaggerfallUnityItem item = null;
            Array enumArray;
            int enumIndex = -1;

            switch (itemGroupLLC)
            {
                default:
                    return null;
                case (int)ChestLootItemGroups.Drugs:
                    return ItemBuilder.CreateRandomDrug(); // Just vanilla selection for now.
                case (int)ChestLootItemGroups.LightArmor:
                    return GenerateLightArmor(gender, race, oddsAverage, totalRoomValueMod, conditionMod);
                case (int)ChestLootItemGroups.MediumArmor:
                    return GenerateMediumArmor(gender, race, oddsAverage, totalRoomValueMod, conditionMod);
                case (int)ChestLootItemGroups.HeavyArmor:
                    return GenerateHeavyArmor(gender, race, oddsAverage, totalRoomValueMod, conditionMod);
                case (int)ChestLootItemGroups.Shields:
                    enumArray = Enum.GetValues(typeof(Shields));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    item = ItemBuilder.CreateArmor(gender, race, (Armor)enumArray.GetValue(enumIndex), (ArmorMaterialTypes)RollWeaponOrArmorMaterial(oddsAverage, totalRoomValueMod, false, true));
                    item.currentCondition = (int)(item.maxCondition * conditionMod);
                    return item;
                case (int)ChestLootItemGroups.SmallWeapons:
                    return GenerateSmallWeapon(oddsAverage, totalRoomValueMod, conditionMod);
                case (int)ChestLootItemGroups.MediumWeapons:
                    return GenerateMediumWeapon(oddsAverage, totalRoomValueMod, conditionMod);
                case (int)ChestLootItemGroups.LargeWeapons:
                    return GenerateLargeWeapon(oddsAverage, totalRoomValueMod, conditionMod);
                case (int)ChestLootItemGroups.MensClothing:
                    if (gender == Genders.Female)
                        return null;
                    enumArray = Enum.GetValues(typeof(MensClothing));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    item = ItemBuilder.CreateMensClothing((DaggerfallWorkshop.Game.Items.MensClothing)enumArray.GetValue(enumIndex), race, -1, ItemBuilder.RandomClothingDye());
                    item.currentCondition = (int)(item.maxCondition * conditionMod);
                    return item;
                case (int)ChestLootItemGroups.WomensClothing:
                    if (gender == Genders.Male)
                        return null;
                    enumArray = Enum.GetValues(typeof(WomensClothing));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    item = ItemBuilder.CreateWomensClothing((DaggerfallWorkshop.Game.Items.WomensClothing)enumArray.GetValue(enumIndex), race, -1, ItemBuilder.RandomClothingDye());
                    item.currentCondition = (int)(item.maxCondition * conditionMod);
                    return item;
                case (int)ChestLootItemGroups.Books:
                    return GenerateRandomBook(conditionMod);
                case (int)ChestLootItemGroups.Gems:
                    return GenerateRandomGem();
                case (int)ChestLootItemGroups.Jewelry:
                    return GenerateRandomJewelry(conditionMod);
                case (int)ChestLootItemGroups.Tools:
                    return GenerateRandomTool(conditionMod);
                case (int)ChestLootItemGroups.Lights:
                    if (Dice100.SuccessRoll(15)) { item = ItemBuilder.CreateItem(ItemGroups.UselessItems2, (int)UselessItems2.Lantern); }
                    else if (Dice100.SuccessRoll(60)) { item = ItemBuilder.CreateItem(ItemGroups.UselessItems2, (int)UselessItems2.Torch); }
                    else { item = ItemBuilder.CreateItem(ItemGroups.UselessItems2, (int)UselessItems2.Candle); }
                    item.currentCondition = (int)(item.maxCondition * conditionMod);
                    return item;
                case (int)ChestLootItemGroups.Supplies:
                    return GenerateRandomSupplies();
                case (int)ChestLootItemGroups.BlessedItems:
                    enumArray = Enum.GetValues(typeof(BlessedItems));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    item = (enumIndex <= 0) ? ItemBuilder.CreateItem(ItemGroups.MiscellaneousIngredients1, (int)MiscellaneousIngredients1.Holy_relic) : ItemBuilder.CreateItem(ItemGroups.ReligiousItems, (int)enumArray.GetValue(enumIndex));
                    item.currentCondition = (int)(item.maxCondition * conditionMod);
                    return item;
                case (int)ChestLootItemGroups.ReligiousStuff:
                    enumArray = Enum.GetValues(typeof(ReligiousStuff));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    item = ItemBuilder.CreateItem(ItemGroups.ReligiousItems, (int)enumArray.GetValue(enumIndex));
                    item.currentCondition = (int)(item.maxCondition * conditionMod);
                    return item;
                case (int)ChestLootItemGroups.GenericPlants:
                    enumArray = Enum.GetValues(typeof(GenericPlants));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    item = ItemBuilder.CreateItem(ItemGroups.PlantIngredients1, (int)enumArray.GetValue(enumIndex));
                    item.stackCount = UnityEngine.Random.Range(1, 7);
                    return item;
                case (int)ChestLootItemGroups.ColdClimatePlants:
                    enumArray = Enum.GetValues(typeof(ColdClimatePlants));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    item = ItemBuilder.CreateItem(ItemGroups.PlantIngredients1, (int)enumArray.GetValue(enumIndex));
                    item.stackCount = UnityEngine.Random.Range(1, 7);
                    return item;
                case (int)ChestLootItemGroups.WarmClimatePlants:
                    enumArray = Enum.GetValues(typeof(WarmClimatePlants));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    item = ItemBuilder.CreateItem(ItemGroups.PlantIngredients2, (int)enumArray.GetValue(enumIndex));
                    item.stackCount = UnityEngine.Random.Range(1, 7);
                    return item;
                case (int)ChestLootItemGroups.CommonCreatureParts:
                    enumArray = Enum.GetValues(typeof(CommonCreatureParts));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    if ((int)enumArray.GetValue(enumIndex) == (int)CreatureIngredients1.Spider_venom) // Just doing a simple but messy chain of if-else statements here since the vanilla ingredients groups are all over the place.
                        return ItemBuilder.CreateItem(ItemGroups.CreatureIngredients1, (int)CreatureIngredients1.Spider_venom);
                    else if ((int)enumArray.GetValue(enumIndex) == (int)CreatureIngredients1.Snake_venom)
                        return ItemBuilder.CreateItem(ItemGroups.CreatureIngredients1, (int)CreatureIngredients1.Snake_venom);
                    else if ((int)enumArray.GetValue(enumIndex) == (int)CreatureIngredients2.Small_scorpion_stinger)
                        return ItemBuilder.CreateItem(ItemGroups.CreatureIngredients2, (int)CreatureIngredients2.Small_scorpion_stinger);
                    else if ((int)enumArray.GetValue(enumIndex) == (int)CreatureIngredients1.Giant_blood)
                        return ItemBuilder.CreateItem(ItemGroups.CreatureIngredients1, (int)CreatureIngredients1.Giant_blood);
                    else if ((int)enumArray.GetValue(enumIndex) == (int)MiscellaneousIngredients1.Big_tooth)
                        return ItemBuilder.CreateItem(ItemGroups.MiscellaneousIngredients1, (int)MiscellaneousIngredients1.Big_tooth);
                    else if ((int)enumArray.GetValue(enumIndex) == (int)MiscellaneousIngredients1.Medium_tooth)
                        return ItemBuilder.CreateItem(ItemGroups.MiscellaneousIngredients1, (int)MiscellaneousIngredients1.Medium_tooth);
                    else if ((int)enumArray.GetValue(enumIndex) == (int)MiscellaneousIngredients1.Small_tooth)
                        return ItemBuilder.CreateItem(ItemGroups.MiscellaneousIngredients1, (int)MiscellaneousIngredients1.Small_tooth);
                    else
                        return ItemBuilder.CreateItem(ItemGroups.CreatureIngredients1, (int)CreatureIngredients1.Orcs_blood);
                case (int)ChestLootItemGroups.UncommonCreatureParts:
                    enumArray = Enum.GetValues(typeof(UncommonCreatureParts));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    if ((int)enumArray.GetValue(enumIndex) == (int)CreatureIngredients1.Werewolfs_blood) // Just doing a simple but messy chain of if-else statements here since the vanilla ingredients groups are all over the place.
                        return ItemBuilder.CreateItem(ItemGroups.CreatureIngredients1, (int)CreatureIngredients1.Werewolfs_blood);
                    else if ((int)enumArray.GetValue(enumIndex) == (int)CreatureIngredients3.Wereboar_tusk)
                        return ItemBuilder.CreateItem(ItemGroups.CreatureIngredients3, (int)CreatureIngredients3.Wereboar_tusk);
                    else if ((int)enumArray.GetValue(enumIndex) == (int)CreatureIngredients3.Nymph_hair)
                        return ItemBuilder.CreateItem(ItemGroups.CreatureIngredients3, (int)CreatureIngredients3.Nymph_hair);
                    else if ((int)enumArray.GetValue(enumIndex) == (int)CreatureIngredients1.Wraith_essence)
                        return ItemBuilder.CreateItem(ItemGroups.CreatureIngredients1, (int)CreatureIngredients1.Wraith_essence);
                    else if ((int)enumArray.GetValue(enumIndex) == (int)CreatureIngredients1.Ectoplasm)
                        return ItemBuilder.CreateItem(ItemGroups.CreatureIngredients1, (int)CreatureIngredients1.Ectoplasm);
                    else if ((int)enumArray.GetValue(enumIndex) == (int)CreatureIngredients1.Ghouls_tongue)
                        return ItemBuilder.CreateItem(ItemGroups.CreatureIngredients1, (int)CreatureIngredients1.Ghouls_tongue);
                    else if ((int)enumArray.GetValue(enumIndex) == (int)CreatureIngredients1.Troll_blood)
                        return ItemBuilder.CreateItem(ItemGroups.CreatureIngredients1, (int)CreatureIngredients1.Troll_blood);
                    else if ((int)enumArray.GetValue(enumIndex) == (int)CreatureIngredients2.Giant_scorpion_stinger)
                        return ItemBuilder.CreateItem(ItemGroups.CreatureIngredients2, (int)CreatureIngredients2.Giant_scorpion_stinger);
                    else if ((int)enumArray.GetValue(enumIndex) == (int)CreatureIngredients2.Mummy_wrappings)
                        return ItemBuilder.CreateItem(ItemGroups.CreatureIngredients2, (int)CreatureIngredients2.Mummy_wrappings);
                    else if ((int)enumArray.GetValue(enumIndex) == (int)CreatureIngredients2.Gryphon_Feather)
                        return ItemBuilder.CreateItem(ItemGroups.CreatureIngredients2, (int)CreatureIngredients2.Gryphon_Feather);
                    else if ((int)enumArray.GetValue(enumIndex) == (int)MiscellaneousIngredients2.Ivory)
                        return ItemBuilder.CreateItem(ItemGroups.MiscellaneousIngredients2, (int)MiscellaneousIngredients2.Ivory);
                    else
                        return ItemBuilder.CreateItem(ItemGroups.MiscellaneousIngredients2, (int)MiscellaneousIngredients2.Pearl);
                case (int)ChestLootItemGroups.RareCreatureParts:
                    enumArray = Enum.GetValues(typeof(RareCreatureParts));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    if ((int)enumArray.GetValue(enumIndex) == (int)CreatureIngredients1.Fairy_dragon_scales) // Just doing a simple but messy chain of if-else statements here since the vanilla ingredients groups are all over the place.
                        return ItemBuilder.CreateItem(ItemGroups.CreatureIngredients1, (int)CreatureIngredients1.Fairy_dragon_scales);
                    else if ((int)enumArray.GetValue(enumIndex) == (int)CreatureIngredients3.Unicorn_horn)
                        return ItemBuilder.CreateItem(ItemGroups.CreatureIngredients3, (int)CreatureIngredients3.Unicorn_horn);
                    else if ((int)enumArray.GetValue(enumIndex) == (int)CreatureIngredients1.Gorgon_snake)
                        return ItemBuilder.CreateItem(ItemGroups.CreatureIngredients1, (int)CreatureIngredients1.Gorgon_snake);
                    else if ((int)enumArray.GetValue(enumIndex) == (int)CreatureIngredients1.Lich_dust)
                        return ItemBuilder.CreateItem(ItemGroups.CreatureIngredients1, (int)CreatureIngredients1.Lich_dust);
                    else if ((int)enumArray.GetValue(enumIndex) == (int)CreatureIngredients2.Dragons_scales)
                        return ItemBuilder.CreateItem(ItemGroups.CreatureIngredients2, (int)CreatureIngredients2.Dragons_scales);
                    else if ((int)enumArray.GetValue(enumIndex) == (int)CreatureIngredients1.Basilisk_eye)
                        return ItemBuilder.CreateItem(ItemGroups.CreatureIngredients1, (int)CreatureIngredients1.Basilisk_eye);
                    else if ((int)enumArray.GetValue(enumIndex) == (int)CreatureIngredients1.Daedra_heart)
                        return ItemBuilder.CreateItem(ItemGroups.CreatureIngredients1, (int)CreatureIngredients1.Daedra_heart);
                    else
                        return ItemBuilder.CreateItem(ItemGroups.CreatureIngredients1, (int)CreatureIngredients1.Saints_hair);
                case (int)ChestLootItemGroups.LiquidSolvents:
                    enumArray = Enum.GetValues(typeof(LiquidSolvents));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    item = ItemBuilder.CreateItem(ItemGroups.MiscellaneousIngredients1, (int)enumArray.GetValue(enumIndex));
                    item.stackCount = UnityEngine.Random.Range(1, 4);
                    return item;
                case (int)ChestLootItemGroups.TraceMetals:
                    enumArray = Enum.GetValues(typeof(TraceMetals));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    item = ItemBuilder.CreateItem(ItemGroups.MetalIngredients, (int)enumArray.GetValue(enumIndex));
                    item.stackCount = UnityEngine.Random.Range(1, 4);
                    return item;
            }
        }

        public static int RollWeaponOrArmorMaterial(float oddsAverage, int totalRoomValueMod, bool isWeapon = true, bool isShield = false, List<int> pArmMats = null, List<int> pWepMats = null)
        {
            // Iron, Steel, Silver, Elven, Dwarven, Mithril, Adamantium, Ebony, Orcish, Daedric
            List<float> matOdds = new List<float>() { 17.8f, 16.5f, 15.0f, 13.7f, 11.6f, 9.5f, 6.8f, 4.8f, 2.8f, 1.5f };

            if (isShield)
            {
                int shieldMat = UnityEngine.Random.Range(0, 4);

                if (shieldMat == 0)
                    return (int)ArmorMaterialTypes.Leather;
                else if (shieldMat == 1)
                    return (int)ArmorMaterialTypes.Chain;
            }

            float cMod = (oddsAverage * 0.2f) + (totalRoomValueMod * 0.05f);

            if (oddsAverage <= 2.5f)
            {
                if (cMod <= 0)
                    matOdds = new List<float>() { 54.0f-(cMod*1.5f), 46.5f-cMod, 42.2f-(cMod*0.5f), 26.4f+(cMod*0.2f), 10.3f+(cMod*0.4f), 6.8f+(cMod*0.6f), 3.7f+(cMod*0.8f), 2.7f+cMod, 1.7f+(cMod*1.2f), 0.8f+(cMod*1.5f) };
                else
                    matOdds = new List<float>() { 54.0f-(cMod*1.5f), 46.5f-cMod, 42.2f-(cMod*0.5f), 26.4f+(cMod*1.5f), 10.3f+(cMod*1.2f), 6.8f+cMod, 3.7f+(cMod*0.8f), 2.7f+(cMod*0.6f), 1.7f+(cMod*0.4f), 0.8f+(cMod*0.2f) };
            }
            else if (oddsAverage > 2.5f && oddsAverage <= 5.25f)
            {
                if (cMod <= 0)
                    matOdds = new List<float>() { 30.0f-(cMod*1.5f), 26.0f-cMod, 19.5f-(cMod*0.5f), 23.8f+(cMod*0.2f), 15.8f+(cMod*0.4f), 11.4f+(cMod*0.6f), 4.8f+(cMod*0.8f), 3.5f+cMod, 2.2f+(cMod*1.2f), 1.0f+(cMod*1.5f) };
                else
                    matOdds = new List<float>() { 30.0f-(cMod*1.5f), 26.0f-cMod, 19.5f-(cMod*0.5f), 23.8f+cMod, 15.8f+(cMod*0.9f), 11.4f+(cMod*0.8f), 4.8f+(cMod*0.7f), 3.5f+(cMod*0.6f), 2.2f+(cMod*0.5f), 1.0f+(cMod*0.3f) };
            }
            else
            {
                if (cMod <= 0)
                    matOdds = new List<float>() { 17.8f-(cMod*1.5f), 15.5f-cMod, 12.3f-(cMod*0.5f), 13.5f+(cMod*0.2f), 10.7f+(cMod*0.3f), 8.4f+(cMod*0.4f), 6.3f+(cMod*0.5f), 4.6f+(cMod*0.6f), 2.9f+(cMod*0.7f), 1.4f+(cMod*0.9f) };
                else
                    matOdds = new List<float>() { 17.8f-(cMod*2f), 15.5f-(cMod*1.5f), 12.3f-cMod, 13.5f+(cMod*0.4f), 10.7f+(cMod*0.7f), 8.4f+cMod, 6.3f+(cMod*1.3f), 4.6f+(cMod*1.6f), 2.9f+(cMod*1.8f), 1.4f+(cMod*2f) };
            }

            // These are here to try and reduce "high tier" armor and weapons from showing up multiple times in the same chest, key word is "try."
            if (!isWeapon && pArmMats != null && pArmMats.Count > 0)
            {
                for (int i = 0; i < pArmMats.Count; i++)
                {
                    if (pArmMats[i] >= (int)ArmorMaterialTypes.Adamantium && pArmMats[i] <= (int)ArmorMaterialTypes.Daedric)
                    {
                        matOdds[6] /= 2f; matOdds[7] /= 2f; matOdds[8] /= 2f; matOdds[9] /= 2f; break;
                    }
                }
            }

            if (isWeapon && pWepMats != null && pWepMats.Count > 0)
            {
                for (int i = 0; i < pWepMats.Count; i++)
                {
                    if (pWepMats[i] >= (int)WeaponMaterialTypes.Adamantium && pWepMats[i] <= (int)WeaponMaterialTypes.Daedric)
                    {
                        matOdds[6] /= 2f; matOdds[7] /= 2f; matOdds[8] /= 2f; matOdds[9] /= 2f; break;
                    }
                }
            }

            // Makes sure any matOdds value can't go below 0.5f at the lowest.
            for (int i = 0; i < matOdds.Count; i++)
            {
                if (matOdds[i] < 0.5f) { matOdds[i] = 0.5f; }
            }

            // Normalize matOdds values to ensure they all add up to 100.
            float totalOddsSum = matOdds.Sum();
            for (int i = 0; i < matOdds.Count; i++)
            {
                matOdds[i] = (matOdds[i] / totalOddsSum) * 100f;
            }

            // Choose a material using the weighted random selection algorithm.
            float randomValue = UnityEngine.Random.Range(0f, 101f);
            float cumulativeOdds = 0f;
            int index = 0;
            for (int i = 0; i < matOdds.Count; i++)
            {
                cumulativeOdds += matOdds[i];
                if (randomValue < cumulativeOdds)
                {
                    index = i; // Material i is chosen
                    break;
                }
            }

            if (isWeapon)
            {
                return index;
            }
            else
            {
                return 0x0200 + index;
            }
        }

        public static bool CheckIfBlacklistedItem(DaggerfallUnityItem item, int[] itemBlacklist)
        {
            for (int i = 0; i < itemBlacklist.Length; i++)
            {
                if (item.TemplateIndex == itemBlacklist[i])
                    return true;
            }
            return false;
        }
    }
}