using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallConnect;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Utility;

namespace LockedLootContainers
{
    public partial class LockedLootContainersMain
    {
        private static void AttemptMeleeChestBash(LLCObject chest, DaggerfallUnityItem weapon)
        {
            if (chest != null)
            {
                ItemCollection closedChestLoot = chest.AttachedLoot;
                Transform closedChestTransform = chest.gameObject.transform; // Not sure if the explicit "gameObject" reference is necessary, will test eventually and determine if so or not.
                Vector3 pos = chest.gameObject.transform.position;
                DaggerfallAudioSource dfAudioSource = GameManager.Instance.PlayerActivate.GetComponent<DaggerfallAudioSource>();

                chest.HasBeenBashed = true;

                if (BashLockRoll(chest, weapon))
                {
                    if (LockHardBashRoll(chest, weapon)) // False = Light Bash, True = Hard Bash
                        chest.LockBashedHardTimes++;
                    else
                        chest.LockBashedLightTimes++;

                    if (BreakLockRoll(chest, weapon))
                    {
                        // Lock was hit with bash and is now broken, so chest loot is accessible.
                        BashingOpenChestDamagesLoot(chest, weapon, false);
                        DaggerfallLoot openChestLoot = GameObjectHelper.CreateLootContainer(LootContainerTypes.Nothing, InventoryContainerImages.Chest, pos, closedChestTransform.parent, 4735, 0, chest.LoadID, null, false);
                        openChestLoot.gameObject.name = GameObjectHelper.GetGoFlatName(4735, 0);
                        openChestLoot.Items.TransferAll(closedChestLoot); // Transfers items from closed chest's items to the new open chest's item collection.

                        Destroy(chest.gameObject); // Removed closed chest from scene, but saved its characteristics we care about for opened chest loot-pile.

                        // Show success and play unlock sound
                        DaggerfallUI.AddHUDText("With use of brute force, the lock finally breaks open...", 4f); // Will possibly change text later on depending on many factors, will see.
                        if (dfAudioSource != null)
                            dfAudioSource.PlayOneShot(SoundClips.Parry1); // Will use custom sounds in the end most likely.
                    }
                    else
                    {
                        // Lock was hit with bash, but is still intact.
                        if (dfAudioSource != null)
                            dfAudioSource.PlayOneShot(SoundClips.Parry5); // Might change this later to emit sound from chest audiosource itself instead of player's? Will use custom sounds later on.
                    }
                }
                else
                {
                    if (ChestHardBashRoll(chest, weapon)) // False = Light Bash, True = Hard Bash
                        chest.ChestBashedHardTimes++;
                    else
                        chest.ChestBashedLightTimes++;

                    if (SmashOpenChestRoll(chest, weapon))
                    {
                        // Chest body has been smashed open and contents are accessible (but damaged greatly most likely.)
                        BashingOpenChestDamagesLoot(chest, weapon, true);
                        DaggerfallLoot openChestLoot = GameObjectHelper.CreateLootContainer(LootContainerTypes.Nothing, InventoryContainerImages.Chest, pos, closedChestTransform.parent, 4735, 0, chest.LoadID, null, false);
                        openChestLoot.gameObject.name = GameObjectHelper.GetGoFlatName(4735, 0);
                        openChestLoot.Items.TransferAll(closedChestLoot); // Transfers items from closed chest's items to the new open chest's item collection.

                        Destroy(chest.gameObject); // Removed closed chest from scene, but saved its characteristics we care about for opened chest loot-pile.

                        // Show success and play unlock sound
                        DaggerfallUI.AddHUDText("You smash a large hole in the body of the chest, granting access to its contents...", 4f); // Will possibly change text later on depending on many factors, will see.
                        if (dfAudioSource != null)
                            dfAudioSource.PlayOneShot(SoundClips.StormLightningThunder); // Will use custom sounds in the end most likely.
                    }
                    else
                    {
                        // Chest body was hit with bash, but is still intact.
                        if (dfAudioSource != null)
                            dfAudioSource.PlayOneShot(SoundClips.PlayerDoorBash); // Might change this later to emit sound from chest audiosource itself instead of player's? Will use custom sounds later on.
                    }
                }
            }
            else
            {
                DaggerfallUI.AddHUDText("ERROR: Chest Was Found As Null.", 5f);
            }
        }

