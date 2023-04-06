using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallConnect;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Serialization;

namespace LockedLootContainers
{
    public partial class LockedLootContainersMain
    {
        public static bool HandleOpenEffect() // Bare basic behavior for the open effect, just for testing for now obviously.
        {
            // Check if player has Open effect running
            Open openEffect = (Open)GameManager.Instance.PlayerEffectManager.FindIncumbentEffect<Open>();
            if (openEffect == null)
                return false;

            if (openEffect.RoundsRemaining <= 0) // Will hopefully prevent the "spam clicking" exploit issue.
                return false;

            if (ChestObjRef != null)
            {
                LLCObject closedChestData = ChestObjRef.GetComponent<LLCObject>();
                DaggerfallAudioSource dfAudioSource = closedChestData.GetComponent<DaggerfallAudioSource>();
                ItemCollection closedChestLoot = closedChestData.AttachedLoot;
                Transform closedChestTransform = ChestObjRef.transform;
                Vector3 pos = ChestObjRef.transform.position;

                IsThisACrime(ChestInteractionType.Magic_Lockpick);

                if (closedChestData.IsLockJammed)
                {
                    DaggerfallUI.AddHUDText(GetMagicLockAlreadyJammedText(), 2f);
                    if (dfAudioSource != null && !dfAudioSource.IsPlaying())
                        dfAudioSource.AudioSource.PlayOneShot(GetMagicLockAlreadyJammedClip(), UnityEngine.Random.Range(1.2f, 1.91f) * DaggerfallUnity.Settings.SoundVolume);
                }
                else if (OpenEffectChance(closedChestData)) // Guess the basic "success" stuff is already here for the time being, so I'll do more with that part later on.
                {
                    closedChestData.PicksAttempted++; // Increase picks attempted counter by 1 on the chest.

                    DaggerfallLoot openChestLoot = null;
                    if (ChestGraphicType == 0) // Use sprite based graphics for chests
                    {
                        openChestLoot = GameObjectHelper.CreateLootContainer(LootContainerTypes.Nothing, InventoryContainerImages.Chest, pos, closedChestTransform.parent, OpenFullChestSpriteID, 0, DaggerfallUnity.NextUID, null, false);
                        openChestLoot.gameObject.name = GameObjectHelper.GetGoFlatName(OpenFullChestSpriteID, 0);
                        openChestLoot.Items.TransferAll(closedChestLoot); // Transfers items from closed chest's items to the new open chest's item collection.
                        Destroy(openChestLoot.GetComponent<SerializableLootContainer>());
                    }
                    else // Use 3D models for chests
                    {
                        GameObject usedModelPrefab = (ChestGraphicType == 1) ? Instance.LowPolyOpenFullChestPrefab : Instance.HighPolyOpenFullChestPrefab;
                        GameObject chestGo = GameObjectHelper.InstantiatePrefab(usedModelPrefab, GameObjectHelper.GetGoModelName(OpenFullChestModelID), closedChestTransform.parent, pos);
                        chestGo.transform.rotation = closedChestData.gameObject.transform.rotation;
                        Collider col = chestGo.AddComponent<BoxCollider>();
                        openChestLoot = chestGo.AddComponent<DaggerfallLoot>();
                        if (openChestLoot)
                        {
                            openChestLoot.ContainerType = LootContainerTypes.Nothing;
                            openChestLoot.ContainerImage = InventoryContainerImages.Chest;
                            openChestLoot.LoadID = DaggerfallUnity.NextUID;
                            openChestLoot.TextureRecord = OpenFullChestModelID;
                            openChestLoot.Items.TransferAll(closedChestLoot); // Transfers items from closed chest's items to the new open chest's item collection.
                        }
                    }

                    // Show success and play unlock sound
                    DaggerfallUI.AddHUDText(GetMagicLockPickSuccessText(), 3f);
                    if (dfAudioSource != null)
                        AudioSource.PlayClipAtPoint(GetMagicLockpickSuccessClip(), closedChestData.gameObject.transform.position, UnityEngine.Random.Range(1.7f, 2.61f) * DaggerfallUnity.Settings.SoundVolume);

                    Destroy(ChestObjRef); // Removed closed chest from scene, but saved its characteristics we care about for opened chest loot-pile.
                    ChestObjRef = null;
                }
                else
                {
                    closedChestData.PicksAttempted++; // Increase picks attempted counter by 1 on the chest.
                    int mechDamDealt = DetermineDamageToLockMechanism(closedChestData);

                    if (DoesLockJam(closedChestData, mechDamDealt))
                    {
                        DaggerfallUI.AddHUDText(GetMagicJammedLockText(), 3f);
                        if (dfAudioSource != null)
                            AudioSource.PlayClipAtPoint(GetMagicLockpickJammedClip(), closedChestData.gameObject.transform.position, UnityEngine.Random.Range(8.2f, 9.71f) * DaggerfallUnity.Settings.SoundVolume);
                    }
                    else
                    {
                        DaggerfallUI.AddHUDText(GetMagicLockPickAttemptText(), 2f);
                        if (dfAudioSource != null && !dfAudioSource.IsPlaying())
                            dfAudioSource.AudioSource.PlayOneShot(GetMagicLockpickAttemptClip(), UnityEngine.Random.Range(1.2f, 1.91f) * DaggerfallUnity.Settings.SoundVolume);
                    }
                }
            }
            else
            {
                DaggerfallUI.AddHUDText("ERROR: Chest Was Found As Null.", 3f);
            }

            openEffect.CancelEffect();
            return true;
        }

