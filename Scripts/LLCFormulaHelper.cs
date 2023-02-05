using UnityEngine;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Items;
using System.Collections.Generic;

namespace LockedLootContainers
{
    public partial class LockedLootContainersMain
    {
        public static int Stren { get { return Player.Stats.LiveStrength - 50; } }
        public static int Intel { get { return Player.Stats.LiveIntelligence - 50; } }
        public static int Willp { get { return Player.Stats.LiveWillpower - 50; } }
        public static int Agili { get { return Player.Stats.LiveAgility - 50; } }
        public static int Endur { get { return Player.Stats.LiveEndurance - 50; } }
        public static int Speed { get { return Player.Stats.LiveSpeed - 50; } }
        public static int Luck { get { return Player.Stats.LiveLuck - 50; } }
        public static int LockP { get { return Player.Skills.GetLiveSkillValue(DFCareer.Skills.Lockpicking); } }
        public static int PickP { get { return Player.Skills.GetLiveSkillValue(DFCareer.Skills.Pickpocket); } }
        public static int Destr { get { return Player.Skills.GetLiveSkillValue(DFCareer.Skills.Destruction); } }
        public static int Mysti { get { return Player.Skills.GetLiveSkillValue(DFCareer.Skills.Mysticism); } }

        public static int LockPickChance(LLCObject chest) // Still not entirely happy with the current form here, but I think it's alot better than the first draft atleast, so fine for now, I think.
        {
            int lockComp = chest.LockComplexity;

            if (lockComp >= 0 && lockComp <= 19)
            {
                int lockP = (int)Mathf.Ceil(LockP * 1.6f);
                float successChance = (lockComp * -1) + lockP + Mathf.Round(PickP / 10) + Mathf.Round(Intel * .3f) + Mathf.Round(Agili * 1.1f) + Mathf.Round(Speed * .35f) + Mathf.Round(Luck * .30f);
                return (int)Mathf.Round(Mathf.Clamp(successChance, 15f, 100f));
            }
            else if (lockComp >= 20 && lockComp <= 39)
            {
                int lockP = (int)Mathf.Ceil(LockP * 1.7f);
                float successChance = (lockComp * -1) + lockP + Mathf.Round(PickP / 10) + Mathf.Round(Intel * .4f) + Mathf.Round(Agili * 1f) + Mathf.Round(Speed * .30f) + Mathf.Round(Luck * .30f);
                return (int)Mathf.Round(Mathf.Clamp(successChance, 10f, 100f));
            }
            else if (lockComp >= 40 && lockComp <= 59)
            {
                int lockP = (int)Mathf.Ceil(LockP * 1.8f);
                float successChance = (lockComp * -1) + lockP + Mathf.Round(PickP / 10) + Mathf.Round(Intel * .5f) + Mathf.Round(Agili * .9f) + Mathf.Round(Speed * .25f) + Mathf.Round(Luck * .30f);
                return (int)Mathf.Round(Mathf.Clamp(successChance, 7f, 95f));
            }
            else if (lockComp >= 60 && lockComp <= 79)
            {
                int lockP = (int)Mathf.Ceil(LockP * 1.9f);
                float successChance = (lockComp * -1) + lockP + Mathf.Round(PickP / 10) + Mathf.Round(Intel * .6f) + Mathf.Round(Agili * .8f) + Mathf.Round(Speed * .20f) + Mathf.Round(Luck * .30f);
                return (int)Mathf.Round(Mathf.Clamp(successChance, 3f, 90f));
            }
            else
            {
                int lockP = (int)Mathf.Ceil(LockP * 2f);
                float successChance = (lockComp * -1) + lockP + Mathf.Round(PickP / 10) + Mathf.Round(Intel * .7f) + Mathf.Round(Agili * .7f) + Mathf.Round(Speed * .15f) + Mathf.Round(Luck * .30f);
                return (int)Mathf.Round(Mathf.Clamp(successChance, 1f, 80f)); // Potentially add specific text depending on initial odds, like "Through dumb-Luck, you somehow unlocked it", etc.
            }
        }

        public static int LockJamChance(LLCObject chest) // Will have to test this out, but I think I'm fairly satisfied with the formula so far.
        {
            float jamResist = (float)chest.JamResist / 100f;
            float resistMod = (jamResist - 1f) * -1f;

            float jamChance = (int)Mathf.Ceil(chest.PicksAttempted * (UnityEngine.Random.Range(14, 26) - (int)Mathf.Round(Luck / 5f)) * resistMod);
            return (int)Mathf.Round(Mathf.Clamp(jamChance, 5f, 95f));
        }

