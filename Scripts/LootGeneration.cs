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
using System.Collections.Generic;
using System;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Formulas;
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

            float oddsAverage = 0f;

            switch (llcObj.ChestMaterial) // For now, these are intended to be the modifier each "lootOdds" value is going to be multipled by the end before item generation rolls happen.
            {
                default:
                case ChestMaterials.Wood:
                    oddsAverage += 0.25f; break;
                case ChestMaterials.Iron:
                    oddsAverage += 0.5f; break;
                case ChestMaterials.Steel:
                    oddsAverage += 1.25f; break;
                case ChestMaterials.Orcish:
                    oddsAverage += 2.75f; break;
                case ChestMaterials.Mithril:
                    oddsAverage += 1.5f; break;
                case ChestMaterials.Dwarven:
                    oddsAverage += 2.0f; break;
                case ChestMaterials.Adamantium:
                    oddsAverage += 3.0f; break;
                case ChestMaterials.Daedric:
                    oddsAverage += 4.25f; break;
            }

            switch (llcObj.LockMaterial) // For now, these are intended to be the modifier each "lootOdds" value is going to be multipled by the end before item generation rolls happen.
            {
                default:
                case LockMaterials.Wood:
                    oddsAverage += 0.25f; break;
                case LockMaterials.Iron:
                    oddsAverage += 0.5f; break;
                case LockMaterials.Steel:
                    oddsAverage += 1.25f; break;
                case LockMaterials.Orcish:
                    oddsAverage += 2.75f; break;
                case LockMaterials.Mithril:
                    oddsAverage += 1.5f; break;
                case LockMaterials.Dwarven:
                    oddsAverage += 2.0f; break;
                case LockMaterials.Adamantium:
                    oddsAverage += 3.0f; break;
                case LockMaterials.Daedric:
                    oddsAverage += 4.25f; break;
            }

            // For now, these are intended to be the modifier each "lootOdds" value is going to be multipled by the end before item generation rolls happen, still placeholder.
            if (llcObj.LockComplexity < 0 || (llcObj.LockComplexity >= 0 && llcObj.LockComplexity <= 19)) { oddsAverage += 0f; }
            else if (llcObj.LockComplexity >= 20 && llcObj.LockComplexity <= 39) { oddsAverage += 0.25f; }
            else if (llcObj.LockComplexity >= 40 && llcObj.LockComplexity <= 59) { oddsAverage += 0.5f; }
            else if (llcObj.LockComplexity >= 60 && llcObj.LockComplexity <= 79) { oddsAverage += 0.75f; }
            else { oddsAverage += 1.0f; }

            for (int i = 0; i < miscGroupOdds.Length; i++) // Heavily placeholder for now, but don't feel like going too deep into this right now.
            {
                if (i <= 0 || i == 12 || i == 13)
                    continue;
                else
                    miscGroupOdds[i] = (int)Mathf.Round(miscGroupOdds[i] * ((oddsAverage) / 6f));
            }

            for (int i = 0; i < itemGroupOdds.Length; i++) // Odds are modified in this loop for each item group within the "itemGroupOdds" array.
            {
                if (itemGroupOdds[i] <= 0)
                    continue;
                else if (itemGroupOdds[i] > 0 && itemGroupOdds[i] <= 15)
                    itemGroupOdds[i] = (int)Mathf.Round((itemGroupOdds[i] + (totalRoomValueMod / 12f)) * (oddsAverage / 2f));
                else if (itemGroupOdds[i] > 15 && itemGroupOdds[i] <= 50)
                    itemGroupOdds[i] = (int)Mathf.Round((itemGroupOdds[i] + (totalRoomValueMod / 4f)) * (oddsAverage / 2f));
                else
                    itemGroupOdds[i] = (int)Mathf.Round((itemGroupOdds[i] + (totalRoomValueMod / 2f)) * (oddsAverage / 2f));
            }

            // Debug log string creator, for testing purposes only.
            string baseString = "";
            for (int i = 0; i < miscGroupOdds.Length; i++)
            {
                string valueString = "[" + miscGroupOdds[i].ToString() + "]";
                baseString = baseString + ", " + valueString;
            }
            Debug.LogFormat("Misc Group Odds For Chest: {0}", baseString);

            for (int i = 0; i < miscGroupOdds.Length; i++) // Groups within "miscGroupOdds" are actually looped through and rolled to determine what items in those groups should be populated in chest.
            {
                int itemChance = miscGroupOdds[i];
                float conditionMod = (float)UnityEngine.Random.Range(miscGroupOdds[13], miscGroupOdds[12] + 1) / 100f;
                DaggerfallUnityItem item = null;
                int amount = 0;

                switch (i)
                {
                    default:
                        break;
                    case 0: // Add Gold
                        amount = (Dice100.SuccessRoll(itemChance)) ? UnityEngine.Random.Range(miscGroupOdds[2], miscGroupOdds[3] + 1) : 0;
                        if (amount > 0)
                            chestItems.AddItem(ItemBuilder.CreateGoldPieces(RolePlayRealismLootRebalanceCheck ? Mathf.CeilToInt(amount / 3f) : amount));
                        break;
                    case 1: // Add Letter of Credit
                        amount = (Dice100.SuccessRoll(itemChance)) ? UnityEngine.Random.Range(miscGroupOdds[4], miscGroupOdds[5] + 1) : 0;
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
                            amount = UnityEngine.Random.Range(1, 5);
                            for (int p = 0; p < amount; p++)
                            {
                                chestItems.AddItem(ItemBuilder.CreateRandomPotion());
                            }
                        }
                        break;
                    case 7: // Add Map
                        if (Dice100.SuccessRoll(itemChance))
                            chestItems.AddItem(new DaggerfallUnityItem(ItemGroups.MiscItems, 8));
                        break;
                    case 8: // Add Potion Recipe
                        if (Dice100.SuccessRoll(itemChance))
                        {
                            int recipeIdx = UnityEngine.Random.Range(0, DaggerfallWorkshop.Game.MagicAndEffects.PotionRecipe.classicRecipeKeys.Length);
                            int recipeKey = DaggerfallWorkshop.Game.MagicAndEffects.PotionRecipe.classicRecipeKeys[recipeIdx];
                            item = new DaggerfallUnityItem(ItemGroups.MiscItems, 4) { PotionRecipeKey = recipeKey };
                            chestItems.AddItem(item);
                        }
                        break;
                    case 9: // Add Painting, no idea if this CreateItem will work, nor if it is properly given a random painting value, etc. Fix this at some point.
                        if (Dice100.SuccessRoll(itemChance))
                        {
                            item = ItemBuilder.CreateItem(ItemGroups.Paintings, (int)Paintings.Painting);
                            item.currentCondition = (int)(item.maxCondition * conditionMod);
                            chestItems.AddItem(item);
                        }
                        break;
                    case 10: // Add Soul-gem
                        if (Dice100.SuccessRoll(itemChance))
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
                        break;
                    case 11: // Add Magic Item, it is unidentified when created.
                        if (Dice100.SuccessRoll(itemChance))
                        {
                            item = ItemBuilder.CreateRandomMagicItem(UnityEngine.Random.Range(1, 22), GameManager.Instance.PlayerEntity.Gender, GameManager.Instance.PlayerEntity.Race);
                            item.currentCondition = (int)(item.maxCondition * conditionMod);
                            chestItems.AddItem(item);
                        }
                        break;
                }
            }

            // Debug log string creator, for testing purposes only.
            baseString = "";
            for (int i = 0; i < itemGroupOdds.Length; i++)
            {
                string valueString = "[" + itemGroupOdds[i].ToString() + "]";
                baseString = baseString + ", " + valueString;
            }
            Debug.LogFormat("Item Group Odds For Chest: {0}", baseString);

            for (int i = 0; i < itemGroupOdds.Length; i++) // Item groups within "itemGroupOdds" are actually looped through and rolled to determine what items in those groups should be populated in chest.
            {
                int itemChance = itemGroupOdds[i];
                float conditionMod = (float)UnityEngine.Random.Range(miscGroupOdds[13], miscGroupOdds[12] + 1) / 100f;

                if (itemChance <= 0)
                    continue;

                int failedRegenAttempts = 0;
                while (Dice100.SuccessRoll(itemChance))
                {
                    if (failedRegenAttempts >= 8) // Allow item-groups that failed to create an item due to being blacklisted, have 8 tries before forcing their generation loop to stop.
                        break;

                    if (itemChance >= 60) // This is to try and reduce certain items basically always appearing due to the multiplication of odds, will see if it makes things better or worse.
                    {
                        if (Dice100.SuccessRoll(20))
                        {
                            itemChance -= 100;
                            continue;
                        }
                        else if (itemChance >= 150) // This is to attempt to reduce the seeming "consistency" of high probability loot filling up any high-value chests with a large multipler.
                        {
                            if (CoinFlip())
                                itemChance -= 100;
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

                        chestItems.AddItem(item);
                    }

                    itemChance -= 100;
                }
            }
        }

        public static DaggerfallUnityItem DetermineLootItem(int itemGroupLLC, float conditionMod, float oddsAverage, int totalRoomValueMod)
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
                    enumArray = Enum.GetValues(typeof(MediumWeapons));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    if (enumIndex >= 8) // Arrows
                    {
                        item = ItemBuilder.CreateWeapon(Weapons.Arrow, WeaponMaterialTypes.Steel);
                        item.stackCount = UnityEngine.Random.Range(8, 36);
                        return item;
                    }
                    else // Other Medium Sized Weapons
                    {
                        item = ItemBuilder.CreateWeapon((Weapons)enumArray.GetValue(enumIndex), (WeaponMaterialTypes)RollWeaponOrArmorMaterial(oddsAverage, totalRoomValueMod));
                        item.currentCondition = (int)(item.maxCondition * conditionMod);
                        return item;
                    }
                case (int)ChestLootItemGroups.LargeWeapons:
                    enumArray = Enum.GetValues(typeof(LargeWeapons));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    item = ItemBuilder.CreateWeapon((Weapons)enumArray.GetValue(enumIndex), (WeaponMaterialTypes)RollWeaponOrArmorMaterial(oddsAverage, totalRoomValueMod));
                    item.currentCondition = (int)(item.maxCondition * conditionMod);
                    return item;
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

        public static int RollWeaponOrArmorMaterial(float oddsAverage, int totalRoomValueMod, bool isWeapon = true, bool isShield = false) // Trying out new method here, will see if it comes out any better.
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

            float cMod = (oddsAverage * 0.5f) + (totalRoomValueMod * 0.1f);

            if (oddsAverage <= 2.5f)
            {
                if (cMod <= 0)
                    matOdds = new List<float>() { 24.1f-(cMod*1.5f), 20.9f-cMod, 18.0f-(cMod*0.5f), 12.0f+(cMod*0.2f), 9.3f+(cMod*0.4f), 6.8f+(cMod*0.6f), 3.7f+(cMod*0.8f), 2.7f+cMod, 1.7f+(cMod*1.2f), 0.8f+(cMod*1.5f) };
                else
                    matOdds = new List<float>() { 24.1f-(cMod*1.5f), 20.9f-cMod, 18.0f-(cMod*0.5f), 12.0f+(cMod*1.5f), 9.3f+(cMod*1.2f), 6.8f+cMod, 3.7f+(cMod*0.8f), 2.7f+(cMod*0.6f), 1.7f+(cMod*0.4f), 0.8f+(cMod*0.2f) };
            }
            else if (oddsAverage > 2.5f && oddsAverage <= 5.25f)
            {
                if (cMod <= 0)
                    matOdds = new List<float>() { 8.8f-(cMod*1.5f), 9.5f-cMod, 10.1f-(cMod*0.5f), 19.5f+(cMod*0.2f), 16.3f+(cMod*0.4f), 13.1f+(cMod*0.6f), 8.9f+(cMod*0.8f), 6.7f+cMod, 4.3f+(cMod*1.2f), 2.8f+(cMod*1.5f) };
                else
                    matOdds = new List<float>() { 8.8f-(cMod*1.5f), 9.5f-cMod, 10.1f-(cMod*0.5f), 19.5f+cMod, 16.3f+(cMod*0.9f), 13.1f+(cMod*0.8f), 8.9f+(cMod*0.7f), 6.7f+(cMod*0.6f), 4.3f+(cMod*0.5f), 2.8f+(cMod*0.3f) };
            }
            else
            {
                if (cMod <= 0)
                    matOdds = new List<float>() { 4.3f-(cMod*1.5f), 6.5f-cMod, 7.8f-(cMod*0.5f), 10.4f+(cMod*0.2f), 11.6f+(cMod*0.3f), 12.2f+(cMod*0.4f), 12.9f+(cMod*0.5f), 15.0f+(cMod*0.6f), 10.8f+(cMod*0.7f), 8.5f+(cMod*0.9f) };
                else
                    matOdds = new List<float>() { 4.3f-(cMod*2f), 6.5f-(cMod*1.5f), 7.8f-cMod, 10.4f+(cMod*0.4f), 11.6f+(cMod*0.7f), 12.2f+cMod, 12.9f+(cMod*1.3f), 15.0f+(cMod*1.6f), 10.8f+(cMod*1.8f), 8.5f+(cMod*2f) };
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