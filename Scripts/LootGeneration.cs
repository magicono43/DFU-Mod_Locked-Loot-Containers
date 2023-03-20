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

namespace LockedLootContainers
{
    public partial class LockedLootContainersMain
    {
        public static void PopulateChestLoot(LLCObject llcObj, int totalRoomValueMod, int[] dungTypeMiscOdds, int[] dungTypeItemOdds)
        {
            int[] miscGroupOdds = (int[])dungTypeMiscOdds.Clone(); // Note to self, make sure to clone an array like this if you plan on having different "instances" of changing the values inside.
            int[] itemGroupOdds = (int[])dungTypeItemOdds.Clone();
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

            float lowOddsMod = 0f;
            float midOddsMod = 0f;
            float highOddsMod = 0f;
            float oddsAverage = 1f;

            switch (llcObj.ChestMaterial) // For now, these are intended to be the modifier each "lootOdds" value is going to be multipled by the end before item generation rolls happen.
            {
                default:
                case ChestMaterials.Wood:
                    lowOddsMod += 0.5f; midOddsMod += 0.75f; highOddsMod += 1.0f; break; // Average = 1.5
                case ChestMaterials.Iron:
                    lowOddsMod += 1.0f; midOddsMod += 1.25f; highOddsMod += 1.5f; break; // Average = 2.5
                case ChestMaterials.Steel:
                    lowOddsMod += 1.75f; midOddsMod += 2.0f; highOddsMod += 2.25f; break; // Average = 4.0
                case ChestMaterials.Orcish:
                    lowOddsMod += 2.75f; midOddsMod += 3.25f; highOddsMod += 3.75f; break; // Average = 6.5
                case ChestMaterials.Mithril:
                    lowOddsMod += 2.5f; midOddsMod += 1.75f; highOddsMod += 1.75f; break; // Average = 4.0
                case ChestMaterials.Dwarven:
                    lowOddsMod += 3.5f; midOddsMod += 2.0f; highOddsMod += 2.0f; break; // Average = 5.0
                case ChestMaterials.Adamantium:
                    lowOddsMod += 4.5f; midOddsMod += 2.25f; highOddsMod += 3.0f; break; // Average = 6.5
                case ChestMaterials.Daedric:
                    lowOddsMod += 7.0f; midOddsMod += 4.0f; highOddsMod += 2.5f; break; // Average = 9.0
            }

            switch (llcObj.LockMaterial) // For now, these are intended to be the modifier each "lootOdds" value is going to be multipled by the end before item generation rolls happen.
            {
                default:
                case LockMaterials.Wood:
                    lowOddsMod += 0.5f; midOddsMod += 0.75f; highOddsMod += 1.0f; break;
                case LockMaterials.Iron:
                    lowOddsMod += 1.0f; midOddsMod += 1.25f; highOddsMod += 1.5f; break;
                case LockMaterials.Steel:
                    lowOddsMod += 1.75f; midOddsMod += 2.0f; highOddsMod += 2.25f; break;
                case LockMaterials.Orcish:
                    lowOddsMod += 2.75f; midOddsMod += 3.25f; highOddsMod += 3.75f; break;
                case LockMaterials.Mithril:
                    lowOddsMod += 2.5f; midOddsMod += 1.75f; highOddsMod += 1.75f; break;
                case LockMaterials.Dwarven:
                    lowOddsMod += 3.5f; midOddsMod += 2.0f; highOddsMod += 2.0f; break;
                case LockMaterials.Adamantium:
                    lowOddsMod += 4.5f; midOddsMod += 2.25f; highOddsMod += 3.0f; break;
                case LockMaterials.Daedric:
                    lowOddsMod += 7.0f; midOddsMod += 4.0f; highOddsMod += 2.5f; break;
            }

            oddsAverage = (lowOddsMod + midOddsMod + highOddsMod) / 3f;

            // For now, these are intended to be the modifier each "lootOdds" value is going to be multipled by the end before item generation rolls happen, still placeholder.
            if (llcObj.LockComplexity < 0 || (llcObj.LockComplexity >= 0 && llcObj.LockComplexity <= 19)) { lowOddsMod += 0f; midOddsMod += 0f; highOddsMod += 0f; }
            else if (llcObj.LockComplexity >= 20 && llcObj.LockComplexity <= 39) { lowOddsMod += 0.25f; midOddsMod += 0.25f; highOddsMod += 0.25f; }
            else if (llcObj.LockComplexity >= 40 && llcObj.LockComplexity <= 59) { lowOddsMod += 0.5f; midOddsMod += 0.5f; highOddsMod += 0.5f; }
            else if (llcObj.LockComplexity >= 60 && llcObj.LockComplexity <= 79) { lowOddsMod += 0.75f; midOddsMod += 0.75f; highOddsMod += 0.75f; }
            else { lowOddsMod += 1.0f; midOddsMod += 1.0f; highOddsMod += 1.0f; }

            for (int i = 0; i < miscGroupOdds.Length; i++) // Heavily placeholder for now, but don't feel like going too deep into this right now.
            {
                if (i <= 0 || i == 12 || i == 13)
                    continue;
                else
                    miscGroupOdds[i] = (int)Mathf.Round(miscGroupOdds[i] * ((lowOddsMod + midOddsMod + highOddsMod) / 6f));
            }

            for (int i = 0; i < itemGroupOdds.Length; i++) // Odds are modified in this loop for each item group within the "itemGroupOdds" array.
            {
                if (itemGroupOdds[i] <= 0)
                    continue;
                else if (itemGroupOdds[i] > 0 && itemGroupOdds[i] <= 15)
                    itemGroupOdds[i] = (int)Mathf.Round((itemGroupOdds[i] + (totalRoomValueMod / 12f)) * (lowOddsMod / 2f));
                else if (itemGroupOdds[i] > 15 && itemGroupOdds[i] <= 50)
                    itemGroupOdds[i] = (int)Mathf.Round((itemGroupOdds[i] + (totalRoomValueMod / 4f)) * (midOddsMod / 2f));
                else
                    itemGroupOdds[i] = (int)Mathf.Round((itemGroupOdds[i] + (totalRoomValueMod / 2f)) * (highOddsMod / 2f));
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
                            chestItems.AddItem(ItemBuilder.CreateGoldPieces(amount));
                        break;
                    case 1: // Add Letter of Credit
                        amount = (Dice100.SuccessRoll(itemChance)) ? UnityEngine.Random.Range(miscGroupOdds[4], miscGroupOdds[5] + 1) : 0;
                        if (amount > 0)
                        {
                            item = ItemBuilder.CreateItem(ItemGroups.MiscItems, (int)MiscItems.Letter_of_credit);
                            item.value = amount;
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
                    case 9: // Add Painting, no idea if this CreateItem will work, nor if it is properly given a random painting value, etc.
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

                while (Dice100.SuccessRoll(itemChance))
                {
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
                        chestItems.AddItem(item);

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
                case (int)ChestLootItemGroups.LightArmor: // Will take modded/custom items into consideration later, after this works for the vanilla stuff atleast, for all item groups here.
                    enumArray = Enum.GetValues(typeof(HeavyArmor));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    item = ItemBuilder.CreateArmor(gender, race, (Armor)enumArray.GetValue(enumIndex), ArmorMaterialTypes.Leather); // Will need testing to see if casting to vanilla enum works here.
                    item.currentCondition = (int)(item.maxCondition * conditionMod);
                    return item;
                case (int)ChestLootItemGroups.MediumArmor:
                    enumArray = Enum.GetValues(typeof(HeavyArmor));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    item = ItemBuilder.CreateArmor(gender, race, (Armor)enumArray.GetValue(enumIndex), ArmorMaterialTypes.Chain); // Will need testing to see if casting to vanilla enum works here.
                    item.currentCondition = (int)(item.maxCondition * conditionMod);
                    return item;
                case (int)ChestLootItemGroups.HeavyArmor:
                    enumArray = Enum.GetValues(typeof(HeavyArmor));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    ArmorMaterialTypes plateType = (ArmorMaterialTypes)RollWeaponOrArmorMaterial(oddsAverage, totalRoomValueMod, false); // Will have materials be determined based on other factors later, placeholder for now.
                    item = ItemBuilder.CreateArmor(gender, race, (Armor)enumArray.GetValue(enumIndex), plateType); // Will need testing to see if casting to vanilla enum works here.
                    item.currentCondition = (int)(item.maxCondition * conditionMod);
                    return item;
                case (int)ChestLootItemGroups.Shields:
                    enumArray = Enum.GetValues(typeof(Shields));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    item = ItemBuilder.CreateArmor(gender, race, (Armor)enumArray.GetValue(enumIndex), (ArmorMaterialTypes)RollWeaponOrArmorMaterial(oddsAverage, totalRoomValueMod, false, true));
                    item.currentCondition = (int)(item.maxCondition * conditionMod);
                    return item;
                case (int)ChestLootItemGroups.SmallWeapons:
                    enumArray = Enum.GetValues(typeof(SmallWeapons));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length - 2);
                    item = (enumIndex >= 0 && enumIndex <= 3) ? ItemBuilder.CreateWeapon((Weapons)enumArray.GetValue(enumIndex), (WeaponMaterialTypes)RollWeaponOrArmorMaterial(oddsAverage, totalRoomValueMod)) : ItemBuilder.CreateItem(ItemGroups.Jewellery, (int)Jewellery.Wand);
                    item.currentCondition = (int)(item.maxCondition * conditionMod);
                    return item;
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
                    item = ItemBuilder.CreateRandomBook(); // Will need alot of work later if wanting to take into consideration book topic, and modded skill books, etc.
                    item.currentCondition = (int)(item.maxCondition * conditionMod);
                    return item;
                case (int)ChestLootItemGroups.Gems:
                    return ItemBuilder.CreateRandomGem(); // Once again, will need work later to take into consideration modded gems and such.
                case (int)ChestLootItemGroups.Jewelry:
                    enumArray = Enum.GetValues(typeof(Jewelry));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length - 8);
                    item = ItemBuilder.CreateItem(ItemGroups.Jewellery, (int)enumArray.GetValue(enumIndex)); // Not sure if this int casting to the "GetValue" will work, have to test and see.
                    item.currentCondition = (int)(item.maxCondition * conditionMod);
                    return item;
                case (int)ChestLootItemGroups.Tools: // Will deal with this when mod compatibility stuff is taken into account, later.
                    return null;
                case (int)ChestLootItemGroups.Lights:
                    enumArray = Enum.GetValues(typeof(Lights));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    item = ItemBuilder.CreateItem(ItemGroups.UselessItems2, (int)enumArray.GetValue(enumIndex)); // Not sure if this int casting to the "GetValue" will work, have to test and see.
                    item.currentCondition = (int)(item.maxCondition * conditionMod);
                    return item;
                case (int)ChestLootItemGroups.Supplies:
                    enumArray = Enum.GetValues(typeof(Supplies));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    item = ItemBuilder.CreateItem(ItemGroups.UselessItems2, (int)enumArray.GetValue(enumIndex)); // Not sure if this int casting to the "GetValue" will work, have to test and see.
                    item.stackCount = UnityEngine.Random.Range(2, 6); // Might want to change values later, but fine for time being.
                    return (item.IsStackable()) ? item : null; // So just for now, instead of checking other mods that make these "supplies" useful, just checking for stackability and spawn based on that.
                case (int)ChestLootItemGroups.BlessedItems:
                    enumArray = Enum.GetValues(typeof(BlessedItems));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    item = (enumIndex <= 0) ? ItemBuilder.CreateItem(ItemGroups.MiscellaneousIngredients1, (int)MiscellaneousIngredients1.Holy_relic) : ItemBuilder.CreateItem(ItemGroups.ReligiousItems, (int)enumArray.GetValue(enumIndex));
                    item.currentCondition = (int)(item.maxCondition * conditionMod);
                    return item;
                case (int)ChestLootItemGroups.ReligiousStuff:
                    enumArray = Enum.GetValues(typeof(ReligiousStuff));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    item = ItemBuilder.CreateItem(ItemGroups.ReligiousItems, (int)enumArray.GetValue(enumIndex)); // Not sure if this int casting to the "GetValue" will work, have to test and see.
                    item.currentCondition = (int)(item.maxCondition * conditionMod);
                    return item;
                case (int)ChestLootItemGroups.GenericPlants:
                    enumArray = Enum.GetValues(typeof(GenericPlants));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    item = ItemBuilder.CreateItem(ItemGroups.PlantIngredients1, (int)enumArray.GetValue(enumIndex)); // Not sure if this int casting to the "GetValue" will work, have to test and see.
                    item.stackCount = UnityEngine.Random.Range(1, 7); // Might want to change values later, but fine for time being.
                    return item;
                case (int)ChestLootItemGroups.ColdClimatePlants:
                    enumArray = Enum.GetValues(typeof(ColdClimatePlants));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    item = ItemBuilder.CreateItem(ItemGroups.PlantIngredients1, (int)enumArray.GetValue(enumIndex)); // Not sure if this int casting to the "GetValue" will work, have to test and see.
                    item.stackCount = UnityEngine.Random.Range(1, 7); // Might want to change values later, but fine for time being.
                    return item;
                case (int)ChestLootItemGroups.WarmClimatePlants:
                    enumArray = Enum.GetValues(typeof(WarmClimatePlants));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    item = ItemBuilder.CreateItem(ItemGroups.PlantIngredients2, (int)enumArray.GetValue(enumIndex)); // Not sure if this int casting to the "GetValue" will work, have to test and see.
                    item.stackCount = UnityEngine.Random.Range(1, 7); // Might want to change values later, but fine for time being.
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
                    item = ItemBuilder.CreateItem(ItemGroups.MiscellaneousIngredients1, (int)enumArray.GetValue(enumIndex)); // Not sure if this int casting to the "GetValue" will work, have to test and see.
                    item.stackCount = UnityEngine.Random.Range(1, 4); // Might want to change values later, but fine for time being.
                    return item;
                case (int)ChestLootItemGroups.TraceMetals:
                    enumArray = Enum.GetValues(typeof(TraceMetals));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    item = ItemBuilder.CreateItem(ItemGroups.MetalIngredients, (int)enumArray.GetValue(enumIndex)); // Not sure if this int casting to the "GetValue" will work, have to test and see.
                    item.stackCount = UnityEngine.Random.Range(1, 4); // Might want to change values later, but fine for time being.
                    return item;
            }
        }

