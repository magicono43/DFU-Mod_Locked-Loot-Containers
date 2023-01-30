// Project:         LockedLootContainers mod for Daggerfall Unity (http://www.dfworkshop.net)
// Copyright:       Copyright (C) 2022 Kirk.O
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Author:          Kirk.O
// Created On: 	    9/8/2022, 11:00 PM
// Last Edit:		10/13/2022, 9:20 PM
// Version:			1.00
// Special Thanks:  
// Modifier:			

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
using DaggerfallConnect.Arena2;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Utility;

namespace LockedLootContainers
{
    public partial class LockedLootContainersMain : MonoBehaviour
    {
        static LockedLootContainersMain instance;

        public static LockedLootContainersMain Instance
        {
            get { return instance ?? (instance = FindObjectOfType<LockedLootContainersMain>()); }
        }

        static Mod mod;

        // Global Variables
        public static GameObject ChestObjRef { get; set; }
        public static GameObject MainCamera { get; set; }
        public static int PlayerLayerMask { get; set; }
        public static PlayerEntity Player { get { return GameManager.Instance.PlayerEntity; } }
        public static PlayerActivateModes CurrentMode { get { return GameManager.Instance.PlayerActivate.CurrentMode; } }
        public static WeaponManager WepManager { get { return GameManager.Instance.WeaponManager; } }

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;
            var go = new GameObject(mod.Title);
            instance = go.AddComponent<LockedLootContainersMain>(); // Add script to the scene.

            go.AddComponent<LLCObject>();

            mod.IsReady = true;
        }

        private void Start()
        {
            Debug.Log("Begin mod init: Locked Loot Containers");

            MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            PlayerLayerMask = ~(1 << LayerMask.NameToLayer("Player"));

            // Definitely consider making a custom activation for those silly 3D chest models that are in-game but never used for anything and add replace those with my custom chest objects, etc.
            PlayerActivate.RegisterCustomActivation(mod, 810, 0, ChestActivation); // Needs our custom texture/billboard flat ID value, 500 is placeholder.

            PlayerEnterExit.OnTransitionDungeonInterior += AddChests_OnTransitionDungeonInterior;

            Debug.Log("Finished mod init: Locked Loot Containers");
        }

        private void Update() // Will obviously have to refine this alot, but for now it's going to probably be the best solution for certain issues going forward for triggering and detection, etc.
        {
            if (GameManager.IsGamePaused)
                return;

            if (!WepManager.ScreenWeapon)
                return;

            if (!WepManager.ScreenWeapon.IsAttacking())
                return;

            if (WepManager.ScreenWeapon.GetCurrentFrame() == WepManager.ScreenWeapon.GetHitFrame())
            {
                if (WepManager.ScreenWeapon.WeaponType == WeaponTypes.Bow) // Don't consider bashing for the bow, atleast not this part, do the actual projectile later for that.
                    return;

                if (!MainCamera)
                    return;

                // Fire ray along player facing using weapon range
                RaycastHit hit;
                Ray ray = new Ray(MainCamera.transform.position, MainCamera.transform.forward);
                if (Physics.Raycast(ray, out hit, WepManager.ScreenWeapon.Reach, PlayerLayerMask)) // Using raycast instead of sphere, as I want it to be only if you are purposely targeting the chest.
                {
                    DaggerfallUnityItem currentRightHandWeapon = Player.ItemEquipTable.GetItem(EquipSlots.RightHand); // Will have to see if these work for the player or not, might just be enemies?
                    DaggerfallUnityItem currentLeftHandWeapon = Player.ItemEquipTable.GetItem(EquipSlots.LeftHand);
                    DaggerfallUnityItem strikingWeapon = WepManager.UsingRightHand ? currentRightHandWeapon : currentLeftHandWeapon;

                    // Check if hit has a LLCObject component
                    LLCObject hitChest = hit.transform.gameObject.GetComponent<LLCObject>();
                    if (hitChest)
                        AttemptMeleeChestBash(hitChest, strikingWeapon);
                }
            }

            // So the fact I got basically everything to work so far with the projectile/collision detection stuff is pretty crazy to me.
            // However, looking at the DaggerfallMissile.cs code under the "DoAreaOfEffect" method, I have a feeling without a PR, I won't be able to do a clean/easy
            // method to have the aoe of spell effects be detected for the chests. But I will have to see, either way not a big deal all other successes considered.
        }

