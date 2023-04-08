using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Utility;
using System;
using DaggerfallWorkshop.Game.Entity;

namespace LockedLootContainers
{
    public partial class LockedLootContainersMain
    {
        public static DaggerfallUnityItem GenerateLightArmor(Genders gender, Races race, float oddsAverage, int totalRoomValueMod, float conditionMod) // Create a random piece of light type armor, but checks and accounts for active mods as well.
        {
            DaggerfallUnityItem item = null;
            if (RolePlayRealismNewArmorCheck)
            {
                if (CoinFlip())
                {
                    Array enumArray = Enum.GetValues(typeof(HeavyArmor));
                    int enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    item = ItemBuilder.CreateArmor(gender, race, (Armor)enumArray.GetValue(enumIndex), ArmorMaterialTypes.Leather);
                }
                else
                {
                    Array enumArray = Enum.GetValues(typeof(LightArmor));
                    int enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    ArmorMaterialTypes plateType = (ArmorMaterialTypes)RollWeaponOrArmorMaterial(oddsAverage, totalRoomValueMod, false, true);
                    item = ItemBuilder.CreateItem(ItemGroups.Armor, (int)enumArray.GetValue(enumIndex));
                    ItemBuilder.ApplyArmorSettings(item, gender, race, plateType);
                }
            }
            else
            {
                Array enumArray = Enum.GetValues(typeof(HeavyArmor));
                int enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                item = ItemBuilder.CreateArmor(gender, race, (Armor)enumArray.GetValue(enumIndex), ArmorMaterialTypes.Leather);
            }

            if (item != null)
                item.currentCondition = (int)(item.maxCondition * conditionMod);

            return item;
        }

        public static DaggerfallUnityItem GenerateMediumArmor(Genders gender, Races race, float oddsAverage, int totalRoomValueMod, float conditionMod) // Create a random piece of medium type armor, but checks and accounts for active mods as well.
        {
            DaggerfallUnityItem item = null;
            if (RolePlayRealismNewArmorCheck)
            {
                if (CoinFlip())
                {
                    Array enumArray = Enum.GetValues(typeof(HeavyArmor));
                    int enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    item = ItemBuilder.CreateArmor(gender, race, (Armor)enumArray.GetValue(enumIndex), ArmorMaterialTypes.Chain);
                }
                else
                {
                    Array enumArray = Enum.GetValues(typeof(MediumArmor));
                    int enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                    ArmorMaterialTypes plateType = (ArmorMaterialTypes)RollWeaponOrArmorMaterial(oddsAverage, totalRoomValueMod, false, true);
                    item = ItemBuilder.CreateItem(ItemGroups.Armor, (int)enumArray.GetValue(enumIndex));
                    ItemBuilder.ApplyArmorSettings(item, gender, race, plateType);
                }
            }
            else
            {
                Array enumArray = Enum.GetValues(typeof(HeavyArmor));
                int enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                item = ItemBuilder.CreateArmor(gender, race, (Armor)enumArray.GetValue(enumIndex), ArmorMaterialTypes.Chain);
            }

            if (item != null)
                item.currentCondition = (int)(item.maxCondition * conditionMod);

            return item;
        }

        public static DaggerfallUnityItem GenerateHeavyArmor(Genders gender, Races race, float oddsAverage, int totalRoomValueMod, float conditionMod) // Create a random piece of heavy type armor, but checks and accounts for active mods as well.
        {
            DaggerfallUnityItem item = null;
            Array enumArray = Enum.GetValues(typeof(HeavyArmor));
            int enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
            ArmorMaterialTypes plateType = (ArmorMaterialTypes)RollWeaponOrArmorMaterial(oddsAverage, totalRoomValueMod, false);
            item = ItemBuilder.CreateArmor(gender, race, (Armor)enumArray.GetValue(enumIndex), plateType);

            if (item != null)
                item.currentCondition = (int)(item.maxCondition * conditionMod);

            return item;
        }

        public static DaggerfallUnityItem GenerateSmallWeapon(float oddsAverage, int totalRoomValueMod, float conditionMod) // Create a random small size weapon, but checks and accounts for active mods as well.
        {
            DaggerfallUnityItem item = null;
            if (RolePlayRealismNewWeaponCheck)
            {
                Array enumArray = Enum.GetValues(typeof(SmallWeapons));
                int enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                if (enumIndex > 4)
                {
                    item = ItemBuilder.CreateItem(ItemGroups.Weapons, (int)enumArray.GetValue(enumIndex));
                    ItemBuilder.ApplyWeaponMaterial(item, (WeaponMaterialTypes)RollWeaponOrArmorMaterial(oddsAverage, totalRoomValueMod));
                }
                else
                {
                    item = (enumIndex == 4) ? ItemBuilder.CreateItem(ItemGroups.Jewellery, (int)Jewellery.Wand) : ItemBuilder.CreateWeapon((Weapons)enumArray.GetValue(enumIndex), (WeaponMaterialTypes)RollWeaponOrArmorMaterial(oddsAverage, totalRoomValueMod));
                }
            }
            else
            {
                if (Dice100.SuccessRoll(15)) { ItemBuilder.CreateItem(ItemGroups.Jewellery, (int)Jewellery.Wand); }
                else
                {
                    Array enumArray = Enum.GetValues(typeof(SmallWeapons));
                    int enumIndex = UnityEngine.Random.Range(0, enumArray.Length - 3);
                    item = ItemBuilder.CreateWeapon((Weapons)enumArray.GetValue(enumIndex), (WeaponMaterialTypes)RollWeaponOrArmorMaterial(oddsAverage, totalRoomValueMod));
                }
            }

            if (item != null)
                item.currentCondition = (int)(item.maxCondition * conditionMod);

            return item;
        }

