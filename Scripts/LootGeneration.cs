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

                    if (item != null) // To prevent null object reference errors if item could not be created for whatever reason by DetermineLootItem method.
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
                    enumArray = Enum.GetValues(typeof(SmallWeapons));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length - 2);
                    return (enumIndex >= 0 && enumIndex <= 3) ? ItemBuilder.CreateWeapon((Weapons)enumArray.GetValue(enumIndex), (WeaponMaterialTypes)UnityEngine.Random.Range(0, 10)) : ItemBuilder.CreateItem(ItemGroups.Jewellery, (int)Jewellery.Wand);
                case (int)ChestLootItemGroups.MediumWeapons:
                    enumArray = Enum.GetValues(typeof(MediumWeapons));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    if (enumIndex >= 8) // Arrows
                        return ItemBuilder.CreateWeapon(Weapons.Arrow, (WeaponMaterialTypes)UnityEngine.Random.Range(0, 10)); // Will likely want to change their stack amount to something else later.
                    else // Other Medium Sized Weapons
                        return ItemBuilder.CreateWeapon((Weapons)enumArray.GetValue(enumIndex), (WeaponMaterialTypes)UnityEngine.Random.Range(0, 10));
                case (int)ChestLootItemGroups.LargeWeapons:
                    enumArray = Enum.GetValues(typeof(LargeWeapons));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    return ItemBuilder.CreateWeapon((Weapons)enumArray.GetValue(enumIndex), (WeaponMaterialTypes)UnityEngine.Random.Range(0, 10));
                case (int)ChestLootItemGroups.MensClothing:
                    enumArray = Enum.GetValues(typeof(MensClothing));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    return ItemBuilder.CreateMensClothing((DaggerfallWorkshop.Game.Items.MensClothing)enumArray.GetValue(enumIndex), race, -1, ItemBuilder.RandomClothingDye());
                case (int)ChestLootItemGroups.WomensClothing:
                    enumArray = Enum.GetValues(typeof(WomensClothing));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    return ItemBuilder.CreateWomensClothing((DaggerfallWorkshop.Game.Items.WomensClothing)enumArray.GetValue(enumIndex), race, -1, ItemBuilder.RandomClothingDye());
                case (int)ChestLootItemGroups.Books:
                    return ItemBuilder.CreateRandomBook(); // Will need alot of work later if wanting to take into consideration book topic, and modded skill books, etc.
                case (int)ChestLootItemGroups.Gems:
                    return ItemBuilder.CreateRandomGem(); // Once again, will need work later to take into consideration modded gems and such.
                case (int)ChestLootItemGroups.Jewelry:
                    enumArray = Enum.GetValues(typeof(Jewelry));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length - 8);
                    return ItemBuilder.CreateItem(ItemGroups.Jewellery, (int)enumArray.GetValue(enumIndex)); // Not sure if this int casting to the "GetValue" will work, have to test and see.
                case (int)ChestLootItemGroups.Tools: // Will deal with this when mod compatibility stuff is taken into account, later.
                    return null;
                case (int)ChestLootItemGroups.Lights:
                    enumArray = Enum.GetValues(typeof(Lights));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    return ItemBuilder.CreateItem(ItemGroups.UselessItems2, (int)enumArray.GetValue(enumIndex)); // Not sure if this int casting to the "GetValue" will work, have to test and see.
                case (int)ChestLootItemGroups.Supplies:
                    enumArray = Enum.GetValues(typeof(Supplies));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    item = ItemBuilder.CreateItem(ItemGroups.UselessItems2, (int)enumArray.GetValue(enumIndex)); // Not sure if this int casting to the "GetValue" will work, have to test and see.
                    item.stackCount = UnityEngine.Random.Range(3, 10); // Might want to change values later, but fine for time being.
                    return item;
                case (int)ChestLootItemGroups.BlessedItems:
                    enumArray = Enum.GetValues(typeof(BlessedItems));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    return (enumIndex <= 0) ? ItemBuilder.CreateItem(ItemGroups.MiscellaneousIngredients1, (int)MiscellaneousIngredients1.Holy_relic) : ItemBuilder.CreateItem(ItemGroups.ReligiousItems, (int)enumArray.GetValue(enumIndex));
                case (int)ChestLootItemGroups.ReligiousStuff:
                    enumArray = Enum.GetValues(typeof(ReligiousStuff));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    return ItemBuilder.CreateItem(ItemGroups.ReligiousItems, (int)enumArray.GetValue(enumIndex)); // Not sure if this int casting to the "GetValue" will work, have to test and see.
                case (int)ChestLootItemGroups.GenericPlants:
                    enumArray = Enum.GetValues(typeof(GenericPlants));
                    enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    return ItemBuilder.CreateItem(ItemGroups.PlantIngredients1, (int)enumArray.GetValue(enumIndex)); // Not sure if this int casting to the "GetValue" will work, have to test and see.
                case (int)ChestLootItemGroups.ColdClimatePlants:
                    return null; // Made good progress on this part today atleast, will continue from here next time I work on this.
            }
        }
    }
}