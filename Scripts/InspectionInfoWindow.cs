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

        public static Outline outline;

        #region Testing Properties

        public static Rect butt1 = new Rect(0, 0, 0, 0);
        public static Rect butt2 = new Rect(0, 0, 0, 0);
        public static Rect butt3 = new Rect(0, 0, 0, 0);

        #endregion

        #region Constructors

        public InspectionInfoWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
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

            // Setup Chest Info panels and text
            for (int i = 0; i < 3; i++)
            {
                string boxText = "", toolTipText = "";
                if (i == 0) { boxText = "Chest Material"; toolTipText = "Chest Material"; }
                else if (i == 1) { boxText = "Chest Sturdiness"; toolTipText = "Chest Sturdiness"; }
                else { boxText = "Chest Magic Resist"; toolTipText = "Chest Magic Resist"; }

                Panel chestInfoPan = DaggerfallUI.AddPanel(new Rect(100, 90 + (i * 6), 57, 5), NativePanel);
                chestInfoPan.BackgroundColor = new Color(0.9f, 0.1f, 0.5f, 0.75f);
                chestInfoPan.ToolTip = defaultToolTip;
                chestInfoPan.ToolTipText = toolTipText;
                TextLabel chestInfoText = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0, 0), string.Empty, chestInfoPan);
                chestInfoText.HorizontalAlignment = HorizontalAlignment.Center;
                chestInfoText.VerticalAlignment = VerticalAlignment.Middle;
                chestInfoText.TextScale = 0.9f;
                //lockInfoText.TextColor = new Color(0.8f, 0.4f, 0.2f, 1f); // Brown?
                chestInfoText.Text = boxText;
            }

            // Setup Lock Info panels and text
            for (int i = 0; i < 5; i++)
            {
                string boxText = "", toolTipText = "";
                if (i == 0) { boxText = "Lock Material"; toolTipText = "Lock Material"; }
                else if (i == 1) { boxText = "Lock Sturdiness"; toolTipText = "Lock Sturdiness"; }
                else if (i == 2) { boxText = "Lock Magic Resist"; toolTipText = "Lock Magic Resist"; }
                else if (i == 3) { boxText = "Lock Complexity"; toolTipText = "Lock Complexity"; }
                else { boxText = "Lock Jam Resist"; toolTipText = "Lock Jam Resist"; }

                Panel lockInfoPan = DaggerfallUI.AddPanel(new Rect(163, 90 + (i * 6), 57, 5), NativePanel);
                lockInfoPan.BackgroundColor = new Color(0.9f, 0.1f, 0.5f, 0.75f);
                lockInfoPan.ToolTip = defaultToolTip;
                lockInfoPan.ToolTipText = toolTipText;
                TextLabel lockInfoText = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, new Vector2(0, 0), string.Empty, lockInfoPan);
                lockInfoText.HorizontalAlignment = HorizontalAlignment.Center;
                lockInfoText.VerticalAlignment = VerticalAlignment.Middle;
                lockInfoText.TextScale = 0.9f;
                //lockInfoText.TextColor = new Color(0.8f, 0.4f, 0.2f, 1f); // Brown?
                lockInfoText.Text = boxText;
            }

            // Maybe next I work on this, add buttons on the info panels, that when clicked will show a message-box giving a summary on what that piece of info means and such, also colored text, etc.

            //SetupSkillProgressText();
        }

        protected virtual void LoadTextures()
        {
            Texture2D baseTex;

            TextureReplacement.TryImportTexture(baseTextureName, true, out baseTex);

            baseTexture = baseTex;
        }

        private void ExitButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            CloseWindow();
        }

        /*
        protected void SetupSkillProgressText()
        {
            // Primary skills button
            Button primarySkillsButton = DaggerfallUI.AddButton(new Rect(144, 70, 33, 16), NativePanel);
            primarySkillsButton.BackgroundColor = new Color(0.9f, 0.1f, 0.5f, 0.75f); // For Testing Purposes
            primarySkillsButton.ToolTip = defaultToolTip;
            primarySkillsButton.ToolTipText = "Inspect Chest";
            primarySkillsButton.OnMouseClick += PrimarySkillsButton_OnMouseClick;
            primarySkillsButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.CharacterSheetPrimarySkills);

            // Major skills button
            Button majorSkillsButton = DaggerfallUI.AddButton(new Rect(144, 92, 33, 16), NativePanel);
            majorSkillsButton.BackgroundColor = new Color(0.9f, 0.1f, 0.5f, 0.75f); // For Testing Purposes
            majorSkillsButton.ToolTip = defaultToolTip;
            majorSkillsButton.ToolTipText = "Attempt Lockpick";
            majorSkillsButton.OnMouseClick += MajorSkillsButton_OnMouseClick;
            majorSkillsButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.CharacterSheetMajorSkills);

            // Miscellaneous skills button
            Button miscSkillsButton = DaggerfallUI.AddButton(new Rect(142, 114, 36, 17), NativePanel);
            miscSkillsButton.BackgroundColor = new Color(0.9f, 0.1f, 0.5f, 0.75f); // For Testing Purposes
            miscSkillsButton.OnMouseClick += MiscSkillsButton_OnMouseClick;
            miscSkillsButton.ClickSound = DaggerfallUI.Instance.GetAudioClip(SoundClips.ButtonClick);
            miscSkillsButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.CharacterSheetMiscSkills);
        }

        private void PrimarySkillsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            //ShowSkillsDialog(PlayerEntity.GetPrimarySkills());
            TextFile.Token[] textToken = DaggerfallUnity.Instance.TextProvider.CreateTokens(TextFile.Formatting.JustifyCenter,
            "Redial The Number");

            DaggerfallMessageBox inspectChestPopup = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
            inspectChestPopup.SetTextTokens(textToken); // Use a text-token here instead for the better debug stuff, better random encounters has good examples how, tomorrow.
            inspectChestPopup.Show();
            inspectChestPopup.ClickAnywhereToClose = true;
        }

        private void MajorSkillsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            //ShowSkillsDialog(PlayerEntity.GetMajorSkills());
            TextFile.Token[] textToken = DaggerfallUnity.Instance.TextProvider.CreateTokens(TextFile.Formatting.JustifyCenter,
            "Lick my balls");

            DaggerfallMessageBox inspectChestPopup = new DaggerfallMessageBox(DaggerfallUI.UIManager, DaggerfallUI.UIManager.TopWindow);
            inspectChestPopup.SetTextTokens(textToken); // Use a text-token here instead for the better debug stuff, better random encounters has good examples how, tomorrow.
            inspectChestPopup.Show();
            inspectChestPopup.ClickAnywhereToClose = true;
        }

        private void MiscSkillsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            //ShowSkillsDialog(PlayerEntity.GetMiscSkills(), true);
            CloseWindow();
        }
        */
    }
}