        public static bool HandleDestroyingLootItem(LLCObject chest, DaggerfallUnityItem item, DaggerfallUnityItem bashingWep, int wepSkillID) // Handles most of the "work" part of breaking/destroying loot items, removing the item and adding the respective "waste" item in its place.
        {
            DaggerfallUnityItem wasteItem;
            int wasteAmount;

            if (chest == null || item == null)
                return false;

            if (item.ItemGroup == ItemGroups.Weapons && item.TemplateIndex != (int)Weapons.Arrow)
            {
                int itemMat = item.NativeMaterialValue;
                int weapMat = (bashingWep != null) ? bashingWep.NativeMaterialValue : -1;
                int matDiff = weapMat - itemMat;

                if (bashingWep == null && wepSkillID == (short)DFCareer.Skills.HandToHand)
                {
                    float conditionMod = (float)Mathf.Max(2, UnityEngine.Random.Range(2, 14 + (int)Mathf.Round(Luck / -10f))) / 100f;
                    int damAmount = (int)(item.maxCondition * conditionMod);
                    return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
                }
                else if (wepSkillID == (short)DFCareer.Skills.BluntWeapon)
                {
                    float conditionMod = (float)Mathf.Max(8, UnityEngine.Random.Range(8, 17 + Mathf.Clamp(Mathf.Round(matDiff / 2), -6, 6) + (int)Mathf.Round(Luck / -10f))) / 100f;
                    int damAmount = (int)(item.maxCondition * conditionMod);
                    return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
                }
                else
                {
                    float conditionMod = (float)Mathf.Max(5, UnityEngine.Random.Range(5, 15 + Mathf.Clamp(matDiff * 2, -6, 14) + (int)Mathf.Round(Luck / -10f))) / 100f;
                    int damAmount = (int)(item.maxCondition * conditionMod);
                    return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
                }
            }
            else if (item.ItemGroup == ItemGroups.Armor)
            {
                int itemMat = (item.NativeMaterialValue - 200 < 0) ? -3 : item.NativeMaterialValue - 200;
                int weapMat = (bashingWep != null) ? bashingWep.NativeMaterialValue : -1;
                int matDiff = weapMat - itemMat;

                if (bashingWep == null && wepSkillID == (short)DFCareer.Skills.HandToHand)
                {
                    float conditionMod = (float)Mathf.Max(3, UnityEngine.Random.Range(3, 15 + (int)Mathf.Round(Luck / -10f))) / 100f;
                    int damAmount = (int)(item.maxCondition * conditionMod);
                    return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
                }
                else if (wepSkillID == (short)DFCareer.Skills.BluntWeapon)
                {
                    float conditionMod = (float)Mathf.Max(9, UnityEngine.Random.Range(9, 19 + Mathf.Clamp(matDiff, -6, 12) + (int)Mathf.Round(Luck / -10f))) / 100f;
                    int damAmount = (int)(item.maxCondition * conditionMod);
                    return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
                }
                else
                {
                    float conditionMod = (float)Mathf.Max(5, UnityEngine.Random.Range(5, 16 + Mathf.Clamp(matDiff, -6, 10) + (int)Mathf.Round(Luck / -10f))) / 100f;
                    int damAmount = (int)(item.maxCondition * conditionMod);
                    return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
                }
            }
            else if (item.ItemGroup == ItemGroups.MensClothing || item.ItemGroup == ItemGroups.WomensClothing || item.ItemGroup == ItemGroups.Books ||
                item.ItemGroup == ItemGroups.Jewellery || item.ItemGroup == ItemGroups.Paintings)
            {
                float conditionMod = (float)Mathf.Max(7, UnityEngine.Random.Range(7, 19 + (int)Mathf.Round(Luck / -10f))) / 100f;
                int damAmount = (int)(item.maxCondition * conditionMod);
                return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
            }
            else if (item.IsAStack())
            {
                int breakNum = UnityEngine.Random.Range(1, item.stackCount + 1);
                DetermineDestroyedLootRefuseType(item, out wasteItem, out wasteAmount);
                wasteItem.stackCount = breakNum * wasteAmount;
                chest.AttachedLoot.AddItem(wasteItem);

                if (breakNum < item.stackCount)
                {
                    item.stackCount -= breakNum;
                    return false;
                }
                else
                {
                    chest.AttachedLoot.RemoveItem(item);
                    return true;
                }
            }
            else
            {
                DetermineDestroyedLootRefuseType(item, out wasteItem, out wasteAmount);
                wasteItem.stackCount = wasteAmount;
                chest.AttachedLoot.AddItem(wasteItem);
                chest.AttachedLoot.RemoveItem(item);

                return true;
            }
        }