        public static bool AttemptChestBashWithArrows(LLCObject chest, DaggerfallMissile missile)
        {
            if (chest != null)
            {
                ItemCollection closedChestLoot = chest.AttachedLoot;
                Transform closedChestTransform = chest.gameObject.transform; // Not sure if the explicit "gameObject" reference is necessary, will test eventually and determine if so or not.
                Vector3 pos = chest.gameObject.transform.position;
                DaggerfallAudioSource dfAudioSource = GameManager.Instance.PlayerActivate.GetComponent<DaggerfallAudioSource>();

                chest.HasBeenBashed = true;

                // Would be interesting when an arrow "pings" off a metal chest it would actually leave a physical object nearby that could be clicked instead of adding to chest inventory, but eh.
                chest.AttachedLoot.AddItem(ItemBuilder.CreateItem(ItemGroups.UselessItems1, 42329)); // "Broken Arrow" custom item

                DaggerfallUnityItem bowUsed = GameManager.Instance.WeaponManager.LastBowUsed;
                if (bowUsed != null)
                    bowUsed.LowerCondition(1, Player);

                if (chest.ChestMaterial == ChestMaterials.Wood)
                {
                    chest.ChestBashedLightTimes++;

                    if (SmashOpenChestWithArrowRoll(chest))
                    {
                        // Chest body has been smashed open and contents are accessible (but damaged significantly.)
                        BashingOpenChestWithArrowsDamagesLoot(chest);
                        DaggerfallLoot openChestLoot = GameObjectHelper.CreateLootContainer(LootContainerTypes.Nothing, InventoryContainerImages.Chest, pos, closedChestTransform.parent, 4735, 0, chest.LoadID, null, false);
                        openChestLoot.gameObject.name = GameObjectHelper.GetGoFlatName(4735, 0);
                        openChestLoot.Items.TransferAll(closedChestLoot); // Transfers items from closed chest's items to the new open chest's item collection.

                        // Show success and play smash open sound
                        DaggerfallUI.AddHUDText("The arrow smashes a large hole in the chest, granting access to its contents...", 4f); // Will possibly change text later on depending on many factors, will see.
                        if (dfAudioSource)
                        {
                            if (dfAudioSource != null)
                                dfAudioSource.PlayClipAtPoint(SoundClips.StormLightningThunder, chest.gameObject.transform.position); // Will use custom sounds in the end most likely.
                        }
                        return true;
                    }
                    else
                    {
                        // Chest body was hit with arrow, but is still intact.
                        if (dfAudioSource)
                        {
                            if (dfAudioSource != null)
                                dfAudioSource.PlayClipAtPoint(SoundClips.PlayerDoorBash, chest.gameObject.transform.position); // Might change this later to emit sound from chest audiosource itself instead of player's? Will use custom sounds later on.
                        }
                        return false;
                    }
                }
                else
                {
                    // Metal chest body was hit with arrow, but it does nothing besides ping off since it's metal.
                    if (dfAudioSource)
                    {
                        if (dfAudioSource != null)
                            dfAudioSource.PlayClipAtPoint(SoundClips.Parry9, chest.gameObject.transform.position); // Will use custom sounds in the end most likely.
                    }
                    return false;
                }
            }
            else
            {
                DaggerfallUI.AddHUDText("ERROR: Chest Was Found As Null.", 5f);
            }
            return false;
        }

