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

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Implements Locked Loot Containers' Chest Choice Interface Window.
    /// </summary>
    public class InspectionInfoWindow : DaggerfallPopupWindow
    {
        PlayerEntity player;

        PlayerEntity Player
        {
            get { return (player != null) ? player : player = GameManager.Instance.PlayerEntity; }
        }

        Color32 textShadowColor = new Color32(0, 0, 0, 255);
        Vector2 textShadowPosition = new Vector2(0.60f, 0.60f);

        protected LLCObject chest = null;

        #region Testing Properties

        public static Rect butt1 = new Rect(0, 0, 0, 0);
        public static Rect butt2 = new Rect(0, 0, 0, 0);
        public static Rect butt3 = new Rect(0, 0, 0, 0);

        #endregion

        #region Constructors

        public InspectionInfoWindow(IUserInterfaceManager uiManager, LLCObject chest = null)
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

            Panel chestPictureBox = DaggerfallUI.AddPanel(new Rect(113, 64, 30, 22), NativePanel);
            //chestPictureBox.BackgroundColor = new Color(0.9f, 0.1f, 0.5f, 0.75f); // For testing purposes
            chestPictureBox.ToolTip = defaultToolTip;
            chestPictureBox.ToolTipText = "The Chest Looks " + LockedLootContainersMain.GetRemainingHealthDescription(chest, false);

            Panel lockPictureBox = DaggerfallUI.AddPanel(new Rect(176, 64, 30, 22), NativePanel);
            //lockPictureBox.BackgroundColor = new Color(0.9f, 0.1f, 0.5f, 0.75f); // For testing purposes
            lockPictureBox.ToolTip = defaultToolTip;
            lockPictureBox.ToolTipText = "The Lock Looks " + LockedLootContainersMain.GetRemainingHealthDescription(chest, true);

            // Exit Button
            Button exitButton = DaggerfallUI.AddButton(new Rect(139, 122, 43, 15), NativePanel);
            //exitButton.BackgroundColor = new Color(0.9f, 0.1f, 0.5f, 0.75f); // For testing purposes
            exitButton.OnMouseClick += ExitButton_OnMouseClick;
            exitButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);

            SetupInfoTextAndButtons();
        }

        protected virtual void LoadTextures()
        {
            baseTexture = LockedLootContainersMain.Instance.InspectionInfoGUITexture;
        }

        protected void SetupInfoTextAndButtons()
        {
            // Setup Chest Info panels and text
            Button chestMatButton = DaggerfallUI.AddButton(new Rect(100, 90, 57, 5), NativePanel);
            chestMatButton.ToolTip = defaultToolTip;
            chestMatButton.ToolTipText = "Chest Material";
            chestMatButton.OnMouseClick += ChestMaterialInfo_OnMouseClick;
            chestMatButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);
            TextLabel chestMatText = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0, 0), string.Empty, chestMatButton);
            chestMatText.HorizontalAlignment = HorizontalAlignment.Center;
            chestMatText.VerticalAlignment = VerticalAlignment.Middle;
            chestMatText.ShadowColor = textShadowColor;
            chestMatText.ShadowPosition = textShadowPosition;
            chestMatText.TextScale = 0.9f;
            LockedLootContainersMain.GetChestMaterialText(chest, chest.RecentInspectValues[0], out string cMatText, out Color32 cMatColor);
            chestMatText.Text = cMatText;
            chestMatText.TextColor = cMatColor;

            Button chestStabButton = DaggerfallUI.AddButton(new Rect(100, 96, 57, 5), NativePanel);
            chestStabButton.ToolTip = defaultToolTip;
            chestStabButton.ToolTipText = "Chest Sturdiness";
            chestStabButton.OnMouseClick += ChestSturdinessInfo_OnMouseClick;
            chestStabButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);
            TextLabel chestStabText = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0, 0), string.Empty, chestStabButton);
            chestStabText.HorizontalAlignment = HorizontalAlignment.Center;
            chestStabText.VerticalAlignment = VerticalAlignment.Middle;
            chestStabText.ShadowColor = textShadowColor;
            chestStabText.ShadowPosition = textShadowPosition;
            chestStabText.TextScale = 0.9f;
            LockedLootContainersMain.GetChestSturdinessText(chest, chest.RecentInspectValues[1], out string cStabText, out Color32 cStabColor);
            chestStabText.Text = cStabText;
            chestStabText.TextColor = cStabColor;

            Button chestMagResButton = DaggerfallUI.AddButton(new Rect(100, 102, 57, 5), NativePanel);
            chestMagResButton.ToolTip = defaultToolTip;
            chestMagResButton.ToolTipText = "Chest Magic Resist";
            chestMagResButton.OnMouseClick += ChestMagicResistInfo_OnMouseClick;
            chestMagResButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);
            TextLabel chestMagResText = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0, 0), string.Empty, chestMagResButton);
            chestMagResText.HorizontalAlignment = HorizontalAlignment.Center;
            chestMagResText.VerticalAlignment = VerticalAlignment.Middle;
            chestMagResText.ShadowColor = textShadowColor;
            chestMagResText.ShadowPosition = textShadowPosition;
            chestMagResText.TextScale = 0.9f;
            LockedLootContainersMain.GetChestMagicResistText(chest, chest.RecentInspectValues[2], out string cMResText, out Color32 cMResColor);
            chestMagResText.Text = cMResText;
            chestMagResText.TextColor = cMResColor;

            // Setup Lock Info panels and text
            Button lockMatButton = DaggerfallUI.AddButton(new Rect(163, 90, 57, 5), NativePanel);
            lockMatButton.ToolTip = defaultToolTip;
            lockMatButton.ToolTipText = "Lock Material";
            lockMatButton.OnMouseClick += LockMaterialInfo_OnMouseClick;
            lockMatButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);
            TextLabel lockMatText = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0, 0), string.Empty, lockMatButton);
            lockMatText.HorizontalAlignment = HorizontalAlignment.Center;
            lockMatText.VerticalAlignment = VerticalAlignment.Middle;
            lockMatText.ShadowColor = textShadowColor;
            lockMatText.ShadowPosition = textShadowPosition;
            lockMatText.TextScale = 0.9f;
            LockedLootContainersMain.GetLockMaterialText(chest, chest.RecentInspectValues[3], out string lMatText, out Color32 lMatColor);
            lockMatText.Text = lMatText;
            lockMatText.TextColor = lMatColor;

            Button lockStabButton = DaggerfallUI.AddButton(new Rect(163, 96, 57, 5), NativePanel);
            lockStabButton.ToolTip = defaultToolTip;
            lockStabButton.ToolTipText = "Lock Sturdiness";
            lockStabButton.OnMouseClick += LockSturdinessInfo_OnMouseClick;
            lockStabButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);
            TextLabel lockStabText = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0, 0), string.Empty, lockStabButton);
            lockStabText.HorizontalAlignment = HorizontalAlignment.Center;
            lockStabText.VerticalAlignment = VerticalAlignment.Middle;
            lockStabText.ShadowColor = textShadowColor;
            lockStabText.ShadowPosition = textShadowPosition;
            lockStabText.TextScale = 0.9f;
            LockedLootContainersMain.GetLockSturdinessText(chest, chest.RecentInspectValues[4], out string lStabText, out Color32 lStabColor);
            lockStabText.Text = lStabText;
            lockStabText.TextColor = lStabColor;

            Button lockMagResButton = DaggerfallUI.AddButton(new Rect(163, 102, 57, 5), NativePanel);
            lockMagResButton.ToolTip = defaultToolTip;
            lockMagResButton.ToolTipText = "Lock Magic Resist";
            lockMagResButton.OnMouseClick += LockMagicResistInfo_OnMouseClick;
            lockMagResButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);
            TextLabel lockMagResText = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0, 0), string.Empty, lockMagResButton);
            lockMagResText.HorizontalAlignment = HorizontalAlignment.Center;
            lockMagResText.VerticalAlignment = VerticalAlignment.Middle;
            lockMagResText.ShadowColor = textShadowColor;
            lockMagResText.ShadowPosition = textShadowPosition;
            lockMagResText.TextScale = 0.9f;
            LockedLootContainersMain.GetLockMagicResistText(chest, chest.RecentInspectValues[5], out string lMResText, out Color32 lMResColor);
            lockMagResText.Text = lMResText;
            lockMagResText.TextColor = lMResColor;

            Button lockComplexButton = DaggerfallUI.AddButton(new Rect(163, 108, 57, 5), NativePanel);
            lockComplexButton.ToolTip = defaultToolTip;
            lockComplexButton.ToolTipText = "Lock Complexity";
            lockComplexButton.OnMouseClick += LockComplexityInfo_OnMouseClick;
            lockComplexButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);
            TextLabel lockComplexText = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0, 0), string.Empty, lockComplexButton);
            lockComplexText.HorizontalAlignment = HorizontalAlignment.Center;
            lockComplexText.VerticalAlignment = VerticalAlignment.Middle;
            lockComplexText.ShadowColor = textShadowColor;
            lockComplexText.ShadowPosition = textShadowPosition;
            lockComplexText.TextScale = 0.9f;
            LockedLootContainersMain.GetLockComplexityText(chest, chest.RecentInspectValues[6], out string lCompText, out Color32 lCompColor);
            lockComplexText.Text = lCompText;
            lockComplexText.TextColor = lCompColor;

            Button lockJamResButton = DaggerfallUI.AddButton(new Rect(163, 114, 57, 5), NativePanel);
            lockJamResButton.ToolTip = defaultToolTip;
            lockJamResButton.ToolTipText = "Lock Jam Resist";
            lockJamResButton.OnMouseClick += LockJamResistInfo_OnMouseClick;
            lockJamResButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);
            TextLabel lockJamResText = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0, 0), string.Empty, lockJamResButton);
            lockJamResText.HorizontalAlignment = HorizontalAlignment.Center;
            lockJamResText.VerticalAlignment = VerticalAlignment.Middle;
            lockJamResText.ShadowColor = textShadowColor;
            lockJamResText.ShadowPosition = textShadowPosition;
            lockJamResText.TextScale = 0.9f;
            LockedLootContainersMain.GetLockJamResistText(chest, chest.RecentInspectValues[7], out string lJResText, out Color32 lJResColor);
            lockJamResText.Text = lJResText;
            lockJamResText.TextColor = lJResColor;
        }

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        private void ChestMaterialInfo_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            TextFile.Token[] textToken = DaggerfallUnity.Instance.TextProvider.CreateTokens(TextFile.Formatting.JustifyCenter,
            "Chest Material",
            "-----------------------",
            "What material the body of the chest is made of, different materials have different properties.",
            "Such differences include, ability to survive physical trauma, and resistance to magic attacks.",
            "The more rare and expensive the material a chest is comprised of, the more likely the odds",
            "that something of value is contained within.",
            "",
            "There are several materials a chest can be made of, these include:",
            "Wood, Iron, Steel, Orcish, Mithril, Dwarven, Adamantium, and Daedric");

            DaggerfallMessageBox inspectChestPopup = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
            inspectChestPopup.SetTextTokens(textToken);
            inspectChestPopup.Show();
            inspectChestPopup.ClickAnywhereToClose = true;
        }

        private void ChestSturdinessInfo_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            TextFile.Token[] textToken = DaggerfallUnity.Instance.TextProvider.CreateTokens(TextFile.Formatting.JustifyCenter,
            "Chest Sturdiness",
            "-----------------------",
            "The higher this value, the more difficult it is to access a chest's contents through brute force.",
            "",
            "This value can be from 1 to 100");

            DaggerfallMessageBox inspectChestPopup = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
            inspectChestPopup.SetTextTokens(textToken);
            inspectChestPopup.Show();
            inspectChestPopup.ClickAnywhereToClose = true;
        }

        private void ChestMagicResistInfo_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            TextFile.Token[] textToken = DaggerfallUnity.Instance.TextProvider.CreateTokens(TextFile.Formatting.JustifyCenter,
            "Chest Magic Resistance",
            "-----------------------",
            "The higher this value, the more likely any magical assault will be outright ignored.",
            "Forms of magical effects include those with:",
            "'Damage Health', 'Continuous Damage Health', and 'Disintegrate'",
            "(Cannot be touch based, only projectile based spells.)",
            "",
            "This value can be from 1 to 100");

            DaggerfallMessageBox inspectChestPopup = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
            inspectChestPopup.SetTextTokens(textToken);
            inspectChestPopup.Show();
            inspectChestPopup.ClickAnywhereToClose = true;
        }

        private void LockMaterialInfo_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            TextFile.Token[] textToken = DaggerfallUnity.Instance.TextProvider.CreateTokens(TextFile.Formatting.JustifyCenter,
            "Lock Material",
            "-----------------------",
            "What material the lock on the chest is made of, different materials have different properties.",
            "Such differences include, ability to survive physical trauma, resistance to magical effects,",
            "average range of lock mechanism complexity, and the average jam resistance of the lock.",
            "The more rare and expensive the material a lock is comprised of, the more likely the odds",
            "that something of value is contained within the chest its attached to.",
            "",
            "There are several materials a lock can be made of, these include:",
            "Wood, Iron, Steel, Orcish, Mithril, Dwarven, Adamantium, and Daedric");

            DaggerfallMessageBox inspectChestPopup = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
            inspectChestPopup.SetTextTokens(textToken);
            inspectChestPopup.Show();
            inspectChestPopup.ClickAnywhereToClose = true;
        }

        private void LockSturdinessInfo_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            TextFile.Token[] textToken = DaggerfallUnity.Instance.TextProvider.CreateTokens(TextFile.Formatting.JustifyCenter,
            "Lock Sturdiness",
            "-----------------------",
            "The higher this value, the more difficult it is to break off a lock through brute force.",
            "",
            "This value can be from 1 to 100");

            DaggerfallMessageBox inspectChestPopup = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
            inspectChestPopup.SetTextTokens(textToken);
            inspectChestPopup.Show();
            inspectChestPopup.ClickAnywhereToClose = true;
        }

        private void LockMagicResistInfo_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            TextFile.Token[] textToken = DaggerfallUnity.Instance.TextProvider.CreateTokens(TextFile.Formatting.JustifyCenter,
            "Lock Magic Resistance",
            "-----------------------",
            "The higher this value, the more likely magical 'Open' effects will fail against the lock.",
            "",
            "This value can be from 1 to 100");

            DaggerfallMessageBox inspectChestPopup = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
            inspectChestPopup.SetTextTokens(textToken);
            inspectChestPopup.Show();
            inspectChestPopup.ClickAnywhereToClose = true;
        }

        private void LockComplexityInfo_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            TextFile.Token[] textToken = DaggerfallUnity.Instance.TextProvider.CreateTokens(TextFile.Formatting.JustifyCenter,
            "Lock Complexity",
            "-----------------------",
            "The higher this value, the more difficult the lock will be to lock-pick successfully.",
            "This added difficult applies to magical 'Open' effects as well, but to a lesser degree.",
            "Locks can get jammed, rendering them inoperable, leaving brute force as the last resort.",
            "Jamming becomes increasingly more likely the more failed attempts that have occured on a lock.",
            "",
            "This value can be from 1 to 100");

            DaggerfallMessageBox inspectChestPopup = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
            inspectChestPopup.SetTextTokens(textToken);
            inspectChestPopup.Show();
            inspectChestPopup.ClickAnywhereToClose = true;
        }

        private void LockJamResistInfo_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            TextFile.Token[] textToken = DaggerfallUnity.Instance.TextProvider.CreateTokens(TextFile.Formatting.JustifyCenter,
            "Lock Jam Resistance",
            "-----------------------",
            "The higher this value, the lower the odds are for a lock to be jammed from failed pick attempts.",
            "Jamming becomes increasingly more likely the more failed attempts that have occured on a lock.",
            "",
            "This value can be from 1 to 100");

            DaggerfallMessageBox inspectChestPopup = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
            inspectChestPopup.SetTextTokens(textToken);
            inspectChestPopup.Show();
            inspectChestPopup.ClickAnywhereToClose = true;
        }
    }
}