        public static bool HandleDestroyingLootItem(LLCObject chest, DaggerfallUnityItem item, int damOrDisin, int spellMag) // Handles most of the "work" part of breaking/destroying loot items, removing the item and adding the respective "waste" item in its place.
        {
            DaggerfallUnityItem wasteItem;
            int wasteAmount;

            if (chest == null || item == null)
                return false;

            if (item.ItemGroup == ItemGroups.Weapons && item.TemplateIndex != (int)Weapons.Arrow)
            {
                if (damOrDisin == 2) // Disintegration Effect
                {
                    float conditionMod = (float)Mathf.Max(35, UnityEngine.Random.Range(35, 51 + (int)Mathf.Round(Luck / -10f))) / 100f;
                    int damAmount = (int)(item.maxCondition * conditionMod);
                    return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
                }
                else if (damOrDisin == 1) // Damage Health Effect
                {
                    float conditionMod = (float)Mathf.Max(15, UnityEngine.Random.Range(15, 17 + Mathf.Clamp(Mathf.Round(spellMag / 3), 1, 50) + (int)Mathf.Round(Luck / -10f))) / 100f;
                    int damAmount = (int)(item.maxCondition * conditionMod);
                    return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
                }
                return false;
            }
            else if (item.ItemGroup == ItemGroups.Armor)
            {
                if (damOrDisin == 2) // Disintegration Effect
                {
                    float conditionMod = (float)Mathf.Max(27, UnityEngine.Random.Range(27, 44 + (int)Mathf.Round(Luck / -10f))) / 100f;
                    int damAmount = (int)(item.maxCondition * conditionMod);
                    return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
                }
                else if (damOrDisin == 1) // Damage Health Effect
                {
                    float conditionMod = (float)Mathf.Max(7, UnityEngine.Random.Range(7, 9 + Mathf.Clamp(Mathf.Round(spellMag / 3), 1, 40) + (int)Mathf.Round(Luck / -10f))) / 100f;
                    int damAmount = (int)(item.maxCondition * conditionMod);
                    return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
                }
                return false;
            }
            else if (item.ItemGroup == ItemGroups.MensClothing || item.ItemGroup == ItemGroups.WomensClothing || item.ItemGroup == ItemGroups.Books ||
                item.ItemGroup == ItemGroups.Jewellery || item.ItemGroup == ItemGroups.Paintings)
            {
                if (damOrDisin == 2) // Disintegration Effect
                {
                    float conditionMod = (float)Mathf.Max(60, UnityEngine.Random.Range(60, 110 + (int)Mathf.Round(Luck / -10f))) / 100f;
                    int damAmount = (int)(item.maxCondition * conditionMod);
                    return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
                }
                else if (damOrDisin == 1) // Damage Health Effect
                {
                    float conditionMod = (float)Mathf.Max(30, UnityEngine.Random.Range(30, 32 + Mathf.Clamp(Mathf.Round(spellMag / 3), 1, 100) + (int)Mathf.Round(Luck / -10f))) / 100f;
                    int damAmount = (int)(item.maxCondition * conditionMod);
                    return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
                }
                return false;
            }
            else if (item.IsAStack())
            {
                int breakNum = UnityEngine.Random.Range((int)Mathf.Ceil(item.stackCount * 0.8f), item.stackCount + 1);
                DetermineDestroyedLootRefuseType(item, out wasteItem, out wasteAmount);
                wasteItem.stackCount = breakNum * wasteAmount;
                chest.AttachedLoot.AddItem(wasteItem);

                if (breakNum < item.stackCount)
                {
                    item.stackCount -= breakNum;
                    return false;
                }
                else
                {
                    chest.AttachedLoot.RemoveItem(item);
                    return true;
                }
            }
            else
            {
                DetermineDestroyedLootRefuseType(item, out wasteItem, out wasteAmount);
                wasteItem.stackCount = wasteAmount;
                chest.AttachedLoot.AddItem(wasteItem);
                chest.AttachedLoot.RemoveItem(item);

                return true;
            }
        }