        public static DaggerfallUnityItem GenerateRandomBook(float conditionMod) // Create a random book, but checks and accounts for active mods as well.
        {
            DaggerfallUnityItem item = null;
            if (SkillBooksCheck)
            {
                if (Dice100.SuccessRoll(70)) // Create a vanilla book most of the time.
                {
                    item = ItemBuilder.CreateRandomBook();
                }
                else
                {
                    int roll = UnityEngine.Random.Range(0, 101);
                    if (roll > 90) { item = ItemBuilder.CreateItem(ItemGroups.UselessItems2, (int)Books.Tablet_of_Arcane_Knowledge); }
                    else if (roll > 75) { item = ItemBuilder.CreateItem(ItemGroups.UselessItems2, (int)Books.Tomes_of_Arcane_Knowledge); }
                    else if (roll > 45) { item = ItemBuilder.CreateItem(ItemGroups.UselessItems2, (int)Books.Advanced_Skill_Book); }
                    else { item = ItemBuilder.CreateItem(ItemGroups.UselessItems2, (int)Books.Basic_Skill_Book); }
                }
            }
            else
            {
                item = ItemBuilder.CreateRandomBook();
            }

            if (item != null)
                item.currentCondition = (int)(item.maxCondition * conditionMod);

            return item;
        }

        public static DaggerfallUnityItem GenerateRandomGem() // Create a random gem, but checks and accounts for active mods as well.
        {
            DaggerfallUnityItem item = null;
            if (JewelryAdditionsCheck)
            {
                Array enumArray = Enum.GetValues(typeof(Gems));
                int enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                item = ItemBuilder.CreateItem(ItemGroups.Gems, (int)enumArray.GetValue(enumIndex));
            }
            else
            {
                item = ItemBuilder.CreateRandomGem();
            }
            return item;
        }

        public static DaggerfallUnityItem GenerateRandomJewelry(float conditionMod) // Create a random piece of jewelry, but checks and accounts for active mods as well.
        {
            DaggerfallUnityItem item = null;
            if (JewelryAdditionsCheck)
            {
                Array enumArray = Enum.GetValues(typeof(Jewelry));
                int enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                item = ItemBuilder.CreateItem(ItemGroups.Jewellery, (int)enumArray.GetValue(enumIndex));
            }
            else
            {
                Array enumArray = Enum.GetValues(typeof(Jewelry));
                int enumIndex = UnityEngine.Random.Range(0, enumArray.Length - 8);
                item = ItemBuilder.CreateItem(ItemGroups.Jewellery, (int)enumArray.GetValue(enumIndex));
            }

            if (item != null)
                item.currentCondition = (int)(item.maxCondition * conditionMod);

            return item;
        }

        public static DaggerfallUnityItem GenerateRandomTool(float conditionMod) // Create a tool item, but checks and accounts for active mods as well.
        {
            DaggerfallUnityItem item = null;
            if (RepairToolsCheck)
            {
                if (Dice100.SuccessRoll(8))
                {
                    item = ItemBuilder.CreateItem(ItemGroups.UselessItems2, (int)Tools.Charging_Powder);
                }
                else
                {
                    Array enumArray = Enum.GetValues(typeof(Tools));
                    int enumIndex = UnityEngine.Random.Range(0, enumArray.Length - 1);
                    item = ItemBuilder.CreateItem(ItemGroups.UselessItems2, (int)enumArray.GetValue(enumIndex));
                }
            }

            if (item != null)
                item.currentCondition = (int)(item.maxCondition * conditionMod);

            return item;
        }

        public static DaggerfallUnityItem GenerateRandomSupplies() // Create a supply item, but checks and accounts for active mods as well.
        {
            DaggerfallUnityItem item = null;
            if (ClimatesAndCaloriesCheck)
            {
                item = Dice100.SuccessRoll(10) ? ItemBuilder.CreateItem(ItemGroups.UselessItems2, (int)Supplies.Rations) : null;
                if (item != null)
                {
                    item.stackCount = UnityEngine.Random.Range(2, 6);
                    return item;
                }
            }

            if (RolePlayRealismBandagingCheck)
            {
                item = Dice100.SuccessRoll(30) ? ItemBuilder.CreateItem(ItemGroups.UselessItems2, (int)Supplies.Bandage) : null;
                if (item != null)
                {
                    item.stackCount = UnityEngine.Random.Range(2, 6);
                    return item;
                }
            }

            if (CoinFlip())
                item = ItemBuilder.CreateItem(ItemGroups.UselessItems2, (int)Supplies.Oil);
            else
                item = ItemBuilder.CreateItem(ItemGroups.UselessItems2, (int)Supplies.Parchment);

            if (item != null)
                item.stackCount = UnityEngine.Random.Range(2, 6);

            return item;
        }
    }
}