        public static bool BashLockRoll(LLCObject chest, DaggerfallUnityItem weapon) // Roll to determine if a bash attempt hit the lock or the chest body instead.
        {
            short skillUsed = (weapon != null) ? weapon.GetWeaponSkillIDAsShort() : (short)DFCareer.Skills.HandToHand;
            int wepSkill = Player.Skills.GetLiveSkillValue(skillUsed); // Might add accuracy modifier for weapon type, but for now just keep it more simple.
            float accuracyCheck = Mathf.Round(wepSkill / 4) + Mathf.Round(Willp * .2f) + Mathf.Round(Agili * .5f) + Mathf.Round(Speed * .1f) + Mathf.Round(Luck * .3f);

            if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(accuracyCheck, 5f, 70f))))
            {
                Player.TallySkill((DFCareer.Skills)skillUsed, 1);
                return true;
            }
            else
                return false;
        }

        public static bool LockHardBashRoll(LLCObject chest, DaggerfallUnityItem weapon) // Roll to determine if bashed lock was hit hard or lightly, I.E. Effectively or not very effectively.
        {
            int lockMat = LockMaterialToDaggerfallValue(chest.LockMaterial);
            int weapMat = (weapon != null) ? weapon.NativeMaterialValue : -1;
            int matDiff = weapMat - lockMat;
            int wepSkillID = (weapon != null) ? weapon.GetWeaponSkillIDAsShort() : (short)DFCareer.Skills.HandToHand;
            bool hardBash = false;

            if (wepSkillID == (int)DFCareer.Skills.HandToHand) // Checks if the "weapon" being used is the player's fists.
            {
                if (lockMat < 0) // If lock is made of wood
                {
                    float hardBashChance = Mathf.Round(Stren + (int)Mathf.Round(Luck / 5f) + 30f);
                    if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(hardBashChance, 5f, 85f))))
                        hardBash = true;

                    ApplyBashingCostLogic(chest, weapon, true, true, hardBash, matDiff);
                    return hardBash;
                }
                else // If lock is made from any metal
                {
                    float hardBashChance = Mathf.Round(Stren + (int)Mathf.Round(Luck / 5f) + 5f);
                    if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(hardBashChance, 1f, 60f))))
                        hardBash = true;

                    ApplyBashingCostLogic(chest, weapon, true, false, hardBash, matDiff);
                    return hardBash;
                }
            }
            else if (wepSkillID == (int)DFCareer.Skills.BluntWeapon) // Checks if the weapon being used is a Blunt Weapon. Later on might check for other weapon types as well, but just this for now.
            {
                if (lockMat < 0) // If lock is made of wood
                {
                    float hardBashChance = Mathf.Round((int)Mathf.Round(Mathf.Clamp(weapon.weightInKg - 2f, 0.75f, 20f) / (0.6f - (Stren / 100f))) + (int)Mathf.Round(Luck / 5f) + 40f);
                    if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(hardBashChance, 10f, 100f))))
                        hardBash = true;

                    ApplyBashingCostLogic(chest, weapon, true, true, hardBash, matDiff);
                    return hardBash;
                }
                else // If lock is made from any metal
                {
                    float hardBashChance = Mathf.Round((int)Mathf.Round(Mathf.Clamp(weapon.weightInKg - 2f, 0.75f, 20f) / (0.6f - (Stren / 100f))) + (int)Mathf.Round(Luck / 5f) + 5f);
                    if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(hardBashChance, 2f, 97f))))
                        hardBash = true;

                    ApplyBashingCostLogic(chest, weapon, true, false, hardBash, matDiff);
                    return hardBash;
                }
            }
            else // For all non-blunt weapons for now.
            {
                if (lockMat < 0) // If lock is made of wood
                {
                    float hardBashChance = Mathf.Round(((matDiff + (int)Mathf.Round(Stren / 10f)) * 10f) + (int)Mathf.Round(Luck / 5f) + 40f); // Just for now, will have to see through testing if this needs work.
                    if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(hardBashChance, 10f, 100f))))
                        hardBash = true;

                    ApplyBashingCostLogic(chest, weapon, true, true, hardBash, matDiff);
                    return hardBash;
                }
                else // If lock is made from any metal
                {
                    float hardBashChance = Mathf.Round(((matDiff + (int)Mathf.Round(Stren / 10f)) * 10f) + (int)Mathf.Round(Luck / 5f) + 30f); // Just for now, will have to see through testing if this needs work.
                    if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(hardBashChance, 2f, 97f))))
                        hardBash = true;

                    ApplyBashingCostLogic(chest, weapon, true, false, hardBash, matDiff);
                    return hardBash;
                }
            }
        }

        public static bool BreakLockRoll(LLCObject chest, DaggerfallUnityItem weapon) // Roll to determine if bashed lock breaks or not this attempt, granting full access to loot.
        {
            int lockMat = LockMaterialToDaggerfallValue(chest.LockMaterial);
            float bashResist = (float)chest.LockSturdiness / 100f;
            float stabilityMod = (bashResist - 1f) * -1f; // Modifier value might need some work, especially for the very sturdy lock materials, but will see with testing later, etc.

            // This if-statement is meant to simulate a sort of "decisive strike", that if you got a good hit in on the lock the first attempt only, there is much higher odds of the lock
            // just breaking off in one go right there. But any times after that, or if other attempts were already made on the chest it's just the normal odds from there on.
            if (chest.LockBashedHardTimes == 1 && chest.ChestBashedHardTimes == 0 && chest.ChestBashedLightTimes == 0 && chest.LockBashedLightTimes == 0)
            {
                float lockBreakChance = Mathf.Round((int)Mathf.Round(Luck / 5f) + 45f);

                if (Dice100.SuccessRoll((int)lockBreakChance))
                    return true;
                else
                    return false;
            }

            if (lockMat < 0) // If lock is made of wood
            {
                float lockBreakChance = (int)Mathf.Round((chest.LockBashedHardTimes * 20) + (chest.LockBashedLightTimes * 4) * stabilityMod) + (int)Mathf.Round(Luck / 5f);

                if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(lockBreakChance, 1f, 95f))))
                    return true;
                else
                    return false;
            }
            else // If lock is made from any metal
            {
                float lockBreakChance = (int)Mathf.Round((chest.LockBashedHardTimes * 15) + (chest.LockBashedLightTimes * 3) * stabilityMod) + (int)Mathf.Round(Luck / 5f);

                if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(lockBreakChance, 1f, 80f))))
                    return true;
                else
                    return false;
            }
        }

        public static bool ChestHardBashRoll(LLCObject chest, DaggerfallUnityItem weapon) // Roll to determine if bashed chest body was hit hard or lightly, I.E. Effectively or not very effectively.
        {
            int chestMat = ChestMaterialToDaggerfallValue(chest.ChestMaterial);
            int weapMat = (weapon != null) ? weapon.NativeMaterialValue : -1;
            int matDiff = weapMat - chestMat;
            int wepSkillID = (weapon != null) ? weapon.GetWeaponSkillIDAsShort() : (short)DFCareer.Skills.HandToHand;
            bool hardBash = false;

            if (wepSkillID == (int)DFCareer.Skills.HandToHand) // Checks if the "weapon" being used is the player's fists.
            {
                if (chestMat < 0) // If chest is made of wood
                {
                    float hardBashChance = Mathf.Round(Stren + (int)Mathf.Round(Luck / 5f) + 20f);
                    if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(hardBashChance, 3f, 70f))))
                        hardBash = true;

                    ApplyBashingCostLogic(chest, weapon, false, true, hardBash, matDiff);
                    return hardBash;
                }
                else // If chest is made from any metal
                {
                    float hardBashChance = Mathf.Round(Stren + (int)Mathf.Round(Luck / 5f) + 3f);
                    if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(hardBashChance, 1f, 40f))))
                        hardBash = true;

                    ApplyBashingCostLogic(chest, weapon, false, false, hardBash, matDiff);
                    return hardBash;
                }
            }
            else if (wepSkillID == (int)DFCareer.Skills.BluntWeapon) // Checks if the weapon being used is a Blunt Weapon. Later on might check for other weapon types as well, but just this for now.
            {
                if (chestMat < 0) // If chest is made of wood
                {
                    float hardBashChance = Mathf.Round((int)Mathf.Round(Mathf.Clamp(weapon.weightInKg - 2f, 0.75f, 20f) / (0.6f - (Stren / 100f))) + (int)Mathf.Round(Luck / 5f) + 30f);
                    if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(hardBashChance, 6f, 96f))))
                        hardBash = true;

                    ApplyBashingCostLogic(chest, weapon, false, true, hardBash, matDiff);
                    return hardBash;
                }
                else // If chest is made from any metal
                {
                    float hardBashChance = Mathf.Round((int)Mathf.Round(Mathf.Clamp(weapon.weightInKg - 2f, 0.75f, 20f) / (0.6f - (Stren / 100f))) + (int)Mathf.Round(Luck / 5f) + 3f);
                    if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(hardBashChance, 1f, 90f))))
                        hardBash = true;

                    ApplyBashingCostLogic(chest, weapon, false, false, hardBash, matDiff);
                    return hardBash;
                }
            }
            else // For all non-blunt weapons for now.
            {
                if (chestMat < 0) // If chest is made of wood
                {
                    float hardBashChance = Mathf.Round(((matDiff + (int)Mathf.Round(Stren / 10f)) * 10f) + (int)Mathf.Round(Luck / 5f) + 30f); // Just for now, will have to see through testing if this needs work.
                    if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(hardBashChance, 6f, 96f))))
                        hardBash = true;

                    ApplyBashingCostLogic(chest, weapon, false, true, hardBash, matDiff);
                    return hardBash;
                }
                else // If chest is made from any metal
                {
                    float hardBashChance = Mathf.Round(((matDiff + (int)Mathf.Round(Stren / 10f)) * 10f) + (int)Mathf.Round(Luck / 5f) + 20f); // Just for now, will have to see through testing if this needs work.
                    if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(hardBashChance, 1f, 90f))))
                        hardBash = true;

                    ApplyBashingCostLogic(chest, weapon, false, false, hardBash, matDiff);
                    return hardBash;
                }
            }
        }

        public static bool SmashOpenChestRoll(LLCObject chest, DaggerfallUnityItem weapon) // Roll to determine if bashed chest body breaks open or not this attempt, granting full access to loot.
        {
            int chestMat = ChestMaterialToDaggerfallValue(chest.ChestMaterial);
            float bashResist = (float)chest.ChestSturdiness / 100f;
            float stabilityMod = (bashResist - 1f) * -1f; // Modifier value might need some work, especially for the very sturdy chest materials, but will see with testing later, etc.

            // This if-statement is meant to simulate a sort of "decisive strike", that if you got a good hit in on the chest the first attempt only, there is much higher odds of the chest
            // just breaking open in one go right there. But any times after that, or if other attempts were already made on the chest it's just the normal odds from there on.
            if (chest.ChestBashedHardTimes == 1 && chest.LockBashedHardTimes == 0 && chest.LockBashedLightTimes == 0 && chest.ChestBashedLightTimes == 0)
            {
                float chestSmashOpenChance = Mathf.Round((int)Mathf.Round(Luck / 5f) + 15f);

                if (Dice100.SuccessRoll((int)chestSmashOpenChance))
                    return true;
                else
                    return false;
            }

            if (chestMat < 0) // If chest is made of wood
            {
                float chestSmashOpenChance = (int)Mathf.Round((chest.ChestBashedHardTimes * 10) + (chest.ChestBashedLightTimes * 2) * stabilityMod) + (int)Mathf.Round(Luck / 5f);

                if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(chestSmashOpenChance, 0f, 85f))))
                    return true;
                else
                    return false;
            }
            else // If chest is made from any metal
            {
                float chestSmashOpenChance = (int)Mathf.Round((chest.ChestBashedHardTimes * 5) + (chest.ChestBashedLightTimes * 1) * stabilityMod) + (int)Mathf.Round(Luck / 5f);

                if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(chestSmashOpenChance, 0f, 70f))))
                    return true;
                else
                    return false;
            }
        }

        public static bool SmashOpenChestWithArrowRoll(LLCObject chest) // Roll to determine if bashed chest body breaks open or not this attempt, granting full access to loot.
        {
            float bashResist = (float)chest.ChestSturdiness / 100f;
            float stabilityMod = (bashResist - 1f) * -1f; // Modifier value might need some work, especially for the very sturdy chest materials, but will see with testing later, etc.
            float chestSmashOpenChance = (int)Mathf.Round((chest.ChestBashedLightTimes * (int)Mathf.Round((Stren / 25f) + 3)) * stabilityMod) + (int)Mathf.Round(Luck / 10f);

            if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(chestSmashOpenChance, 0f, 75f))))
            {
                Player.TallySkill(DFCareer.Skills.Archery, 1);
                return true;
            }
            else
                return false;
        }

        public static void ApplyBashingCostLogic(LLCObject chest, DaggerfallUnityItem weapon, bool hitLock = false, bool hitWood = false, bool hardBash = false, int matDiff = -100) // Applies vitals damage to player and the weapon used during bash attempt.
        {
            // Will have to consider later when doing the stuff for "bashing" with the bow, I.E. shooting arrows at a chest, and how to deal with that stuff, possibly different method entirely.
            int wepSkillID = (weapon != null) ? weapon.GetWeaponSkillIDAsShort() : (short)DFCareer.Skills.HandToHand;
            int wepSkill = Player.Skills.GetLiveSkillValue(wepSkillID); // Have not used this value yet, nor "matDiff" will see if I eventually do or not later.
            float bashHitMod = 1f;
            float healthDam = 0f;
            float fatigueDam = 0f;

            if (chest == null)
                return;

            BashingDamageLootContents(chest, weapon, wepSkillID, hitLock, hitWood, hardBash);

            if (hitLock && hardBash) { bashHitMod = 0.6f; } // Math was confusing me on how to combine these values together for some reason, so just doing this weird if-else chain here instead, for now.
            else if (hitLock && !hardBash) { bashHitMod = 0.25f; }
            else if (!hitLock && hardBash) { bashHitMod = 1f; }
            else { bashHitMod = 0.4f; }

            if (weapon == null && wepSkillID == (short)DFCareer.Skills.HandToHand) // If player is bashing chest with their bare fists
            {
                if (hitWood) // Hitting either wooden lock or chest body with bare fists
                {
                    float rolledHpPercent = Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.Round(5 + (int)Mathf.Round(Endur / -10f) + (int)Mathf.Round(Willp / -25f) * bashHitMod)));
                    healthDam = Mathf.Max(1, (int)Mathf.Floor(Player.MaxHealth * (rolledHpPercent / 100f)));
                    healthDam = (Player.CurrentHealth - healthDam < Mathf.Ceil(Player.MaxHealth * 0.4f)) ? Mathf.Max(0, Player.CurrentHealth - (Player.MaxHealth * 0.4f)) : healthDam;

                    if (healthDam > 0)
                    {
                        float rolledFatiguePercent = Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.Round(5 + (int)Mathf.Round(Endur / -10f) + (int)Mathf.Round(Willp / -25f) * bashHitMod)));
                        fatigueDam = Mathf.Max((int)Mathf.Floor(Player.MaxFatigue * 0.01f), (int)Mathf.Floor(Player.MaxFatigue * (rolledFatiguePercent / 100f)));
                        fatigueDam = (Player.CurrentFatigue - fatigueDam < 0) ? Player.CurrentFatigue : fatigueDam;

                        Player.DecreaseHealth((int)healthDam); // Currently could be abused by something like the "Health Regen" trait, but whatever for now.
                        Player.DecreaseFatigue((int)fatigueDam, true); // Will need to do testing to make sure this works with the fatigue multiplier, including above formulas and such, bit confusing.
                    }
                    else // Since player health is currently at "minimum" level that bashing with fists can reduce it, health is not damaged, but instead fatigue damage is multiplied alot as a cost.
                    {
                        float rolledFatiguePercent = Mathf.Max(3, UnityEngine.Random.Range(3, Mathf.Round(16 + (int)Mathf.Round(Endur / -10f) + (int)Mathf.Round(Willp / -25f) * bashHitMod)));
                        fatigueDam = Mathf.Max((int)Mathf.Floor(Player.MaxFatigue * 0.03f), (int)Mathf.Floor(Player.MaxFatigue * (rolledFatiguePercent / 100f)));
                        fatigueDam = (Player.CurrentFatigue - fatigueDam < 0) ? Player.CurrentFatigue : fatigueDam;

                        Player.DecreaseFatigue((int)fatigueDam, true); // Will need to do testing to make sure this works with the fatigue multiplier, including above formulas and such, bit confusing.
                    }
                }
                else // Hitting either metal lock or chest body with bare fists
                {
                    float rolledHpPercent = Mathf.Max(3, UnityEngine.Random.Range(3, Mathf.Round(11 + (int)Mathf.Round(Endur / -10f) + (int)Mathf.Round(Willp / -25f) * bashHitMod)));
                    healthDam = Mathf.Max(1, (int)Mathf.Floor(Player.MaxHealth * (rolledHpPercent / 100f)));
                    healthDam = (Player.CurrentHealth - healthDam < Mathf.Ceil(Player.MaxHealth * 0.4f)) ? Mathf.Max(0, Player.CurrentHealth - (Player.MaxHealth * 0.4f)) : healthDam;

                    if (healthDam > 0)
                    {
                        float rolledFatiguePercent = Mathf.Max(3, UnityEngine.Random.Range(3, Mathf.Round(11 + (int)Mathf.Round(Endur / -10f) + (int)Mathf.Round(Willp / -25f) * bashHitMod)));
                        fatigueDam = Mathf.Max((int)Mathf.Floor(Player.MaxFatigue * 0.03f), (int)Mathf.Floor(Player.MaxFatigue * (rolledFatiguePercent / 100f)));
                        fatigueDam = (Player.CurrentFatigue - fatigueDam < 0) ? Player.CurrentFatigue : fatigueDam;

                        Player.DecreaseHealth((int)healthDam); // Currently could be abused by something like the "Health Regen" trait, but whatever for now.
                        Player.DecreaseFatigue((int)fatigueDam, true); // Will need to do testing to make sure this works with the fatigue multiplier, including above formulas and such, bit confusing.
                    }
                    else // Since player health is currently at "minimum" level that bashing with fists can reduce it, health is not damaged, but instead fatigue damage is multiplied alot as a cost.
                    {
                        float rolledFatiguePercent = Mathf.Max(7, UnityEngine.Random.Range(7, Mathf.Round(25 + (int)Mathf.Round(Endur / -10f) + (int)Mathf.Round(Willp / -25f) * bashHitMod)));
                        fatigueDam = Mathf.Max((int)Mathf.Floor(Player.MaxFatigue * 0.07f), (int)Mathf.Floor(Player.MaxFatigue * (rolledFatiguePercent / 100f)));
                        fatigueDam = (Player.CurrentFatigue - fatigueDam < 0) ? Player.CurrentFatigue : fatigueDam;

                        Player.DecreaseFatigue((int)fatigueDam, true); // Will need to do testing to make sure this works with the fatigue multiplier, including above formulas and such, bit confusing.
                    }
                }
            }
            else if (weapon != null && wepSkillID == (short)DFCareer.Skills.BluntWeapon) // If player is bashing chest with a blunt type weapon
            {
                if (hitWood) // Hitting either wooden lock or chest body with a blunt type weapon
                {
                    float rolledWepDamPercent = Mathf.Max(1, UnityEngine.Random.Range(1, 4 + (int)Mathf.Round((Stren / 10f) * 0.4f) + ((Dice100.SuccessRoll(50 + (int)Mathf.Round(Luck / 2f))) ? -1 : 0))); // Won't include "bashHitMod" for now, since this is already getting messy and confusing as it is, do testing before worrying.
                    float rolledFatiguePercent = Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.Round(3 + (int)Mathf.Round(Endur / -10f) + (int)Mathf.Round(Willp / -25f) * bashHitMod)));
                    fatigueDam = Mathf.Max((int)Mathf.Floor(Player.MaxFatigue * 0.01f), (int)Mathf.Floor(Player.MaxFatigue * (rolledFatiguePercent / 100f)));
                    fatigueDam = (Player.CurrentFatigue - fatigueDam < 0) ? Player.CurrentFatigue : fatigueDam;

                    Player.DecreaseFatigue((int)fatigueDam, true); // Will need to do testing to make sure this works with the fatigue multiplier, including above formulas and such, bit confusing.
                    weapon.LowerCondition((int)Mathf.Max(1, Mathf.Round(weapon.maxCondition * (rolledWepDamPercent / 100f))), Player);
                }
                else // Hitting either metal lock or chest body with a blunt type weapon
                {
                    float rolledWepDamPercent = Mathf.Max(3, UnityEngine.Random.Range(3, 6 + (int)Mathf.Round((Stren / 10f) * 0.4f) + ((Dice100.SuccessRoll(50 + (int)Mathf.Round(Luck / 2f))) ? -1 : 0))); // Won't include "bashHitMod" for now, since this is already getting messy and confusing as it is, do testing before worrying.
                    float rolledFatiguePercent = Mathf.Max(4, UnityEngine.Random.Range(4, Mathf.Round(10 + (int)Mathf.Round(Endur / -10f) + (int)Mathf.Round(Willp / -25f) * bashHitMod)));
                    fatigueDam = Mathf.Max((int)Mathf.Floor(Player.MaxFatigue * 0.04f), (int)Mathf.Floor(Player.MaxFatigue * (rolledFatiguePercent / 100f)));
                    fatigueDam = (Player.CurrentFatigue - fatigueDam < 0) ? Player.CurrentFatigue : fatigueDam;

                    Player.DecreaseFatigue((int)fatigueDam, true); // Will need to do testing to make sure this works with the fatigue multiplier, including above formulas and such, bit confusing.
                    weapon.LowerCondition((int)Mathf.Max(1, Mathf.Round(weapon.maxCondition * (rolledWepDamPercent / 100f))), Player);
                }
            }
            else if (weapon != null) // If player is bashing chest with any other types of weapons (in this case all the bladed ones)
            {
                if (hitWood) // Hitting either wooden lock or chest body with a bladed type weapon
                {
                    float rolledWepDamPercent = Mathf.Max(1, UnityEngine.Random.Range(1, 4 + (int)Mathf.Round((Stren / 10f) * 0.2f) + ((Dice100.SuccessRoll(50 + (int)Mathf.Round(Luck / 2f))) ? -1 : 0))); // Won't include "bashHitMod" for now, since this is already getting messy and confusing as it is, do testing before worrying.
                    rolledWepDamPercent = (!hitLock) ? Mathf.Round(rolledWepDamPercent * (1 + chest.ChestSturdiness / 100f)) : Mathf.Round(rolledWepDamPercent * (1 + chest.LockSturdiness / 300f));
                    float rolledFatiguePercent = Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.Round(3 + (int)Mathf.Round(Endur / -10f) + (int)Mathf.Round(Willp / -25f) * bashHitMod)));
                    fatigueDam = Mathf.Max((int)Mathf.Floor(Player.MaxFatigue * 0.01f), (int)Mathf.Floor(Player.MaxFatigue * (rolledFatiguePercent / 100f)));
                    fatigueDam = (Player.CurrentFatigue - fatigueDam < 0) ? Player.CurrentFatigue : fatigueDam;

                    Player.DecreaseFatigue((int)fatigueDam, true); // Will need to do testing to make sure this works with the fatigue multiplier, including above formulas and such, bit confusing.
                    weapon.LowerCondition((int)Mathf.Max(1, Mathf.Round(weapon.maxCondition * (rolledWepDamPercent / 100f))), Player);
                }
                else // Hitting either metal lock or chest body with a bladed type weapon
                {
                    float rolledWepDamPercent = Mathf.Max(3, UnityEngine.Random.Range(3, 6 + (int)Mathf.Round((Stren / 10f) * 0.2f) + ((Dice100.SuccessRoll(50 + (int)Mathf.Round(Luck / 2f))) ? -1 : 0))); // Won't include "bashHitMod" for now, since this is already getting messy and confusing as it is, do testing before worrying.
                    rolledWepDamPercent = (!hitLock) ? Mathf.Round(rolledWepDamPercent * (1 + chest.ChestSturdiness / 100f)) : Mathf.Round(rolledWepDamPercent * (1 + chest.LockSturdiness / 300f));
                    float rolledFatiguePercent = Mathf.Max(2, UnityEngine.Random.Range(2, Mathf.Round(8 + (int)Mathf.Round(Endur / -10f) + (int)Mathf.Round(Willp / -25f) * bashHitMod)));
                    fatigueDam = Mathf.Max((int)Mathf.Floor(Player.MaxFatigue * 0.02f), (int)Mathf.Floor(Player.MaxFatigue * (rolledFatiguePercent / 100f)));
                    fatigueDam = (Player.CurrentFatigue - fatigueDam < 0) ? Player.CurrentFatigue : fatigueDam;

                    Player.DecreaseFatigue((int)fatigueDam, true); // Will need to do testing to make sure this works with the fatigue multiplier, including above formulas and such, bit confusing.
                    weapon.LowerCondition((int)Mathf.Max(1, Mathf.Round(weapon.maxCondition * (rolledWepDamPercent / 100f))), Player);
                }
            }
            else
            {
                return;
            }
        }

        public static void BashingDamageLootContents(LLCObject chest, DaggerfallUnityItem weapon, int wepSkillID, bool hitLock, bool hitWood, bool hardBash)
        {
            int initialItemCount = chest.AttachedLoot.Count;

            if (weapon == null && wepSkillID == (short)DFCareer.Skills.HandToHand) // If player is bashing chest with their bare fists
            {
                if (hitLock)
                {
                    if (hardBash)
                    {
                        for (int i = 0; i < initialItemCount; i++)
                        {
                            DaggerfallUnityItem item = chest.AttachedLoot.GetItem(i);
                            if (item == null)
                                continue;

                            LootItemSturdiness itemStab = DetermineLootItemSturdiness(item);

                            if (!item.IsQuestItem)
                            {
                                if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(25 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                            }
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if (hardBash)
                    {
                        for (int i = 0; i < initialItemCount; i++)
                        {
                            DaggerfallUnityItem item = chest.AttachedLoot.GetItem(i);
                            if (item == null)
                                continue;

                            LootItemSturdiness itemStab = DetermineLootItemSturdiness(item);

                            if (!item.IsQuestItem)
                            {
                                if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(55 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(30 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(15 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Resilient && Dice100.SuccessRoll(5 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < initialItemCount; i++)
                        {
                            DaggerfallUnityItem item = chest.AttachedLoot.GetItem(i);
                            if (item == null)
                                continue;

                            LootItemSturdiness itemStab = DetermineLootItemSturdiness(item);

                            if (!item.IsQuestItem)
                            {
                                if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(25 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(10 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                            }
                        }
                    }
                }
            }
            else if (weapon != null && wepSkillID == (short)DFCareer.Skills.BluntWeapon) // If player is bashing chest with a blunt type weapon
            {
                if (hitLock)
                {
                    if (hardBash)
                    {
                        for (int i = 0; i < initialItemCount; i++)
                        {
                            DaggerfallUnityItem item = chest.AttachedLoot.GetItem(i);
                            if (item == null)
                                continue;

                            LootItemSturdiness itemStab = DetermineLootItemSturdiness(item);

                            if (!item.IsQuestItem)
                            {
                                if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(45 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(20 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(5 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < initialItemCount; i++)
                        {
                            DaggerfallUnityItem item = chest.AttachedLoot.GetItem(i);
                            if (item == null)
                                continue;

                            LootItemSturdiness itemStab = DetermineLootItemSturdiness(item);

                            if (!item.IsQuestItem)
                            {
                                if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(25 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(5 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (hardBash)
                    {
                        for (int i = 0; i < initialItemCount; i++)
                        {
                            DaggerfallUnityItem item = chest.AttachedLoot.GetItem(i);
                            if (item == null)
                                continue;

                            LootItemSturdiness itemStab = DetermineLootItemSturdiness(item);

                            if (!item.IsQuestItem)
                            {
                                if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(80 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(40 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(20 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Resilient && Dice100.SuccessRoll(40 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < initialItemCount; i++)
                        {
                            DaggerfallUnityItem item = chest.AttachedLoot.GetItem(i);
                            if (item == null)
                                continue;

                            LootItemSturdiness itemStab = DetermineLootItemSturdiness(item);

                            if (!item.IsQuestItem)
                            {
                                if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(65 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(25 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(15 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Resilient && Dice100.SuccessRoll(25 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                            }
                        }
                    }
                }
            }
            else if (weapon != null) // If player is bashing chest with any other types of weapons (in this case all the bladed ones)
            {
                if (hitLock)
                {
                    if (hardBash)
                    {
                        for (int i = 0; i < initialItemCount; i++)
                        {
                            DaggerfallUnityItem item = chest.AttachedLoot.GetItem(i);
                            if (item == null)
                                continue;

                            LootItemSturdiness itemStab = DetermineLootItemSturdiness(item);

                            if (!item.IsQuestItem)
                            {
                                if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(15 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(25 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < initialItemCount; i++)
                        {
                            DaggerfallUnityItem item = chest.AttachedLoot.GetItem(i);
                            if (item == null)
                                continue;

                            LootItemSturdiness itemStab = DetermineLootItemSturdiness(item);

                            if (!item.IsQuestItem)
                            {
                                if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(10 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(15 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (hardBash)
                    {
                        for (int i = 0; i < initialItemCount; i++)
                        {
                            DaggerfallUnityItem item = chest.AttachedLoot.GetItem(i);
                            if (item == null)
                                continue;

                            LootItemSturdiness itemStab = DetermineLootItemSturdiness(item);

                            if (!item.IsQuestItem)
                            {
                                if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(30 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(60 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(55 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Resilient && Dice100.SuccessRoll(15 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < initialItemCount; i++)
                        {
                            DaggerfallUnityItem item = chest.AttachedLoot.GetItem(i);
                            if (item == null)
                                continue;

                            LootItemSturdiness itemStab = DetermineLootItemSturdiness(item);

                            if (!item.IsQuestItem)
                            {
                                if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(20 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(40 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(35 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Resilient && Dice100.SuccessRoll(10 + (int)Mathf.Round(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                return;
            }
        }

        public static void BashingOpenChestDamagesLoot(LLCObject chest, DaggerfallUnityItem weapon, bool smashedChest)
        {
            int wepSkillID = (weapon != null) ? weapon.GetWeaponSkillIDAsShort() : (short)DFCareer.Skills.HandToHand;
            int initialItemCount = chest.AttachedLoot.Count;

            if (!smashedChest) // Broke lock off
            {
                if (weapon == null && wepSkillID == (short)DFCareer.Skills.HandToHand) // If player is bashing chest with their bare fists
                {
                    for (int i = 0; i < initialItemCount; i++)
                    {
                        DaggerfallUnityItem item = chest.AttachedLoot.GetItem(i);
                        if (item == null)
                            continue;

                        LootItemSturdiness itemStab = DetermineLootItemSturdiness(item);

                        if (!item.IsQuestItem)
                        {
                            if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(40 + (int)Mathf.Round(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(30 + (int)Mathf.Round(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(20 + (int)Mathf.Round(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Resilient && Dice100.SuccessRoll(10 + (int)Mathf.Round(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                        }
                    }
                }
                else if (weapon != null && wepSkillID == (short)DFCareer.Skills.BluntWeapon) // If player is bashing chest with a blunt type weapon
                {
                    for (int i = 0; i < initialItemCount; i++)
                    {
                        DaggerfallUnityItem item = chest.AttachedLoot.GetItem(i);
                        if (item == null)
                            continue;

                        LootItemSturdiness itemStab = DetermineLootItemSturdiness(item);

                        if (!item.IsQuestItem)
                        {
                            if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(55 + (int)Mathf.Round(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(35 + (int)Mathf.Round(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(20 + (int)Mathf.Round(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Resilient && Dice100.SuccessRoll(25 + (int)Mathf.Round(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                        }
                    }
                }
                else if (weapon != null) // If player is bashing chest with any other types of weapons (in this case all the bladed ones)
                {
                    for (int i = 0; i < initialItemCount; i++)
                    {
                        DaggerfallUnityItem item = chest.AttachedLoot.GetItem(i);
                        if (item == null)
                            continue;

                        LootItemSturdiness itemStab = DetermineLootItemSturdiness(item);

                        if (!item.IsQuestItem)
                        {
                            if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(25 + (int)Mathf.Round(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(55 + (int)Mathf.Round(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(30 + (int)Mathf.Round(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Resilient && Dice100.SuccessRoll(15 + (int)Mathf.Round(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            else // Smashed chest body open
            {
                if (weapon == null && wepSkillID == (short)DFCareer.Skills.HandToHand) // If player is bashing chest with their bare fists
                {
                    for (int i = 0; i < initialItemCount; i++)
                    {
                        DaggerfallUnityItem item = chest.AttachedLoot.GetItem(i);
                        if (item == null)
                            continue;

                        LootItemSturdiness itemStab = DetermineLootItemSturdiness(item);

                        if (!item.IsQuestItem)
                        {
                            if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(70 + (int)Mathf.Round(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(50 + (int)Mathf.Round(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(35 + (int)Mathf.Round(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Resilient && Dice100.SuccessRoll(30 + (int)Mathf.Round(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                        }
                    }
                }
                else if (weapon != null && wepSkillID == (short)DFCareer.Skills.BluntWeapon) // If player is bashing chest with a blunt type weapon
                {
                    for (int i = 0; i < initialItemCount; i++)
                    {
                        DaggerfallUnityItem item = chest.AttachedLoot.GetItem(i);
                        if (item == null)
                            continue;

                        LootItemSturdiness itemStab = DetermineLootItemSturdiness(item);

                        if (!item.IsQuestItem)
                        {
                            if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(80 + (int)Mathf.Round(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(55 + (int)Mathf.Round(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(50 + (int)Mathf.Round(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Resilient && Dice100.SuccessRoll(60 + (int)Mathf.Round(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                        }
                    }
                }
                else if (weapon != null) // If player is bashing chest with any other types of weapons (in this case all the bladed ones)
                {
                    for (int i = 0; i < initialItemCount; i++)
                    {
                        DaggerfallUnityItem item = chest.AttachedLoot.GetItem(i);
                        if (item == null)
                            continue;

                        LootItemSturdiness itemStab = DetermineLootItemSturdiness(item);

                        if (!item.IsQuestItem)
                        {
                            if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(50 + (int)Mathf.Round(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(70 + (int)Mathf.Round(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(70 + (int)Mathf.Round(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Resilient && Dice100.SuccessRoll(45 + (int)Mathf.Round(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                        }
                    }
                }
                else
                {
                    return;
                }
            }
        }

        public static void BashingOpenChestWithArrowsDamagesLoot(LLCObject chest)
        {
            int initialItemCount = chest.AttachedLoot.Count;

            for (int i = 0; i < initialItemCount; i++)
            {
                DaggerfallUnityItem item = chest.AttachedLoot.GetItem(i);
                if (item == null)
                    continue;

                LootItemSturdiness itemStab = DetermineLootItemSturdiness(item);

                if (!item.IsQuestItem)
                {
                    if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(75 + (int)Mathf.Round(Luck / -5f)))
                    {
                        if (HandleDestroyingLootItem(chest, item)) { i--; continue; }
                    }
                    else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(90 + (int)Mathf.Round(Luck / -5f)))
                    {
                        if (HandleDestroyingLootItem(chest, item)) { i--; continue; }
                    }
                    else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(70 + (int)Mathf.Round(Luck / -5f)))
                    {
                        if (HandleDestroyingLootItem(chest, item)) { i--; continue; }
                    }
                    else if (itemStab == LootItemSturdiness.Resilient && Dice100.SuccessRoll(60 + (int)Mathf.Round(Luck / -5f)))
                    {
                        if (HandleDestroyingLootItem(chest, item)) { i--; continue; }
                    }
                }
            }
        }
    }
}