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
        const string baseTextureName = "Inspection_Info_GUI_1";

        #endregion

        #region UI Panels

        Panel exitButPanel = new Panel();
        Panel idenButPanel = new Panel();
        Panel lPikButPanel = new Panel();

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
            chestPictureBox.BackgroundColor = new Color(0.9f, 0.1f, 0.5f, 0.75f);
            chestPictureBox.ToolTip = defaultToolTip;
            chestPictureBox.ToolTipText = "Chest Info";

            Panel lockPictureBox = DaggerfallUI.AddPanel(new Rect(176, 64, 30, 22), NativePanel);
            lockPictureBox.BackgroundColor = new Color(0.9f, 0.1f, 0.5f, 0.75f);
            lockPictureBox.ToolTip = defaultToolTip;
            lockPictureBox.ToolTipText = "Lock Info";

            // Exit Button
            Button exitButton = DaggerfallUI.AddButton(new Rect(139, 122, 43, 15), NativePanel);
            exitButton.BackgroundColor = new Color(0.9f, 0.1f, 0.5f, 0.75f);
            exitButton.OnMouseClick += ExitButton_OnMouseClick;
            exitButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);

            // Next I work on this, fill in info "buttons" with summary of what that info means. Then also make methods to determine info text color displayed, also make chest teleporting command, etc.

            SetupInfoTextAndButtons();
        }

        protected virtual void LoadTextures()
        {
            Texture2D baseTex;

            TextureReplacement.TryImportTexture(baseTextureName, true, out baseTex);

            baseTexture = baseTex;
        }

        protected void SetupInfoTextAndButtons()
        {
            // Setup Chest Info panels and text
            Button chestMatButton = DaggerfallUI.AddButton(new Rect(100, 90, 57, 5), NativePanel);
            chestMatButton.BackgroundColor = new Color(0.9f, 0.1f, 0.5f, 0.75f);
            chestMatButton.ToolTip = defaultToolTip;
            chestMatButton.ToolTipText = "Chest Material";
            chestMatButton.OnMouseClick += ChestMaterialInfo_OnMouseClick;
            chestMatButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);
            TextLabel chestMatText = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0, 0), string.Empty, chestMatButton);
            chestMatText.HorizontalAlignment = HorizontalAlignment.Center;
            chestMatText.VerticalAlignment = VerticalAlignment.Middle;
            chestMatText.TextScale = 0.9f;
            chestMatText.Text = LockedLootContainersMain.GetChestMaterialText(chest, chest.RecentInspectValues[0]);
            chestMatText.TextColor = new Color32(243, 239, 44, 255); // Make this a method to determine text-color

            Button chestStabButton = DaggerfallUI.AddButton(new Rect(100, 96, 57, 5), NativePanel);
            chestStabButton.BackgroundColor = new Color(0.9f, 0.1f, 0.5f, 0.75f);
            chestStabButton.ToolTip = defaultToolTip;
            chestStabButton.ToolTipText = "Chest Sturdiness";
            chestStabButton.OnMouseClick += ChestSturdinessInfo_OnMouseClick;
            chestStabButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);
            TextLabel chestStabText = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0, 0), string.Empty, chestStabButton);
            chestStabText.HorizontalAlignment = HorizontalAlignment.Center;
            chestStabText.VerticalAlignment = VerticalAlignment.Middle;
            chestStabText.TextScale = 0.9f;
            chestStabText.Text = LockedLootContainersMain.GetChestSturdinessText(chest, chest.RecentInspectValues[1]);
            chestStabText.TextColor = new Color32(243, 239, 44, 255); // Make this a method to determine text-color

            Button chestMagResButton = DaggerfallUI.AddButton(new Rect(100, 102, 57, 5), NativePanel);
            chestMagResButton.BackgroundColor = new Color(0.9f, 0.1f, 0.5f, 0.75f);
            chestMagResButton.ToolTip = defaultToolTip;
            chestMagResButton.ToolTipText = "Chest Magic Resist";
            chestMagResButton.OnMouseClick += ChestMagicResistInfo_OnMouseClick;
            chestMagResButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);
            TextLabel chestMagResText = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0, 0), string.Empty, chestMagResButton);
            chestMagResText.HorizontalAlignment = HorizontalAlignment.Center;
            chestMagResText.VerticalAlignment = VerticalAlignment.Middle;
            chestMagResText.TextScale = 0.9f;
            chestMagResText.Text = LockedLootContainersMain.GetChestMagicResistText(chest, chest.RecentInspectValues[2]);
            chestMagResText.TextColor = new Color32(243, 239, 44, 255); // Make this a method to determine text-color

            // Setup Lock Info panels and text
            Button lockMatButton = DaggerfallUI.AddButton(new Rect(163, 90, 57, 5), NativePanel);
            lockMatButton.BackgroundColor = new Color(0.9f, 0.1f, 0.5f, 0.75f);
            lockMatButton.ToolTip = defaultToolTip;
            lockMatButton.ToolTipText = "Lock Material";
            lockMatButton.OnMouseClick += LockMaterialInfo_OnMouseClick;
            lockMatButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);
            TextLabel lockMatText = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0, 0), string.Empty, lockMatButton);
            lockMatText.HorizontalAlignment = HorizontalAlignment.Center;
            lockMatText.VerticalAlignment = VerticalAlignment.Middle;
            lockMatText.TextScale = 0.9f;
            lockMatText.Text = LockedLootContainersMain.GetLockMaterialText(chest, chest.RecentInspectValues[3]);
            lockMatText.TextColor = new Color32(243, 239, 44, 255); // Make this a method to determine text-color

            Button lockStabButton = DaggerfallUI.AddButton(new Rect(163, 96, 57, 5), NativePanel);
            lockStabButton.BackgroundColor = new Color(0.9f, 0.1f, 0.5f, 0.75f);
            lockStabButton.ToolTip = defaultToolTip;
            lockStabButton.ToolTipText = "Lock Sturdiness";
            lockStabButton.OnMouseClick += LockSturdinessInfo_OnMouseClick;
            lockStabButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);
            TextLabel lockStabText = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0, 0), string.Empty, lockStabButton);
            lockStabText.HorizontalAlignment = HorizontalAlignment.Center;
            lockStabText.VerticalAlignment = VerticalAlignment.Middle;
            lockStabText.TextScale = 0.9f;
            lockStabText.Text = LockedLootContainersMain.GetLockSturdinessText(chest, chest.RecentInspectValues[4]);
            lockStabText.TextColor = new Color32(243, 239, 44, 255); // Make this a method to determine text-color

            Button lockMagResButton = DaggerfallUI.AddButton(new Rect(163, 102, 57, 5), NativePanel);
            lockMagResButton.BackgroundColor = new Color(0.9f, 0.1f, 0.5f, 0.75f);
            lockMagResButton.ToolTip = defaultToolTip;
            lockMagResButton.ToolTipText = "Lock Magic Resist";
            lockMagResButton.OnMouseClick += LockMagicResistInfo_OnMouseClick;
            lockMagResButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);
            TextLabel lockMagResText = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0, 0), string.Empty, lockMagResButton);
            lockMagResText.HorizontalAlignment = HorizontalAlignment.Center;
            lockMagResText.VerticalAlignment = VerticalAlignment.Middle;
            lockMagResText.TextScale = 0.9f;
            lockMagResText.Text = LockedLootContainersMain.GetLockMagicResistText(chest, chest.RecentInspectValues[5]);
            lockMagResText.TextColor = new Color32(243, 239, 44, 255); // Make this a method to determine text-color

            Button lockComplexButton = DaggerfallUI.AddButton(new Rect(163, 108, 57, 5), NativePanel);
            lockComplexButton.BackgroundColor = new Color(0.9f, 0.1f, 0.5f, 0.75f);
            lockComplexButton.ToolTip = defaultToolTip;
            lockComplexButton.ToolTipText = "Lock Complexity";
            lockComplexButton.OnMouseClick += LockComplexityInfo_OnMouseClick;
            lockComplexButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);
            TextLabel lockComplexText = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0, 0), string.Empty, lockComplexButton);
            lockComplexText.HorizontalAlignment = HorizontalAlignment.Center;
            lockComplexText.VerticalAlignment = VerticalAlignment.Middle;
            lockComplexText.TextScale = 0.9f;
            lockComplexText.Text = LockedLootContainersMain.GetLockComplexityText(chest, chest.RecentInspectValues[6]);
            lockComplexText.TextColor = new Color32(243, 239, 44, 255); // Make this a method to determine text-color

            Button lockJamResButton = DaggerfallUI.AddButton(new Rect(163, 114, 57, 5), NativePanel);
            lockJamResButton.BackgroundColor = new Color(0.9f, 0.1f, 0.5f, 0.75f);
            lockJamResButton.ToolTip = defaultToolTip;
            lockJamResButton.ToolTipText = "Lock Jam Resist";
            lockJamResButton.OnMouseClick += LockJamResistInfo_OnMouseClick;
            lockJamResButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);
            TextLabel lockJamResText = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0, 0), string.Empty, lockJamResButton);
            lockJamResText.HorizontalAlignment = HorizontalAlignment.Center;
            lockJamResText.VerticalAlignment = VerticalAlignment.Middle;
            lockJamResText.TextScale = 0.9f;
            lockJamResText.Text = LockedLootContainersMain.GetLockJamResistText(chest, chest.RecentInspectValues[7]);
            lockJamResText.TextColor = new Color32(243, 239, 44, 255); // Make this a method to determine text-color
        }

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        private void ChestMaterialInfo_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            TextFile.Token[] textToken = DaggerfallUnity.Instance.TextProvider.CreateTokens(TextFile.Formatting.JustifyCenter,
            "Showing Chest Material Info");

            DaggerfallMessageBox inspectChestPopup = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
            inspectChestPopup.SetTextTokens(textToken);
            inspectChestPopup.Show();
            inspectChestPopup.ClickAnywhereToClose = true;
        }

        private void ChestSturdinessInfo_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            TextFile.Token[] textToken = DaggerfallUnity.Instance.TextProvider.CreateTokens(TextFile.Formatting.JustifyCenter,
            "Showing Chest Sturdiness Info");

            DaggerfallMessageBox inspectChestPopup = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
            inspectChestPopup.SetTextTokens(textToken);
            inspectChestPopup.Show();
            inspectChestPopup.ClickAnywhereToClose = true;
        }

        private void ChestMagicResistInfo_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            TextFile.Token[] textToken = DaggerfallUnity.Instance.TextProvider.CreateTokens(TextFile.Formatting.JustifyCenter,
            "Showing Chest Magic Resist Info");

            DaggerfallMessageBox inspectChestPopup = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
            inspectChestPopup.SetTextTokens(textToken);
            inspectChestPopup.Show();
            inspectChestPopup.ClickAnywhereToClose = true;
        }

        private void LockMaterialInfo_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            TextFile.Token[] textToken = DaggerfallUnity.Instance.TextProvider.CreateTokens(TextFile.Formatting.JustifyCenter,
            "Showing Lock Material Info");

            DaggerfallMessageBox inspectChestPopup = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
            inspectChestPopup.SetTextTokens(textToken);
            inspectChestPopup.Show();
            inspectChestPopup.ClickAnywhereToClose = true;
        }

        private void LockSturdinessInfo_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            TextFile.Token[] textToken = DaggerfallUnity.Instance.TextProvider.CreateTokens(TextFile.Formatting.JustifyCenter,
            "Showing Lock Sturdiness Info");

            DaggerfallMessageBox inspectChestPopup = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
            inspectChestPopup.SetTextTokens(textToken);
            inspectChestPopup.Show();
            inspectChestPopup.ClickAnywhereToClose = true;
        }

        private void LockMagicResistInfo_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            TextFile.Token[] textToken = DaggerfallUnity.Instance.TextProvider.CreateTokens(TextFile.Formatting.JustifyCenter,
            "Showing Lock Magic Resist Info");

            DaggerfallMessageBox inspectChestPopup = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
            inspectChestPopup.SetTextTokens(textToken);
            inspectChestPopup.Show();
            inspectChestPopup.ClickAnywhereToClose = true;
        }

        private void LockComplexityInfo_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            TextFile.Token[] textToken = DaggerfallUnity.Instance.TextProvider.CreateTokens(TextFile.Formatting.JustifyCenter,
            "Showing Lock Complexity Info");

            DaggerfallMessageBox inspectChestPopup = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
            inspectChestPopup.SetTextTokens(textToken);
            inspectChestPopup.Show();
            inspectChestPopup.ClickAnywhereToClose = true;
        }

        private void LockJamResistInfo_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            TextFile.Token[] textToken = DaggerfallUnity.Instance.TextProvider.CreateTokens(TextFile.Formatting.JustifyCenter,
            "Showing Lock Jam Resist Info");

            DaggerfallMessageBox inspectChestPopup = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
            inspectChestPopup.SetTextTokens(textToken);
            inspectChestPopup.Show();
            inspectChestPopup.ClickAnywhereToClose = true;
        }
    }
}