        private static void AttemptMeleeChestBash(LLCObject chest, DaggerfallUnityItem weapon) // May eventually put this in LLCObject itself, but for now just keep it here with everything else.
        {
            if (chest != null)
            {
                ItemCollection closedChestLoot = chest.AttachedLoot;
                Transform closedChestTransform = chest.gameObject.transform; // Not sure if the explicit "gameObject" reference is necessary, will test eventually and determine if so or not.
                Vector3 pos = chest.gameObject.transform.position;
                DaggerfallAudioSource dfAudioSource = GameManager.Instance.PlayerActivate.GetComponent<DaggerfallAudioSource>();

                chest.HasBeenBashed = true; // Sets "HasBeenBashed" flag to true.

                if (BashLockRoll(chest, weapon))
                {
                    if (LockHardBashRoll(chest, weapon)) // False = Light Bash, True = Hard Bash
                        chest.LockBashedHardTimes++;
                    else
                        chest.LockBashedLightTimes++;

                    if (BreakLockRoll(chest, weapon))
                    {
                        // Lock was hit with bash and is now broken, so chest loot is accessible. Still have to deal with durability loss logic of chest content, and other logic like weapon damage, etc.
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
                        // Chest body has been smashed open and contents are accessible (but damaged greatly most likely.) Still have to deal with durability loss logic of chest content, and other logic like weapon damage, etc.
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

        private static void ChestActivation(RaycastHit hit)
        {
            ChestObjRef = hit.collider.gameObject; // Sets clicked chest as global variable reference for later user in other methods.

            if (hit.distance > PlayerActivate.DefaultActivationDistance)
                DaggerfallUI.SetMidScreenText(TextManager.Instance.GetLocalizedText("youAreTooFarAway"));

            // Handle Open spell effects (if active)
            if (HandleOpenEffect())
                return;

            switch (CurrentMode)
            {
                case PlayerActivateModes.Info: // Attempt To Inspect Chest
                case PlayerActivateModes.Talk:
                    if (ChestObjRef != null)
                    {
                        LLCObject chestData = ChestObjRef.GetComponent<LLCObject>();

                        TextFile.Token[] textToken = DaggerfallUnity.Instance.TextProvider.CreateTokens(TextFile.Formatting.JustifyCenter,
                        "This chest is made of: " + chestData.ChestMaterial.ToString(),
                        "Chest sturdiness is: " + chestData.ChestSturdiness,
                        "Chest magic resist is: " + chestData.ChestMagicResist,
                        "Its lock is made of: " + chestData.LockMaterial.ToString(),
                        "Lock sturdiness is: " + chestData.LockSturdiness,
                        "Lock magic resist is: " + chestData.LockMagicResist,
                        "Lock complexity is: " + chestData.LockComplexity,
                        "Lock jam resist is: " + chestData.JamResist);

                        DaggerfallMessageBox inspectChestPopup = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
                        inspectChestPopup.SetTextTokens(textToken); // Use a text-token here instead for the better debug stuff, better random encounters has good examples how, tomorrow.
                        inspectChestPopup.Show();
                        inspectChestPopup.ClickAnywhereToClose = true;
                    }
                    else
                    {
                        DaggerfallUI.AddHUDText("ERROR: Chest Was Found As Null.", 5f);
                    }
                    break;
                case PlayerActivateModes.Steal: // Attempt To Lock-pick Chest
                    if (ChestObjRef != null) // Oh yeah, don't forget to give skill XP as well for various things, failures and successes, etc.
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
                        else if (Dice100.SuccessRoll(LockPickChance(closedChestData))) // Guess the basic "success" stuff is already here for the time being, so I'll do more with that part later on.
                        {
                            DaggerfallLoot openChestLoot = GameObjectHelper.CreateLootContainer(LootContainerTypes.Nothing, InventoryContainerImages.Chest, pos, closedChestTransform.parent, 811, 0, closedChestData.LoadID, null, false);
                            openChestLoot.gameObject.name = GameObjectHelper.GetGoFlatName(811, 0);
                            openChestLoot.Items.TransferAll(closedChestLoot); // Transfers items from closed chest's items to the new open chest's item collection.

                            Destroy(ChestObjRef); // Removed closed chest from scene, but saved its characteristics we care about for opened chest loot-pile.
                            ChestObjRef = null;

                            // Show success and play unlock sound
                            DaggerfallUI.AddHUDText("The lock clicks open...", 4f);
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
                                DaggerfallUI.AddHUDText("You fail to pick the lock...", 4f);
                                if (dfAudioSource != null && !dfAudioSource.IsPlaying())
                                    dfAudioSource.PlayOneShot(SoundClips.ActivateGears); // Will use custom sounds in the end most likely.
                            }
                        }
                    }
                    else
                    {
                        DaggerfallUI.AddHUDText("ERROR: Chest Was Found As Null.", 5f);
                    }
                    break;
                case PlayerActivateModes.Grab: // Bring Up Pop-up Message With Buttons To Choose What To Do, Also Might Be How You Can Attempt Disarming Traps Maybe?
                    DaggerfallMessageBox chestChoicePopUp = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
                    chestChoicePopUp.SetText("What do you want to do with this chest?");
                    chestChoicePopUp.OnButtonClick += chestChoicePopUp_OnButtonClick;
                    chestChoicePopUp.AddButton(DaggerfallMessageBox.MessageBoxButtons.Copy); // Inspect
                    chestChoicePopUp.AddButton(DaggerfallMessageBox.MessageBoxButtons.Accept); // Pick-lock
                    chestChoicePopUp.AddButton(DaggerfallMessageBox.MessageBoxButtons.Cancel); // Cancel
                    chestChoicePopUp.Show(); // Will possibly have another button show if the chest has been inspected and confirmed to have traps that can be disarmed.
                    break;
                default:
                    break;
            }

            // Where most of heavy lifting will happen, once locked chest object has been clicked. Will check for being locked, trapped, already unlocked, what to do when and such.
        }

        public static bool HandleOpenEffect() // Bare basic behavior for the open effect, just for testing for now obviously.
        {
            // Check if player has Open effect running
            Open openEffect = (Open)GameManager.Instance.PlayerEffectManager.FindIncumbentEffect<Open>();
            if (openEffect == null)
                return false;

            if (ChestObjRef != null)
            {
                LLCObject closedChestData = ChestObjRef.GetComponent<LLCObject>();
                ItemCollection closedChestLoot = closedChestData.AttachedLoot;
                Transform closedChestTransform = ChestObjRef.transform;
                Vector3 pos = ChestObjRef.transform.position;

                DaggerfallLoot openChestLoot = GameObjectHelper.CreateLootContainer(LootContainerTypes.Nothing, InventoryContainerImages.Chest, pos, closedChestTransform.parent, 811, 0, closedChestData.LoadID, null, false);
                openChestLoot.gameObject.name = GameObjectHelper.GetGoFlatName(811, 0);
                openChestLoot.Items.TransferAll(closedChestLoot); // Transfers items from closed chest's items to the new open chest's item collection.

                Destroy(ChestObjRef); // Removed closed chest from scene, but saved its characteristics we care about for opened chest loot-pile.
                ChestObjRef = null;

                // Show success and play unlock sound
                DaggerfallUI.AddHUDText("The lock effortlessly unlatches through use of magic...", 4f);
                DaggerfallAudioSource dfAudioSource = GameManager.Instance.PlayerActivate.GetComponent<DaggerfallAudioSource>();
                if (dfAudioSource != null)
                    dfAudioSource.PlayOneShot(SoundClips.ActivateLockUnlock);
            }
            else
            {
                DaggerfallUI.AddHUDText("ERROR: Chest Was Found As Null.", 5f);
            }

            openEffect.CancelEffect();
            return true;
        }

        private static void chestChoicePopUp_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Copy) // Inspect
            {
                sender.CloseWindow();

                if (ChestObjRef != null)
                {
                    LLCObject chestData = ChestObjRef.GetComponent<LLCObject>();

                    TextFile.Token[] textToken = DaggerfallUnity.Instance.TextProvider.CreateTokens(TextFile.Formatting.JustifyCenter,
                    "This chest is made of: " + chestData.ChestMaterial.ToString(),
                    "Chest sturdiness is: " + chestData.ChestSturdiness,
                    "Chest magic resist is: " + chestData.ChestMagicResist,
                    "Its lock is made of: " + chestData.LockMaterial.ToString(),
                    "Lock sturdiness is: " + chestData.LockSturdiness,
                    "Lock magic resist is: " + chestData.LockMagicResist,
                    "Lock complexity is: " + chestData.LockComplexity,
                    "Lock jam resist is: " + chestData.JamResist);

                    DaggerfallMessageBox inspectChestPopup = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
                    inspectChestPopup.SetTextTokens(textToken); // Use a text-token here instead for the better debug stuff, better random encounters has good examples how, tomorrow.
                    inspectChestPopup.Show();
                    inspectChestPopup.ClickAnywhereToClose = true;
                }
                else
                {
                    DaggerfallUI.AddHUDText("ERROR: Chest Was Found As Null.", 5f);
                }
            }
            else if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Accept) // Pick-lock
            {
                sender.CloseWindow();

                if (ChestObjRef != null) // Oh yeah, don't forget to give skill XP as well for various things, failures and successes, etc.
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
                    else if (Dice100.SuccessRoll(LockPickChance(closedChestData))) // Guess the basic "success" stuff is already here for the time being, so I'll do more with that part later on.
                    {
                        DaggerfallLoot openChestLoot = GameObjectHelper.CreateLootContainer(LootContainerTypes.Nothing, InventoryContainerImages.Chest, pos, closedChestTransform.parent, 811, 0, closedChestData.LoadID, null, false);
                        openChestLoot.gameObject.name = GameObjectHelper.GetGoFlatName(811, 0);
                        openChestLoot.Items.TransferAll(closedChestLoot); // Transfers items from closed chest's items to the new open chest's item collection.

                        Destroy(ChestObjRef); // Removed closed chest from scene, but saved its characteristics we care about for opened chest loot-pile.
                        ChestObjRef = null;

                        // Show success and play unlock sound
                        DaggerfallUI.AddHUDText("The lock clicks open...", 4f);
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
                            DaggerfallUI.AddHUDText("You fail to pick the lock...", 4f);
                            if (dfAudioSource != null && !dfAudioSource.IsPlaying())
                                dfAudioSource.PlayOneShot(SoundClips.ActivateGears); // Will use custom sounds in the end most likely.
                        }
                    }
                }
                else
                {
                    DaggerfallUI.AddHUDText("ERROR: Chest Was Found As Null.", 5f);
                }

                /*DaggerfallMessageBox inspectChestPopup = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
                string[] message = { "You attempted picking the lock!" };
                inspectChestPopup.SetText(message);
                inspectChestPopup.Show();
                inspectChestPopup.ClickAnywhereToClose = true;*/
            }
            else // Cancel
            {
                sender.CloseWindow();
            }
        }

        // Consider adding these to their own script called like, "LLCFormulaHelper" or something.

        public static int LockPickChance(LLCObject chest) // Still not entirely happy with the current form here, but I think it's alot better than the first draft atleast, so fine for now, I think.
        {
            int lockComp = chest.LockComplexity;
            int lockP = Player.Skills.GetLiveSkillValue(DFCareer.Skills.Lockpicking);
            int pickP = Mathf.Clamp((int)Mathf.Round(Player.Skills.GetLiveSkillValue(DFCareer.Skills.Pickpocket) / 10), 0, 11);
            int intel = Player.Stats.LiveIntelligence - 50;
            int agili = Player.Stats.LiveAgility - 50;
            int speed = Player.Stats.LiveSpeed - 50;
            int luck = Player.Stats.LiveLuck - 50;
            float successChance = 0f;

            if (lockComp >= 0 && lockComp <= 19)
            {
                lockP = (int)Mathf.Ceil(lockP * 1.6f);
                successChance = lockComp + lockP + pickP + Mathf.Round(intel * .3f) + Mathf.Round(agili * 1.1f) + Mathf.Round(speed * .35f) + Mathf.Round(luck * .30f);
                return (int)Mathf.Round(Mathf.Clamp(successChance, 15f, 100f));
            }
            else if (lockComp >= 20 && lockComp <= 39)
            {
                lockP = (int)Mathf.Ceil(lockP * 1.7f);
                successChance = lockComp + lockP + pickP + Mathf.Round(intel * .4f) + Mathf.Round(agili * 1f) + Mathf.Round(speed * .30f) + Mathf.Round(luck * .30f);
                return (int)Mathf.Round(Mathf.Clamp(successChance, 10f, 100f));
            }
            else if (lockComp >= 40 && lockComp <= 59)
            {
                lockP = (int)Mathf.Ceil(lockP * 1.8f);
                successChance = lockComp + lockP + pickP + Mathf.Round(intel * .5f) + Mathf.Round(agili * .9f) + Mathf.Round(speed * .25f) + Mathf.Round(luck * .30f);
                return (int)Mathf.Round(Mathf.Clamp(successChance, 7f, 95f));
            }
            else if (lockComp >= 60 && lockComp <= 79)
            {
                lockP = (int)Mathf.Ceil(lockP * 1.9f);
                successChance = lockComp + lockP + pickP + Mathf.Round(intel * .6f) + Mathf.Round(agili * .8f) + Mathf.Round(speed * .20f) + Mathf.Round(luck * .30f);
                return (int)Mathf.Round(Mathf.Clamp(successChance, 3f, 90f));
            }
            else
            {
                lockP = (int)Mathf.Ceil(lockP * 2f);
                successChance = lockComp + lockP + pickP + Mathf.Round(intel * .7f) + Mathf.Round(agili * .7f) + Mathf.Round(speed * .15f) + Mathf.Round(luck * .30f);
                return (int)Mathf.Round(Mathf.Clamp(successChance, 1f, 80f)); // Potentially add specific text depending on initial odds, like "Through dumb-luck, you somehow unlocked it", etc.
            }
        }

        public static int LockJamChance(LLCObject chest) // Will have to test this out, but I think I'm fairly satisfied with the formula so far.
        {
            float jamResist = (float)chest.JamResist / 100f;
            float resistMod = (jamResist - 1f) * -1f;
            int luck = (int)Mathf.Round((Player.Stats.LiveLuck - 50) / 5f);

            float jamChance = (int)Mathf.Ceil(chest.PicksAttempted * (UnityEngine.Random.Range(14, 26) - luck) * resistMod);
            return (int)Mathf.Round(Mathf.Clamp(jamChance, 5f, 95f));
        }

        public static bool BashLockRoll(LLCObject chest, DaggerfallUnityItem weapon) // Roll to determine if a bash attempt hit the lock or the chest body instead.
        {
            int wepSkill = Player.Skills.GetLiveSkillValue((weapon != null) ? weapon.GetWeaponSkillIDAsShort() : (short)DFCareer.Skills.HandToHand); // Might add accuracy modifier for weapon type, but for now just keep it more simple.
            int willp = Player.Stats.LiveWillpower - 50; // I sort of see willpower as a very generic sort of thing like a more vague "Perception" attribute in some ways. So for now just include it here.
            int agili = Player.Stats.LiveAgility - 50;
            int speed = Player.Stats.LiveSpeed - 50;
            int luck = Player.Stats.LiveLuck - 50;

            float accuracyCheck = Mathf.Round(wepSkill / 4) + Mathf.Round(willp * .2f) + Mathf.Round(agili * .5f) + Mathf.Round(speed * .1f) + Mathf.Round(luck * .3f);

            if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(accuracyCheck, 5f, 70f))))
                return true;
            else
                return false;
        }

        public static bool LockHardBashRoll(LLCObject chest, DaggerfallUnityItem weapon) // Roll to determine if bashed lock was hit hard or lightly, I.E. Effectively or not very effectively.
        {
            int stren = Player.Stats.LiveStrength - 50;
            int luck = Player.Stats.LiveLuck - 50;
            int lockMat = lockMaterialToDaggerfallValue(chest.LockMaterial);
            int weapMat = (weapon != null) ? weapon.NativeMaterialValue : -1;
            int matDiff = weapMat - lockMat;
            int wepSkillID = (weapon != null) ? weapon.GetWeaponSkillIDAsShort() : (short)DFCareer.Skills.HandToHand;
            float hardBashChance = 0f;
            bool hardBash = false;

            // I think I'll work on the other "logic/work" parts of this particular action at some other point, so WIP here.
            // Not sure if I'll do the various other logic outside of the rolls in this method or just have this roll the single bool, then do that other stuff later,
            // possibly making this do an "out" return values for the other things, but will see, for stuff like damaging health, fatigue, weapon durability, other stuff and such, etc.

            if (wepSkillID == (int)DFCareer.Skills.HandToHand) // Checks if the "weapon" being used is the player's fists.
            {
                if (lockMat < 0) // If lock is made of wood
                {
                    hardBashChance = Mathf.Round(stren + (int)Mathf.Round(luck / 5f) + 30f);

                    if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(hardBashChance, 5f, 85f))))
                        hardBash = true;

                    ApplyBashingCostLogic(chest, weapon, true, true, hardBash, matDiff);

                    return hardBash;
                }
                else // If lock is made from any metal
                {
                    hardBashChance = Mathf.Round(stren + (int)Mathf.Round(luck / 5f) + 5f);

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
                    hardBashChance = Mathf.Round((int)Mathf.Round(Mathf.Clamp(weapon.weightInKg - 2f, 0.75f, 20f) / (0.6f - (stren / 100f))) + (int)Mathf.Round(luck / 5f) + 40f);

                    if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(hardBashChance, 10f, 100f))))
                        hardBash = true;

                    ApplyBashingCostLogic(chest, weapon, true, true, hardBash, matDiff);

                    return hardBash;
                }
                else // If lock is made from any metal
                {
                    hardBashChance = Mathf.Round((int)Mathf.Round(Mathf.Clamp(weapon.weightInKg - 2f, 0.75f, 20f) / (0.6f - (stren / 100f))) + (int)Mathf.Round(luck / 5f) + 5f);

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
                    hardBashChance = Mathf.Round(((matDiff + (int)Mathf.Round(stren / 10f)) * 10f) + (int)Mathf.Round(luck / 5f) + 40f); // Just for now, will have to see through testing if this needs work.

                    if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(hardBashChance, 10f, 100f))))
                        hardBash = true;

                    ApplyBashingCostLogic(chest, weapon, true, true, hardBash, matDiff);

                    return hardBash;
                }
                else // If lock is made from any metal
                {
                    hardBashChance = Mathf.Round(((matDiff + (int)Mathf.Round(stren / 10f)) * 10f) + (int)Mathf.Round(luck / 5f) + 30f); // Just for now, will have to see through testing if this needs work.

                    if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(hardBashChance, 2f, 97f))))
                        hardBash = true;

                    ApplyBashingCostLogic(chest, weapon, true, false, hardBash, matDiff);

                    return hardBash;
                }
            }
        }

        public static bool BreakLockRoll(LLCObject chest, DaggerfallUnityItem weapon) // Roll to determine if bashed lock breaks or not this attempt, granting full access to loot.
        {
            int luck = Player.Stats.LiveLuck - 50;
            int lockMat = lockMaterialToDaggerfallValue(chest.LockMaterial);
            float bashResist = (float)chest.LockSturdiness / 100f;
            float stabilityMod = (bashResist - 1f) * -1f; // Modifier value might need some work, especially for the very sturdy lock materials, but will see with testing later, etc.
            float lockBreakChance = 0f;

            // This if-statement is meant to simulate a sort of "decisive strike", that if you got a good hit in on the lock the first attempt only, there is much higher odds of the lock
            // just breaking off in one go right there. But any times after that, or if other attempts were already made on the chest it's just the normal odds from there on.
            if (chest.LockBashedHardTimes == 1 && chest.ChestBashedHardTimes == 0 && chest.ChestBashedLightTimes == 0 && chest.LockBashedLightTimes == 0)
            {
                lockBreakChance = Mathf.Round((int)Mathf.Round(luck / 5f) + 45f);

                if (Dice100.SuccessRoll((int)lockBreakChance))
                    return true;
                else
                    return false;
            }

            if (lockMat < 0) // If lock is made of wood
            {
                lockBreakChance = (int)Mathf.Round((chest.LockBashedHardTimes * 20) + (chest.LockBashedLightTimes * 4) * stabilityMod) + (int)Mathf.Round(luck / 5f);

                if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(lockBreakChance, 1f, 95f))))
                    return true;
                else
                    return false;
            }
            else // If lock is made from any metal
            {
                lockBreakChance = (int)Mathf.Round((chest.LockBashedHardTimes * 15) + (chest.LockBashedLightTimes * 3) * stabilityMod) + (int)Mathf.Round(luck / 5f);

                if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(lockBreakChance, 1f, 80f))))
                    return true;
                else
                    return false;
            }
        }

        public static bool ChestHardBashRoll(LLCObject chest, DaggerfallUnityItem weapon) // Roll to determine if bashed chest body was hit hard or lightly, I.E. Effectively or not very effectively.
        {
            int stren = Player.Stats.LiveStrength - 50;
            int luck = Player.Stats.LiveLuck - 50;
            int chestMat = chestMaterialToDaggerfallValue(chest.ChestMaterial);
            int weapMat = (weapon != null) ? weapon.NativeMaterialValue : -1;
            int matDiff = weapMat - chestMat;
            int wepSkillID = (weapon != null) ? weapon.GetWeaponSkillIDAsShort() : (short)DFCareer.Skills.HandToHand;
            float hardBashChance = 0f;
            bool hardBash = false;

            // I think I'll work on the other "logic/work" parts of this particular action at some other point, so WIP here.
            // Not sure if I'll do the various other logic outside of the rolls in this method or just have this roll the single bool, then do that other stuff later,
            // possibly making this do an "out" return values for the other things, but will see, for stuff like damaging health, fatigue, weapon durability, other stuff and such, etc.

            if (wepSkillID == (int)DFCareer.Skills.HandToHand) // Checks if the "weapon" being used is the player's fists.
            {
                if (chestMat < 0) // If chest is made of wood
                {
                    hardBashChance = Mathf.Round(stren + (int)Mathf.Round(luck / 5f) + 20f);

                    if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(hardBashChance, 3f, 70f))))
                        hardBash = true;

                    ApplyBashingCostLogic(chest, weapon, false, true, hardBash, matDiff);

                    return hardBash;
                }
                else // If chest is made from any metal
                {
                    hardBashChance = Mathf.Round(stren + (int)Mathf.Round(luck / 5f) + 3f);

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
                    hardBashChance = Mathf.Round((int)Mathf.Round(Mathf.Clamp(weapon.weightInKg - 2f, 0.75f, 20f) / (0.6f - (stren / 100f))) + (int)Mathf.Round(luck / 5f) + 30f);

                    if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(hardBashChance, 6f, 96f))))
                        hardBash = true;

                    ApplyBashingCostLogic(chest, weapon, false, true, hardBash, matDiff);

                    return hardBash;
                }
                else // If chest is made from any metal
                {
                    hardBashChance = Mathf.Round((int)Mathf.Round(Mathf.Clamp(weapon.weightInKg - 2f, 0.75f, 20f) / (0.6f - (stren / 100f))) + (int)Mathf.Round(luck / 5f) + 3f);

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
                    hardBashChance = Mathf.Round(((matDiff + (int)Mathf.Round(stren / 10f)) * 10f) + (int)Mathf.Round(luck / 5f) + 30f); // Just for now, will have to see through testing if this needs work.

                    if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(hardBashChance, 6f, 96f))))
                        hardBash = true;

                    ApplyBashingCostLogic(chest, weapon, false, true, hardBash, matDiff);

                    return hardBash;
                }
                else // If chest is made from any metal
                {
                    hardBashChance = Mathf.Round(((matDiff + (int)Mathf.Round(stren / 10f)) * 10f) + (int)Mathf.Round(luck / 5f) + 20f); // Just for now, will have to see through testing if this needs work.

                    if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(hardBashChance, 1f, 90f))))
                        hardBash = true;

                    ApplyBashingCostLogic(chest, weapon, false, false, hardBash, matDiff);

                    return hardBash;
                }
            }
        }

        public static bool SmashOpenChestRoll(LLCObject chest, DaggerfallUnityItem weapon) // Roll to determine if bashed chest body breaks open or not this attempt, granting full access to loot.
        {
            int luck = Player.Stats.LiveLuck - 50;
            int chestMat = chestMaterialToDaggerfallValue(chest.ChestMaterial);
            float bashResist = (float)chest.ChestSturdiness / 100f;
            float stabilityMod = (bashResist - 1f) * -1f; // Modifier value might need some work, especially for the very sturdy chest materials, but will see with testing later, etc.
            float chestSmashOpenChance = 0f;

            // This if-statement is meant to simulate a sort of "decisive strike", that if you got a good hit in on the chest the first attempt only, there is much higher odds of the chest
            // just breaking open in one go right there. But any times after that, or if other attempts were already made on the chest it's just the normal odds from there on.
            // Will have to see through testing and feedback if this should work like this for hitting the chest body, similar to the lock and such.
            if (chest.ChestBashedHardTimes == 1 && chest.LockBashedHardTimes == 0 && chest.LockBashedLightTimes == 0 && chest.ChestBashedLightTimes == 0)
            {
                chestSmashOpenChance = Mathf.Round((int)Mathf.Round(luck / 5f) + 15f);

                if (Dice100.SuccessRoll((int)chestSmashOpenChance))
                    return true;
                else
                    return false;
            }

            if (chestMat < 0) // If chest is made of wood
            {
                chestSmashOpenChance = (int)Mathf.Round((chest.ChestBashedHardTimes * 10) + (chest.ChestBashedLightTimes * 2) * stabilityMod) + (int)Mathf.Round(luck / 5f);

                if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(chestSmashOpenChance, 0f, 85f))))
                    return true;
                else
                    return false;
            }
            else // If chest is made from any metal
            {
                chestSmashOpenChance = (int)Mathf.Round((chest.ChestBashedHardTimes * 5) + (chest.ChestBashedLightTimes * 1) * stabilityMod) + (int)Mathf.Round(luck / 5f);

                if (Dice100.SuccessRoll((int)Mathf.Round(Mathf.Clamp(chestSmashOpenChance, 0f, 70f))))
                    return true;
                else
                    return false;
            }
        }

        public static void ApplyBashingCostLogic(LLCObject chest, DaggerfallUnityItem weapon, bool hitLock = false, bool hitWood = false, bool hardBash = false, int matDiff = -100) // Applies vitals damage to player and the weapon used during bash attempt.
        {
            // Will have to consider later when doing the stuff for "bashing" with the bow, I.E. shooting arrows at a chest, and how to deal with that stuff, possibly different method entirely.
            int stren = Player.Stats.LiveStrength - 50;
            int willp = Player.Stats.LiveWillpower - 50;
            int endur = Player.Stats.LiveEndurance - 50;
            int luck = Player.Stats.LiveLuck - 50;
            int wepSkillID = (weapon != null) ? weapon.GetWeaponSkillIDAsShort() : (short)DFCareer.Skills.HandToHand;
            int wepSkill = Player.Skills.GetLiveSkillValue(wepSkillID); // Have not used this value yet, nor "matDiff" will see if I eventually do or not later.
            float bashHitMod = 1f;
            float healthDam = 0f;
            float fatigueDam = 0f;

            if (chest == null)
                return;

            BashingDamageLootContents(chest, weapon, wepSkillID, hitLock, hitWood, hardBash, matDiff);

            if (hitLock && hardBash) { bashHitMod = 0.6f; } // Math was confusing me on how to combine these values together for some reason, so just doing this weird if-else chain here instead, for now.
            else if (hitLock && !hardBash) { bashHitMod = 0.25f; }
            else if (!hitLock && hardBash) { bashHitMod = 1f; }
            else { bashHitMod = 0.4f; }

            if (weapon == null && wepSkillID == (short)DFCareer.Skills.HandToHand) // If player is bashing chest with their bare fists
            {
                if (hitWood) // Hitting either wooden lock or chest body with bare fists
                {
                    float rolledHpPercent = Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.Round(5 + (int)Mathf.Round(endur / -10f) + (int)Mathf.Round(willp / -25f) * bashHitMod)));
                    healthDam = Mathf.Max(1, (int)Mathf.Floor(Player.MaxHealth * (rolledHpPercent / 100f)));
                    healthDam = (Player.CurrentHealth - healthDam < Mathf.Ceil(Player.MaxHealth * 0.4f)) ? Mathf.Max(0, Player.CurrentHealth - (Player.MaxHealth * 0.4f)) : healthDam;

                    if (healthDam > 0)
                    {
                        float rolledFatiguePercent = Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.Round(5 + (int)Mathf.Round(endur / -10f) + (int)Mathf.Round(willp / -25f) * bashHitMod)));
                        fatigueDam = Mathf.Max((int)Mathf.Floor(Player.MaxFatigue * 0.01f), (int)Mathf.Floor(Player.MaxFatigue * (rolledFatiguePercent / 100f)));
                        fatigueDam = (Player.CurrentFatigue - fatigueDam < 0) ? Player.CurrentFatigue : fatigueDam;

                        Player.DecreaseHealth((int)healthDam); // Currently could be abused by something like the "Health Regen" trait, but whatever for now.
                        Player.DecreaseFatigue((int)fatigueDam, true); // Will need to do testing to make sure this works with the fatigue multiplier, including above formulas and such, bit confusing.
                    }
                    else // Since player health is currently at "minimum" level that bashing with fists can reduce it, health is not damaged, but instead fatigue damage is multiplied alot as a cost.
                    {
                        float rolledFatiguePercent = Mathf.Max(3, UnityEngine.Random.Range(3, Mathf.Round(16 + (int)Mathf.Round(endur / -10f) + (int)Mathf.Round(willp / -25f) * bashHitMod)));
                        fatigueDam = Mathf.Max((int)Mathf.Floor(Player.MaxFatigue * 0.03f), (int)Mathf.Floor(Player.MaxFatigue * (rolledFatiguePercent / 100f)));
                        fatigueDam = (Player.CurrentFatigue - fatigueDam < 0) ? Player.CurrentFatigue : fatigueDam;

                        Player.DecreaseFatigue((int)fatigueDam, true); // Will need to do testing to make sure this works with the fatigue multiplier, including above formulas and such, bit confusing.
                    }
                }
                else // Hitting either metal lock or chest body with bare fists
                {
                    float rolledHpPercent = Mathf.Max(3, UnityEngine.Random.Range(3, Mathf.Round(11 + (int)Mathf.Round(endur / -10f) + (int)Mathf.Round(willp / -25f) * bashHitMod)));
                    healthDam = Mathf.Max(1, (int)Mathf.Floor(Player.MaxHealth * (rolledHpPercent / 100f)));
                    healthDam = (Player.CurrentHealth - healthDam < Mathf.Ceil(Player.MaxHealth * 0.4f)) ? Mathf.Max(0, Player.CurrentHealth - (Player.MaxHealth * 0.4f)) : healthDam;

                    if (healthDam > 0)
                    {
                        float rolledFatiguePercent = Mathf.Max(3, UnityEngine.Random.Range(3, Mathf.Round(11 + (int)Mathf.Round(endur / -10f) + (int)Mathf.Round(willp / -25f) * bashHitMod)));
                        fatigueDam = Mathf.Max((int)Mathf.Floor(Player.MaxFatigue * 0.03f), (int)Mathf.Floor(Player.MaxFatigue * (rolledFatiguePercent / 100f)));
                        fatigueDam = (Player.CurrentFatigue - fatigueDam < 0) ? Player.CurrentFatigue : fatigueDam;

                        Player.DecreaseHealth((int)healthDam); // Currently could be abused by something like the "Health Regen" trait, but whatever for now.
                        Player.DecreaseFatigue((int)fatigueDam, true); // Will need to do testing to make sure this works with the fatigue multiplier, including above formulas and such, bit confusing.
                    }
                    else // Since player health is currently at "minimum" level that bashing with fists can reduce it, health is not damaged, but instead fatigue damage is multiplied alot as a cost.
                    {
                        float rolledFatiguePercent = Mathf.Max(7, UnityEngine.Random.Range(7, Mathf.Round(25 + (int)Mathf.Round(endur / -10f) + (int)Mathf.Round(willp / -25f) * bashHitMod)));
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
                    float rolledWepDamPercent = Mathf.Max(1, UnityEngine.Random.Range(1, 4 + (int)Mathf.Round((stren / 10f) * 0.4f) + ((Dice100.SuccessRoll(50 + (int)Mathf.Round(luck / 2f))) ? -1 : 0))); // Won't include "bashHitMod" for now, since this is already getting messy and confusing as it is, do testing before worrying.
                    float rolledFatiguePercent = Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.Round(3 + (int)Mathf.Round(endur / -10f) + (int)Mathf.Round(willp / -25f) * bashHitMod)));
                    fatigueDam = Mathf.Max((int)Mathf.Floor(Player.MaxFatigue * 0.01f), (int)Mathf.Floor(Player.MaxFatigue * (rolledFatiguePercent / 100f)));
                    fatigueDam = (Player.CurrentFatigue - fatigueDam < 0) ? Player.CurrentFatigue : fatigueDam;

                    Player.DecreaseFatigue((int)fatigueDam, true); // Will need to do testing to make sure this works with the fatigue multiplier, including above formulas and such, bit confusing.
                    weapon.LowerCondition((int)Mathf.Max(1, Mathf.Round(weapon.maxCondition * (rolledWepDamPercent / 100f))), Player, Player.Items);
                }
                else // Hitting either metal lock or chest body with a blunt type weapon
                {
                    float rolledWepDamPercent = Mathf.Max(3, UnityEngine.Random.Range(3, 6 + (int)Mathf.Round((stren / 10f) * 0.4f) + ((Dice100.SuccessRoll(50 + (int)Mathf.Round(luck / 2f))) ? -1 : 0))); // Won't include "bashHitMod" for now, since this is already getting messy and confusing as it is, do testing before worrying.
                    float rolledFatiguePercent = Mathf.Max(4, UnityEngine.Random.Range(4, Mathf.Round(10 + (int)Mathf.Round(endur / -10f) + (int)Mathf.Round(willp / -25f) * bashHitMod)));
                    fatigueDam = Mathf.Max((int)Mathf.Floor(Player.MaxFatigue * 0.04f), (int)Mathf.Floor(Player.MaxFatigue * (rolledFatiguePercent / 100f)));
                    fatigueDam = (Player.CurrentFatigue - fatigueDam < 0) ? Player.CurrentFatigue : fatigueDam;

                    Player.DecreaseFatigue((int)fatigueDam, true); // Will need to do testing to make sure this works with the fatigue multiplier, including above formulas and such, bit confusing.
                    weapon.LowerCondition((int)Mathf.Max(1, Mathf.Round(weapon.maxCondition * (rolledWepDamPercent / 100f))), Player, Player.Items);
                }
            }
            else if (weapon != null) // If player is bashing chest with any other types of weapons (in this case all the bladed ones)
            {
                if (hitWood) // Hitting either wooden lock or chest body with a bladed type weapon
                {
                    float rolledWepDamPercent = Mathf.Max(1, UnityEngine.Random.Range(1, 4 + (int)Mathf.Round((stren / 10f) * 0.2f) + ((Dice100.SuccessRoll(50 + (int)Mathf.Round(luck / 2f))) ? -1 : 0))); // Won't include "bashHitMod" for now, since this is already getting messy and confusing as it is, do testing before worrying.
                    rolledWepDamPercent = (!hitLock) ? Mathf.Round(rolledWepDamPercent * (1 + chest.ChestSturdiness / 100f)) : Mathf.Round(rolledWepDamPercent * (1 + chest.LockSturdiness / 300f));
                    float rolledFatiguePercent = Mathf.Max(1, UnityEngine.Random.Range(1, Mathf.Round(3 + (int)Mathf.Round(endur / -10f) + (int)Mathf.Round(willp / -25f) * bashHitMod)));
                    fatigueDam = Mathf.Max((int)Mathf.Floor(Player.MaxFatigue * 0.01f), (int)Mathf.Floor(Player.MaxFatigue * (rolledFatiguePercent / 100f)));
                    fatigueDam = (Player.CurrentFatigue - fatigueDam < 0) ? Player.CurrentFatigue : fatigueDam;

                    Player.DecreaseFatigue((int)fatigueDam, true); // Will need to do testing to make sure this works with the fatigue multiplier, including above formulas and such, bit confusing.
                    weapon.LowerCondition((int)Mathf.Max(1, Mathf.Round(weapon.maxCondition * (rolledWepDamPercent / 100f))), Player, Player.Items);
                }
                else // Hitting either metal lock or chest body with a bladed type weapon
                {
                    float rolledWepDamPercent = Mathf.Max(3, UnityEngine.Random.Range(3, 6 + (int)Mathf.Round((stren / 10f) * 0.2f) + ((Dice100.SuccessRoll(50 + (int)Mathf.Round(luck / 2f))) ? -1 : 0))); // Won't include "bashHitMod" for now, since this is already getting messy and confusing as it is, do testing before worrying.
                    rolledWepDamPercent = (!hitLock) ? Mathf.Round(rolledWepDamPercent * (1 + chest.ChestSturdiness / 100f)) : Mathf.Round(rolledWepDamPercent * (1 + chest.LockSturdiness / 300f));
                    float rolledFatiguePercent = Mathf.Max(2, UnityEngine.Random.Range(2, Mathf.Round(8 + (int)Mathf.Round(endur / -10f) + (int)Mathf.Round(willp / -25f) * bashHitMod)));
                    fatigueDam = Mathf.Max((int)Mathf.Floor(Player.MaxFatigue * 0.02f), (int)Mathf.Floor(Player.MaxFatigue * (rolledFatiguePercent / 100f)));
                    fatigueDam = (Player.CurrentFatigue - fatigueDam < 0) ? Player.CurrentFatigue : fatigueDam;

                    Player.DecreaseFatigue((int)fatigueDam, true); // Will need to do testing to make sure this works with the fatigue multiplier, including above formulas and such, bit confusing.
                    weapon.LowerCondition((int)Mathf.Max(1, Mathf.Round(weapon.maxCondition * (rolledWepDamPercent / 100f))), Player, Player.Items);
                }
            }
            else // Catch any other edge cases here
            {
                return;
            }
        }

        public static void BashingDamageLootContents(LLCObject chest, DaggerfallUnityItem weapon, int wepSkillID, bool hitLock, bool hitWood, bool hardBash, int matDiff)
        {
            int luck = Player.Stats.LiveLuck - 50;
            int initialItemCount = chest.AttachedLoot.Count;

            if (weapon == null && wepSkillID == (short)DFCareer.Skills.HandToHand) // If player is bashing chest with their bare fists
            {
                if (hitLock)
                {
                    if (hardBash)
                    {
                        for (int i = 0; i < initialItemCount; i++)
                        {
                            DaggerfallUnityItem item = chest.AttachedLoot.GetItem(i); // Hmm, this will almost definitely cause an "Index out of range" error, so I'll have to consider how to solve that issue.
                            LootItemSturdiness itemStab = DetermineLootItemSturdiness(item);

                            if (!item.IsQuestItem)
                            {
                                if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(20 + (int)Mathf.Round(luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item)) { i--; continue; }
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
                            DaggerfallUnityItem item = chest.AttachedLoot.GetItem(i); // Hmm, this will almost definitely cause an "Index out of range" error, so I'll have to consider how to solve that issue.
                            LootItemSturdiness itemStab = DetermineLootItemSturdiness(item);

                            if (!item.IsQuestItem)
                            {
                                if (itemStab == LootItemSturdiness.Very_Fragile && Dice100.SuccessRoll(45 + (int)Mathf.Round(luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Fragile && Dice100.SuccessRoll(20 + (int)Mathf.Round(luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item)) { i--; continue; }
                                }
                                else if (itemStab == LootItemSturdiness.Solid && Dice100.SuccessRoll(5 + (int)Mathf.Round(luck / -5f)))
                                {
                                    if (HandleDestroyingLootItem(chest, item)) { i--; continue; }
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
            else if (weapon != null && wepSkillID == (short)DFCareer.Skills.BluntWeapon) // If player is bashing chest with a blunt type weapon
            {
                if (hitLock)
                {

                }
                else
                {

                }
            }
            else if (weapon != null) // If player is bashing chest with any other types of weapons (in this case all the bladed ones)
            {
                if (hitLock)
                {

                }
                else
                {

                }
            }
            else
            {
                return;
            }

            // Next I work on this add variables I will need like chest object and other stuff to properly go through chest loot contents and do stuff to all of it depending on various factors, etc.

            // Guess will just have this here for now, might have bashing the lock only potentially damage certain things like potions and delicate stuff?
            // Guess will just have this here for now, likely most loot damage will be when hitting the "body" of the chest like in this case, will see.

            // Work on the condition damage stuff for the actual content of the chest from bashing and such. Probably will do the other logic for damaging player first though, etc.
            // Idea being if the lock is broken open you are given access with relatively little penalty to the chest contents, but still effected by how many times the chest was hit previously.
            // However, breaking the chest body open will cause significantly more damage to the contents than breaking the lock off instead, generally.
        }

        public static bool HandleDestroyingLootItem(LLCObject chest, DaggerfallUnityItem item) // Handles most of the "work" part of breaking/destroying loot items, removing the item and adding the respective "waste" item in its place.
        {
            if (chest == null || item == null)
                return false;

            if ((item.ItemGroup == ItemGroups.Weapons && item.TemplateIndex != (int)Weapons.Arrow) || item.ItemGroup == ItemGroups.Armor)
            {

            }
            else if (item.ItemGroup == ItemGroups.MensClothing || item.ItemGroup == ItemGroups.WomensClothing)
            {
                float conditionMod = (float)UnityEngine.Random.Range(5, 16) / 100f;

                item.currentCondition = (int)(item.maxCondition * conditionMod); // Check math first here to make sure.
                // Continue work on this next time, so certain items take durability damage instead of being destroyed outright, but have to add the extra logic for that here.
            }

            if (item.IsAStack())
            {
                int breakNum = UnityEngine.Random.Range(1, item.stackCount + 1);
                DaggerfallUnityItem wasteItem = DetermineDestroyedLootRefuseType(item);
                wasteItem.stackCount = breakNum;
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
                DaggerfallUnityItem wasteItem = DetermineDestroyedLootRefuseType(item);
                chest.AttachedLoot.AddItem(wasteItem);
                chest.AttachedLoot.RemoveItem(item);

                return true;
            }
        }

        public static LootItemSturdiness DetermineLootItemSturdiness(DaggerfallUnityItem item)
        {
            if (item.TemplateIndex >= 0 && item.TemplateIndex <= 511) // Vanilla Item Templates
            {
                switch(item.ItemGroup)
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

        public static DaggerfallUnityItem DetermineDestroyedLootRefuseType(DaggerfallUnityItem item) // Obviously need to add these custom "Junk Items" in at some point.
        {
            if (item.TemplateIndex >= 0 && item.TemplateIndex <= 511) // Vanilla Item Templates
            {
                switch (item.ItemGroup)
                {
                    case ItemGroups.UselessItems1:
                        if (item.IsPotion) { return ItemBuilder.CreateItem(ItemGroups.UselessItems1, 31111); } // "Glass Fragments" custom item
                        else { return ItemBuilder.CreateItem(ItemGroups.UselessItems1, 91111); }
                    case ItemGroups.Armor:
                        return ItemBuilder.CreateItem(ItemGroups.UselessItems1, 11119); // "(Material) Scraps" custom item
                    case ItemGroups.Weapons:
                        if (item.TemplateIndex == (int)Weapons.Arrow) { return ItemBuilder.CreateItem(ItemGroups.UselessItems1, 42329); } // "Broken Arrow" custom item
                        else { return ItemBuilder.CreateItem(ItemGroups.UselessItems1, 11119); }
                    case ItemGroups.MensClothing:
                    case ItemGroups.WomensClothing:
                        return ItemBuilder.CreateItem(ItemGroups.UselessItems1, 11113); // "Tattered Cloth" custom item
                    case ItemGroups.Books:
                    case ItemGroups.Maps:
                        return ItemBuilder.CreateItem(ItemGroups.UselessItems1, 33333); // "Paper Shreds" custom item
                    case ItemGroups.UselessItems2:
                        if (item.TemplateIndex == (int)UselessItems2.Oil || item.TemplateIndex == (int)UselessItems2.Lantern) { return ItemBuilder.CreateItem(ItemGroups.UselessItems1, 31111); }
                        else if (item.TemplateIndex == (int)UselessItems2.Bandage) { return ItemBuilder.CreateItem(ItemGroups.UselessItems1, 11113); }
                        else if (item.TemplateIndex == (int)UselessItems2.Parchment) { return ItemBuilder.CreateItem(ItemGroups.UselessItems1, 33333); }
                        else { return ItemBuilder.CreateItem(ItemGroups.UselessItems1, 91111); }
                    case ItemGroups.Gems:
                    case ItemGroups.MiscellaneousIngredients2:
                    case ItemGroups.MetalIngredients:
                        if (item.TemplateIndex == (int)MiscellaneousIngredients2.Ivory) { return ItemBuilder.CreateItem(ItemGroups.UselessItems1, 71111); } // "Ivory Fragments" custom item
                        else { return ItemBuilder.CreateItem(ItemGroups.UselessItems1, 32323); } // "Shiny Rubble" custom item
                    case ItemGroups.Drugs:
                    case ItemGroups.PlantIngredients1:
                    case ItemGroups.PlantIngredients2:
                        return ItemBuilder.CreateItem(ItemGroups.UselessItems1, 42424); // "Clump of Plant Matter" custom item
                    case ItemGroups.MiscellaneousIngredients1:
                        if (item.TemplateIndex >= 59 && item.TemplateIndex <= 64) { return ItemBuilder.CreateItem(ItemGroups.UselessItems1, 31111); }
                        else { return ItemBuilder.CreateItem(ItemGroups.UselessItems1, 71111); }
                    case ItemGroups.CreatureIngredients1:
                    case ItemGroups.CreatureIngredients2:
                        return ItemBuilder.CreateItem(ItemGroups.UselessItems1, 11117); // "Glob of Gore" custom item
                    case ItemGroups.CreatureIngredients3:
                        if (item.TemplateIndex == (int)CreatureIngredients3.Nymph_hair) { return ItemBuilder.CreateItem(ItemGroups.UselessItems1, 11117); }
                        else { return ItemBuilder.CreateItem(ItemGroups.UselessItems1, 71111); }
                    case ItemGroups.Jewellery:
                        return ItemBuilder.CreateItem(ItemGroups.UselessItems1, 24242); // "Destroyed Jewelry" custom item
                    case ItemGroups.MiscItems:
                        if (item.TemplateIndex == (int)MiscItems.Soul_trap) { return ItemBuilder.CreateItem(ItemGroups.UselessItems1, 32323); }
                        else if (item.TemplateIndex == (int)MiscItems.Letter_of_credit || item.TemplateIndex == (int)MiscItems.Potion_recipe || item.TemplateIndex == (int)MiscItems.Map) { return ItemBuilder.CreateItem(ItemGroups.UselessItems1, 33333); }
                        else { return ItemBuilder.CreateItem(ItemGroups.UselessItems1, 91111); }
                    case ItemGroups.Currency:
                        return ItemBuilder.CreateItem(ItemGroups.UselessItems1, 91191); // "Ruined Coins" custom item
                    default:
                        return ItemBuilder.CreateItem(ItemGroups.UselessItems1, 91111); // "Useless Refuse" custom item
                }
            }
            else // Modded Item Templates
            {
                return ItemBuilder.CreateItem(ItemGroups.UselessItems1, 91111); // "Useless Refuse" custom item?
            }
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

        // Consider adding these to their own script called like, "LLCFormulaHelper" or something.

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
