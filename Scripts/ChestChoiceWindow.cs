using UnityEngine;
using System;
using System.IO;
using DaggerfallConnect;
using DaggerfallConnect.Utility;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterface;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Utility.AssetInjection;
using LockedLootContainers;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements Locked Loot Containers' Chest Choice Interface Window.
    /// </summary>
    public class ChestChoiceWindow : DaggerfallPopupWindow
    {
        PlayerEntity player;

        PlayerEntity Player
        {
            get { return (player != null) ? player : player = GameManager.Instance.PlayerEntity; }
        }

        protected LLCObject chest = null;

        #region Testing Properties

        public static Rect butt1 = new Rect(0, 0, 0, 0);
        public static Rect butt2 = new Rect(0, 0, 0, 0);
        public static Rect butt3 = new Rect(0, 0, 0, 0);

        #endregion

        #region Constructors

        public ChestChoiceWindow(IUserInterfaceManager uiManager, LLCObject chest = null)
            : base(uiManager)
        {
            this.chest = chest;
        }

        #endregion

        #region UI Textures

        Texture2D baseTexture;

        #endregion

        protected override void Setup()
        {
            base.Setup();

            // Load textures
            LoadTextures();

            // This makes the background "transparent" instead of a blank black screen when opening this window.
            ParentPanel.BackgroundColor = ScreenDimColor;

            // Setup native panel background
            NativePanel.BackgroundColor = ScreenDimColor;
            NativePanel.BackgroundTexture = baseTexture;

            SetupChestChoiceButtons();
        }

        protected virtual void LoadTextures()
        {
            baseTexture = LockedLootContainersMain.Instance.ChestChoiceMenuTexture;
        }

        protected void SetupChestChoiceButtons()
        {
            // Inspect Chest button
            Button inspectChestButton = DaggerfallUI.AddButton(new Rect(144, 70, 33, 16), NativePanel);
            inspectChestButton.ToolTip = defaultToolTip;
            inspectChestButton.ToolTipText = "Inspect Chest";
            inspectChestButton.OnMouseClick += InspectChestButton_OnMouseClick;
            inspectChestButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);

            // Attempt Lockpick button
            Button attemptLockpickButton = DaggerfallUI.AddButton(new Rect(144, 92, 33, 16), NativePanel);
            attemptLockpickButton.ToolTip = defaultToolTip;
            attemptLockpickButton.ToolTipText = "Attempt Lockpick";
            attemptLockpickButton.OnMouseClick += AttemptLockpickButton_OnMouseClick;
            attemptLockpickButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick); // May remove this click sound if it interfers with lockpicking sounds, etc.

            // Exit button
            Button exitButton = DaggerfallUI.AddButton(new Rect(142, 114, 36, 17), NativePanel);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;
            exitButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);
        }

        private void InspectChestButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
            if (chest.HasBeenInspected) { } // Do nothing, will likely change this eventually, so reinspection/rerolling for inspection results is possible at some cost or something.
            else
            {
                LockedLootContainersMain.ApplyInspectionCosts();
                chest.RecentInspectValues = LockedLootContainersMain.GetInspectionValues(chest);
                chest.HasBeenInspected = true;
            }
            InspectionInfoWindow inspectionInfoWindow = new InspectionInfoWindow(DaggerfallUI.UIManager, chest);
            DaggerfallUI.UIManager.PushWindow(inspectionInfoWindow);
        }

        private void AttemptLockpickButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
            if (chest != null) // Oh yeah, don't forget to give skill XP as well for various things, failures and successes, etc.
            {
                DaggerfallAudioSource dfAudioSource = chest.GetComponent<DaggerfallAudioSource>();
                ItemCollection closedChestLoot = chest.AttachedLoot;
                Transform closedChestTransform = chest.transform;
                Vector3 pos = chest.transform.position;

                if (chest.IsLockJammed)
                {
                    DaggerfallUI.AddHUDText("The lock is jammed and inoperable...", 4f);
                    if (dfAudioSource != null && !dfAudioSource.IsPlaying())
                        dfAudioSource.PlayClipAtPoint(SoundClips.ActivateRatchet, chest.gameObject.transform.position); // Will use custom sounds in the end most likely.
                }
                else if (LockedLootContainersMain.LockPickChance(chest)) // Guess the basic "success" stuff is already here for the time being, so I'll do more with that part later on.
                {
                    chest.PicksAttempted++;
                    LockedLootContainersMain.ApplyLockPickAttemptCosts();
                    DaggerfallLoot openChestLoot = GameObjectHelper.CreateLootContainer(LootContainerTypes.Nothing, InventoryContainerImages.Chest, pos, closedChestTransform.parent, 4734, 0, chest.LoadID, null, false);
                    openChestLoot.gameObject.name = GameObjectHelper.GetGoFlatName(4734, 0);
                    openChestLoot.Items.TransferAll(closedChestLoot); // Transfers items from closed chest's items to the new open chest's item collection.

                    // Show success and play unlock sound
                    DaggerfallUI.AddHUDText("The lock clicks open...", 4f);
                    if (dfAudioSource != null)
                        dfAudioSource.PlayClipAtPoint(SoundClips.ActivateLockUnlock, chest.gameObject.transform.position); // Might use custom sound here, or atleast varied pitches of the same sound, etc.

                    UnityEngine.Object.Destroy(LockedLootContainersMain.ChestObjRef); // Removed closed chest from scene, but saved its characteristics we care about for opened chest loot-pile.
                    LockedLootContainersMain.ChestObjRef = null; // This may cause issues? But really no clue, just a note.
                }
                else
                {
                    chest.PicksAttempted++; // Increase picks attempted counter by 1 on the chest.
                    LockedLootContainersMain.ApplyLockPickAttemptCosts();
                    if (Dice100.SuccessRoll(LockedLootContainersMain.LockJamChance(chest)))
                    {
                        chest.IsLockJammed = true;
                        DaggerfallUI.AddHUDText("You jammed the lock, now brute force is the only option.", 4f);
                        if (dfAudioSource != null)
                            AudioSource.PlayClipAtPoint(LockedLootContainersMain.GetLockpickJammedClip(), chest.gameObject.transform.position, UnityEngine.Random.Range(0.9f, 1.42f) * DaggerfallUnity.Settings.SoundVolume);
                    }
                    else
                    {
                        DaggerfallUI.AddHUDText("You fail to pick the lock...", 4f);
                        if (dfAudioSource != null && !dfAudioSource.IsPlaying())
                            AudioSource.PlayClipAtPoint(LockedLootContainersMain.GetLockpickAttemptClip(), chest.gameObject.transform.position, UnityEngine.Random.Range(0.9f, 1.42f) * DaggerfallUnity.Settings.SoundVolume);
                    }
                }
            }
            else
            {
                DaggerfallUI.AddHUDText("ERROR: Chest Was Found As Null.", 5f);
            }
        }

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }
    }
}