        public static int RollWeaponOrArmorMaterial(float oddsAverage, int totalRoomValueMod, bool isWeapon = true, bool isShield = false) // Testing this out for now, no clue if it's actually going to be a decent method or not for rarity.
        {
            // Iron, Steel, Silver, Elven, Dwarven, Mithril, Adamantium, Ebony, Orcish, Daedric
            // int[] rarity = { 240, 222, 168, 150, 96, 84, 60, 48, 36, 24 };
            float[] lootOdds = new float[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] matRolls = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            float roomMod = 0f;
            float mod = 1f;
            if (totalRoomValueMod < 0)
            {
                totalRoomValueMod = Mathf.FloorToInt(UnityEngine.Random.Range(totalRoomValueMod * 0.5f, totalRoomValueMod * 1.5f));
                roomMod = (totalRoomValueMod / 100f);
                mod = (oddsAverage + roomMod) / 2f;
            }
            else
            {
                totalRoomValueMod = Mathf.CeilToInt(UnityEngine.Random.Range(totalRoomValueMod * 0.2f, totalRoomValueMod * 1.2f));
                roomMod = Mathf.Abs(totalRoomValueMod / 100f); // Work more on this loot and material generation stuff tomorrow, try getting similar to chest generation stuff, that being a better place.
                mod = (oddsAverage + roomMod) / 2f;
            }

            if (isShield)
            {
                int shieldMat = UnityEngine.Random.Range(0, 4);

                if (shieldMat == 0)
                    return (int)ArmorMaterialTypes.Leather;
                else if (shieldMat == 1)
                    return (int)ArmorMaterialTypes.Chain;
            }

            if (mod <= 1.8f)
            {
                lootOdds = new float[] { 240*mod, 222*(mod*0.9f), 168*(mod*0.8f), 150*(mod*0.7f), 96*(mod*0.5f), 84*(mod*0.4f), 60*(mod*0.3f), 48*(mod*0.25f), 36*(mod*0.2f), 24*(mod*0.1f) };
            }
            else if (mod <= 3.1f)
            {
                lootOdds = new float[] { 240*(mod*0.1f), 222*(mod*0.2f), 168*(mod*0.3f), 150*(mod*0.4f), 96*mod, 84*(mod*0.9f), 60*(mod*0.8f), 48*(mod*0.7f), 36*(mod*0.6f), 24*(mod*0.5f) };
            }
            else
            {
                lootOdds = new float[] { 240*(mod*0.05f), 222*(mod*0.1f), 168*(mod*0.2f), 150*(mod*0.3f), 96*(mod*0.45f), 84*(mod*0.6f), 60*(mod*0.7f), 48*(mod*0.8f), 36*(mod*0.9f), 24*mod };
            }

            for (int i = 0; i < lootOdds.Length; i++)
            {
                int randomModifier = UnityEngine.Random.Range(0, Mathf.RoundToInt(lootOdds[i]) + 1);
                matRolls[i] = randomModifier;
            }

            int max = matRolls[0];
            int index = 0;

            for (int i = 0; i < matRolls.Length; i++)
            {
                if (index == i)
                    continue;

                if (max == matRolls[i])
                {
                    if (CoinFlip())
                        continue;
                    else
                    {
                        max = matRolls[i];
                        index = i;
                        continue;
                    }
                }

                if (max < matRolls[i])
                {
                    max = matRolls[i];
                    index = i;
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
    }
}