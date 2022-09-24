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

            PlayerActivate.RegisterCustomActivation(mod, 810, 0, ChestActivation); // Needs our custom texture/billboard flat ID value, 500 is placeholder.

            PlayerEnterExit.OnTransitionDungeonInterior += AddLootChests_OnTransitionDungeonInterior;

            Debug.Log("Finished mod init: Locked Loot Containers");
        }

        private static void ChestActivation(RaycastHit hit)
        {
            ChestObjRef = hit.collider.gameObject; // Sets clicked chest as global variable reference for later user in other methods.

            DaggerfallMessageBox chestChoicePopUp = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
            string[] message = { "Good job, Asshole. You clicked the chest." };

            chestChoicePopUp.SetText(message);
            chestChoicePopUp.OnButtonClick += chestChoicePopUp_OnButtonClick;
            chestChoicePopUp.AddButton(DaggerfallMessageBox.MessageBoxButtons.Copy); // Inspect
            chestChoicePopUp.AddButton(DaggerfallMessageBox.MessageBoxButtons.Accept); // Pick-lock
            chestChoicePopUp.AddButton(DaggerfallMessageBox.MessageBoxButtons.Reject); // Bash
            chestChoicePopUp.AddButton(DaggerfallMessageBox.MessageBoxButtons.Anchor); // Open Spell
            chestChoicePopUp.AddButton(DaggerfallMessageBox.MessageBoxButtons.Cancel); // Cancel
            chestChoicePopUp.Show();

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
                    DaggerfallUI.AddHUDText(TextManager.Instance.GetLocalizedText("lockpickingSuccess"), 4f);
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
            else if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Reject) // Bash
            {
                sender.CloseWindow();
                DaggerfallMessageBox inspectChestPopup = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
                string[] message = { "You attempted bashing the lock, dummy!" };
                inspectChestPopup.SetText(message);
                inspectChestPopup.Show();
                inspectChestPopup.ClickAnywhereToClose = true;
            }
            else if (messageBoxButton == DaggerfallMessageBox.MessageBoxButtons.Anchor) // Open Spell
            {
                sender.CloseWindow();
                DaggerfallMessageBox inspectChestPopup = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
                string[] message = { "You attempted to magically open the lock!" };
                inspectChestPopup.SetText(message);
                inspectChestPopup.Show();
                inspectChestPopup.ClickAnywhereToClose = true;
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