        public static bool HandleDestroyingLootItem(LLCObject chest, DaggerfallUnityItem item) // Handles most of the "work" part of breaking/destroying loot items, removing the item and adding the respective "waste" item in its place.
        {
            DaggerfallUnityItem wasteItem;
            int wasteAmount;

            if (chest == null || item == null)
                return false;

            if (item.ItemGroup == ItemGroups.Weapons && item.TemplateIndex != (int)Weapons.Arrow)
            {
                float conditionMod = (float)Mathf.Max(11, UnityEngine.Random.Range(11, 26 + (int)Mathf.Round(Luck / -10f))) / 100f;
                int damAmount = (int)(item.maxCondition * conditionMod);
                return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
            }
            else if (item.ItemGroup == ItemGroups.Armor)
            {
                float conditionMod = (float)Mathf.Max(9, UnityEngine.Random.Range(9, 23 + (int)Mathf.Round(Luck / -10f))) / 100f;
                int damAmount = (int)(item.maxCondition * conditionMod);
                return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
            }
            else if (item.ItemGroup == ItemGroups.MensClothing || item.ItemGroup == ItemGroups.WomensClothing || item.ItemGroup == ItemGroups.Books ||
                item.ItemGroup == ItemGroups.Jewellery || item.ItemGroup == ItemGroups.Paintings)
            {
                float conditionMod = (float)Mathf.Max(20, UnityEngine.Random.Range(20, 52 + (int)Mathf.Round(Luck / -10f))) / 100f;
                int damAmount = (int)(item.maxCondition * conditionMod);
                return RemoveOrDamageBasedOnCondition(chest, item, damAmount);
            }
            else if (item.IsAStack())
            {
                int breakNum = UnityEngine.Random.Range((int)Mathf.Ceil(item.stackCount * 0.55f), item.stackCount + 1);
                DetermineDestroyedLootRefuseType(item, out wasteItem, out wasteAmount);
                wasteItem.stackCount = breakNum * wasteAmount;
                chest.AttachedLoot.AddItem(wasteItem);

                if (breakNum < item.stackCount)
                {
                    item.stackCount -= breakNum;
                    return false;
                }
                else
                {
                    chest.AttachedLoot.RemoveItem(item);
                    return true;
                }
            }
            else
            {
                DetermineDestroyedLootRefuseType(item, out wasteItem, out wasteAmount);
                wasteItem.stackCount = wasteAmount;
                chest.AttachedLoot.AddItem(wasteItem);
                chest.AttachedLoot.RemoveItem(item);

                return true;
            }
        }

        public static bool RemoveOrDamageBasedOnCondition(LLCObject chest, DaggerfallUnityItem item, int damAmount) // Mainly here to reduce repetition in code of parent method a bit.
        {
            DaggerfallUnityItem wasteItem;
            int wasteAmount;

            if (damAmount < item.currentCondition)
            {
                item.currentCondition -= damAmount;
                return false;
            }
            else
            {
                DetermineDestroyedLootRefuseType(item, out wasteItem, out wasteAmount);
                wasteItem.stackCount = wasteAmount;
                chest.AttachedLoot.AddItem(wasteItem);
                chest.AttachedLoot.RemoveItem(item);
                return true;
            }
        }

        public static LootItemSturdiness DetermineLootItemSturdiness(DaggerfallUnityItem item)
        {
            if (item.TemplateIndex >= 0 && item.TemplateIndex <= 511) // Vanilla Item Templates
            {
                switch (item.ItemGroup)
                {
                    case ItemGroups.UselessItems1:
                        return LootItemSturdiness.Very_Fragile;
                    case ItemGroups.Drugs:
                    case ItemGroups.MensClothing:
                    case ItemGroups.Books:
                    case ItemGroups.UselessItems2:
                    case ItemGroups.ReligiousItems:
                    case ItemGroups.Maps:
                    case ItemGroups.WomensClothing:
                    case ItemGroups.Paintings:
                    case ItemGroups.PlantIngredients1:
                    case ItemGroups.PlantIngredients2:
                    case ItemGroups.MiscellaneousIngredients1:
                    case ItemGroups.Jewellery:
                    case ItemGroups.MiscItems:
                        return LootItemSturdiness.Fragile;
                    case ItemGroups.Weapons:
                    case ItemGroups.Gems:
                    case ItemGroups.CreatureIngredients1:
                    case ItemGroups.CreatureIngredients2:
                    case ItemGroups.CreatureIngredients3:
                    case ItemGroups.MetalIngredients:
                    case ItemGroups.MiscellaneousIngredients2:
                    case ItemGroups.Currency:
                    default:
                        return LootItemSturdiness.Solid;
                    case ItemGroups.Armor:
                        return LootItemSturdiness.Resilient;
                }
            }
            else // Modded Item Templates
            {
                return LootItemSturdiness.Unbreakable; // Will change eventually, placeholder value for now.
            }
        }

