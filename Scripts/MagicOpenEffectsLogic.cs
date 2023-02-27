using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallConnect;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using DaggerfallWorkshop.Game.MagicAndEffects;

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

                if (closedChestData.IsLockJammed)
                {
                    DaggerfallUI.AddHUDText("The lock is jammed and inoperable...", 4f);
                    if (dfAudioSource != null && !dfAudioSource.IsPlaying())
                        dfAudioSource.PlayClipAtPoint(SoundClips.ActivateRatchet, closedChestData.gameObject.transform.position); // Will use custom sounds in the end most likely.
                }
                else if (OpenEffectChance(closedChestData)) // Guess the basic "success" stuff is already here for the time being, so I'll do more with that part later on.
                {
                    DaggerfallLoot openChestLoot = GameObjectHelper.CreateLootContainer(LootContainerTypes.Nothing, InventoryContainerImages.Chest, pos, closedChestTransform.parent, 4734, 0, closedChestData.LoadID, null, false);
                    openChestLoot.gameObject.name = GameObjectHelper.GetGoFlatName(4734, 0);
                    openChestLoot.Items.TransferAll(closedChestLoot); // Transfers items from closed chest's items to the new open chest's item collection.

                    // Show success and play unlock sound
                    DaggerfallUI.AddHUDText("The lock effortlessly unlatches through use of magic...", 4f);
                    if (dfAudioSource != null)
                        dfAudioSource.PlayClipAtPoint(SoundClips.ActivateLockUnlock, closedChestData.gameObject.transform.position); // Might use custom sound here, or atleast varied pitches of the same sound, etc.

                    Destroy(ChestObjRef); // Removed closed chest from scene, but saved its characteristics we care about for opened chest loot-pile.
                    ChestObjRef = null;
                }
                else
                {
                    closedChestData.PicksAttempted++; // Increase picks attempted counter by 1 on the chest.
                    if (Dice100.SuccessRoll(LockJamChance(closedChestData)))
                    {
                        closedChestData.IsLockJammed = true;
                        DaggerfallUI.AddHUDText("You jammed the lock, now brute force is the only option.", 4f);
                        if (dfAudioSource != null)
                            dfAudioSource.PlayClipAtPoint(SoundClips.ActivateGrind, closedChestData.gameObject.transform.position); // Will use custom sounds in the end most likely.
                    }
                    else
                    {
                        DaggerfallUI.AddHUDText("The unlock attempt failed...", 4f);
                        if (dfAudioSource != null && !dfAudioSource.IsPlaying())
                            dfAudioSource.PlayClipAtPoint(SoundClips.ActivateGears, closedChestData.gameObject.transform.position); // Will use custom sounds in the end most likely.
                    }
                }
            }
            else
            {
                DaggerfallUI.AddHUDText("ERROR: Chest Was Found As Null.", 5f);
            }

            openEffect.CancelEffect();
            return true;
        }

        public static bool OpenEffectChance(LLCObject chest) // Still not entirely happy with the current form here, but I think it's alot better than the first draft atleast, so fine for now, I think.
        {
            int magicResist = chest.LockMagicResist;
            int lockComp = chest.LockComplexity;

            if (magicResist >= 0 && magicResist <= 19)
            {
                int mysti = (int)Mathf.Ceil(Mysti * 1.6f);
                float successChance = (magicResist * -1) + Mathf.Floor(lockComp / -4) + mysti + Mathf.Round(Intel * .6f) + Mathf.Round(Willp * 1.5f) + Mathf.Round(Luck * .35f);
                if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(successChance, 8f, 100f))))
                    return true;
                else
                    return false;
            }
            else if (magicResist >= 20 && magicResist <= 39)
            {
                int mysti = (int)Mathf.Ceil(Mysti * 1.7f);
                float successChance = (magicResist * -1) + Mathf.Floor(lockComp / -4) + mysti + Mathf.Round(Intel * .7f) + Mathf.Round(Willp * 1.4f) + Mathf.Round(Luck * .35f);
                if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(successChance, 6f, 95f))))
                {
                    Player.TallySkill(DFCareer.Skills.Mysticism, 1);
                    return true;
                }
                else
                    return false;
            }
            else if (magicResist >= 40 && magicResist <= 59)
            {
                int mysti = (int)Mathf.Ceil(Mysti * 1.8f);
                float successChance = (magicResist * -1) + Mathf.Floor(lockComp / -4) + mysti + Mathf.Round(Intel * .8f) + Mathf.Round(Willp * 1.3f) + Mathf.Round(Luck * .35f);
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
                int mysti = (int)Mathf.Ceil(Mysti * 1.9f);
                float successChance = (magicResist * -1) + Mathf.Floor(lockComp / -4) + mysti + Mathf.Round(Intel * .9f) + Mathf.Round(Willp * 1.2f) + Mathf.Round(Luck * .35f);
                if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(successChance, 2f, 85f))))
                {
                    Player.TallySkill(DFCareer.Skills.Mysticism, 3);
                    return true;
                }
                else
                    return false;
            }
            else
            {
                int mysti = (int)Mathf.Ceil(Mysti * 2f);
                float successChance = (magicResist * -1) + Mathf.Floor(lockComp / -4) + mysti + Mathf.Round(Intel * 1f) + Mathf.Round(Willp * 1.1f) + Mathf.Round(Luck * .35f);
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

                for (int i = 0; i < damOrDisin.Length; i++)
                {
                    if (damOrDisin[i] == 2)
                    {
                        if (ChestDisintegrationRoll(chest, magOrChance[i], missile.ElementType))
                        {
                            // Chest has been disintegrated and contents are accessible (but damaged greatly, if not outright destroyed.)
                            SpellDestroyingChestDamagesLoot(chest, damOrDisin[i], magOrChance[i]);
                            DaggerfallLoot openChestLoot = GameObjectHelper.CreateLootContainer(LootContainerTypes.Nothing, InventoryContainerImages.Chest, pos, closedChestTransform.parent, 4735, 0, chest.LoadID, null, false);
                            openChestLoot.gameObject.name = GameObjectHelper.GetGoFlatName(4735, 0);
                            openChestLoot.Items.TransferAll(closedChestLoot); // Transfers items from closed chest's items to the new open chest's item collection.

                            // Show success and play disintegrate sound
                            DaggerfallUI.AddHUDText("The spell causes the chest to disintegrate into an unrecognizable pile, granting access to whatever is left...", 4f); // Will possibly change text later on depending on many factors, will see.
                            if (dfAudioSource)
                            {
                                if (dfAudioSource != null)
                                    AudioSource.PlayClipAtPoint(GetSpellImpactChestAudioClip(chest, true, true), chest.gameObject.transform.position, UnityEngine.Random.Range(0.9f, 1.42f) * DaggerfallUnity.Settings.SoundVolume);
                            }
                            return true;
                        }
                    }
                    else if (damOrDisin[i] == 1)
                    {
                        if (!alreadyCheckedDamEffects)
                        {
                            alreadyCheckedDamEffects = true;
                            if (ChestBlownOpenRoll(chest, totalDamageMag, missile.ElementType))
                            {
                                // Chest has been blown open by damage health spell and contents are accessible (but damaged greatly, if not outright destroyed.)
                                SpellDestroyingChestDamagesLoot(chest, damOrDisin[i], totalDamageMag);
                                DaggerfallLoot openChestLoot = GameObjectHelper.CreateLootContainer(LootContainerTypes.Nothing, InventoryContainerImages.Chest, pos, closedChestTransform.parent, 4735, 0, chest.LoadID, null, false);
                                openChestLoot.gameObject.name = GameObjectHelper.GetGoFlatName(4735, 0);
                                openChestLoot.Items.TransferAll(closedChestLoot); // Transfers items from closed chest's items to the new open chest's item collection.

                                // Show success and play explosion type sound
                                DaggerfallUI.AddHUDText("The spell causes the chest to erupt into a chaotic mess, granting access to its contents...", 4f); // Will possibly change text later on depending on many factors, will see.
                                if (dfAudioSource)
                                {
                                    if (dfAudioSource != null)
                                        AudioSource.PlayClipAtPoint(GetSpellImpactChestAudioClip(chest, true, false), chest.gameObject.transform.position, UnityEngine.Random.Range(0.9f, 1.42f) * DaggerfallUnity.Settings.SoundVolume);
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
                        AudioSource.PlayClipAtPoint(GetSpellImpactChestAudioClip(chest, false, false), chest.gameObject.transform.position, UnityEngine.Random.Range(0.9f, 1.42f) * DaggerfallUnity.Settings.SoundVolume);
                }
            }
            else
            {
                DaggerfallUI.AddHUDText("ERROR: Chest Was Found As Null.", 5f);
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

        public static bool ChestBlownOpenRoll(LLCObject chest, int effectMagnitude, ElementTypes element) // Roll to determine if damage health spell effect(s) "blow open" the chest hit and gives access to what remains.
        {
            int magicResist = chest.ChestMagicResist;
            int sturdiness = chest.ChestSturdiness;
            float elementModifier = ElementModBasedOnChestMaterial(chest, element);
            float blowOpenChestChance = (magicResist * -1f) + Mathf.Floor(sturdiness / -4f) + Mathf.Round(effectMagnitude * (elementModifier + ((float)Destr / 500f))) + (int)Mathf.Round(Luck / 10f);

            if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(blowOpenChestChance, 0f, 93f))))
                return true;
            else
                return false;
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
                        if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(100 + (int)Mathf.Round(Luck / -5f)))
                        {
                            if (HandleDestroyingLootItem(chest, item, damOrDisin, spellMag)) { i--; continue; }
                        }
                        else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(90 + (int)Mathf.Round(Luck / -5f)))
                        {
                            if (HandleDestroyingLootItem(chest, item, damOrDisin, spellMag)) { i--; continue; }
                        }
                        else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(80 + (int)Mathf.Round(Luck / -5f)))
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
                        if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(85 + (int)Mathf.Round(Luck / -5f)))
                        {
                            if (HandleDestroyingLootItem(chest, item, damOrDisin, spellMag)) { i--; continue; }
                        }
                        else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(75 + (int)Mathf.Round(Luck / -5f)))
                        {
                            if (HandleDestroyingLootItem(chest, item, damOrDisin, spellMag)) { i--; continue; }
                        }
                        else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(65 + (int)Mathf.Round(Luck / -5f)))
                        {
                            if (HandleDestroyingLootItem(chest, item, damOrDisin, spellMag)) { i--; continue; }
                        }
                        else if (itemStab == LootItemSturdiness.Resilient && Dice100.SuccessRoll(55 + (int)Mathf.Round(Luck / -5f)))
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