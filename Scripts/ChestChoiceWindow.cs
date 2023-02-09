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
    public class ChestChoiceWindow : DaggerfallPopupWindow
    {
        PlayerEntity player;

        PlayerEntity Player
        {
            get { return (player != null) ? player : player = GameManager.Instance.PlayerEntity; }
        }

        public static Outline outline;

        #region Constructors

        public ChestChoiceWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        #endregion

        #region UI Textures

        Texture2D baseTexture;
        const string baseTextureName = "Chest_Choice_Menu_1";

        #endregion

        #region UI Panels
		/*
        Panel exitButPanel = new Panel();
        Panel idenButPanel = new Panel();
        Panel lPikButPanel = new Panel();
		*/
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
			
			/*
            // Setup button locations and textures around native panel background
            exitButPanel.Size = new Vector2(24, 16);
            exitButPanel.Position = new Vector2(0, 131);
            exitButPanel.HorizontalAlignment = HorizontalAlignment.Center;
            exitButPanel.BackgroundTexture = exitTexture;
            NativePanel.Components.Add(exitButPanel);

            idenButPanel.Size = new Vector2(28, 28);
            idenButPanel.Position = new Vector2(192, 0);
            idenButPanel.VerticalAlignment = VerticalAlignment.Middle;
            idenButPanel.BackgroundTexture = idenTexture;
            NativePanel.Components.Add(idenButPanel);

            lPikButPanel.Size = new Vector2(28, 28);
            lPikButPanel.Position = new Vector2(99, 0);
            lPikButPanel.VerticalAlignment = VerticalAlignment.Middle;
            lPikButPanel.BackgroundTexture = lPikTexture;
            NativePanel.Components.Add(lPikButPanel);
			*/

            SetupSkillProgressText();
        }

        protected virtual void LoadTextures()
        {
            Texture2D baseTex;

            TextureReplacement.TryImportTexture(baseTextureName, true, out baseTex);

            baseTexture = baseTex;
        }

        protected void SetupSkillProgressText()
        {
            // Primary skills button
            Button primarySkillsButton = DaggerfallUI.AddButton(new Rect(11, 106, 115, 8), NativePanel);
            primarySkillsButton.BackgroundColor = new Color(0.9f, 0.1f, 0.5f, 0.75f); // For Testing Purposes
            primarySkillsButton.OnMouseClick += PrimarySkillsButton_OnMouseClick;
            primarySkillsButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.CharacterSheetPrimarySkills);

            // Major skills button
            Button majorSkillsButton = DaggerfallUI.AddButton(new Rect(11, 116, 115, 8), NativePanel);
            majorSkillsButton.BackgroundColor = new Color(0.9f, 0.1f, 0.5f, 0.75f); // For Testing Purposes
            majorSkillsButton.OnMouseClick += MajorSkillsButton_OnMouseClick;
            majorSkillsButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.CharacterSheetMajorSkills);

            // Minor skills button
            Button minorSkillsButton = DaggerfallUI.AddButton(new Rect(11, 126, 115, 8), NativePanel);
            minorSkillsButton.BackgroundColor = new Color(0.9f, 0.1f, 0.5f, 0.75f); // For Testing Purposes
            minorSkillsButton.OnMouseClick += MinorSkillsButton_OnMouseClick;
            minorSkillsButton.Hotkey = DaggerfallShortcut.GetBinding(DaggerfallShortcut.Buttons.CharacterSheetMinorSkills);

            // Miscellaneous skills button
            Button miscSkillsButton = DaggerfallUI.AddButton(new Rect(11, 136, 115, 8), NativePanel);
            miscSkillsButton.BackgroundColor = new Color(0.9f, 0.1f, 0.5f, 0.75f); // For Testing Purposes
            miscSkillsButton.OnMouseClick += MiscSkillsButton_OnMouseClick;
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

        private void MinorSkillsButton_OnMouseClick(BaseScreenComponent sender, Vector2 position)
        {
            //ShowSkillsDialog(PlayerEntity.GetMinorSkills());
            TextFile.Token[] textToken = DaggerfallUnity.Instance.TextProvider.CreateTokens(TextFile.Formatting.JustifyCenter,
            "Lick my asshole");

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
    }
}
