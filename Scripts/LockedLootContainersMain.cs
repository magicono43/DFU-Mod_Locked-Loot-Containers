// Project:         LockedLootContainers mod for Daggerfall Unity (http://www.dfworkshop.net)
// Copyright:       Copyright (C) 2022 Kirk.O
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Author:          Kirk.O
// Created On: 	    9/8/2022, 11:00 PM
// Last Edit:		2/13/2023, 12:45 AM
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
using DaggerfallConnect.Arena2;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Utility;
using Wenzil.Console;
using System;

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

            RegisterLLCCommands(); // For testing custom windows

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

            if (WepManager.ScreenWeapon.GetCurrentFrame() == WepManager.ScreenWeapon.GetHitFrame()) // May be acting strange, will need to troubleshoot and bug fix next time.
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

        // For Testing Custom Windows

        public static void RegisterLLCCommands()
        {
            Debug.Log("[LockLootContainers] Trying to register console commands.");
            try
            {
                ConsoleCommandsDatabase.RegisterCommand(ChangeButtonRect.command, ChangeButtonRect.description, ChangeButtonRect.usage, ChangeButtonRect.Execute);
                ConsoleCommandsDatabase.RegisterCommand(TeleportToRandomChest.command, TeleportToRandomChest.description, TeleportToRandomChest.usage, TeleportToRandomChest.Execute);
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Error Registering LockLootContainers Console commands: {0}", e.Message));
            }
        }

        private static class ChangeButtonRect
        {
            public static readonly string command = "butt";
            public static readonly string description = "Changes the dimensions of this GUI button.";
            public static readonly string usage = "butt [butt#] [x] [y] [w] [h]";

            public static string Execute(params string[] args)
            {
                if (args.Length < 5 || args.Length > 5) return "Invalid entry, see usage notes.";

                if (!int.TryParse(args[0], out int buttNum))
                    return string.Format("`{0}` is not a number, please use a number for [butt#].", args[0]);
                if (!int.TryParse(args[1], out int x))
                    return string.Format("`{0}` is not a number, please use a number for [x].", args[1]);
                if (!int.TryParse(args[2], out int y))
                    return string.Format("`{0}` is not a number, please use a number for [y].", args[2]);
                if (!int.TryParse(args[3], out int w))
                    return string.Format("`{0}` is not a number, please use a number for [w].", args[3]);
                if (!int.TryParse(args[4], out int h))
                    return string.Format("`{0}` is not a number, please use a number for [h].", args[4]);

                if (buttNum == 1)
                    InspectionInfoWindow.butt1 = new Rect(x, y, w, h);
                else if (buttNum == 2)
                    InspectionInfoWindow.butt2 = new Rect(x, y, w, h);
                else if (buttNum == 3)
                    InspectionInfoWindow.butt3 = new Rect(x, y, w, h);
                else
                    return "Error: Something went wrong.";
                return string.Format("Button {0} Rect Adjusted.", buttNum);
            }
        }

        private static class TeleportToRandomChest // Just did some testing, and besides sometimes being dropped into the void, appears to work perfectly fine for testing purposes.
        {
            public static readonly string command = "tele2chest";
            public static readonly string description = "Chooses random chest object from current scene and teleports player to its location.";
            public static readonly string usage = "tele2chest";

            public static string Execute(params string[] args)
            {
                LLCObject[] chests = FindObjectsOfType<LLCObject>();
                if (chests.Length - 1 <= 0) // I think it's detecting the actual LLCObject that is the base of them all but not an actual chest? No big deal either way.
                    return "Error: No chests in scene.";

                LLCObject chest = chests[UnityEngine.Random.Range(0, chests.Length)];
                if (chest == null)
                    return "Error: Something went wrong.";

                Vector3 chestPos = chest.transform.position;

                if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon)
                {
                    GameManager.Instance.PlayerObject.transform.localPosition = chestPos;
                }
                else if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideBuilding)
                {
                    GameManager.Instance.PlayerObject.transform.position = chestPos;
                }
                else
                {
                    return "Error: Something went wrong.";
                }
                GameManager.Instance.PlayerMotor.FixStanding();

                return string.Format("Teleport Finished, there are currently {0} chests left in the scene.", chests.Length);
            }
        }

        // For Testing Custom Windows

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
                case PlayerActivateModes.Info: // Inspect Chest
                case PlayerActivateModes.Talk:
                    if (ChestObjRef != null)
                    {
                        LLCObject chestData = ChestObjRef.GetComponent<LLCObject>();
                        if (chestData.HasBeenInspected) { } // Do nothing, will likely change this eventually, so reinspection/rerolling for inspection results is possible at some cost or something.
                        else
                        {
                            chestData.RecentInspectValues = GetInspectionValues(chestData);
                            chestData.HasBeenInspected = true;
                        }
                        InspectionInfoWindow inspectionInfoWindow = new InspectionInfoWindow(DaggerfallUI.UIManager, chestData);
                        DaggerfallUI.UIManager.PushWindow(inspectionInfoWindow);
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
                case PlayerActivateModes.Grab: // Bring Up Custom Chest Choice Window With Buttons To Choose What To Do, Also Might Be How You Can Attempt Disarming Traps Eventually?
                    if (ChestObjRef != null)
                    {
                        LLCObject chestData = ChestObjRef.GetComponent<LLCObject>();
                        ChestChoiceWindow chestChoiceWindow = new ChestChoiceWindow(DaggerfallUI.UIManager, chestData);
                        DaggerfallUI.UIManager.PushWindow(chestChoiceWindow);
                    }
                    else
                    {
                        DaggerfallUI.AddHUDText("ERROR: Chest Was Found As Null.", 5f);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}