        public static void DetermineDestroyedLootRefuseType(DaggerfallUnityItem item, out DaggerfallUnityItem wasteItem, out int wasteAmount) // Obviously need to add these custom "Junk Items" in at some point.
        {
            wasteAmount = 1;

            if (item.TemplateIndex >= 0 && item.TemplateIndex <= 511) // Vanilla Item Templates
            {
                switch (item.ItemGroup)
                {
                    case ItemGroups.UselessItems1:
                        if (item.IsPotion) { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, 31111); return; } // "Glass Fragments" custom item
                        else { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, 91111); return; }
                    case ItemGroups.Armor:
                        wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, 11119); wasteAmount = GetWasteAmount(item); return; // "(Material) Scraps" custom item
                    case ItemGroups.Weapons:
                        if (item.TemplateIndex == (int)Weapons.Arrow) { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, 42329); return; } // "Broken Arrow" custom item
                        else { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, 11119); wasteAmount = GetWasteAmount(item); return; }
                    case ItemGroups.MensClothing:
                    case ItemGroups.WomensClothing:
                        wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, 11113); return; // "Tattered Cloth" custom item
                    case ItemGroups.Books:
                    case ItemGroups.Maps:
                        wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, 33333); wasteAmount = GetWasteAmount(item); return; // "Paper Shreds" custom item
                    case ItemGroups.UselessItems2:
                        if (item.TemplateIndex == (int)UselessItems2.Oil || item.TemplateIndex == (int)UselessItems2.Lantern) { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, 31111); return; }
                        else if (item.TemplateIndex == (int)UselessItems2.Bandage) { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, 11113); return; }
                        else if (item.TemplateIndex == (int)UselessItems2.Parchment) { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, 33333); return; }
                        else { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, 91111); return; }
                    case ItemGroups.Gems:
                    case ItemGroups.MiscellaneousIngredients2:
                    case ItemGroups.MetalIngredients:
                        if (item.TemplateIndex == (int)MiscellaneousIngredients2.Ivory) { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, 71111); return; } // "Ivory Fragments" custom item
                        else { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, 32323); return; } // "Shiny Rubble" custom item
                    case ItemGroups.Drugs:
                    case ItemGroups.PlantIngredients1:
                    case ItemGroups.PlantIngredients2:
                        wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, 42424); return; // "Clump of Plant Matter" custom item
                    case ItemGroups.MiscellaneousIngredients1:
                        if (item.TemplateIndex >= 59 && item.TemplateIndex <= 64) { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, 31111); return; }
                        else { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, 71111); return; }
                    case ItemGroups.CreatureIngredients1:
                    case ItemGroups.CreatureIngredients2:
                        wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, 11117); return; // "Glob of Gore" custom item
                    case ItemGroups.CreatureIngredients3:
                        if (item.TemplateIndex == (int)CreatureIngredients3.Nymph_hair) { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, 11117); return; }
                        else { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, 71111); return; }
                    case ItemGroups.Jewellery:
                        wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, 24242); return; // "Destroyed Jewelry" custom item
                    case ItemGroups.MiscItems:
                        if (item.TemplateIndex == (int)MiscItems.Soul_trap) { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, 32323); return; }
                        else if (item.TemplateIndex == (int)MiscItems.Letter_of_credit || item.TemplateIndex == (int)MiscItems.Potion_recipe || item.TemplateIndex == (int)MiscItems.Map) { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, 33333); return; }
                        else { wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, 91111); return; }
                    case ItemGroups.Currency:
                        wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, 91191); return; // "Ruined Coins" custom item
                    default:
                        wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, 91111); return; // "Useless Refuse" custom item
                }
            }
            else // Modded Item Templates
            {
                wasteItem = ItemBuilder.CreateItem(ItemGroups.UselessItems1, 91111); return; // "Useless Refuse" custom item?
            }
        }

        public static int GetWasteAmount(DaggerfallUnityItem item)
        {
            if (item == null)
                return 1;
            // Next I work on this, fill in these values depending on how much "scrap" should be given, likely make a random range between some set values for each, will determine then.
            if (item.ItemGroup == ItemGroups.Armor)
            {
                switch (item.TemplateIndex)
                {
                    case (int)Armor.Cuirass:
                    case (int)Armor.Gauntlets:
                    case (int)Armor.Greaves:
                    case (int)Armor.Left_Pauldron:
                    case (int)Armor.Right_Pauldron:
                    case (int)Armor.Helm:
                    case (int)Armor.Boots:
                    case (int)Armor.Buckler:
                    case (int)Armor.Round_Shield:
                    case (int)Armor.Kite_Shield:
                    case (int)Armor.Tower_Shield:
                    default:
                        return 1;
                }
            }
            else if (item.ItemGroup == ItemGroups.Weapons)
            {
                switch (item.TemplateIndex)
                {
                    case (int)Weapons.Dagger:
                    case (int)Weapons.Tanto:
                    case (int)Weapons.Staff:
                    case (int)Weapons.Shortsword:
                    case (int)Weapons.Wakazashi:
                    case (int)Weapons.Broadsword:
                    case (int)Weapons.Saber:
                    case (int)Weapons.Longsword:
                    case (int)Weapons.Katana:
                    case (int)Weapons.Claymore:
                    case (int)Weapons.Dai_Katana:
                    case (int)Weapons.Mace:
                    case (int)Weapons.Flail:
                    case (int)Weapons.Warhammer:
                    case (int)Weapons.Battle_Axe:
                    case (int)Weapons.War_Axe:
                    case (int)Weapons.Short_Bow:
                    case (int)Weapons.Long_Bow:
                    default:
                        return 1;
                }
            }
            else if (item.ItemGroup == ItemGroups.Books)
                return UnityEngine.Random.Range(3, 11);
            else
                return 1;
        }

        public static int daggerfallMatsToLLCValue(int nativeMaterialValue) // For determining "material difference" between weapon and LLC material estimated equivalent, mostly placeholder for now.
        {
            switch ((WeaponMaterialTypes)nativeMaterialValue)
            {
                default:
                case WeaponMaterialTypes.Iron:
                    return 0;
                case WeaponMaterialTypes.Steel:
                case WeaponMaterialTypes.Silver:
                    return 1;
                case WeaponMaterialTypes.Elven:
                case WeaponMaterialTypes.Dwarven:
                    return 2;
                case WeaponMaterialTypes.Mithril:
                    return 3;
                case WeaponMaterialTypes.Adamantium:
                case WeaponMaterialTypes.Ebony:
                    return 4;
                case WeaponMaterialTypes.Orcish:
                    return 5;
                case WeaponMaterialTypes.Daedric:
                    return 6;
            }
        }

        public static int chestMaterialToDaggerfallValue(ChestMaterials chestMat)
        {
            switch (chestMat)
            {
                default:
                case ChestMaterials.Wood:
                    return -1;
                case ChestMaterials.Iron:
                    return 0;
                case ChestMaterials.Steel:
                    return 1;
                case ChestMaterials.Orcish:
                    return 5;
                case ChestMaterials.Mithril:
                    return 3;
                case ChestMaterials.Dwarven:
                    return 2;
                case ChestMaterials.Adamantium:
                    return 4;
                case ChestMaterials.Daedric:
                    return 6;
            }
        }

        public static int lockMaterialToDaggerfallValue(LockMaterials lockMat)
        {
            switch (lockMat)
            {
                default:
                case LockMaterials.Wood:
                    return -1;
                case LockMaterials.Iron:
                    return 0;
                case LockMaterials.Steel:
                    return 1;
                case LockMaterials.Orcish:
                    return 5;
                case LockMaterials.Mithril:
                    return 3;
                case LockMaterials.Dwarven:
                    return 2;
                case LockMaterials.Adamantium:
                    return 4;
                case LockMaterials.Daedric:
                    return 6;
            }
        }

        public static T[] FillArray<T>(List<T> list, int start, int count, T value)
        {
            for (var i = start; i < start + count; i++)
            {
                list.Add(value);
            }

            return list.ToArray();
        }

        public static int PickOneOf(params int[] values) // Pango provided assistance in making this much cleaner way of doing the random value choice part, awesome.
        {
            return values[UnityEngine.Random.Range(0, values.Length)];
        }
    }
}