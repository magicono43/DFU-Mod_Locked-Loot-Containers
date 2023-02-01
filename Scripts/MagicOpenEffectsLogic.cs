using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallConnect;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;

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

                if (closedChestData.IsLockJammed) // Continue work here next time. Redo all the below for the magic effect specifically. Odds primarily based on lock magic resist, player mysticism skill, willpower, intelligence, and luck, etc.
                {
                    DaggerfallUI.AddHUDText("The lock is jammed and inoperable...", 4f);
                    if (dfAudioSource != null && !dfAudioSource.IsPlaying())
                        dfAudioSource.PlayOneShot(SoundClips.ActivateRatchet); // Will use custom sounds in the end most likely.
                }
                else if (Dice100.SuccessRoll(LockPickChance(closedChestData))) // Guess the basic "success" stuff is already here for the time being, so I'll do more with that part later on.
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
    }
}