        public static bool OpenEffectChance(LLCObject chest)
        {
            int magicResist = chest.LockMagicResist;
            int lockComp = chest.LockComplexity;

            if (magicResist >= 0 && magicResist <= 19)
            {
                int mysti = (int)Mathf.Ceil(Mysti * 1.7f);
                float successChance = (magicResist * -1.3f) + Mathf.Floor(lockComp * -0.2f) + mysti + Mathf.Round(Intel * 0.5f) + Mathf.Round(Willp * 1f) + Mathf.Round(Luck * 0.3f);
                if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(successChance, 6f, 100f))))
                    return true;
                else
                    return false;
            }
            else if (magicResist >= 20 && magicResist <= 39)
            {
                int mysti = (int)Mathf.Ceil(Mysti * 1.7f);
                float successChance = (magicResist * -1.3f) + Mathf.Floor(lockComp * -0.2f) + mysti + Mathf.Round(Intel * 0.6f) + Mathf.Round(Willp * 0.9f) + Mathf.Round(Luck * 0.3f);
                if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(successChance, 5f, 95f))))
                {
                    Player.TallySkill(DFCareer.Skills.Mysticism, 1);
                    return true;
                }
                else
                    return false;
            }
            else if (magicResist >= 40 && magicResist <= 59)
            {
                int mysti = (int)Mathf.Ceil(Mysti * 1.7f);
                float successChance = (magicResist * -1.3f) + Mathf.Floor(lockComp * -0.2f) + mysti + Mathf.Round(Intel * 0.7f) + Mathf.Round(Willp * 0.8f) + Mathf.Round(Luck * 0.3f);
                if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(successChance, 4f, 90f))))
                {
                    Player.TallySkill(DFCareer.Skills.Mysticism, 2);
                    return true;
                }
                else
                    return false;
            }
            else if (magicResist >= 60 && magicResist <= 79)
            {
                int mysti = (int)Mathf.Ceil(Mysti * 1.7f);
                float successChance = (magicResist * -1.3f) + Mathf.Floor(lockComp * -0.2f) + mysti + Mathf.Round(Intel * 0.8f) + Mathf.Round(Willp * 0.7f) + Mathf.Round(Luck * 0.3f);
                if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(successChance, 3f, 85f))))
                {
                    Player.TallySkill(DFCareer.Skills.Mysticism, 3);
                    return true;
                }
                else
                    return false;
            }
            else
            {
                int mysti = (int)Mathf.Ceil(Mysti * 1.7f);
                float successChance = (magicResist * -1.3f) + Mathf.Floor(lockComp * -0.2f) + mysti + Mathf.Round(Intel * 0.9f) + Mathf.Round(Willp * 0.6f) + Mathf.Round(Luck * 0.3f);
                if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(successChance, 1f, 80f)))) // Potentially add specific text depending on initial odds, like "Through dumb-Luck, you somehow unlocked it", etc.
                {
                    Player.TallySkill(DFCareer.Skills.Mysticism, 4);
                    return true;
                }
                else
                    return false;
            }
        }

        public static bool AttemptDestructiveMagicChestImpact(LLCObject chest, DaggerfallMissile missile)
        {
            if (chest != null)
            {
                ItemCollection closedChestLoot = chest.AttachedLoot;
                Transform closedChestTransform = chest.gameObject.transform; // Not sure if the explicit "gameObject" reference is necessary, will test eventually and determine if so or not.
                Vector3 pos = chest.gameObject.transform.position;
                DaggerfallAudioSource dfAudioSource = chest.GetComponent<DaggerfallAudioSource>();

                EntityEffectBundle payload = missile.Payload;
                bool hasDamageHealth = false;
                bool hasDisintegrate = false;
                bool alreadyCheckedDamEffects = false;
                int totalDamageMag = 0;
                int[] damOrDisin = new int[payload.Settings.Effects.Length];
                int[] magOrChance = new int[payload.Settings.Effects.Length];

                for (int i = 0; i < payload.Settings.Effects.Length; i++)
                {
                    damOrDisin[i] = 0; // 0 = no spell or invalid effect, 1 = damage or cont. damage effect, 2 = disintegrate effect.
                    magOrChance[i] = 0;
                    EffectEntry effect = payload.Settings.Effects[i];
                    if (effect.Key != "")
                    {
                        if (effect.Key == "Damage-Health" || effect.Key == "ContinuousDamage-Health")
                        {
                            hasDamageHealth = true;
                            damOrDisin[i] = 1;
                            magOrChance[i] = RollEffectMagnitude(effect);
                            totalDamageMag += magOrChance[i];
                        }
                        else if (effect.Key == "Disintegrate")
                        {
                            hasDisintegrate = true;
                            damOrDisin[i] = 2;
                            magOrChance[i] = effect.Settings.ChanceBase + effect.Settings.ChancePlus * (int)Mathf.Floor(Player.Level / effect.Settings.ChancePerLevel);
                        }
                    }
                }

                if (!hasDamageHealth && !hasDisintegrate)
                    return false;

                chest.HasBeenBashed = true;
                IsThisACrime(ChestInteractionType.Magic_Bash);

                for (int i = 0; i < damOrDisin.Length; i++)
                {
                    if (damOrDisin[i] == 2)
                    {
                        if (ChestDisintegrationRoll(chest, magOrChance[i], missile.ElementType))
                        {
                            // Chest has been disintegrated and contents are accessible (but damaged greatly, if not outright destroyed.)
                            SpellDestroyingChestDamagesLoot(chest, damOrDisin[i], magOrChance[i]);

                            DaggerfallLoot openChestLoot = null;
                            if (ChestGraphicType == 0) // Use sprite based graphics for chests
                            {
                                openChestLoot = GameObjectHelper.CreateLootContainer(LootContainerTypes.Nothing, InventoryContainerImages.Chest, pos, closedChestTransform.parent, DisintegratedChestSpriteID, 0, DaggerfallUnity.NextUID, null, false);
                                openChestLoot.gameObject.name = GameObjectHelper.GetGoFlatName(DisintegratedChestSpriteID, 0);
                                openChestLoot.Items.TransferAll(closedChestLoot); // Transfers items from closed chest's items to the new open chest's item collection.
                                Destroy(openChestLoot.GetComponent<SerializableLootContainer>());
                            }
                            else // Use 3D models for chests
                            {
                                GameObject usedModelPrefab = (ChestGraphicType == 1) ? Instance.LowPolyDisintegratedChestPrefab : Instance.HighPolyDisintegratedChestPrefab;
                                GameObject chestGo = GameObjectHelper.InstantiatePrefab(usedModelPrefab, GameObjectHelper.GetGoModelName(DisintegratedChestModelID), closedChestTransform.parent, pos);
                                Collider col = chestGo.AddComponent<BoxCollider>();
                                openChestLoot = chestGo.AddComponent<DaggerfallLoot>();
                                if (openChestLoot)
                                {
                                    openChestLoot.ContainerType = LootContainerTypes.Nothing;
                                    openChestLoot.ContainerImage = InventoryContainerImages.Chest;
                                    openChestLoot.LoadID = DaggerfallUnity.NextUID;
                                    openChestLoot.TextureRecord = DisintegratedChestModelID;
                                    openChestLoot.Items.TransferAll(closedChestLoot); // Transfers items from closed chest's items to the new open chest's item collection.
                                }
                            }

                            // Show success and play disintegrate sound
                            DaggerfallUI.AddHUDText(GetChestDisintegratedBySpellText(), 4f); // Will possibly change text later on depending on many factors, will see.
                            if (dfAudioSource)
                            {
                                if (dfAudioSource != null)
                                    AudioSource.PlayClipAtPoint(GetSpellImpactChestAudioClip(chest, true, true), chest.gameObject.transform.position, UnityEngine.Random.Range(3.5f, 4.11f) * DaggerfallUnity.Settings.SoundVolume);
                            }
                            return true;
                        }
                    }
                    else if (damOrDisin[i] == 1)
                    {
                        if (!alreadyCheckedDamEffects)
                        {
                            alreadyCheckedDamEffects = true;
                            int damDealt = DetermineMagicDamageToChest(chest, totalDamageMag, missile.ElementType);

                            if (ChestBlownOpenAttempt(chest, damDealt))
                            {
                                // Chest has been blown open by damage health spell and contents are accessible (but damaged greatly, if not outright destroyed.)
                                SpellDestroyingChestDamagesLoot(chest, damOrDisin[i], totalDamageMag);

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
                                    if (openChestLoot)
                                    {
                                        openChestLoot.ContainerType = LootContainerTypes.Nothing;
                                        openChestLoot.ContainerImage = InventoryContainerImages.Chest;
                                        openChestLoot.LoadID = DaggerfallUnity.NextUID;
                                        openChestLoot.TextureRecord = SmashedChestModelID;
                                        openChestLoot.Items.TransferAll(closedChestLoot); // Transfers items from closed chest's items to the new open chest's item collection.
                                    }
                                }

                                // Show success and play explosion type sound
                                DaggerfallUI.AddHUDText(GetChestBlownOpenBySpellText(), 4f); // Will possibly change text later on depending on many factors, will see.
                                if (dfAudioSource)
                                {
                                    if (dfAudioSource != null)
                                        AudioSource.PlayClipAtPoint(GetSpellImpactChestAudioClip(chest, true, false), chest.gameObject.transform.position, UnityEngine.Random.Range(7.4f, 9.21f) * DaggerfallUnity.Settings.SoundVolume);
                                }
                                return true;
                            }
                        }
                    }
                }
                // Destruction spell impact was attempted, but chest is still intact.
                if (dfAudioSource)
                {
                    if (dfAudioSource != null && !dfAudioSource.IsPlaying())
                        dfAudioSource.AudioSource.PlayOneShot(GetSpellImpactChestAudioClip(chest, false, false), UnityEngine.Random.Range(1.0f, 1.72f) * DaggerfallUnity.Settings.SoundVolume);
                }
            }
            else
            {
                DaggerfallUI.AddHUDText("ERROR: Chest Was Found As Null.", 3f);
            }
            return false;
        }

        public static bool ChestDisintegrationRoll(LLCObject chest, int effectChance, ElementTypes element) // Roll to determine if disintegrate spell effect "destroys" the chest hit and gives access to what remains.
        {
            int magicResist = chest.ChestMagicResist;
            float elementModifier = ElementModBasedOnChestMaterial(chest, element);
            float disintegrateChestChance = (magicResist * -1f) + Mathf.Round(effectChance * (elementModifier + ((float)Destr / 285f))) + (int)Mathf.Round(Luck / 10f);

            if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(disintegrateChestChance, 0f, 93f))))
                return true;
            else
                return false;
        }

        public static int DetermineMagicDamageToChest(LLCObject chest, int effectMagnitude, ElementTypes element) // Determine how much damage the chest takes from a bash attempt, if any.
        {
            float elementModifier = ElementModBasedOnChestMaterial(chest, element);

            if (Dice100.SuccessRoll(ChestMagicDamageNegationChance(chest, elementModifier)))
                return 0;
            else
                return CalculateChestMagicDamage(chest, effectMagnitude, elementModifier);
        }

        public static bool ChestBlownOpenAttempt(LLCObject chest, int damage) // Determine if damage health spell effect(s) "blow open" the chest hit and gives access to what remains.
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

        public static void SpellDestroyingChestDamagesLoot(LLCObject chest, int damOrDisin, int spellMag)
        {
            int initialItemCount = chest.AttachedLoot.Count;

            if (damOrDisin == 2) // Disintegration Effect
            {
                for (int i = 0; i < initialItemCount; i++)
                {
                    DaggerfallUnityItem item = chest.AttachedLoot.GetItem(i);
                    if (item == null)
                        continue;

                    LootItemSturdiness itemStab = DetermineLootItemSturdiness(item);

                    if (!item.IsQuestItem)
                    {
                        if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(85 + (int)Mathf.Round(Luck / -5f)))
                        {
                            if (HandleDestroyingLootItem(chest, item, damOrDisin, spellMag)) { i--; continue; }
                        }
                        else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(85 + (int)Mathf.Round(Luck / -5f)))
                        {
                            if (HandleDestroyingLootItem(chest, item, damOrDisin, spellMag)) { i--; continue; }
                        }
                        else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(85 + (int)Mathf.Round(Luck / -5f)))
                        {
                            if (HandleDestroyingLootItem(chest, item, damOrDisin, spellMag)) { i--; continue; }
                        }
                        else if (itemStab == LootItemSturdiness.Resilient && Dice100.SuccessRoll(85 + (int)Mathf.Round(Luck / -5f)))
                        {
                            if (HandleDestroyingLootItem(chest, item, damOrDisin, spellMag)) { i--; continue; }
                        }
                    }
                }
            }
            else if (damOrDisin == 1) // Damage Health Effect
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
                            if (HandleDestroyingLootItem(chest, item, damOrDisin, spellMag)) { i--; continue; }
                        }
                        else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(70 + (int)Mathf.Round(Luck / -5f)))
                        {
                            if (HandleDestroyingLootItem(chest, item, damOrDisin, spellMag)) { i--; continue; }
                        }
                        else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(70 + (int)Mathf.Round(Luck / -5f)))
                        {
                            if (HandleDestroyingLootItem(chest, item, damOrDisin, spellMag)) { i--; continue; }
                        }
                        else if (itemStab == LootItemSturdiness.Resilient && Dice100.SuccessRoll(70 + (int)Mathf.Round(Luck / -5f)))
                        {
                            if (HandleDestroyingLootItem(chest, item, damOrDisin, spellMag)) { i--; continue; }
                        }
                    }
                }
            }
        }

        public static float ElementModBasedOnChestMaterial(LLCObject chest, ElementTypes element)
        {
            ChestMaterials chestMat = chest.ChestMaterial;
            if (chestMat == ChestMaterials.Wood)
            {
                if (element == ElementTypes.Fire) { return 1.5f; }
                else if (element == ElementTypes.Magic) { return 0.7f; }
                else if (element == ElementTypes.Shock) { return 0.5f; }
                else if (element == ElementTypes.Poison) { return 0.9f; }
                else if (element == ElementTypes.Cold) { return 1.0f; }
                else { return 1.0f; }
            }
            else
            {
                if (element == ElementTypes.Fire) { return 0.7f; }
                else if (element == ElementTypes.Magic) { return 1.0f; }
                else if (element == ElementTypes.Shock) { return 1.3f; }
                else if (element == ElementTypes.Poison) { return 1.3f; }
                else if (element == ElementTypes.Cold) { return 1.1f; }
                else { return 1.0f; }
            }
        }

        public static int RollEffectMagnitude(EffectEntry effect)
        {
            int magnitude = 0;
            IEntityEffect template = GameManager.Instance.EntityEffectBroker.GetEffectTemplate(effect.Key);
            if (template != null)
            {
                if (template.Properties.SupportMagnitude)
                {
                    int level = Player.Level;
                    int baseMagnitude = UnityEngine.Random.Range(effect.Settings.MagnitudeBaseMin, effect.Settings.MagnitudeBaseMax + 1);
                    int plusMagnitude = UnityEngine.Random.Range(effect.Settings.MagnitudePlusMin, effect.Settings.MagnitudePlusMax + 1);
                    int multiplier = (int)Mathf.Floor(level / effect.Settings.MagnitudePerLevel);
                    magnitude = baseMagnitude + plusMagnitude * multiplier;
                }
            }
            return magnitude;
        }
    }
}