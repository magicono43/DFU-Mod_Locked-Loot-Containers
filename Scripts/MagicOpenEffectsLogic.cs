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

            if (ChestObjRef != null)
            {
                LLCObject closedChestData = ChestObjRef.GetComponent<LLCObject>();
                DaggerfallAudioSource dfAudioSource = GameManager.Instance.PlayerActivate.GetComponent<DaggerfallAudioSource>();
                ItemCollection closedChestLoot = closedChestData.AttachedLoot;
                Transform closedChestTransform = ChestObjRef.transform;
                Vector3 pos = ChestObjRef.transform.position;

                if (closedChestData.IsLockJammed)
                {
                    DaggerfallUI.AddHUDText("The lock is jammed and inoperable...", 4f);
                    if (dfAudioSource != null && !dfAudioSource.IsPlaying())
                        dfAudioSource.PlayOneShot(SoundClips.ActivateRatchet); // Will use custom sounds in the end most likely.
                }
                else if (Dice100.SuccessRoll(OpenEffectChance(closedChestData))) // Guess the basic "success" stuff is already here for the time being, so I'll do more with that part later on.
                {
                    DaggerfallLoot openChestLoot = GameObjectHelper.CreateLootContainer(LootContainerTypes.Nothing, InventoryContainerImages.Chest, pos, closedChestTransform.parent, 811, 0, closedChestData.LoadID, null, false);
                    openChestLoot.gameObject.name = GameObjectHelper.GetGoFlatName(811, 0);
                    openChestLoot.Items.TransferAll(closedChestLoot); // Transfers items from closed chest's items to the new open chest's item collection.

                    Destroy(ChestObjRef); // Removed closed chest from scene, but saved its characteristics we care about for opened chest loot-pile.
                    ChestObjRef = null;

                    // Show success and play unlock sound
                    DaggerfallUI.AddHUDText("The lock effortlessly unlatches through use of magic...", 4f);
                    if (dfAudioSource != null && !dfAudioSource.IsPlaying())
                        dfAudioSource.PlayOneShot(SoundClips.ActivateLockUnlock); // Might use custom sound here, or atleast varied pitches of the same sound, etc.
                }
                else
                {
                    closedChestData.PicksAttempted++; // Increase picks attempted counter by 1 on the chest.
                    if (Dice100.SuccessRoll(LockJamChance(closedChestData)))
                    {
                        closedChestData.IsLockJammed = true;
                        DaggerfallUI.AddHUDText("You jammed the lock, now brute force is the only option.", 4f);
                        if (dfAudioSource != null && !dfAudioSource.IsPlaying())
                            dfAudioSource.PlayOneShot(SoundClips.ActivateGrind); // Will use custom sounds in the end most likely.
                    }
                    else
                    {
                        DaggerfallUI.AddHUDText("The unlock attempt failed...", 4f);
                        if (dfAudioSource != null && !dfAudioSource.IsPlaying())
                            dfAudioSource.PlayOneShot(SoundClips.ActivateGears); // Will use custom sounds in the end most likely.
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

        public static int OpenEffectChance(LLCObject chest) // Still not entirely happy with the current form here, but I think it's alot better than the first draft atleast, so fine for now, I think.
        {
            int magicResist = chest.LockMagicResist;
            int lockComp = chest.LockComplexity;

            if (magicResist >= 0 && magicResist <= 19)
            {
                int mysti = (int)Mathf.Ceil(Mysti * 1.6f);
                float successChance = (magicResist * -1) + Mathf.Floor(lockComp / -4) + mysti + Mathf.Round(Intel * .6f) + Mathf.Round(Willp * 1.5f) + Mathf.Round(Luck * .35f);
                return (int)Mathf.Round(Mathf.Clamp(successChance, 8f, 100f));
            }
            else if (magicResist >= 20 && magicResist <= 39)
            {
                int mysti = (int)Mathf.Ceil(Mysti * 1.7f);
                float successChance = (magicResist * -1) + Mathf.Floor(lockComp / -4) + mysti + Mathf.Round(Intel * .7f) + Mathf.Round(Willp * 1.4f) + Mathf.Round(Luck * .35f);
                return (int)Mathf.Round(Mathf.Clamp(successChance, 6f, 95f));
            }
            else if (magicResist >= 40 && magicResist <= 59)
            {
                int mysti = (int)Mathf.Ceil(Mysti * 1.8f);
                float successChance = (magicResist * -1) + Mathf.Floor(lockComp / -4) + mysti + Mathf.Round(Intel * .8f) + Mathf.Round(Willp * 1.3f) + Mathf.Round(Luck * .35f);
                return (int)Mathf.Round(Mathf.Clamp(successChance, 4f, 90f));
            }
            else if (magicResist >= 60 && magicResist <= 79)
            {
                int mysti = (int)Mathf.Ceil(Mysti * 1.9f);
                float successChance = (magicResist * -1) + Mathf.Floor(lockComp / -4) + mysti + Mathf.Round(Intel * .9f) + Mathf.Round(Willp * 1.2f) + Mathf.Round(Luck * .35f);
                return (int)Mathf.Round(Mathf.Clamp(successChance, 2f, 85f));
            }
            else
            {
                int mysti = (int)Mathf.Ceil(Mysti * 2f);
                float successChance = (magicResist * -1) + Mathf.Floor(lockComp / -4) + mysti + Mathf.Round(Intel * 1f) + Mathf.Round(Willp * 1.1f) + Mathf.Round(Luck * .35f);
                return (int)Mathf.Round(Mathf.Clamp(successChance, 1f, 80f)); // Potentially add specific text depending on initial odds, like "Through dumb-Luck, you somehow unlocked it", etc.
            }
        }

        public static void AttemptDestructiveMagicChestImpact(LLCObject chest, DaggerfallMissile missile)
        {
            if (chest != null)
            {
                ItemCollection closedChestLoot = chest.AttachedLoot;
                Transform closedChestTransform = chest.gameObject.transform; // Not sure if the explicit "gameObject" reference is necessary, will test eventually and determine if so or not.
                Vector3 pos = chest.gameObject.transform.position;
                DaggerfallAudioSource dfAudioSource = GameManager.Instance.PlayerActivate.GetComponent<DaggerfallAudioSource>();

                bool validEffectPresent = false;
                EntityEffectBundle payload = missile.Payload;
                TargetTypes targetType = missile.TargetType;
                ElementTypes element = missile.ElementType;

                for (int i = 0; i < payload.Settings.Effects.Length; i++)
                {
                    IEntityEffect effect = GameManager.Instance.EntityEffectBroker.GetEffectTemplate(payload.Settings.Effects[i].Key);
                    if (effect != null)
                    {
                        if (effect.Key == "Damage-Health" || effect.Key == "ContinuousDamage-Health" || effect.Key == "Disintegrate")
                            validEffectPresent = true;
                    }
                }

                // Continue work on this next time. Figure out how I'm going to check the individual spell-effects and try to be "process efficient" with it if possible and such.

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
                        DaggerfallLoot openChestLoot = GameObjectHelper.CreateLootContainer(LootContainerTypes.Nothing, InventoryContainerImages.Chest, pos, closedChestTransform.parent, 812, 0, chest.LoadID, null, false);
                        openChestLoot.gameObject.name = GameObjectHelper.GetGoFlatName(811, 0);
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
                        DaggerfallLoot openChestLoot = GameObjectHelper.CreateLootContainer(LootContainerTypes.Nothing, InventoryContainerImages.Chest, pos, closedChestTransform.parent, 812, 0, chest.LoadID, null, false);
                        openChestLoot.gameObject.name = GameObjectHelper.GetGoFlatName(812, 0);
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
    }
}