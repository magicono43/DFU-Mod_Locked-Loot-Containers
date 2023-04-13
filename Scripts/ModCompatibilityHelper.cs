using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Utility;
using System;
using DaggerfallWorkshop.Game.Entity;
using System.Collections.Generic;
using System.Linq;
using DaggerfallWorkshop.Game;

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

        public static DaggerfallUnityItem GenerateSmallWeapon(float oddsAverage, int totalRoomValueMod, float conditionMod, bool removeWands = false) // Create a random small size weapon, but checks and accounts for active mods as well.
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

            if (removeWands && item != null && item.TemplateIndex == (int)Jewellery.Wand) { item = null; }

            if (item != null)
                item.currentCondition = (int)(item.maxCondition * conditionMod);

            return item;
        }

        public static DaggerfallUnityItem GenerateMediumWeapon(float oddsAverage, int totalRoomValueMod, float conditionMod, bool removeArrows = false) // Create a random medium size weapon.
        {
            DaggerfallUnityItem item = null;
            Array enumArray = Enum.GetValues(typeof(MediumWeapons));
            int enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
            if (enumIndex >= 8) // Arrows
            {
                item = ItemBuilder.CreateWeapon(Weapons.Arrow, WeaponMaterialTypes.Steel);
                item.stackCount = UnityEngine.Random.Range(8, 36);
            }
            else // Other Medium Sized Weapons
            {
                item = ItemBuilder.CreateWeapon((Weapons)enumArray.GetValue(enumIndex), (WeaponMaterialTypes)RollWeaponOrArmorMaterial(oddsAverage, totalRoomValueMod));
            }

            if (removeArrows && item != null && item.TemplateIndex == (int)Weapons.Arrow) { item = null; }

            if (item != null && item.TemplateIndex != (int)Weapons.Arrow)
                item.currentCondition = (int)(item.maxCondition * conditionMod);

            return item;
        }

        public static DaggerfallUnityItem GenerateLargeWeapon(float oddsAverage, int totalRoomValueMod, float conditionMod) // Create a random large size weapon.
        {
            DaggerfallUnityItem item = null;
            Array enumArray = Enum.GetValues(typeof(LargeWeapons));
            int enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
            item = ItemBuilder.CreateWeapon((Weapons)enumArray.GetValue(enumIndex), (WeaponMaterialTypes)RollWeaponOrArmorMaterial(oddsAverage, totalRoomValueMod));

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

        public static DaggerfallUnityItem[] RetrieveEquipmentSet(float oddsAverage, int totalRoomValueMod, float conditionMod) // Roll and create a set of equipment to be placed into a chest.
        {
            Genders gender = GameManager.Instance.PlayerEntity.Gender;
            Races race = GameManager.Instance.PlayerEntity.Race;
            List<DaggerfallUnityItem> itemSet = new List<DaggerfallUnityItem>();
            int[] equipSlotsUsed = ChooseSetType();

            for (int i = 0; i < equipSlotsUsed.Length; i++)
            {
                if ((EquipSetSlots)equipSlotsUsed[i] == EquipSetSlots.MainHand)
                    itemSet.Add(CoinFlip() ? GenerateSmallWeapon(oddsAverage, totalRoomValueMod, conditionMod, true) : GenerateMediumWeapon(oddsAverage, totalRoomValueMod, conditionMod));
                else if ((EquipSetSlots)equipSlotsUsed[i] == EquipSetSlots.BothHands)
                {
                    if (CoinFlip()) { itemSet.Add(GenerateLargeWeapon(oddsAverage, totalRoomValueMod, conditionMod)); }
                    else
                    {
                        itemSet.Add(GenerateSpecificArmor(EquipSetSlots.OffHand, gender, race, oddsAverage, totalRoomValueMod, conditionMod));
                        itemSet.Add(CoinFlip() ? GenerateSmallWeapon(oddsAverage, totalRoomValueMod, conditionMod, true) : GenerateMediumWeapon(oddsAverage, totalRoomValueMod, conditionMod, true));
                    }
                }
                else
                    itemSet.Add(GenerateSpecificArmor((EquipSetSlots)equipSlotsUsed[i], gender, race, oddsAverage, totalRoomValueMod, conditionMod));
            }
            return itemSet.ToArray();
        }

        public static DaggerfallUnityItem GenerateSpecificArmor(EquipSetSlots armorType, Genders gender, Races race, float oddsAverage, int totalRoomValueMod, float conditionMod)
        {
            DaggerfallUnityItem item = null;
            ArmorMaterialTypes armorMat;
            if (Dice100.SuccessRoll(35)) { armorMat = ArmorMaterialTypes.Leather; }
            else if (Dice100.SuccessRoll(35)) { armorMat = ArmorMaterialTypes.Chain; }
            else { armorMat = (ArmorMaterialTypes)RollWeaponOrArmorMaterial(oddsAverage, totalRoomValueMod, false); }

            if (RolePlayRealismNewArmorCheck)
            {
                switch (armorType)
                {
                    case EquipSetSlots.Head:
                        if (CoinFlip()) { item = ItemBuilder.CreateArmor(gender, race, Armor.Helm, armorMat); }
                        else
                        {
                            item = ItemBuilder.CreateItem(ItemGroups.Armor, (int)LightArmor.Helmet);
                            ItemBuilder.ApplyArmorSettings(item, gender, race, armorMat);
                        } break;
                    case EquipSetSlots.RightArm:
                        if (CoinFlip()) { item = ItemBuilder.CreateArmor(gender, race, Armor.Right_Pauldron, armorMat); }
                        else
                        {
                            item = CoinFlip() ? ItemBuilder.CreateItem(ItemGroups.Armor, (int)LightArmor.Right_Vambrace) : ItemBuilder.CreateItem(ItemGroups.Armor, (int)MediumArmor.Right_Spaulders);
                            ItemBuilder.ApplyArmorSettings(item, gender, race, armorMat);
                        } break;
                    case EquipSetSlots.LeftArm:
                        if (CoinFlip()) { item = ItemBuilder.CreateArmor(gender, race, Armor.Left_Pauldron, armorMat); }
                        else
                        {
                            item = CoinFlip() ? ItemBuilder.CreateItem(ItemGroups.Armor, (int)LightArmor.Left_Vambrace) : ItemBuilder.CreateItem(ItemGroups.Armor, (int)MediumArmor.Left_Spaulders);
                            ItemBuilder.ApplyArmorSettings(item, gender, race, armorMat);
                        } break;
                    case EquipSetSlots.Chest:
                        if (CoinFlip()) { item = ItemBuilder.CreateArmor(gender, race, Armor.Cuirass, armorMat); }
                        else
                        {
                            item = CoinFlip() ? ItemBuilder.CreateItem(ItemGroups.Armor, (int)LightArmor.Jerkin) : ItemBuilder.CreateItem(ItemGroups.Armor, (int)MediumArmor.Hauberk);
                            ItemBuilder.ApplyArmorSettings(item, gender, race, armorMat);
                        } break;
                    case EquipSetSlots.Hands:
                        if (CoinFlip()) { item = ItemBuilder.CreateArmor(gender, race, Armor.Gauntlets, armorMat); }
                        else
                        {
                            item = ItemBuilder.CreateItem(ItemGroups.Armor, (int)LightArmor.Gloves);
                            ItemBuilder.ApplyArmorSettings(item, gender, race, armorMat);
                        } break;
                    case EquipSetSlots.Legs:
                        if (CoinFlip()) { item = ItemBuilder.CreateArmor(gender, race, Armor.Greaves, armorMat); }
                        else
                        {
                            item = CoinFlip() ? ItemBuilder.CreateItem(ItemGroups.Armor, (int)LightArmor.Cuisse) : ItemBuilder.CreateItem(ItemGroups.Armor, (int)MediumArmor.Chausses);
                            ItemBuilder.ApplyArmorSettings(item, gender, race, armorMat);
                        } break;
                    case EquipSetSlots.Feet:
                        if (CoinFlip()) { item = ItemBuilder.CreateArmor(gender, race, Armor.Boots, armorMat); }
                        else
                        {
                            item = CoinFlip() ? ItemBuilder.CreateItem(ItemGroups.Armor, (int)LightArmor.Boots) : ItemBuilder.CreateItem(ItemGroups.Armor, (int)MediumArmor.Sollerets);
                            ItemBuilder.ApplyArmorSettings(item, gender, race, armorMat);
                        } break;
                    case EquipSetSlots.OffHand:
                        Array enumArray = Enum.GetValues(typeof(Shields));
                        int enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                        item = ItemBuilder.CreateArmor(gender, race, (Armor)enumArray.GetValue(enumIndex), (ArmorMaterialTypes)RollWeaponOrArmorMaterial(oddsAverage, totalRoomValueMod, false, true));
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (armorType)
                {
                    case EquipSetSlots.Head:
                        item = ItemBuilder.CreateArmor(gender, race, Armor.Helm, armorMat); break;
                    case EquipSetSlots.RightArm:
                        item = ItemBuilder.CreateArmor(gender, race, Armor.Right_Pauldron, armorMat); break;
                    case EquipSetSlots.LeftArm:
                        item = ItemBuilder.CreateArmor(gender, race, Armor.Left_Pauldron, armorMat); break;
                    case EquipSetSlots.Chest:
                        item = ItemBuilder.CreateArmor(gender, race, Armor.Cuirass, armorMat); break;
                    case EquipSetSlots.Hands:
                        item = ItemBuilder.CreateArmor(gender, race, Armor.Gauntlets, armorMat); break;
                    case EquipSetSlots.Legs:
                        item = ItemBuilder.CreateArmor(gender, race, Armor.Greaves, armorMat); break;
                    case EquipSetSlots.Feet:
                        item = ItemBuilder.CreateArmor(gender, race, Armor.Boots, armorMat); break;
                    case EquipSetSlots.OffHand:
                        Array enumArray = Enum.GetValues(typeof(Shields));
                        int enumIndex = UnityEngine.Random.Range(0, enumArray.Length);
                        item = ItemBuilder.CreateArmor(gender, race, (Armor)enumArray.GetValue(enumIndex), (ArmorMaterialTypes)RollWeaponOrArmorMaterial(oddsAverage, totalRoomValueMod, false, true));
                        break;
                    default:
                        break;
                }
            }

            if (item != null)
                item.currentCondition = (int)(item.maxCondition * conditionMod);

            return item;
        }

        public static int[] ChooseSetType()
        {
            int chosenSet = -1;
            List<int> equipSlots = new List<int>();
            List<float> odds = new List<float>() { 3.0f, 3.5f, 3.5f, 4.0f, 4.0f, 4.0f, 4.2f, 4.2f, 4.2f, 4.5f, 4.5f };

            // Normalize odds values to ensure they all add up to 100.
            float totalOddsSum = odds.Sum();
            for (int i = 0; i < odds.Count; i++)
            {
                odds[i] = (odds[i] / totalOddsSum) * 100f;
            }

            // Choose a value using the weighted random selection algorithm.
            float randomValue = UnityEngine.Random.Range(0f, 101f);
            float cumulativeOdds = 0f;
            for (int i = 0; i < odds.Count; i++)
            {
                cumulativeOdds += odds[i];
                if (randomValue < cumulativeOdds)
                {
                    chosenSet = i; // Value i is chosen
                    break;
                }
            }

            if (chosenSet == 0) { equipSlots = new List<int> { (int)EquipSetSlots.Head, (int)EquipSetSlots.RightArm, (int)EquipSetSlots.LeftArm, (int)EquipSetSlots.Chest, (int)EquipSetSlots.Hands, (int)EquipSetSlots.Legs, (int)EquipSetSlots.Feet }; }
            else if (chosenSet == 1) { equipSlots = new List<int> { (int)EquipSetSlots.Head, (int)EquipSetSlots.RightArm, (int)EquipSetSlots.LeftArm, (int)EquipSetSlots.Chest, (int)EquipSetSlots.Hands }; }
            else if (chosenSet == 2) { equipSlots = new List<int> { (int)EquipSetSlots.RightArm, (int)EquipSetSlots.LeftArm, (int)EquipSetSlots.Chest, (int)EquipSetSlots.Hands, (int)EquipSetSlots.Legs }; }
            else if (chosenSet == 3) { equipSlots = new List<int> { (int)EquipSetSlots.Head, (int)EquipSetSlots.Chest, (int)EquipSetSlots.Legs, (int)EquipSetSlots.Feet }; }
            else if (chosenSet == 4) { equipSlots = new List<int> { (int)EquipSetSlots.RightArm, (int)EquipSetSlots.LeftArm, (int)EquipSetSlots.Chest, (int)EquipSetSlots.Hands }; }
            else if (chosenSet == 5) { equipSlots = new List<int> { (int)EquipSetSlots.RightArm, (int)EquipSetSlots.LeftArm, (int)EquipSetSlots.Legs, (int)EquipSetSlots.Feet }; }
            else if (chosenSet == 6) { equipSlots = new List<int> { (int)EquipSetSlots.Head, (int)EquipSetSlots.Hands, (int)EquipSetSlots.Feet }; }
            else if (chosenSet == 7) { equipSlots = new List<int> { (int)EquipSetSlots.Hands, (int)EquipSetSlots.Legs, (int)EquipSetSlots.Feet }; }
            else if (chosenSet == 8) { equipSlots = new List<int> { (int)EquipSetSlots.Chest, (int)EquipSetSlots.Legs, (int)EquipSetSlots.Feet }; }
            else if (chosenSet == 9) { equipSlots = new List<int> { (int)EquipSetSlots.Chest, (int)EquipSetSlots.Legs }; }
            else if (chosenSet == 10) { equipSlots = new List<int> { (int)EquipSetSlots.Legs, (int)EquipSetSlots.Feet }; }

            if (CoinFlip()) { equipSlots.Add((int)EquipSetSlots.BothHands); }
            else
            {
                if (CoinFlip()) { equipSlots.Add((int)EquipSetSlots.MainHand); }
                else { equipSlots.Add((int)EquipSetSlots.OffHand); }
            }
            return equipSlots.ToArray();
        }
    }
}
