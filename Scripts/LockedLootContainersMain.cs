// Project:         LockedLootContainers mod for Daggerfall Unity (http://www.dfworkshop.net)
// Copyright:       Copyright (C) 2022 Kirk.O
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Author:          Kirk.O
// Created On: 	    9/8/2022, 11:00 PM
// Last Edit:		9/8/2022, 11:00 PM
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

namespace LockedLootContainers
{
    public class LockedLootContainersMain : MonoBehaviour
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

            PlayerActivate.RegisterCustomActivation(mod, 810, 0, ChestActivation); // Needs our custom texture/billboard flat ID value, 500 is placeholder.

            PlayerEnterExit.OnTransitionDungeonInterior += AddLootChests_OnTransitionDungeonInterior;

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

            // Now that I have some basics for bashing out of the way (still needs testing). Tomorrow I should look into how I might do this for spell detection and projectile detection, etc.
            // For the "open" spell effect atleast, check under PlayerActivate.cs in the "HandleOpenEffect" method about details involving that part atleast seems simple enough.
            // But still need to do the whole projectile thing, which hopefully will be easier than it seems in my head.
        }

        private static void AttemptMeleeChestBash(LLCObject chest, DaggerfallUnityItem weapon) // May eventually put this in LLCObject itself, but for now just keep it here with everything else.
        {
            if (chest != null)
            {
                ItemCollection closedChestLoot = chest.AttachedLoot;
                Transform closedChestTransform = chest.gameObject.transform; // Not sure if the explicit "gameObject" reference is necessary, will test eventually and determine if so or not.
                Vector3 pos = chest.gameObject.transform.position;

                DaggerfallLoot openChestLoot = GameObjectHelper.CreateLootContainer(LootContainerTypes.Nothing, InventoryContainerImages.Chest, pos, closedChestTransform.parent, 812, 0, chest.LoadID, null, false);
                openChestLoot.gameObject.name = GameObjectHelper.GetGoFlatName(812, 0);
                openChestLoot.Items.TransferAll(closedChestLoot); // Transfers items from closed chest's items to the new open chest's item collection.

                Destroy(chest.gameObject); // Removed closed chest from scene, but saved its characteristics we care about for opened chest loot-pile.

                // Show success and play unlock sound
                DaggerfallUI.AddHUDText("With use of brute force, the lock finally breaks open...", 4f);
                DaggerfallAudioSource dfAudioSource = GameManager.Instance.PlayerActivate.GetComponent<DaggerfallAudioSource>();
                if (dfAudioSource != null)
                    dfAudioSource.PlayOneShot(SoundClips.ActivateLockUnlock);
            }
            else
            {
                DaggerfallUI.AddHUDText("ERROR: Chest Was Found As Null.", 5f);
            }

            // Will do rest of bashing logic later when all of this part atleast is confirmed to work and such.
            // Oh yeah, make sure to check ActionDoor.cs under "AttemptBash" for making sounds of bashing and such when hitting the chest, don't want to forget about that detail.
        }

        private static void ChestActivation(RaycastHit hit)
        {
            ChestObjRef = hit.collider.gameObject; // Sets clicked chest as global variable reference for later user in other methods.

            string[] message = null;

            if (hit.distance > PlayerActivate.DefaultActivationDistance)
                DaggerfallUI.SetMidScreenText(TextManager.Instance.GetLocalizedText("youAreTooFarAway"));

            switch (CurrentMode)
            {
                case PlayerActivateModes.Info: // Attempt To Inspect Chest
                case PlayerActivateModes.Talk:
                    DaggerfallMessageBox inspectChestPopup = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
                    message[0] = "Good job, you inspected the chest!";
                    inspectChestPopup.SetText(message);
                    inspectChestPopup.Show();
                    inspectChestPopup.ClickAnywhereToClose = true;
                    break;
                case PlayerActivateModes.Steal: // Attempt To Lock-pick Chest
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
                        DaggerfallUI.AddHUDText("The lock clicks open...", 4f);
                        DaggerfallAudioSource dfAudioSource = GameManager.Instance.PlayerActivate.GetComponent<DaggerfallAudioSource>();
                        if (dfAudioSource != null)
                            dfAudioSource.PlayOneShot(SoundClips.ActivateLockUnlock);
                    }
                    else
                    {
                        DaggerfallUI.AddHUDText("ERROR: Chest Was Found As Null.", 5f);
                    }
                    break;
                case PlayerActivateModes.Grab: // Bring Up Pop-up Message With Buttons To Choose What To Do, Also Might Be How You Can Attempt Disarming Traps Maybe?
                    DaggerfallMessageBox chestChoicePopUp = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
                    message[0] = "What do you want to do with this chest?";
                    chestChoicePopUp.SetText(message);
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

        private static void chestChoicePopUp_OnButtonClick(DaggerfallMessageBox sender, DaggerfallMessageBox.MessageBoxButtons messageBoxButton)
        {
            if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Copy) // Inspect
            {
                sender.CloseWindow();
                DaggerfallMessageBox inspectChestPopup = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
                string[] message = { "Good job, you inspected the chest!" };
                inspectChestPopup.SetText(message);
                inspectChestPopup.Show();
                inspectChestPopup.ClickAnywhereToClose = true;
            }
            else if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Accept) // Pick-lock
            {
                sender.CloseWindow();

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
                    DaggerfallUI.AddHUDText("The lock clicks open...", 4f);
                    DaggerfallAudioSource dfAudioSource = GameManager.Instance.PlayerActivate.GetComponent<DaggerfallAudioSource>();
                    if (dfAudioSource != null)
                        dfAudioSource.PlayOneShot(SoundClips.ActivateLockUnlock);
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
            else if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Anchor) // Open Spell
            {
                sender.CloseWindow();

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

                /*DaggerfallMessageBox inspectChestPopup = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
                string[] message = { "You attempted to magically open the lock!" };
                inspectChestPopup.SetText(message);
                inspectChestPopup.Show();
                inspectChestPopup.ClickAnywhereToClose = true;*/
            }
            else // Cancel
            {
                sender.CloseWindow();
            }
        }

        public void AddLootChests_OnTransitionDungeonInterior(PlayerEnterExit.TransitionEventArgs args)
        {
            DFLocation locationData = GameManager.Instance.PlayerGPS.CurrentLocation;
            DaggerfallLoot[] lootPiles;

            if (GameManager.Instance.PlayerEnterExit.IsPlayerInside)
            {
                if (locationData.MapTableData.DungeonType == DFRegion.DungeonTypes.Cemetery)
                {
                    // Make list of loot-piles currently in the dungeon "scene."
                    lootPiles = FindObjectsOfType<DaggerfallLoot>();

                    for (int i = 0; i < lootPiles.Length; i++)
                    {
                        if (lootPiles[i].ContainerType == LootContainerTypes.RandomTreasure)
                        {
                            ItemCollection oldPileLoot = lootPiles[i].Items;
                            Transform oldLootPileTransform = lootPiles[i].transform;
                            Vector3 pos = lootPiles[i].transform.position;
                            ulong oldLoadID = lootPiles[i].LoadID;

                            GameObject chestParentObj = GameObjectHelper.CreateDaggerfallBillboardGameObject(810, 0, oldLootPileTransform.parent.parent);

                            LLCObject llcObj = chestParentObj.AddComponent<LLCObject>();
                            llcObj.Oldloot = oldPileLoot;
                            llcObj.AttachedLoot = llcObj.Oldloot; // Will change this later, but placeholder for testing.
                            llcObj.LoadID = oldLoadID;

                            // Set position
                            Billboard dfBillboard = chestParentObj.GetComponent<Billboard>();
                            chestParentObj.transform.position = pos;
                            chestParentObj.transform.position += new Vector3(0, dfBillboard.Summary.Size.y / 2, 0);
                            GameObjectHelper.AlignBillboardToGround(chestParentObj, dfBillboard.Summary.Size);

                            Destroy(lootPiles[i].gameObject); // Removed old loot-pile from scene, but saved its characteristics we care about.

                            // Possibly create custom gameobject locked chest component thing to attach here and do main functions with, next time.
                        }
                    }
                }
            }
        }
    }
}
