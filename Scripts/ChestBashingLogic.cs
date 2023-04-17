using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallConnect;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Serialization;

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
                DaggerfallAudioSource dfAudioSource = chest.GetComponent<DaggerfallAudioSource>();

                int damDealt = 0;
                chest.HasBeenBashed = true;
                IsThisACrime(ChestInteractionType.Bash);

                if (BashLockRoll(chest, weapon))
                {
                    damDealt = DetermineDamageToLock(chest, weapon);

                    if (BreakLockAttempt(chest, damDealt))
                    {
                        // Lock was hit with bash and is now broken, so chest loot is accessible.
                        BashingOpenChestDamagesLoot(chest, weapon, false);

                        DaggerfallLoot openChestLoot = null;
                        if (ChestGraphicType == 0) // Use sprite based graphics for chests
                        {
                            int spriteID = closedChestLoot.Count <= 0 ? OpenEmptyChestSpriteID : OpenFullChestSpriteID;
                            openChestLoot = GameObjectHelper.CreateLootContainer(LootContainerTypes.Nothing, InventoryContainerImages.Chest, pos, closedChestTransform.parent, spriteID, 0, DaggerfallUnity.NextUID, null, false);
                            openChestLoot.gameObject.name = GameObjectHelper.GetGoFlatName(spriteID, 0);
                            openChestLoot.Items.TransferAll(closedChestLoot); // Transfers items from closed chest's items to the new open chest's item collection.
                            Destroy(openChestLoot.GetComponent<SerializableLootContainer>());
                        }
                        else // Use 3D models for chests
                        {
                            GameObject usedModelPrefab = null;
                            int modelID = 0;
                            if (closedChestLoot.Count <= 0) { usedModelPrefab = (ChestGraphicType == 1) ? Instance.LowPolyOpenEmptyChestPrefab : Instance.HighPolyOpenEmptyChestPrefab; modelID = OpenEmptyChestModelID; }
                            else { usedModelPrefab = (ChestGraphicType == 1) ? Instance.LowPolyOpenFullChestPrefab : Instance.HighPolyOpenFullChestPrefab; modelID = OpenFullChestModelID; }
                            GameObject chestGo = GameObjectHelper.InstantiatePrefab(usedModelPrefab, GameObjectHelper.GetGoModelName((uint)modelID), closedChestTransform.parent, pos);
                            chestGo.transform.rotation = chest.gameObject.transform.rotation;
                            Collider col = chestGo.AddComponent<BoxCollider>();
                            openChestLoot = chestGo.AddComponent<DaggerfallLoot>();
                            ToggleChestShadowsOrCollision(chestGo);
                            if (openChestLoot)
                            {
                                openChestLoot.ContainerType = LootContainerTypes.Nothing;
                                openChestLoot.ContainerImage = InventoryContainerImages.Chest;
                                openChestLoot.LoadID = DaggerfallUnity.NextUID;
                                openChestLoot.TextureRecord = modelID;
                                openChestLoot.Items.TransferAll(closedChestLoot); // Transfers items from closed chest's items to the new open chest's item collection.
                            }
                        }

                        // Show success and play unlock sound
                        DaggerfallUI.AddHUDText(GetBashedLockOffText(), 3f); // Will possibly change text later on depending on many factors, will see.
                        if (dfAudioSource != null)
                            AudioSource.PlayClipAtPoint(GetLockBashAudioClip(chest, true), chest.gameObject.transform.position, UnityEngine.Random.Range(1.4f, 1.92f) * DaggerfallUnity.Settings.SoundVolume);

                        Destroy(chest.gameObject); // Removed closed chest from scene, but saved its characteristics we care about for opened chest loot-pile.
                    }
                    else
                    {
                        // Lock was hit with bash, but is still intact.
                        if (dfAudioSource != null)
                            AudioSource.PlayClipAtPoint(GetLockBashAudioClip(chest, false), chest.gameObject.transform.position, UnityEngine.Random.Range(0.9f, 1.42f) * DaggerfallUnity.Settings.SoundVolume);
                    }
                }
                else
                {
                    bool critBash = false;
                    damDealt = DetermineDamageToChest(chest, weapon, out critBash);

                    if (SmashOpenChestAttempt(chest, damDealt))
                    {
                        // Chest body has been smashed open and contents are accessible (but damaged greatly most likely.)
                        BashingOpenChestDamagesLoot(chest, weapon, true);

                        DaggerfallLoot openChestLoot = null;
                        if (ChestGraphicType == 0) // Use sprite based graphics for chests
                        {
                            openChestLoot = GameObjectHelper.CreateLootContainer(LootContainerTypes.Nothing, InventoryContainerImages.Chest, pos, closedChestTransform.parent, SmashedChestSpriteID, 0, DaggerfallUnity.NextUID, null, false);
                            openChestLoot.gameObject.name = GameObjectHelper.GetGoFlatName(SmashedChestSpriteID, 0);
                            openChestLoot.Items.TransferAll(closedChestLoot); // Transfers items from closed chest's items to the new open chest's item collection.
                            Destroy(openChestLoot.GetComponent<SerializableLootContainer>());
                        }
                        else // Use 3D models for chests
                        {
                            GameObject usedModelPrefab = (ChestGraphicType == 1) ? Instance.LowPolySmashedChestPrefab : Instance.HighPolySmashedChestPrefab;
                            GameObject chestGo = GameObjectHelper.InstantiatePrefab(usedModelPrefab, GameObjectHelper.GetGoModelName(SmashedChestModelID), closedChestTransform.parent, pos);
                            Collider col = chestGo.AddComponent<BoxCollider>();
                            openChestLoot = chestGo.AddComponent<DaggerfallLoot>();
                            ToggleChestShadowsOrCollision(chestGo);
                            if (openChestLoot)
                            {
                                openChestLoot.ContainerType = LootContainerTypes.Nothing;
                                openChestLoot.ContainerImage = InventoryContainerImages.Chest;
                                openChestLoot.LoadID = DaggerfallUnity.NextUID;
                                openChestLoot.TextureRecord = SmashedChestModelID;
                                openChestLoot.Items.TransferAll(closedChestLoot); // Transfers items from closed chest's items to the new open chest's item collection.
                            }
                        }

                        // Show success and play unlock sound
                        DaggerfallUI.AddHUDText(GetBashedOpenChestText(), 3f); // Will possibly change text later on depending on many factors, will see.
                        if (dfAudioSource != null)
                            AudioSource.PlayClipAtPoint(GetChestBashAudioClip(chest, weapon, true, critBash), chest.gameObject.transform.position, UnityEngine.Random.Range(1.4f, 1.92f) * DaggerfallUnity.Settings.SoundVolume);

                        Destroy(chest.gameObject); // Removed closed chest from scene, but saved its characteristics we care about for opened chest loot-pile.
                    }
                    else
                    {
                        // Chest body was hit with bash, but is still intact.
                        if (dfAudioSource != null)
                            AudioSource.PlayClipAtPoint(GetChestBashAudioClip(chest, weapon, false, critBash), chest.gameObject.transform.position, UnityEngine.Random.Range(0.9f, 1.42f) * DaggerfallUnity.Settings.SoundVolume);
                    }
                }
            }
            else
            {
                DaggerfallUI.AddHUDText("ERROR: Chest Was Found As Null.", 3f);
            }
        }

        public static bool AttemptChestBashWithArrows(LLCObject chest, DaggerfallMissile missile)
        {
            if (chest != null)
            {
                ItemCollection closedChestLoot = chest.AttachedLoot;
                Transform closedChestTransform = chest.gameObject.transform; // Not sure if the explicit "gameObject" reference is necessary, will test eventually and determine if so or not.
                Vector3 pos = chest.gameObject.transform.position;
                DaggerfallAudioSource dfAudioSource = chest.GetComponent<DaggerfallAudioSource>();

                chest.HasBeenBashed = true;

                // Would be interesting when an arrow "pings" off a metal chest it would actually leave a physical object nearby that could be clicked instead of adding to chest inventory, but eh.
                chest.AttachedLoot.AddItem(ItemBuilder.CreateItem(ItemGroups.UselessItems1, ItemBrokenArrow.templateIndex));

                DaggerfallUnityItem bowUsed = GameManager.Instance.WeaponManager.LastBowUsed;
                if (bowUsed != null)
                    bowUsed.LowerCondition(1, Player);

                if (chest.ChestMaterial == ChestMaterials.Wood)
                {
                    IsThisACrime(ChestInteractionType.Bash);

                    if (SmashOpenChestWithArrowAttempt(chest, bowUsed))
                    {
                        // Chest body has been smashed open and contents are accessible (but damaged significantly.)
                        BashingOpenChestWithArrowsDamagesLoot(chest);

                        DaggerfallLoot openChestLoot = null;
                        if (ChestGraphicType == 0) // Use sprite based graphics for chests
                        {
                            openChestLoot = GameObjectHelper.CreateLootContainer(LootContainerTypes.Nothing, InventoryContainerImages.Chest, pos, closedChestTransform.parent, SmashedChestSpriteID, 0, DaggerfallUnity.NextUID, null, false);
                            openChestLoot.gameObject.name = GameObjectHelper.GetGoFlatName(SmashedChestSpriteID, 0);
                            openChestLoot.Items.TransferAll(closedChestLoot); // Transfers items from closed chest's items to the new open chest's item collection.
                            Destroy(openChestLoot.GetComponent<SerializableLootContainer>());
                        }
                        else // Use 3D models for chests
                        {
                            GameObject usedModelPrefab = (ChestGraphicType == 1) ? Instance.LowPolySmashedChestPrefab : Instance.HighPolySmashedChestPrefab;
                            GameObject chestGo = GameObjectHelper.InstantiatePrefab(usedModelPrefab, GameObjectHelper.GetGoModelName(SmashedChestModelID), closedChestTransform.parent, pos);
                            Collider col = chestGo.AddComponent<BoxCollider>();
                            openChestLoot = chestGo.AddComponent<DaggerfallLoot>();
                            ToggleChestShadowsOrCollision(chestGo);
                            if (openChestLoot)
                            {
                                openChestLoot.ContainerType = LootContainerTypes.Nothing;
                                openChestLoot.ContainerImage = InventoryContainerImages.Chest;
                                openChestLoot.LoadID = DaggerfallUnity.NextUID;
                                openChestLoot.TextureRecord = SmashedChestModelID;
                                openChestLoot.Items.TransferAll(closedChestLoot); // Transfers items from closed chest's items to the new open chest's item collection.
                            }
                        }

                        // Show success and play smash open sound
                        DaggerfallUI.AddHUDText(GetArrowSmashedOpenChestText(), 3f); // Will possibly change text later on depending on many factors, will see.
                        if (dfAudioSource)
                        {
                            if (dfAudioSource != null)
                                AudioSource.PlayClipAtPoint(GetArrowHitChestAudioClip(chest, true), chest.gameObject.transform.position, UnityEngine.Random.Range(1.5f, 2.61f) * DaggerfallUnity.Settings.SoundVolume);
                        }
                        return true;
                    }
                    else
                    {
                        // Chest body was hit with arrow, but is still intact.
                        if (dfAudioSource)
                        {
                            if (dfAudioSource != null)
                                AudioSource.PlayClipAtPoint(GetArrowHitChestAudioClip(chest, false), chest.gameObject.transform.position, UnityEngine.Random.Range(2.8f, 3.52f) * DaggerfallUnity.Settings.SoundVolume);
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
                            AudioSource.PlayClipAtPoint(GetArrowHitChestAudioClip(chest, false), chest.gameObject.transform.position, UnityEngine.Random.Range(2.8f, 3.52f) * DaggerfallUnity.Settings.SoundVolume);
                    }
                    return false;
                }
            }
            else
            {
                DaggerfallUI.AddHUDText("ERROR: Chest Was Found As Null.", 3f);
            }
            return false;
        }

        public static bool BashLockRoll(LLCObject chest, DaggerfallUnityItem weapon) // Roll to determine if a bash attempt hit the lock or the chest body instead.
        {
            short skillUsed = (weapon != null) ? weapon.GetWeaponSkillIDAsShort() : (short)DFCareer.Skills.HandToHand;
            int wepSkill = Player.Skills.GetLiveSkillValue(skillUsed); // Might add accuracy modifier for weapon type, but for now just keep it more simple.
            float accuracyCheck = Mathf.Round(wepSkill / 5) + Mathf.Round(Willp * .1f) + Mathf.Round(Agili * .3f) + Mathf.Round(Luck * .2f);

            if (Dice100.SuccessRoll(Mathf.RoundToInt(Mathf.Clamp(accuracyCheck, 4f, 50f))))
            {
                Player.TallySkill((DFCareer.Skills)skillUsed, 1);
                return true;
            }
            else
                return false;
        }

        public static int DetermineDamageToLock(LLCObject chest, DaggerfallUnityItem weapon) // Determine how much damage the lock takes from a bash attempt, if any.
        {
            int lockMat = LockMaterialToDaggerfallValue(chest.LockMaterial);
            int weapMat = (weapon != null) ? weapon.NativeMaterialValue : -1;
            int matDiff = weapMat - lockMat;
            int wepSkillID = (weapon != null) ? weapon.GetWeaponSkillIDAsShort() : (short)DFCareer.Skills.HandToHand;
            bool critBash = false;

            if (wepSkillID == (int)DFCareer.Skills.HandToHand) // Checks if the "weapon" being used is the player's fists.
            {
                if (lockMat < 0) // If lock is made of wood
                {
                    float critBashChance = Mathf.Round(Stren + Mathf.RoundToInt(Luck / 5f) + 30f);
                    if (Dice100.SuccessRoll(Mathf.RoundToInt(Mathf.Clamp(critBashChance, 4f, 60f))))
                        critBash = true;

                    ApplyBashingCostLogic(chest, weapon, true, true, critBash, matDiff);
                }
                else // If lock is made from any metal
                {
                    float critBashChance = Mathf.Round(Stren + Mathf.RoundToInt(Luck / 5f) + 5f);
                    if (Dice100.SuccessRoll(Mathf.RoundToInt(Mathf.Clamp(critBashChance, 1f, 30f))))
                        critBash = true;

                    ApplyBashingCostLogic(chest, weapon, true, false, critBash, matDiff);
                }
            }
            else if (wepSkillID == (int)DFCareer.Skills.BluntWeapon) // Checks if the weapon being used is a Blunt Weapon. Later on might check for other weapon types as well, but just this for now.
            {
                if (lockMat < 0) // If lock is made of wood
                {
                    float critBashChance = Mathf.Round(Mathf.RoundToInt(Mathf.Clamp(weapon.weightInKg - 2f, 0.75f, 20f) / (0.6f - (Stren / 100f))) + Mathf.RoundToInt(Luck / 5f) + 40f);
                    if (Dice100.SuccessRoll(Mathf.RoundToInt(Mathf.Clamp(critBashChance, 10f, 100f))))
                        critBash = true;

                    ApplyBashingCostLogic(chest, weapon, true, true, critBash, matDiff);
                }
                else // If lock is made from any metal
                {
                    float critBashChance = Mathf.Round(Mathf.RoundToInt(Mathf.Clamp(weapon.weightInKg - 2f, 0.75f, 20f) / (0.6f - (Stren / 100f))) + Mathf.RoundToInt(Luck / 5f) + 5f);
                    if (Dice100.SuccessRoll(Mathf.RoundToInt(Mathf.Clamp(critBashChance, 2f, 90f))))
                        critBash = true;

                    ApplyBashingCostLogic(chest, weapon, true, false, critBash, matDiff);
                }
            }
            else // For all non-blunt weapons for now.
            {
                if (lockMat < 0) // If lock is made of wood
                {
                    float critBashChance = Mathf.Round(((matDiff + Mathf.RoundToInt(Stren / 10f)) * 10f) + Mathf.RoundToInt(Luck / 5f) + 40f); // Just for now, will have to see through testing if this needs work.
                    if (Dice100.SuccessRoll(Mathf.RoundToInt(Mathf.Clamp(critBashChance, 10f, 100f))))
                        critBash = true;

                    ApplyBashingCostLogic(chest, weapon, true, true, critBash, matDiff);
                }
                else // If lock is made from any metal
                {
                    float critBashChance = Mathf.Round(((matDiff + Mathf.RoundToInt(Stren / 10f)) * 10f) + Mathf.RoundToInt(Luck / 5f) + 30f); // Just for now, will have to see through testing if this needs work.
                    if (Dice100.SuccessRoll(Mathf.RoundToInt(Mathf.Clamp(critBashChance, 2f, 90f))))
                        critBash = true;

                    ApplyBashingCostLogic(chest, weapon, true, false, critBash, matDiff);
                }
            }

            if (!critBash && Dice100.SuccessRoll(LockDamageNegationChance(chest, weapon, wepSkillID)))
                return 0;
            else
                return CalculateLockDamage(chest, weapon, wepSkillID, critBash);
        }

        public static bool BreakLockAttempt(LLCObject chest, int damage) // Determine if bashed lock breaks or not this attempt, granting full access to loot.
        {
            int lockHP = chest.LockCurrentHP - damage;

            if (lockHP <= 0)
            {
                chest.LockCurrentHP = -1;
                return true;
            }
            else
            {
                chest.LockCurrentHP = lockHP;
                return false;
            }
        }

        public static int DetermineDamageToChest(LLCObject chest, DaggerfallUnityItem weapon, out bool critBash) // Determine how much damage the chest takes from a bash attempt, if any.
        {
            int chestMat = ChestMaterialToDaggerfallValue(chest.ChestMaterial);
            int weapMat = (weapon != null) ? weapon.NativeMaterialValue : -1;
            int matDiff = weapMat - chestMat;
            int wepSkillID = (weapon != null) ? weapon.GetWeaponSkillIDAsShort() : (short)DFCareer.Skills.HandToHand;
            critBash = false;

            if (wepSkillID == (int)DFCareer.Skills.HandToHand) // Checks if the "weapon" being used is the player's fists.
            {
                if (chestMat < 0) // If chest is made of wood
                {
                    float hardBashChance = Mathf.Round(Stren + Mathf.RoundToInt(Luck / 5f) + 15f);
                    if (Dice100.SuccessRoll(Mathf.RoundToInt(Mathf.Clamp(hardBashChance, 2f, 50f))))
                        critBash = true;

                    ApplyBashingCostLogic(chest, weapon, false, true, critBash, matDiff);
                }
                else // If chest is made from any metal
                {
                    float hardBashChance = Mathf.Round(Stren + Mathf.RoundToInt(Luck / 5f) + 3f);
                    if (Dice100.SuccessRoll(Mathf.RoundToInt(Mathf.Clamp(hardBashChance, 0f, 40f))))
                        critBash = true;

                    ApplyBashingCostLogic(chest, weapon, false, false, critBash, matDiff);
                }
            }
            else if (wepSkillID == (int)DFCareer.Skills.BluntWeapon) // Checks if the weapon being used is a Blunt Weapon. Later on might check for other weapon types as well, but just this for now.
            {
                if (chestMat < 0) // If chest is made of wood
                {
                    float hardBashChance = Mathf.Round(Mathf.RoundToInt(Mathf.Clamp(weapon.weightInKg - 2f, 0.75f, 20f) / (0.6f - (Stren / 100f))) + Mathf.RoundToInt(Luck / 5f) + 25f);
                    if (Dice100.SuccessRoll(Mathf.RoundToInt(Mathf.Clamp(hardBashChance, 6f, 96f))))
                        critBash = true;

                    ApplyBashingCostLogic(chest, weapon, false, true, critBash, matDiff);
                }
                else // If chest is made from any metal
                {
                    float hardBashChance = Mathf.Round(Mathf.RoundToInt(Mathf.Clamp(weapon.weightInKg - 2f, 0.75f, 20f) / (0.6f - (Stren / 100f))) + Mathf.RoundToInt(Luck / 5f) + 10f);
                    if (Dice100.SuccessRoll(Mathf.RoundToInt(Mathf.Clamp(hardBashChance, 1f, 85f))))
                        critBash = true;

                    ApplyBashingCostLogic(chest, weapon, false, false, critBash, matDiff);
                }
            }
            else // For all non-blunt weapons for now.
            {
                if (chestMat < 0) // If chest is made of wood
                {
                    float hardBashChance = Mathf.Round(((matDiff + Mathf.RoundToInt(Stren / 10f)) * 10f) + Mathf.RoundToInt(Luck / 5f) + 17f); // Just for now, will have to see through testing if this needs work.
                    if (Dice100.SuccessRoll(Mathf.RoundToInt(Mathf.Clamp(hardBashChance, 3f, 90f))))
                        critBash = true;

                    ApplyBashingCostLogic(chest, weapon, false, true, critBash, matDiff);
                }
                else // If chest is made from any metal
                {
                    float hardBashChance = Mathf.Round(((matDiff + Mathf.RoundToInt(Stren / 10f)) * 10f) + Mathf.RoundToInt(Luck / 5f) + 7f); // Just for now, will have to see through testing if this needs work.
                    if (Dice100.SuccessRoll(Mathf.RoundToInt(Mathf.Clamp(hardBashChance, 0f, 70f))))
                        critBash = true;

                    ApplyBashingCostLogic(chest, weapon, false, false, critBash, matDiff);
                }
            }

            if (!critBash && Dice100.SuccessRoll(ChestDamageNegationChance(chest, weapon, wepSkillID)))
                return 0;
            else
                return CalculateChestDamage(chest, weapon, wepSkillID, critBash);
        }

        public static bool SmashOpenChestAttempt(LLCObject chest, int damage) // Determine if bashed chest body breaks open or not this attempt, granting full access to loot.
        {
            int chestHP = chest.ChestCurrentHP - damage;

            if (chestHP <= 0)
            {
                chest.ChestCurrentHP = -1;
                return true;
            }
            else
            {
                chest.ChestCurrentHP = chestHP;
                return false;
            }
        }

        public static bool SmashOpenChestWithArrowAttempt(LLCObject chest, DaggerfallUnityItem bowUsed) // Determine if bashed chest body breaks open or not this attempt, granting full access to loot.
        {
            int chestStab = chest.ChestSturdiness;
            float chestDamRes = Mathf.Abs((chestStab * 0.4f / 100f) - 1f);
            float luckMod = (Luck / 250f) + 1f;

            if (Dice100.SuccessRoll(10 + chestStab - (Mathf.RoundToInt(Stren * 0.6f) + Mathf.RoundToInt(Luck * 0.2f))))
                return false;
            else
            {
                int damage = Mathf.Max(1, Mathf.RoundToInt(CalculateWeaponDamage(bowUsed) * 0.8f * chestDamRes * luckMod));
                bool smashSuccess = SmashOpenChestAttempt(chest, damage);

                if (smashSuccess)
                {
                    Player.TallySkill(DFCareer.Skills.Archery, 1);
                    return true;
                }
                else
                    return false;
            }
        }

        public static void ApplyBashingCostLogic(LLCObject chest, DaggerfallUnityItem weapon, bool hitLock = false, bool hitWood = false, bool critBash = false, int matDiff = -100) // Applies vitals damage to player and the weapon used during bash attempt.
        {
            int wepSkillID = (weapon != null) ? weapon.GetWeaponSkillIDAsShort() : (short)DFCareer.Skills.HandToHand;
            int wepSkill = Player.Skills.GetLiveSkillValue(wepSkillID); // Have not used this value yet, nor "matDiff" will see if I eventually do or not later.
            float bashHitMod = 1f;
            float healthDam = 0f;
            float fatigueDam = 0f;

            if (chest == null)
                return;

            BashingDamageLootContents(chest, weapon, wepSkillID, hitLock, hitWood, critBash);

            if (hitLock && critBash) { bashHitMod = 0.7f; } // Math was confusing me on how to combine these values together for some reason, so just doing this weird if-else chain here instead, for now.
            else if (hitLock && !critBash) { bashHitMod = 0.35f; }
            else if (!hitLock && critBash) { bashHitMod = 1f; }
            else { bashHitMod = 0.5f; }

            if (weapon == null && wepSkillID == (short)DFCareer.Skills.HandToHand) // If player is bashing chest with their bare fists
            {
                if (hitWood) // Hitting either wooden lock or chest body with bare fists
                {
                    float rolledHpPercent = Mathf.Max(4, UnityEngine.Random.Range(5, Mathf.Round(14 + Mathf.RoundToInt(Endur / -3f) + Mathf.RoundToInt(Willp / -10f) * bashHitMod)));
                    healthDam = Mathf.Max(1, (int)Mathf.Floor(Player.MaxHealth * (rolledHpPercent / 100f)));
                    healthDam = (Player.CurrentHealth - healthDam < Mathf.Ceil(Player.MaxHealth * 0.3f)) ? Mathf.Max(0, Player.CurrentHealth - (Player.MaxHealth * 0.3f)) : healthDam;

                    if (healthDam > 0)
                    {
                        float rolledFatiguePercent = Mathf.Max(4, UnityEngine.Random.Range(5, Mathf.Round(14 + Mathf.RoundToInt(Endur / -3f) + Mathf.RoundToInt(Willp / -10f) * bashHitMod)));
                        fatigueDam = Mathf.Max((int)Mathf.Floor(Player.MaxFatigue * 0.04f), (int)Mathf.Floor(Player.MaxFatigue * (rolledFatiguePercent / 100f)));
                        fatigueDam = (Player.CurrentFatigue - fatigueDam < 0) ? Player.CurrentFatigue : fatigueDam;

                        Player.DecreaseHealth((int)healthDam); // Currently could be abused by something like the "Health Regen" trait, but whatever for now.
                        Player.DecreaseFatigue((int)fatigueDam, false); // Will need to do testing to make sure this works with the fatigue multiplier, including above formulas and such, bit confusing.
                    }
                    else // Since player health is currently at "minimum" level that bashing with fists can reduce it, health is not damaged, but instead fatigue damage is multiplied alot as a cost.
                    {
                        float rolledFatiguePercent = Mathf.Max(8, UnityEngine.Random.Range(14, Mathf.Round(36 + Mathf.RoundToInt(Endur / -3f) + Mathf.RoundToInt(Willp / -10f) * bashHitMod)));
                        fatigueDam = Mathf.Max((int)Mathf.Floor(Player.MaxFatigue * 0.08f), (int)Mathf.Floor(Player.MaxFatigue * (rolledFatiguePercent / 100f)));
                        fatigueDam = (Player.CurrentFatigue - fatigueDam < 0) ? Player.CurrentFatigue : fatigueDam;

                        Player.DecreaseFatigue((int)fatigueDam, false); // Will need to do testing to make sure this works with the fatigue multiplier, including above formulas and such, bit confusing.
                    }
                }
                else // Hitting either metal lock or chest body with bare fists
                {
                    float rolledHpPercent = Mathf.Max(7, UnityEngine.Random.Range(12, Mathf.Round(22 + Mathf.RoundToInt(Endur / -3f) + Mathf.RoundToInt(Willp / -10f) * bashHitMod)));
                    healthDam = Mathf.Max(1, (int)Mathf.Floor(Player.MaxHealth * (rolledHpPercent / 100f)));
                    healthDam = (Player.CurrentHealth - healthDam < Mathf.Ceil(Player.MaxHealth * 0.3f)) ? Mathf.Max(0, Player.CurrentHealth - (Player.MaxHealth * 0.3f)) : healthDam;

                    if (healthDam > 0)
                    {
                        float rolledFatiguePercent = Mathf.Max(7, UnityEngine.Random.Range(12, Mathf.Round(22 + Mathf.RoundToInt(Endur / -3f) + Mathf.RoundToInt(Willp / -10f) * bashHitMod)));
                        fatigueDam = Mathf.Max((int)Mathf.Floor(Player.MaxFatigue * 0.07f), (int)Mathf.Floor(Player.MaxFatigue * (rolledFatiguePercent / 100f)));
                        fatigueDam = (Player.CurrentFatigue - fatigueDam < 0) ? Player.CurrentFatigue : fatigueDam;

                        Player.DecreaseHealth((int)healthDam); // Currently could be abused by something like the "Health Regen" trait, but whatever for now.
                        Player.DecreaseFatigue((int)fatigueDam, false); // Will need to do testing to make sure this works with the fatigue multiplier, including above formulas and such, bit confusing.
                    }
                    else // Since player health is currently at "minimum" level that bashing with fists can reduce it, health is not damaged, but instead fatigue damage is multiplied alot as a cost.
                    {
                        float rolledFatiguePercent = Mathf.Max(14, UnityEngine.Random.Range(22, Mathf.Round(56 + Mathf.RoundToInt(Endur / -3f) + Mathf.RoundToInt(Willp / -10f) * bashHitMod)));
                        fatigueDam = Mathf.Max((int)Mathf.Floor(Player.MaxFatigue * 0.14f), (int)Mathf.Floor(Player.MaxFatigue * (rolledFatiguePercent / 100f)));
                        fatigueDam = (Player.CurrentFatigue - fatigueDam < 0) ? Player.CurrentFatigue : fatigueDam;

                        Player.DecreaseFatigue((int)fatigueDam, false); // Will need to do testing to make sure this works with the fatigue multiplier, including above formulas and such, bit confusing.
                    }
                }
            }
            else if (weapon != null && wepSkillID == (short)DFCareer.Skills.BluntWeapon) // If player is bashing chest with a blunt type weapon
            {
                if (hitWood) // Hitting either wooden lock or chest body with a blunt type weapon
                {
                    float rolledWepDamPercent = Mathf.Max(1, UnityEngine.Random.Range(1, (3 + Mathf.RoundToInt((Stren / 10f) * 0.4f) + ((Dice100.SuccessRoll(50 + Mathf.RoundToInt(Luck / 2f))) ? -1 : 0)) * bashHitMod));
                    float rolledFatiguePercent = Mathf.Max(2, UnityEngine.Random.Range(2, Mathf.Round(4 + Mathf.RoundToInt(Endur / -10f) + Mathf.RoundToInt(Willp / -25f) * bashHitMod)));
                    fatigueDam = Mathf.Max((int)Mathf.Floor(Player.MaxFatigue * 0.02f), (int)Mathf.Floor(Player.MaxFatigue * (rolledFatiguePercent / 100f)));
                    fatigueDam = (Player.CurrentFatigue - fatigueDam < 0) ? Player.CurrentFatigue : fatigueDam;

                    Player.DecreaseFatigue((int)fatigueDam, false); // Will need to do testing to make sure this works with the fatigue multiplier, including above formulas and such, bit confusing.
                    weapon.LowerCondition((int)Mathf.Max(1, Mathf.Round(weapon.maxCondition * (rolledWepDamPercent / 100f))), Player);
                }
                else // Hitting either metal lock or chest body with a blunt type weapon
                {
                    float rolledWepDamPercent = Mathf.Max(2, UnityEngine.Random.Range(2, (4 + Mathf.RoundToInt((Stren / 10f) * 0.4f) + ((Dice100.SuccessRoll(50 + Mathf.RoundToInt(Luck / 2f))) ? -1 : 0)) * bashHitMod));
                    float rolledFatiguePercent = Mathf.Max(4, UnityEngine.Random.Range(4, Mathf.Round(9 + Mathf.RoundToInt(Endur / -10f) + Mathf.RoundToInt(Willp / -25f) * bashHitMod)));
                    fatigueDam = Mathf.Max((int)Mathf.Floor(Player.MaxFatigue * 0.04f), (int)Mathf.Floor(Player.MaxFatigue * (rolledFatiguePercent / 100f)));
                    fatigueDam = (Player.CurrentFatigue - fatigueDam < 0) ? Player.CurrentFatigue : fatigueDam;

                    Player.DecreaseFatigue((int)fatigueDam, false); // Will need to do testing to make sure this works with the fatigue multiplier, including above formulas and such, bit confusing.
                    weapon.LowerCondition((int)Mathf.Max(1, Mathf.Round(weapon.maxCondition * (rolledWepDamPercent / 100f))), Player);
                }
            }
            else if (weapon != null) // If player is bashing chest with any other types of weapons (in this case all the bladed ones)
            {
                if (hitWood) // Hitting either wooden lock or chest body with a bladed type weapon
                {
                    float rolledWepDamPercent = Mathf.Max(1, UnityEngine.Random.Range(1, (3 + Mathf.RoundToInt((Stren / 10f) * 0.2f) + ((Dice100.SuccessRoll(50 + Mathf.RoundToInt(Luck / 2f))) ? -1 : 0)) * bashHitMod));
                    rolledWepDamPercent = (!hitLock) ? Mathf.Round(rolledWepDamPercent * (1 + chest.ChestSturdiness / 100f)) : Mathf.Round(rolledWepDamPercent * (1 + chest.LockSturdiness / 300f));
                    float rolledFatiguePercent = Mathf.Max(2, UnityEngine.Random.Range(2, Mathf.Round(4 + Mathf.RoundToInt(Endur / -10f) + Mathf.RoundToInt(Willp / -25f) * bashHitMod)));
                    fatigueDam = Mathf.Max((int)Mathf.Floor(Player.MaxFatigue * 0.02f), (int)Mathf.Floor(Player.MaxFatigue * (rolledFatiguePercent / 100f)));
                    fatigueDam = (Player.CurrentFatigue - fatigueDam < 0) ? Player.CurrentFatigue : fatigueDam;

                    Player.DecreaseFatigue((int)fatigueDam, false); // Will need to do testing to make sure this works with the fatigue multiplier, including above formulas and such, bit confusing.
                    weapon.LowerCondition((int)Mathf.Max(1, Mathf.Round(weapon.maxCondition * (rolledWepDamPercent / 100f))), Player);
                }
                else // Hitting either metal lock or chest body with a bladed type weapon
                {
                    float rolledWepDamPercent = Mathf.Max(2, UnityEngine.Random.Range(2, (3 + Mathf.RoundToInt((Stren / 10f) * 0.2f) + ((Dice100.SuccessRoll(50 + Mathf.RoundToInt(Luck / 2f))) ? -1 : 0)) * bashHitMod));
                    rolledWepDamPercent = (!hitLock) ? Mathf.Round(rolledWepDamPercent * (1 + chest.ChestSturdiness / 100f)) : Mathf.Round(rolledWepDamPercent * (1 + chest.LockSturdiness / 300f));
                    float rolledFatiguePercent = Mathf.Max(4, UnityEngine.Random.Range(4, Mathf.Round(11 + Mathf.RoundToInt(Endur / -10f) + Mathf.RoundToInt(Willp / -25f) * bashHitMod)));
                    fatigueDam = Mathf.Max((int)Mathf.Floor(Player.MaxFatigue * 0.04f), (int)Mathf.Floor(Player.MaxFatigue * (rolledFatiguePercent / 100f)));
                    fatigueDam = (Player.CurrentFatigue - fatigueDam < 0) ? Player.CurrentFatigue : fatigueDam;

                    Player.DecreaseFatigue((int)fatigueDam, false); // Will need to do testing to make sure this works with the fatigue multiplier, including above formulas and such, bit confusing.
                    weapon.LowerCondition((int)Mathf.Max(1, Mathf.Round(weapon.maxCondition * (rolledWepDamPercent / 100f))), Player);
                }
            }
            else
            {
                return;
            }
        }

        public static void BashingDamageLootContents(LLCObject chest, DaggerfallUnityItem weapon, int wepSkillID, bool hitLock, bool hitWood, bool critBash)
        {
            int initialItemCount = chest.AttachedLoot.Count;

            if (weapon == null && wepSkillID == (short)DFCareer.Skills.HandToHand) // If player is bashing chest with their bare fists
            {
                if (hitLock)
                {
                    if (critBash)
                    {
                        for (int i = 0; i < initialItemCount; i++)
                        {
                            DaggerfallUnityItem item = chest.AttachedLoot.GetItem(i);
                            if (item == null)
                                continue;

                            LootItemSturdiness itemStab = DetermineLootItemSturdiness(item);

                            if (!item.IsQuestItem)
                            {
                                if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(45 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(20 + Mathf.RoundToInt(Luck / -5f)))
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
                                if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(20 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(5 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (critBash)
                    {
                        for (int i = 0; i < initialItemCount; i++)
                        {
                            DaggerfallUnityItem item = chest.AttachedLoot.GetItem(i);
                            if (item == null)
                                continue;

                            LootItemSturdiness itemStab = DetermineLootItemSturdiness(item);

                            if (!item.IsQuestItem)
                            {
                                if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(75 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(50 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(35 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Resilient && Dice100.SuccessRoll(35 + Mathf.RoundToInt(Luck / -5f)))
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
                                if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(45 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(25 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(15 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Resilient && Dice100.SuccessRoll(15 + Mathf.RoundToInt(Luck / -5f)))
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
                    if (critBash)
                    {
                        for (int i = 0; i < initialItemCount; i++)
                        {
                            DaggerfallUnityItem item = chest.AttachedLoot.GetItem(i);
                            if (item == null)
                                continue;

                            LootItemSturdiness itemStab = DetermineLootItemSturdiness(item);

                            if (!item.IsQuestItem)
                            {
                                if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(70 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(30 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(15 + Mathf.RoundToInt(Luck / -5f)))
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
                                if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(45 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(20 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (critBash)
                    {
                        for (int i = 0; i < initialItemCount; i++)
                        {
                            DaggerfallUnityItem item = chest.AttachedLoot.GetItem(i);
                            if (item == null)
                                continue;

                            LootItemSturdiness itemStab = DetermineLootItemSturdiness(item);

                            if (!item.IsQuestItem)
                            {
                                if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(90 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(60 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(40 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Resilient && Dice100.SuccessRoll(40 + Mathf.RoundToInt(Luck / -5f)))
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
                                if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(55 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(35 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(20 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Resilient && Dice100.SuccessRoll(20 + Mathf.RoundToInt(Luck / -5f)))
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
                    if (critBash)
                    {
                        for (int i = 0; i < initialItemCount; i++)
                        {
                            DaggerfallUnityItem item = chest.AttachedLoot.GetItem(i);
                            if (item == null)
                                continue;

                            LootItemSturdiness itemStab = DetermineLootItemSturdiness(item);

                            if (!item.IsQuestItem)
                            {
                                if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(30 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(65 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(10 + Mathf.RoundToInt(Luck / -5f)))
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
                                if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(15 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(40 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (critBash)
                    {
                        for (int i = 0; i < initialItemCount; i++)
                        {
                            DaggerfallUnityItem item = chest.AttachedLoot.GetItem(i);
                            if (item == null)
                                continue;

                            LootItemSturdiness itemStab = DetermineLootItemSturdiness(item);

                            if (!item.IsQuestItem)
                            {
                                if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(50 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(75 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(40 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Resilient && Dice100.SuccessRoll(40 + Mathf.RoundToInt(Luck / -5f)))
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
                                if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(25 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(55 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(20 + Mathf.RoundToInt(Luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Resilient && Dice100.SuccessRoll(20 + Mathf.RoundToInt(Luck / -5f)))
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
                            if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(50 + Mathf.RoundToInt(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(25 + Mathf.RoundToInt(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(20 + Mathf.RoundToInt(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Resilient && Dice100.SuccessRoll(20 + Mathf.RoundToInt(Luck / -5f)))
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
                            if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(60 + Mathf.RoundToInt(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(35 + Mathf.RoundToInt(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(30 + Mathf.RoundToInt(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Resilient && Dice100.SuccessRoll(30 + Mathf.RoundToInt(Luck / -5f)))
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
                            if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(30 + Mathf.RoundToInt(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(65 + Mathf.RoundToInt(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(30 + Mathf.RoundToInt(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Resilient && Dice100.SuccessRoll(30 + Mathf.RoundToInt(Luck / -5f)))
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
                            if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(75 + Mathf.RoundToInt(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(55 + Mathf.RoundToInt(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(35 + Mathf.RoundToInt(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Resilient && Dice100.SuccessRoll(35 + Mathf.RoundToInt(Luck / -5f)))
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
                            if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(90 + Mathf.RoundToInt(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(65 + Mathf.RoundToInt(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(45 + Mathf.RoundToInt(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Resilient && Dice100.SuccessRoll(45 + Mathf.RoundToInt(Luck / -5f)))
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
                            if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(45 + Mathf.RoundToInt(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(90 + Mathf.RoundToInt(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(55 + Mathf.RoundToInt(Luck / -5f)))
                            {
                                if (HandleDestroyingLootItem(chest, item, weapon, wepSkillID)) { i--; continue; }
                            }
                            else if (itemStab == LootItemSturdiness.Resilient && Dice100.SuccessRoll(55 + Mathf.RoundToInt(Luck / -5f)))
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
                    if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(55 + Mathf.RoundToInt(Luck / -5f)))
                    {
                        if (HandleDestroyingLootItem(chest, item)) { i--; continue; }
                    }
                    else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(75 + Mathf.RoundToInt(Luck / -5f)))
                    {
                        if (HandleDestroyingLootItem(chest, item)) { i--; continue; }
                    }
                    else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(50 + Mathf.RoundToInt(Luck / -5f)))
                    {
                        if (HandleDestroyingLootItem(chest, item)) { i--; continue; }
                    }
                    else if (itemStab == LootItemSturdiness.Resilient && Dice100.SuccessRoll(50 + Mathf.RoundToInt(Luck / -5f)))
                    {
                        if (HandleDestroyingLootItem(chest, item)) { i--; continue; }
                    }
                }
            }
        }
    }
}