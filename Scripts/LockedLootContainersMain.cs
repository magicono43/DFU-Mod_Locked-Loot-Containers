// Project:         LockedLootContainers mod for Daggerfall Unity (http://www.dfworkshop.net)
// Copyright:       Copyright (C) 2025 Kirk.O
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Author:          Kirk.O
// Created On: 	    9/8/2022, 11:00 PM
// Last Edit:		8/1/2025, 10:30 PM
// Version:			1.10
// Special Thanks:  
// Modifier:			

using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.Entity;
using System.Collections.Generic;
using Wenzil.Console;
using System;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallWorkshop.Game.Utility.ModSupport.ModSettings;
using DaggerfallConnect.Arena2;

namespace LockedLootContainers
{
    public partial class LockedLootContainersMain : MonoBehaviour
    {
        public static LockedLootContainersMain Instance;
        public static LLCSaveData ModSaveData = new LLCSaveData();

        static Mod mod;

        // Options
        public static int ChestGraphicType { get; set; }
        public static bool AllowChestCollision { get; set; }
        public static bool AllowChestShadows { get; set; }

        public static bool AllowDungeonQuestMarkerChestSpawning { get; set; }
        public static int DungeonQuestMarkerSpawnOdds { get; set; }
        public static int DungeonQuestMarkerLootMultiMinRange { get; set; }
        public static int DungeonQuestMarkerLootMultiMaxRange { get; set; }

        public static int MegaChestLootMultiMinRange { get; set; }
        public static int MegaChestLootMultiMaxRange { get; set; }

        public static bool RemoveUnprotectedDungeonLootPiles { get; set; }
        public static bool PreserveDeletedLootPileItems { get; set; }
        public static bool DungeonChestsOnlySpawnAtQuestMarkers { get; set; }
        public static bool SpawnSingularMegaChestInDungeons { get; set; }
        public static int MegaChestSpawnOdds { get; set; }

        public static bool AllowCompatibilityWarnings { get; set; }
        public static bool AllowVerboseErrorLogging { get; set; }
        public static bool DoNotSpamExceptionsLogs { get; set; }

        // Mod Compatibility Check Values
        public static bool RepairToolsCheck { get; set; }
        public static bool JewelryAdditionsCheck { get; set; }
        public static bool ClimatesAndCaloriesCheck { get; set; }
        public static bool SkillBooksCheck { get; set; }
        public static bool RolePlayRealismLootRebalanceCheck { get; set; }
        public static bool RolePlayRealismBandagingCheck { get; set; }
        public static bool RolePlayRealismNewWeaponCheck { get; set; }
        public static bool RolePlayRealismNewArmorCheck { get; set; }
        public static bool TemperedInteriorsCheck { get; set; }

        // Global Variables
        public static GameObject ChestObjRef { get; set; }
        public static GameObject MainCamera { get; set; }
        public static int PlayerLayerMask { get; set; }
        public static AudioClip LastSoundPlayed { get { return lastSoundPlayed; } set { lastSoundPlayed = value; } }
        public static bool ForceChestSpawnToggle { get; set; }
        public static IUserInterfaceWindow LastUIWindow { get; set; }
        public static DaggerfallLoot LastLootedChest { get; set; }
        public static PlayerEntity Player { get { return GameManager.Instance.PlayerEntity; } }
        public static PlayerActivateModes CurrentMode { get { return GameManager.Instance.PlayerActivate.CurrentMode; } }
        public static WeaponManager WepManager { get { return GameManager.Instance.WeaponManager; } }

        bool isBashReady = true;

        public static bool exceptionLogging = true;

        // Mod Textures || GUI
        public Texture2D ChestChoiceMenuTexture;
        public Texture2D InspectionInfoGUITexture;
        // Chest Sprites
        public const int ClosedChestSpriteID = 4733;
        public const int OpenFullChestSpriteID = 4734;
        public const int OpenEmptyChestSpriteID = 4735;
        public const int SmashedChestSpriteID = 4736;
        public const int DisintegratedChestSpriteID = 4737;

        // Mod Prefabs/Imported Models || High Poly
        public GameObject HighPolyClosedChestPrefab;
        public GameObject HighPolyOpenFullChestPrefab;
        public GameObject HighPolyOpenEmptyChestPrefab;
        public GameObject HighPolySmashedChestPrefab;
        public GameObject HighPolyDisintegratedChestPrefab;
        // Low Poly
        public GameObject LowPolyClosedChestPrefab;
        public GameObject LowPolyOpenFullChestPrefab;
        public GameObject LowPolyOpenEmptyChestPrefab;
        public GameObject LowPolySmashedChestPrefab;
        public GameObject LowPolyDisintegratedChestPrefab;
        // Model IDs
        public const int ClosedChestModelID = 47330;
        public const int OpenFullChestModelID = 47331;
        public const int OpenEmptyChestModelID = 47332;
        public const int SmashedChestModelID = 47333;
        public const int DisintegratedChestModelID = 47334;

        #region Mod Sound Variables

        // Mod Sounds
        private static AudioClip lastSoundPlayed = null;
        public static AudioClip[] UnarmedHitWoodLightClips = { null, null, null, null };
        public static AudioClip[] UnarmedHitWoodHardClips = { null, null, null };
        public static AudioClip[] BluntHitWoodLightClips = { null, null, null, null };
        public static AudioClip[] BluntHitWoodHardClips = { null, null, null, null };
        public static AudioClip[] BladeHitWoodLightClips = { null, null, null, null };
        public static AudioClip[] BladeHitWoodHardClips = { null, null, null };
        public static AudioClip[] UnarmedHitMetalClips = { null, null, null };
        public static AudioClip[] BluntHitMetalLightClips = { null, null, null, null };
        public static AudioClip[] BluntHitMetalHardClips = { null, null, null, null };
        public static AudioClip[] BladeHitMetalLightClips = { null, null, null, null };
        public static AudioClip[] BladeHitMetalHardClips = { null, null, null, null };
        public static AudioClip[] ArrowHitWoodClips = { null, null, null, null };
        public static AudioClip[] ArrowHitMetalClips = { null, null, null };
        public static AudioClip[] HitMetalLockClips = { null, null, null, null, null, null };
        public static AudioClip[] BashOpenWoodChestClips = { null, null, null, null };
        public static AudioClip[] BashOpenMetalChestClips = { null, null, null };
        public static AudioClip[] BashOffLockClips = { null, null, null, null };

        public static AudioClip[] ChestResistedSpellClips = { null, null, null, null, null };
        public static AudioClip[] ChestBlownOpenSpellClips = { null, null, null, null, null, null };
        public static AudioClip[] ChestDisintegratedSpellClips = { null, null, null };

        public static AudioClip[] LockpickAttemptClips = { null, null, null, null, null };
        public static AudioClip[] LockpickJammedClips = { null, null, null, null };
        public static AudioClip[] LockAlreadyJammedClips = { null, null, null };
        public static AudioClip[] LockpickSuccessfulClips = { null, null, null, null, null, null };

        public static AudioClip[] MagicLockpickAttemptClips = { null, null, null, null, null };
        public static AudioClip[] MagicLockpickJammedClips = { null, null, null, null };
        public static AudioClip[] MagicLockAlreadyJammedClips = { null, null, null };
        public static AudioClip[] MagicLockpickSuccessfulClips = { null, null, null, null, null, null };

        #endregion

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;
            var go = new GameObject(mod.Title);
            go.AddComponent<LockedLootContainersMain>(); // Add script to the scene.

            go.AddComponent<LLCObject>();

            mod.LoadSettingsCallback = LoadSettings; // To enable use of the "live settings changes" feature in-game.

            mod.IsReady = true;
        }

        private void Start()
        {
            Debug.Log("Begin mod init: Locked Loot Containers");

            Instance = this;

            mod.SaveDataInterface = ModSaveData;

            mod.LoadSettings();

            ModCompatibilityChecking();

            MainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            PlayerLayerMask = ~(1 << LayerMask.NameToLayer("Player"));

            DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(ItemMatScraps.templateIndex, ItemGroups.UselessItems1, typeof(ItemMatScraps));
            DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(ItemBrokenArrow.templateIndex, ItemGroups.UselessItems1, typeof(ItemBrokenArrow));
            DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(ItemTatteredCloth.templateIndex, ItemGroups.UselessItems1, typeof(ItemTatteredCloth));
            DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(ItemGlassFragments.templateIndex, ItemGroups.UselessItems1, typeof(ItemGlassFragments));
            DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(ItemPaperShreds.templateIndex, ItemGroups.UselessItems1, typeof(ItemPaperShreds));
            DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(ItemShinyRubble.templateIndex, ItemGroups.UselessItems1, typeof(ItemShinyRubble));
            DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(ItemIvoryFragments.templateIndex, ItemGroups.UselessItems1, typeof(ItemIvoryFragments));
            DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(ItemDestroyedJewelry.templateIndex, ItemGroups.UselessItems1, typeof(ItemDestroyedJewelry));
            DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(ItemRuinedCoin.templateIndex, ItemGroups.UselessItems1, typeof(ItemRuinedCoin));
            DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(ItemClumpofPlantMatter.templateIndex, ItemGroups.UselessItems1, typeof(ItemClumpofPlantMatter));
            DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(ItemGlobofGore.templateIndex, ItemGroups.UselessItems1, typeof(ItemGlobofGore));
            DaggerfallUnity.Instance.ItemHelper.RegisterCustomItem(ItemUselessRefuse.templateIndex, ItemGroups.UselessItems1, typeof(ItemUselessRefuse));

            DaggerfallWorkshop.Game.Formulas.FormulaHelper.RegisterOverride(mod, "IsItemStackable", (Func<DaggerfallUnityItem, bool>)LLCItemBuilder.IsItemStackable); // To make vanilla blank parchment items stackable.

            PlayerActivate.RegisterCustomActivation(mod, ClosedChestSpriteID, 0, ChestActivation);
            PlayerActivate.RegisterCustomActivation(mod, 41811, DoNothingActivation);
            PlayerActivate.RegisterCustomActivation(mod, 41812, DoNothingActivation);
            PlayerActivate.RegisterCustomActivation(mod, 41813, DoNothingActivation);

            PlayerActivate.RegisterCustomActivation(mod, ClosedChestModelID, ChestActivation);

            PlayerEnterExit.OnTransitionInterior += AddChests_OnTransitionInterior;
            PlayerEnterExit.OnTransitionDungeonInterior += AddChests_OnTransitionDungeonInterior;
            DaggerfallUI.UIManager.OnWindowChange += UIManager_ChangeChestGraphicOnInventoryWindowClosed;

            // Load Resources
            LoadTextures();
            LoadPrefabs();
            LoadAudio();

            RegisterLLCCommands();

            Debug.Log("Finished mod init: Locked Loot Containers");
        }

        private void Update()
        {
            if (GameManager.IsGamePaused)
                return;

            if (!WepManager.ScreenWeapon)
                return;

            if (!WepManager.ScreenWeapon.IsAttacking())
            {
                isBashReady = true; // This basically resets the "attack cooldown" so you don't get spammed hits during the entire attack animation.
                return;
            }

            if (isBashReady && WepManager.ScreenWeapon.GetCurrentFrame() == WepManager.ScreenWeapon.GetHitFrame())
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
                    DaggerfallUnityItem currentRightHandWeapon = Player.ItemEquipTable.GetItem(EquipSlots.RightHand);
                    DaggerfallUnityItem currentLeftHandWeapon = Player.ItemEquipTable.GetItem(EquipSlots.LeftHand);
                    DaggerfallUnityItem strikingWeapon = WepManager.UsingRightHand ? currentRightHandWeapon : currentLeftHandWeapon;

                    // Check if hit has a LLCObject component
                    LLCObject hitChest = hit.transform.gameObject.GetComponent<LLCObject>();
                    if (hitChest)
                        AttemptMeleeChestBash(hitChest, strikingWeapon);
                }
                isBashReady = false;
            }

            // Looking at the DaggerfallMissile.cs code under the "DoAreaOfEffect" method, I have a feeling without a PR, I won't be able to do a clean/easy
            // method to have the aoe of spell effects be detected for the chests. But I will have to see, either way not a big deal atm.
        }

        #region Settings

        private static void LoadSettings(ModSettings modSettings, ModSettingsChange change)
        {
            ChestGraphicType = mod.GetSettings().GetValue<int>("GraphicsSettings", "ChestGraphicType");
            AllowChestCollision = mod.GetSettings().GetValue<bool>("GraphicsSettings", "ChestCollisionToggle");
            AllowChestShadows = mod.GetSettings().GetValue<bool>("GraphicsSettings", "ChestShadowsToggle");

            AllowDungeonQuestMarkerChestSpawning = mod.GetSettings().GetValue<bool>("ChestGenerationSettings", "DungeonQuestMarkerChestSpawnToggle");
            DungeonQuestMarkerSpawnOdds = mod.GetSettings().GetValue<int>("ChestGenerationSettings", "DungeonQuestMarkerChestSpawnOdds");
            DungeonQuestMarkerLootMultiMinRange = mod.GetSettings().GetValue<int>("ChestGenerationSettings", "DungeonQuestMarkerLootModMinRange");
            DungeonQuestMarkerLootMultiMaxRange = mod.GetSettings().GetValue<int>("ChestGenerationSettings", "DungeonQuestMarkerLootModMaxRange");

            MegaChestLootMultiMinRange = mod.GetSettings().GetValue<int>("ChestGenerationSettings", "MegaChestLootModMinRange");
            MegaChestLootMultiMaxRange = mod.GetSettings().GetValue<int>("ChestGenerationSettings", "MegaChestLootModMaxRange");

            RemoveUnprotectedDungeonLootPiles = mod.GetSettings().GetValue<bool>("ChestGenerationSettings", "UnprotectedDungeonLootPilesToggle");
            PreserveDeletedLootPileItems = mod.GetSettings().GetValue<bool>("ChestGenerationSettings", "PreserveDeletedLootPileItemsToggle");
            DungeonChestsOnlySpawnAtQuestMarkers = mod.GetSettings().GetValue<bool>("ChestGenerationSettings", "ChestsOnlySpawnAtQuestMarkersToggle");
            SpawnSingularMegaChestInDungeons = mod.GetSettings().GetValue<bool>("ChestGenerationSettings", "SpawnSingularMegaChestInDungeonsToggle");
            MegaChestSpawnOdds = mod.GetSettings().GetValue<int>("ChestGenerationSettings", "MegaChestChestSpawnOdds");

            AllowCompatibilityWarnings = mod.GetSettings().GetValue<bool>("ErrorLoggingAndCompatibilitySettings", "AllowModCompatWarnings");
            AllowVerboseErrorLogging = mod.GetSettings().GetValue<bool>("ErrorLoggingAndCompatibilitySettings", "AllowVerboseErrorLogging");
            DoNotSpamExceptionsLogs = mod.GetSettings().GetValue<bool>("ErrorLoggingAndCompatibilitySettings", "DoNotSpamExceptionsLogs");

            if (DungeonQuestMarkerLootMultiMinRange >= DungeonQuestMarkerLootMultiMaxRange)
            {
                DungeonQuestMarkerLootMultiMaxRange = DungeonQuestMarkerLootMultiMinRange;
            }

            if (change.HasChanged("ErrorLoggingAndCompatibilitySettings", "DoNotSpamExceptionsLogs"))
            {
                exceptionLogging = true;
            }

            RefreshChestGraphics();
        }

        private static void RefreshChestGraphics() // Need to take into consideration chests that are in the "Smashed" or "Disintegrated" states as well, currently not the case.
        {
            if (!GameManager.Instance.StateManager.GameInProgress)
                return;

            if (GameManager.Instance.StateManager.LastState == StateManager.StateTypes.Game || GameManager.Instance.StateManager.LastState == StateManager.StateTypes.UI)
            {
                if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon || GameManager.Instance.PlayerEnterExit.IsPlayerInsideBuilding)
                {
                    LLCObject[] closedChests = GameObject.FindObjectsOfType<LLCObject>();
                    for (int i = 0; i < closedChests.Length; i++)
                    {
                        if (closedChests[i].LoadID <= 0)
                            continue;

                        LLCObject oldChest = closedChests[i];

                        if (ChestGraphicType == 0) // Use sprite based graphics for chests
                        {
                            GameObject chestGo = GameObjectHelper.CreateDaggerfallBillboardGameObject(ClosedChestSpriteID, 0, oldChest.transform.parent);

                            // Set position
                            Billboard dfBillboard = chestGo.GetComponent<Billboard>();
                            chestGo.transform.position = oldChest.transform.position;

                            LLCObject llcObjClone = chestGo.AddComponent<LLCObject>();
                            CloneLLCObjectProperties(llcObjClone, oldChest);
                            Destroy(oldChest.gameObject);
                        }
                        else // Use 3D models for chests
                        {
                            GameObject usedModelPrefab = (ChestGraphicType == 1) ? Instance.LowPolyClosedChestPrefab : Instance.HighPolyClosedChestPrefab;
                            GameObject chestGo = GameObjectHelper.InstantiatePrefab(usedModelPrefab, GameObjectHelper.GetGoModelName(ClosedChestModelID), oldChest.transform.parent, oldChest.transform.position);
                            chestGo.transform.rotation = oldChest.gameObject.transform.rotation;
                            Collider col = chestGo.AddComponent<BoxCollider>();
                            ToggleChestShadowsOrCollision(chestGo);
                            if (oldChest.transform.rotation.x == 0)
                                chestGo.transform.Rotate(-90f, 0f, UnityEngine.Random.Range(0, 9) * 45f);

                            LLCObject llcObjClone = chestGo.AddComponent<LLCObject>();
                            CloneLLCObjectProperties(llcObjClone, oldChest);
                            Destroy(oldChest.gameObject);
                        }
                    }

                    DaggerfallLoot[] allLoot = GameObject.FindObjectsOfType<DaggerfallLoot>();
                    List<DaggerfallLoot> openChestList = new List<DaggerfallLoot>();
                    for (int i = 0; i < allLoot.Length; i++)
                    {
                        if (allLoot[i].ContainerType == LootContainerTypes.Nothing && allLoot[i].ContainerImage == InventoryContainerImages.Chest && allLoot[i].customDrop == false)
                        {
                            openChestList.Add(allLoot[i]);
                        }
                    }

                    for (int i = 0; i < openChestList.Count; i++)
                    {
                        if (openChestList[i].LoadID <= 0)
                            continue;

                        DaggerfallLoot oldLootPile = openChestList[i];

                        int spriteOrModelID = 0;
                        if (oldLootPile.TextureArchive == SmashedChestSpriteID || oldLootPile.TextureArchive == DisintegratedChestSpriteID || oldLootPile.TextureRecord == SmashedChestModelID || oldLootPile.TextureRecord == DisintegratedChestModelID)
                        {
                            if (oldLootPile.TextureArchive == DisintegratedChestSpriteID || oldLootPile.TextureRecord == DisintegratedChestModelID)
                                spriteOrModelID = ChestGraphicType == 0 ? DisintegratedChestSpriteID : DisintegratedChestModelID;
                            else
                                spriteOrModelID = ChestGraphicType == 0 ? SmashedChestSpriteID : SmashedChestModelID;
                        }
                        else
                        {
                            if (oldLootPile.Items.Count == 0)
                                spriteOrModelID = ChestGraphicType == 0 ? OpenEmptyChestSpriteID : OpenEmptyChestModelID;
                            else
                                spriteOrModelID = ChestGraphicType == 0 ? OpenFullChestSpriteID : OpenFullChestModelID;
                        }

                        if (ChestGraphicType == 0) // Use sprite based graphics for chests
                        {
                            DaggerfallLoot openChestLoot = GameObjectHelper.CreateLootContainer(LootContainerTypes.Nothing, InventoryContainerImages.Chest, oldLootPile.gameObject.transform.position, oldLootPile.gameObject.transform.parent, spriteOrModelID, 0, oldLootPile.LoadID, null, false);
                            openChestLoot.gameObject.name = GameObjectHelper.GetGoFlatName(spriteOrModelID, 0);
                            openChestLoot.Items.TransferAll(oldLootPile.Items);
                            Destroy(openChestLoot.GetComponent<SerializableLootContainer>());
                            Destroy(oldLootPile.gameObject);
                        }
                        else // Use 3D models for chests
                        {
                            GameObject usedModelPrefab = null;
                            if (spriteOrModelID == DisintegratedChestModelID)
                                usedModelPrefab = ChestGraphicType == 1 ? Instance.LowPolyDisintegratedChestPrefab : Instance.HighPolyDisintegratedChestPrefab;
                            else if (spriteOrModelID == SmashedChestModelID)
                                usedModelPrefab = ChestGraphicType == 1 ? Instance.LowPolySmashedChestPrefab : Instance.HighPolySmashedChestPrefab;
                            else if (oldLootPile.Items.Count == 0)
                                usedModelPrefab = ChestGraphicType == 1 ? Instance.LowPolyOpenEmptyChestPrefab : Instance.HighPolyOpenEmptyChestPrefab;
                            else
                                usedModelPrefab = ChestGraphicType == 1 ? Instance.LowPolyOpenFullChestPrefab : Instance.HighPolyOpenFullChestPrefab;

                            GameObject chestGo = GameObjectHelper.InstantiatePrefab(usedModelPrefab, GameObjectHelper.GetGoModelName((uint)spriteOrModelID), oldLootPile.gameObject.transform.parent, oldLootPile.gameObject.transform.position);
                            chestGo.transform.rotation = oldLootPile.gameObject.transform.rotation;
                            if (spriteOrModelID == SmashedChestModelID| spriteOrModelID == DisintegratedChestModelID) { if (oldLootPile.transform.rotation.x != 0) { RotateBackToZero(chestGo.transform); } }
                            else { if (oldLootPile.transform.rotation.x == 0) { chestGo.transform.Rotate(-90f, 0f, UnityEngine.Random.Range(0, 9) * 45f); } }
                            Collider col = chestGo.AddComponent<BoxCollider>();
                            DaggerfallLoot chestLoot = chestGo.AddComponent<DaggerfallLoot>();
                            ToggleChestShadowsOrCollision(chestGo);
                            if (chestLoot)
                            {
                                chestLoot.ContainerType = LootContainerTypes.Nothing;
                                chestLoot.ContainerImage = InventoryContainerImages.Chest;
                                chestLoot.LoadID = oldLootPile.LoadID;
                                chestLoot.TextureRecord = spriteOrModelID;
                                chestLoot.Items.TransferAll(oldLootPile.Items);
                            }
                            Destroy(oldLootPile.gameObject);
                        }
                    }
                }
            }
        }

        #endregion

        public static TextFile.Token[] TextTokenFromRawString(string rawString)
        {
            var listOfCompLines = new List<string>();
            int partLength = 115;
            if (!DaggerfallUnity.Settings.SDFFontRendering)
                partLength = 65;
            string sentence = rawString;
            string[] words = sentence.Split(' ');
            var parts = new Dictionary<int, string>();
            string part = string.Empty;
            int partCounter = 0;
            foreach (var word in words)
            {
                if (part.Length + word.Length < partLength)
                {
                    part += string.IsNullOrEmpty(part) ? word : " " + word;
                }
                else
                {
                    parts.Add(partCounter, part);
                    part = word;
                    partCounter++;
                }
            }
            parts.Add(partCounter, part);

            foreach (var item in parts)
            {
                listOfCompLines.Add(item.Value);
            }

            return DaggerfallUnity.Instance.TextProvider.CreateTokens(TextFile.Formatting.JustifyCenter, listOfCompLines.ToArray());
        }

        public static void CreateScreenWrappedHudText(TextFile.Token[] tokens, float textDelay = 3f)
        {
            if (tokens != null && tokens.Length > 0)
            {
                DaggerfallUI.AddHUDText(tokens, textDelay);
            }
        }

        private void ModCompatibilityChecking()
        {
            Mod repairTools = ModManager.Instance.GetMod("RepairTools");
            RepairToolsCheck = repairTools != null ? true : false;

            Mod jewelryAdditions = ModManager.Instance.GetMod("JewelryAdditions");
            JewelryAdditionsCheck = jewelryAdditions != null ? true : false;

            Mod climatesAndCalories = ModManager.Instance.GetMod("Climates & Calories");
            ClimatesAndCaloriesCheck = climatesAndCalories != null ? true : false;

            Mod skillBooks = ModManager.Instance.GetMod("Skill Books");
            SkillBooksCheck = skillBooks != null ? true : false;

            Mod roleplayRealismItems = ModManager.Instance.GetMod("RoleplayRealism-Items");
            if (roleplayRealismItems != null)
            {
                ModSettings rolePlayRealismSettings = roleplayRealismItems.GetSettings();
                RolePlayRealismLootRebalanceCheck = rolePlayRealismSettings.GetBool("Modules", "lootRebalance");
                RolePlayRealismBandagingCheck = rolePlayRealismSettings.GetBool("Modules", "bandaging");
                RolePlayRealismNewWeaponCheck = rolePlayRealismSettings.GetBool("Modules", "newWeapons");
                RolePlayRealismNewArmorCheck = rolePlayRealismSettings.GetBool("Modules", "newArmor");
            }

            // Tempered Interiors mod: https://www.nexusmods.com/daggerfallunity/mods/392
            Mod temperedInteriors = ModManager.Instance.GetModFromGUID("a1ec918d-fad6-4050-99ec-d07043a0308a");
            TemperedInteriorsCheck = temperedInteriors != null ? true : false;
        }

        public static void LogModException(Exception e, byte errorType = 0)
        {
            string errorName = "something generic was occuring";

            switch (errorType)
            {
                case 1: errorName = "transitioning into an interior"; break;
                case 2: errorName = "transitioning to an exterior"; break;
                case 3: errorName = "transitioning into a dungeon"; break;
                case 4: errorName = "the inventory window was being closed"; break;
                case 5: errorName = "an existing save was being loaded"; break;
                case 6: errorName = "a new character was just created"; break;
                case 0:
                default:
                    break;
            }

            if (exceptionLogging)
            {
                if (AllowVerboseErrorLogging)
                {
                    Debug.LogErrorFormat("[ERROR] Locked Loot Containers: Exception has occured while {0}: " + e.ToString(), errorName);
                    if (errorType == 1 && TemperedInteriorsCheck)
                    {
                        Debug.LogWarning("[Warning] Locked Loot Containers: The 'Tempered Interiors' mod appears to be active, this may be the cause of the above exception");
                    }
                }
                else
                {
                    Debug.LogFormat("[ERROR] Locked Loot Containers: Exception has occured while {0}", errorName);
                    if (errorType == 1 && TemperedInteriorsCheck)
                    {
                        Debug.Log("[Warning] Locked Loot Containers: The 'Tempered Interiors' mod appears to be active, this may be the cause of the above exception");
                    }
                }

                if (DoNotSpamExceptionsLogs)
                {
                    exceptionLogging = false;
                }
            }
        }

        #region Load Audio Clips

        private void LoadAudio() // Example taken from Penwick Papers Mod
        {
            ModManager modManager = ModManager.Instance;
            bool success = true;

            // Bashing
            success &= modManager.TryGetAsset("WoW_UnarmedHitWoodShield_3", false, out UnarmedHitWoodLightClips[0]);
            success &= modManager.TryGetAsset("WoW_UnarmedHitWoodShield_6", false, out UnarmedHitWoodLightClips[1]);
            success &= modManager.TryGetAsset("WoW_UnarmedHitWoodShield_7", false, out UnarmedHitWoodLightClips[2]);
            success &= modManager.TryGetAsset("WoW_UnarmedHitWoodShield_8", false, out UnarmedHitWoodLightClips[3]);

            success &= modManager.TryGetAsset("WoW_UnarmedHitWoodShield_1", false, out UnarmedHitWoodHardClips[0]);
            success &= modManager.TryGetAsset("WoW_UnarmedHitWoodShield_2", false, out UnarmedHitWoodHardClips[1]);
            success &= modManager.TryGetAsset("WoW_UnarmedHitWoodShield_5", false, out UnarmedHitWoodHardClips[2]);

            success &= modManager.TryGetAsset("WoW_2hMaceHitWoodShield_1", false, out BluntHitWoodLightClips[0]);
            success &= modManager.TryGetAsset("WoW_2hMaceHitWoodShield_2", false, out BluntHitWoodLightClips[1]);
            success &= modManager.TryGetAsset("WoW_2hMaceHitWoodShield_3", false, out BluntHitWoodLightClips[2]);
            success &= modManager.TryGetAsset("WoW_2hMaceHitWoodShield_4", false, out BluntHitWoodLightClips[3]);

            success &= modManager.TryGetAsset("WoW_WoodenShieldBlock_1", false, out BluntHitWoodHardClips[0]);
            success &= modManager.TryGetAsset("WoW_WoodenShieldBlock_2", false, out BluntHitWoodHardClips[1]);
            success &= modManager.TryGetAsset("WoW_WoodenShieldBlock_3", false, out BluntHitWoodHardClips[2]);
            success &= modManager.TryGetAsset("WoW_WoodenShieldBlock_4", false, out BluntHitWoodHardClips[3]);

            success &= modManager.TryGetAsset("WoW_1HAxeHitWood_1", false, out BladeHitWoodLightClips[0]);
            success &= modManager.TryGetAsset("WoW_1HAxeHitWood_2", false, out BladeHitWoodLightClips[1]);
            success &= modManager.TryGetAsset("WoW_1HAxeHitWood_3", false, out BladeHitWoodLightClips[2]);
            success &= modManager.TryGetAsset("WoW_1HAxeHitWood_4", false, out BladeHitWoodLightClips[3]);

            success &= modManager.TryGetAsset("WoW_1HAxeHitWood_5", false, out BladeHitWoodHardClips[0]);
            success &= modManager.TryGetAsset("WoW_1HAxeHitWood_6", false, out BladeHitWoodHardClips[1]);
            success &= modManager.TryGetAsset("WoW_1HAxeHitWood_7", false, out BladeHitWoodHardClips[2]);

            success &= modManager.TryGetAsset("WoW_UnarmedHitMetalShield_1", false, out UnarmedHitMetalClips[0]);
            success &= modManager.TryGetAsset("WoW_UnarmedHitMetalShield_2", false, out UnarmedHitMetalClips[1]);
            success &= modManager.TryGetAsset("WoW_UnarmedHitMetalShield_3", false, out UnarmedHitMetalClips[2]);

            success &= modManager.TryGetAsset("WoW_2hMaceHitMetalShield_1", false, out BluntHitMetalLightClips[0]);
            success &= modManager.TryGetAsset("WoW_2hMaceHitMetalShield_2", false, out BluntHitMetalLightClips[1]);
            success &= modManager.TryGetAsset("WoW_2hMaceHitMetalShield_3", false, out BluntHitMetalLightClips[2]);
            success &= modManager.TryGetAsset("WoW_2hMaceHitMetalShield_4", false, out BluntHitMetalLightClips[3]);

            success &= modManager.TryGetAsset("WoW_2hMaceHitMetalShield_5", false, out BluntHitMetalHardClips[0]);
            success &= modManager.TryGetAsset("WoW_2hMaceHitMetalShield_6", false, out BluntHitMetalHardClips[1]);
            success &= modManager.TryGetAsset("WoW_2hMaceHitMetalShield_7", false, out BluntHitMetalHardClips[2]);
            success &= modManager.TryGetAsset("WoW_2hMaceHitMetalShield_8", false, out BluntHitMetalHardClips[3]);

            success &= modManager.TryGetAsset("WoW_1HDaggerHitMetalShield_1", false, out BladeHitMetalLightClips[0]);
            success &= modManager.TryGetAsset("WoW_1HDaggerHitMetalShield_2", false, out BladeHitMetalLightClips[1]);
            success &= modManager.TryGetAsset("WoW_1HDaggerHitMetalShield_3", false, out BladeHitMetalLightClips[2]);
            success &= modManager.TryGetAsset("WoW_1HDaggerHitMetalShield_4", false, out BladeHitMetalLightClips[3]);

            success &= modManager.TryGetAsset("WoW_2hAxeHitMetalShield_1", false, out BladeHitMetalHardClips[0]);
            success &= modManager.TryGetAsset("WoW_2hAxeHitMetalShield_2", false, out BladeHitMetalHardClips[1]);
            success &= modManager.TryGetAsset("WoW_2hAxeHitMetalShield_3", false, out BladeHitMetalHardClips[2]);
            success &= modManager.TryGetAsset("WoW_2hAxeHitMetalShield_4", false, out BladeHitMetalHardClips[3]);

            success &= modManager.TryGetAsset("WoW_ArrowImpactsShieldBlock_1", false, out ArrowHitWoodClips[0]);
            success &= modManager.TryGetAsset("WoW_ArrowImpactsShieldBlock_2", false, out ArrowHitWoodClips[1]);
            success &= modManager.TryGetAsset("WoW_ArrowImpactsShieldBlock_3", false, out ArrowHitWoodClips[2]);
            success &= modManager.TryGetAsset("WoW_ArrowImpactsShieldBlock_4", false, out ArrowHitWoodClips[3]);

            success &= modManager.TryGetAsset("ArrowPingsOffMetal_1", false, out ArrowHitMetalClips[0]);
            success &= modManager.TryGetAsset("ArrowPingsOffMetal_2", false, out ArrowHitMetalClips[1]);
            success &= modManager.TryGetAsset("ArrowPingsOffMetal_3", false, out ArrowHitMetalClips[2]);

            success &= modManager.TryGetAsset("WoW_1HMaceNPCHitShieldMetal_10", false, out HitMetalLockClips[0]);
            success &= modManager.TryGetAsset("WoW_EquippingPlateSound_1", false, out HitMetalLockClips[1]);
            success &= modManager.TryGetAsset("WoW_HittingChainSound_2", false, out HitMetalLockClips[2]);
            success &= modManager.TryGetAsset("WoW_1HAxeHitChain_1", false, out HitMetalLockClips[3]);
            success &= modManager.TryGetAsset("WoW_1HAxeHitChain_2", false, out HitMetalLockClips[4]);
            success &= modManager.TryGetAsset("WoW_1HAxeHitChain_3", false, out HitMetalLockClips[5]);

            success &= modManager.TryGetAsset("WoW_Bomb_Explode_Pumpkin_1", false, out BashOpenWoodChestClips[0]);
            success &= modManager.TryGetAsset("WoW_Bomb_Explode_Pumpkin_2", false, out BashOpenWoodChestClips[1]);
            success &= modManager.TryGetAsset("WoW_Bomb_Explode_Pumpkin_3", false, out BashOpenWoodChestClips[2]);
            success &= modManager.TryGetAsset("WoW_WoodenShieldBlock_1", false, out BashOpenWoodChestClips[3]);

            success &= modManager.TryGetAsset("Rocky_Smash_Sound_1", false, out BashOpenMetalChestClips[0]);
            success &= modManager.TryGetAsset("Rocky_Smash_Sound_2", false, out BashOpenMetalChestClips[1]);
            success &= modManager.TryGetAsset("Rocky_Smash_Sound_3", false, out BashOpenMetalChestClips[2]);

            success &= modManager.TryGetAsset("WoW_HittingChainSound_1", false, out BashOffLockClips[0]);
            success &= modManager.TryGetAsset("WoW_HittingChainSound_3", false, out BashOffLockClips[1]);
            success &= modManager.TryGetAsset("WoW_HittingChainSound_5", false, out BashOffLockClips[2]);
            success &= modManager.TryGetAsset("WoW_Warrior_ShieldBreak_1", false, out BashOffLockClips[3]);

            // Spell Impacts
            success &= modManager.TryGetAsset("WoW_ShadowSpellFizzleSound_1", false, out ChestResistedSpellClips[0]);
            success &= modManager.TryGetAsset("WoW_FireSpellFizzleSound_1", false, out ChestResistedSpellClips[1]);
            success &= modManager.TryGetAsset("WoW_HolySpellFizzleSound_1", false, out ChestResistedSpellClips[2]);
            success &= modManager.TryGetAsset("WoW_SlightlyMagicalWooshSound_1", false, out ChestResistedSpellClips[3]);
            success &= modManager.TryGetAsset("WoW_SlightlyMagicalWooshSound_2", false, out ChestResistedSpellClips[4]);

            success &= modManager.TryGetAsset("WoW_DynamiteExplodes_1", false, out ChestBlownOpenSpellClips[0]);
            success &= modManager.TryGetAsset("WoW_DynamiteExplodes_2", false, out ChestBlownOpenSpellClips[1]);
            success &= modManager.TryGetAsset("WoW_DynamiteExplodes_3", false, out ChestBlownOpenSpellClips[2]);
            success &= modManager.TryGetAsset("WoW_DynamiteExplodes_4", false, out ChestBlownOpenSpellClips[3]);
            success &= modManager.TryGetAsset("WoW_DynamiteExplodes_5", false, out ChestBlownOpenSpellClips[4]);
            success &= modManager.TryGetAsset("WoW_DynamiteExplodes_6", false, out ChestBlownOpenSpellClips[5]);

            success &= modManager.TryGetAsset("Disintegration_1", false, out ChestDisintegratedSpellClips[0]);
            success &= modManager.TryGetAsset("Disintegration_2", false, out ChestDisintegratedSpellClips[1]);
            success &= modManager.TryGetAsset("Disintegration_3", false, out ChestDisintegratedSpellClips[2]);

            // Lockpicking
            success &= modManager.TryGetAsset("Lockpick_Attempt_1", false, out LockpickAttemptClips[0]);
            success &= modManager.TryGetAsset("Lockpick_Attempt_2", false, out LockpickAttemptClips[1]);
            success &= modManager.TryGetAsset("Lockpick_Attempt_3", false, out LockpickAttemptClips[2]);
            success &= modManager.TryGetAsset("Lockpick_Attempt_4", false, out LockpickAttemptClips[3]);
            success &= modManager.TryGetAsset("Lockpick_Attempt_5", false, out LockpickAttemptClips[4]);

            success &= modManager.TryGetAsset("Lockpick_Jammed_Lock_1", false, out LockpickJammedClips[0]);
            success &= modManager.TryGetAsset("Lockpick_Jammed_Lock_2", false, out LockpickJammedClips[1]);
            success &= modManager.TryGetAsset("Lockpick_Jammed_Lock_3", false, out LockpickJammedClips[2]);
            success &= modManager.TryGetAsset("Lockpick_Jammed_Lock_4", false, out LockpickJammedClips[3]);

            success &= modManager.TryGetAsset("Lock_Already_Jammed_1", false, out LockAlreadyJammedClips[0]);
            success &= modManager.TryGetAsset("Lock_Already_Jammed_2", false, out LockAlreadyJammedClips[1]);
            success &= modManager.TryGetAsset("Lock_Already_Jammed_3", false, out LockAlreadyJammedClips[2]);

            success &= modManager.TryGetAsset("Lockpick_Successful_1", false, out LockpickSuccessfulClips[0]);
            success &= modManager.TryGetAsset("Lockpick_Successful_2", false, out LockpickSuccessfulClips[1]);
            success &= modManager.TryGetAsset("Lockpick_Successful_3", false, out LockpickSuccessfulClips[2]);
            success &= modManager.TryGetAsset("Lockpick_Successful_4", false, out LockpickSuccessfulClips[3]);
            success &= modManager.TryGetAsset("Lockpick_Successful_5", false, out LockpickSuccessfulClips[4]);
            success &= modManager.TryGetAsset("Lockpick_Successful_6", false, out LockpickSuccessfulClips[5]);

            // Magic Lockpicking / Open Effect
            success &= modManager.TryGetAsset("Magic_Lockpick_Attempt_Failed_1", false, out MagicLockpickAttemptClips[0]);
            success &= modManager.TryGetAsset("Magic_Lockpick_Attempt_Failed_2", false, out MagicLockpickAttemptClips[1]);
            success &= modManager.TryGetAsset("Magic_Lockpick_Attempt_Failed_3", false, out MagicLockpickAttemptClips[2]);
            success &= modManager.TryGetAsset("Magic_Lockpick_Attempt_Failed_4", false, out MagicLockpickAttemptClips[3]);
            success &= modManager.TryGetAsset("Magic_Lockpick_Attempt_Failed_5", false, out MagicLockpickAttemptClips[4]);

            success &= modManager.TryGetAsset("Magic_Lockpick_Jammed_Lock_1", false, out MagicLockpickJammedClips[0]);
            success &= modManager.TryGetAsset("Magic_Lockpick_Jammed_Lock_2", false, out MagicLockpickJammedClips[1]);
            success &= modManager.TryGetAsset("Magic_Lockpick_Jammed_Lock_3", false, out MagicLockpickJammedClips[2]);
            success &= modManager.TryGetAsset("Magic_Lockpick_Jammed_Lock_4", false, out MagicLockpickJammedClips[3]);

            success &= modManager.TryGetAsset("Magic_Lock_Already_Jammed_1", false, out MagicLockAlreadyJammedClips[0]);
            success &= modManager.TryGetAsset("Magic_Lock_Already_Jammed_2", false, out MagicLockAlreadyJammedClips[1]);
            success &= modManager.TryGetAsset("Magic_Lock_Already_Jammed_3", false, out MagicLockAlreadyJammedClips[2]);

            success &= modManager.TryGetAsset("Lockpick_Successful_Magic_1", false, out MagicLockpickSuccessfulClips[0]);
            success &= modManager.TryGetAsset("Lockpick_Successful_Magic_2", false, out MagicLockpickSuccessfulClips[1]);
            success &= modManager.TryGetAsset("Lockpick_Successful_Magic_3", false, out MagicLockpickSuccessfulClips[2]);
            success &= modManager.TryGetAsset("Lockpick_Successful_Magic_4", false, out MagicLockpickSuccessfulClips[3]);
            success &= modManager.TryGetAsset("Lockpick_Successful_Magic_5", false, out MagicLockpickSuccessfulClips[4]);
            success &= modManager.TryGetAsset("Lockpick_Successful_Magic_6", false, out MagicLockpickSuccessfulClips[5]);

            if (!success)
                throw new Exception("LockedLootContainers: Missing sound asset");
        }

        #endregion

        private void LoadTextures() // Example taken from Penwick Papers Mod
        {
            ModManager modManager = ModManager.Instance;
            bool success = true;

            success &= modManager.TryGetAsset("Chest_Choice_Menu", false, out ChestChoiceMenuTexture);
            success &= modManager.TryGetAsset("Inspection_Info_GUI", false, out InspectionInfoGUITexture);

            if (!success)
                throw new Exception("LockedLootContainers: Missing texture asset");
        }

        private void LoadPrefabs() // Example taken from Penwick Papers Mod
        {
            ModManager modManager = ModManager.Instance;
            bool success = true;
            
            success &= modManager.TryGetAsset("chest_close", false, out LowPolyClosedChestPrefab);
            success &= modManager.TryGetAsset("chest_open_full", false, out LowPolyOpenFullChestPrefab);
            success &= modManager.TryGetAsset("chest_open_empty", false, out LowPolyOpenEmptyChestPrefab);
            success &= modManager.TryGetAsset("BrokenChestPilePrefab1", false, out LowPolySmashedChestPrefab);
            success &= modManager.TryGetAsset("DisintegratedChestPilePrefab1", false, out LowPolyDisintegratedChestPrefab);

            success &= modManager.TryGetAsset("chest_close", false, out HighPolyClosedChestPrefab);
            success &= modManager.TryGetAsset("chest_open_full", false, out HighPolyOpenFullChestPrefab);
            success &= modManager.TryGetAsset("chest_open_empty", false, out HighPolyOpenEmptyChestPrefab);
            success &= modManager.TryGetAsset("BrokenChestPilePrefab1", false, out HighPolySmashedChestPrefab);
            success &= modManager.TryGetAsset("DisintegratedChestPilePrefab1", false, out HighPolyDisintegratedChestPrefab);

            if (!success)
                throw new Exception("LockedLootContainers: Missing prefab/model asset");
        }

        public static void RegisterLLCCommands()
        {
            Debug.Log("[LockedLootContainers] Trying to register console commands.");
            try
            {
                ConsoleCommandsDatabase.RegisterCommand(TeleportToRandomChest.command, TeleportToRandomChest.description, TeleportToRandomChest.usage, TeleportToRandomChest.Execute);
                ConsoleCommandsDatabase.RegisterCommand(TeleportToRandomQuestMarker.command, TeleportToRandomQuestMarker.description, TeleportToRandomQuestMarker.usage, TeleportToRandomQuestMarker.Execute);
                ConsoleCommandsDatabase.RegisterCommand(AlwaysSpawnChests.command, AlwaysSpawnChests.description, AlwaysSpawnChests.usage, AlwaysSpawnChests.Execute);
                //ConsoleCommandsDatabase.RegisterCommand(LLCSoundTest.command, LLCSoundTest.description, LLCSoundTest.usage, LLCSoundTest.Execute);
                ConsoleCommandsDatabase.RegisterCommand(MakeJunkItems.command, MakeJunkItems.description, MakeJunkItems.usage, MakeJunkItems.Execute);
                //ConsoleCommandsDatabase.RegisterCommand(ChangeButtonRect.command, ChangeButtonRect.description, ChangeButtonRect.usage, ChangeButtonRect.Execute);
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Error Registering LockLootContainers Console commands: {0}", e.Message));
            }
        }

        private static class TeleportToRandomChest // Just did some testing, and besides sometimes being dropped into the void, appears to work perfectly fine for testing purposes.
        {
            public static readonly string command = "tele2chest";
            public static readonly string description = "Chooses random chest object from current scene and teleports player to its location.";
            public static readonly string usage = "tele2chest";

            public static string Execute(params string[] args)
            {
                LLCObject[] gos = FindObjectsOfType<LLCObject>();
                List<LLCObject> chestList = new List<LLCObject>();
                for (int i = 0; i < gos.Length; i++)
                {
                    if (gos[i].ChestStartHP == 0 && gos[i].LockStartHP == 0) // Meant to remove the actual LLCObject that is the base of them all but not an actual chests.
                        continue;
                    else
                        chestList.Add(gos[i]);
                }
                LLCObject[] chests = chestList.ToArray();

                if (chests.Length <= 0)
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

        private static class TeleportToRandomQuestMarker
        {
            public static readonly string command = "tele2randomquestmarker";
            public static readonly string description = "Chooses random quest marker in current dungeon and teleports player onto it.";
            public static readonly string usage = "tele2randomquestmarker";

            public static string Execute(params string[] args)
            {
                DaggerfallConnect.DFLocation locationData = GameManager.Instance.PlayerGPS.CurrentLocation;
                QuestMarkerChestSpawnPoints[] questMarkerSpawnPoints = MakeListOfQuestMarkerChestSpawnPoints(locationData);

                if (questMarkerSpawnPoints.Length > 0)
                {
                    // Teleport Player To Random Marker Location From This List.
                    int index = UnityEngine.Random.Range(0, questMarkerSpawnPoints.Length);
                    QuestMarkerChestSpawnPoints selectedMarker = questMarkerSpawnPoints[index];

                    Vector3 dungeonBlockPosition = new Vector3(selectedMarker.dungeonX * RDBLayout.RDBSide, 0, selectedMarker.dungeonZ * RDBLayout.RDBSide);
                    Vector3 pos = dungeonBlockPosition + selectedMarker.flatPosition;

                    if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon)
                    {
                        GameManager.Instance.PlayerObject.transform.localPosition = pos;
                        GameManager.Instance.PlayerMotor.FixStanding();
                    }
                    else if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideBuilding)
                    {
                        GameManager.Instance.PlayerObject.transform.position = pos;
                        GameManager.Instance.PlayerMotor.FixStanding();
                    }
                    else
                    {
                        return "Error: Not currently in a building interior or dungeon.";
                    }
                }
                else
                {
                    return "Error: No valid quest markers in this location.";
                }

                return "Teleport Finished.";
            }
        }

        private static class AlwaysSpawnChests
        {
            public static readonly string command = "forcespawnchest";
            public static readonly string description = "Toggle To Force All Loot-piles To Be Replaced By LLC Chests, No Matter The Odds, For Testing.";
            public static readonly string usage = "forcespawnchest";

            public static string Execute(params string[] args)
            {
                if (ForceChestSpawnToggle == false)
                {
                    ForceChestSpawnToggle = true;
                    return "ON: Chests will now ALWAYS replace loot-piles.";
                }
                else
                {
                    ForceChestSpawnToggle = false;
                    return "OFF: Chests will no longer always replace loot-piles.";
                }
            }
        }

        // Will disable this for the live mod since it's only really useful for testing.
        /*
        private static class LLCSoundTest
        {
            public static readonly string command = "llcsound";
            public static readonly string description = "Allowed playing of mod's sound-clips for testing purposes.";
            public static readonly string usage = "llcsound [groupName] [clipIndex] [volumeMod]";

            public static string Execute(params string[] args)
            {
                string errorText = "Error: something went wrong";
                string group = args[0];
                float volume = 1f;
                AudioClip[] clips = { null };

                if (args.Length < 2 || args.Length > 3) return "Invalid entry, see usage notes.";

                if (!int.TryParse(args[1], out int clipIndex))
                    return string.Format("`{0}` is not a number, please use a number for [clipIndex].", args[1]);

                if (args.Length == 3)
                {
                    if (!float.TryParse(args[2], out volume))
                        return string.Format("`{0}` is not a number, please use a number for [volumeMod].", args[2]);
                }

                if (clipIndex < 0)
                    clipIndex = 0;

                if (Player != null)
                {
                    if (args[0] == "lastsound") { clips[0] = lastSoundPlayed; }
                    else if (args[0] == "unarmedhitwoodlight") { clips = UnarmedHitWoodLightClips; }
                    else if (args[0] == "unarmedhitwoodhard") { clips = UnarmedHitWoodHardClips; }
                    else if (args[0] == "unarmedhitmetal") { clips = UnarmedHitMetalClips; }
                    else if (args[0] == "blunthitwoodlight") { clips = BluntHitWoodLightClips; }
                    else if (args[0] == "blunthitwoodhard") { clips = BluntHitWoodHardClips; }
                    else if (args[0] == "blunthitmetallight") { clips = BluntHitMetalLightClips; }
                    else if (args[0] == "blunthitmetalhard") { clips = BluntHitMetalHardClips; }
                    else if (args[0] == "bladehitwoodlight") { clips = BladeHitWoodLightClips; }
                    else if (args[0] == "bladehitwoodhard") { clips = BladeHitWoodHardClips; }
                    else if (args[0] == "bladehitmetallight") { clips = BladeHitMetalLightClips; }
                    else if (args[0] == "bladehitmetalhard") { clips = BladeHitMetalHardClips; }
                    else if (args[0] == "arrowhitwood") { clips = ArrowHitWoodClips; }
                    else if (args[0] == "arrowhitmetal") { clips = ArrowHitMetalClips; }
                    else if (args[0] == "hitmetallock") { clips = HitMetalLockClips; }
                    else if (args[0] == "bashopenwood") { clips = BashOpenWoodChestClips; }
                    else if (args[0] == "bashopenmetal") { clips = BashOpenMetalChestClips; }
                    else if (args[0] == "bashofflock") { clips = BashOffLockClips; }
                    else if (args[0] == "resistspell") { clips = ChestResistedSpellClips; }
                    else if (args[0] == "blownopen") { clips = ChestBlownOpenSpellClips; }
                    else if (args[0] == "turntodust") { clips = ChestDisintegratedSpellClips; }
                    else if (args[0] == "lockpickfailed") { clips = LockpickAttemptClips; }
                    else if (args[0] == "lockpickjammed") { clips = LockpickJammedClips; }
                    else if (args[0] == "lockstilljammed") { clips = LockAlreadyJammedClips; }
                    else if (args[0] == "lockpickworked") { clips = LockpickSuccessfulClips; }
                    else if (args[0] == "magiclockpickfailed") { clips = MagicLockpickAttemptClips; }
                    else if (args[0] == "magiclockpickjammed") { clips = MagicLockpickJammedClips; }
                    else if (args[0] == "magiclockstilljammed") { clips = MagicLockAlreadyJammedClips; }
                    else if (args[0] == "magiclockpickworked") { clips = MagicLockpickSuccessfulClips; }
                    else { return "Error: invalid soundclip group name"; }
                }

                if (clips.Length <= 0)
                    return "Error: soundclip group is empty";

                if (clips.Length == 1)
                {
                    clipIndex = 0;
                    if (clips[0] == null)
                        return "Error: last soundclip is null";
                }
                else
                {
                    if (clips.Length <= clipIndex)
                    {
                        clipIndex = clips.Length - 1;
                        errorText = string.Format("clipIndex entry larger than clip group, using max index {0} instead", clipIndex);
                    }
                }

                if (clips[clipIndex] == null)
                    return "Error: that clip group entry is null or empty";

                if (DaggerfallUI.Instance.DaggerfallAudioSource != null)
                {
                    DaggerfallUI.Instance.AudioSource.PlayOneShot(clips[clipIndex], volume * DaggerfallUnity.Settings.SoundVolume);
                    errorText = string.Format("Played entry {0}, of clip group {1}, at {2}x volume, clip name was: {3}", clipIndex, args[0], volume, clips[clipIndex].name);
                }

                return errorText;
            }
        }
        */

        private static class MakeJunkItems
        {
            public static readonly string command = "addjunkLLC";
            public static readonly string description = "Spawns LLC Junk Items.";
            public static readonly string usage = "addjunkLLC";

            public static string Execute(params string[] args)
            {
                for (int i = 0; i < 11; i++)
                {
                    if (i == 0)
                    {
                        DaggerfallUnityItem item = LLCItemBuilder.CreateScrapMaterial(WeaponMaterialTypes.None, ArmorMaterialTypes.Leather);
                        Player.Items.AddItem(item);
                    }
                    else
                    {
                        DaggerfallUnityItem item = LLCItemBuilder.CreateScrapMaterial((WeaponMaterialTypes)(i - 1));
                        Player.Items.AddItem(item);
                    }
                }

                for (int i = 0; i < 11; i++)
                {
                    DaggerfallUnityItem item = ItemBuilder.CreateItem(ItemGroups.UselessItems2, 4722 + i);
                    Player.Items.AddItem(item);
                }

                return "Gave you ALL the junk items.";
            }
        }

        // Will likely have use for this console command when working with more interface stuff in the future
        /*
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
        */

        // Triggered when the DaggerfallInventoryWindow UI window is closed
        public static void UIManager_ChangeChestGraphicOnInventoryWindowClosed(object sender, EventArgs e)
        {
            if (!GameManager.Instance.StateManager.GameInProgress)
                return;

            if (GameManager.Instance.StateManager.LastState == StateManager.StateTypes.Game || GameManager.Instance.StateManager.LastState == StateManager.StateTypes.UI)
            {
                if (DaggerfallUI.Instance.UserInterfaceManager.TopWindow is DaggerfallInventoryWindow)
                    LastLootedChest = DaggerfallUI.Instance.InventoryWindow.LootTarget;

                if (DaggerfallUI.UIManager.WindowCount > 0)
                    LastUIWindow = DaggerfallUI.Instance.UserInterfaceManager.TopWindow;

                if (DaggerfallUI.UIManager.WindowCount == 0 && LastUIWindow is DaggerfallInventoryWindow)
                {
                    if (LastLootedChest != null)
                    {
                        if (LastLootedChest.ContainerType == LootContainerTypes.Nothing && LastLootedChest.ContainerImage == InventoryContainerImages.Chest)
                        {
                            if (LastLootedChest.TextureArchive == SmashedChestSpriteID || LastLootedChest.TextureArchive == DisintegratedChestSpriteID || LastLootedChest.TextureRecord == SmashedChestModelID || LastLootedChest.TextureRecord == DisintegratedChestModelID)
                            {
                                // Do Nothing, since sprite or model won't need to be changed if they are the smashed or disintegrated versions
                            }
                            else if (LastLootedChest.Items.Count == 0)
                            {
                                if (ChestGraphicType == 0) // Use sprite based graphics for chests
                                {
                                    if (LastLootedChest.TextureArchive == OpenEmptyChestSpriteID) { } // Do Nothing, since sprite is already this value.
                                    else
                                    {
                                        DaggerfallLoot openChestLoot = GameObjectHelper.CreateLootContainer(LootContainerTypes.Nothing, InventoryContainerImages.Chest, LastLootedChest.gameObject.transform.position, LastLootedChest.gameObject.transform.parent, OpenEmptyChestSpriteID, 0, LastLootedChest.LoadID, null, false);
                                        openChestLoot.gameObject.name = GameObjectHelper.GetGoFlatName(OpenEmptyChestSpriteID, 0);
                                        openChestLoot.Items.TransferAll(LastLootedChest.Items); // Transfers items from closed chest's items to the new open chest's item collection.
                                        Destroy(openChestLoot.GetComponent<SerializableLootContainer>());
                                        Destroy(LastLootedChest.gameObject);
                                    }
                                }
                                else // Use 3D models for chests
                                {
                                    if (LastLootedChest.TextureRecord == OpenEmptyChestModelID) { } // Do Nothing, since current model is already this value.
                                    else
                                    {
                                        GameObject usedModelPrefab = (ChestGraphicType == 1) ? Instance.LowPolyOpenEmptyChestPrefab : Instance.HighPolyOpenEmptyChestPrefab;
                                        GameObject emptyChestGo = GameObjectHelper.InstantiatePrefab(usedModelPrefab, GameObjectHelper.GetGoModelName(OpenEmptyChestModelID), LastLootedChest.gameObject.transform.parent, LastLootedChest.gameObject.transform.position);
                                        emptyChestGo.transform.rotation = LastLootedChest.gameObject.transform.rotation;
                                        Collider col = emptyChestGo.AddComponent<BoxCollider>();
                                        DaggerfallLoot chestLoot = emptyChestGo.AddComponent<DaggerfallLoot>();
                                        ToggleChestShadowsOrCollision(emptyChestGo);
                                        if (chestLoot)
                                        {
                                            chestLoot.ContainerType = LootContainerTypes.Nothing;
                                            chestLoot.ContainerImage = InventoryContainerImages.Chest;
                                            chestLoot.LoadID = LastLootedChest.LoadID;
                                            chestLoot.TextureRecord = OpenEmptyChestModelID;
                                            chestLoot.Items.TransferAll(LastLootedChest.Items);
                                        }
                                        Destroy(LastLootedChest.gameObject);
                                    }
                                }
                            }
                            else
                            {
                                if (ChestGraphicType == 0) // Use sprite based graphics for chests
                                {
                                    if (LastLootedChest.TextureArchive == OpenFullChestSpriteID) { } // Do Nothing, since sprite is already this value.
                                    else
                                    {
                                        DaggerfallLoot openChestLoot = GameObjectHelper.CreateLootContainer(LootContainerTypes.Nothing, InventoryContainerImages.Chest, LastLootedChest.gameObject.transform.position, LastLootedChest.gameObject.transform.parent, OpenFullChestSpriteID, 0, LastLootedChest.LoadID, null, false);
                                        openChestLoot.gameObject.name = GameObjectHelper.GetGoFlatName(OpenFullChestSpriteID, 0);
                                        openChestLoot.Items.TransferAll(LastLootedChest.Items); // Transfers items from closed chest's items to the new open chest's item collection.
                                        Destroy(openChestLoot.GetComponent<SerializableLootContainer>());
                                        Destroy(LastLootedChest.gameObject);
                                    }
                                }
                                else // Use 3D models for chests
                                {
                                    if (LastLootedChest.TextureRecord == OpenFullChestModelID) { } // Do Nothing, since current model is already this value.
                                    else
                                    {
                                        GameObject usedModelPrefab = (ChestGraphicType == 1) ? Instance.LowPolyOpenFullChestPrefab : Instance.HighPolyOpenFullChestPrefab;
                                        GameObject filledChestGo = GameObjectHelper.InstantiatePrefab(usedModelPrefab, GameObjectHelper.GetGoModelName(OpenFullChestModelID), LastLootedChest.gameObject.transform.parent, LastLootedChest.gameObject.transform.position);
                                        filledChestGo.transform.rotation = LastLootedChest.gameObject.transform.rotation;
                                        Collider col = filledChestGo.AddComponent<BoxCollider>();
                                        DaggerfallLoot chestLoot = filledChestGo.AddComponent<DaggerfallLoot>();
                                        ToggleChestShadowsOrCollision(filledChestGo);
                                        if (chestLoot)
                                        {
                                            chestLoot.ContainerType = LootContainerTypes.Nothing;
                                            chestLoot.ContainerImage = InventoryContainerImages.Chest;
                                            chestLoot.LoadID = LastLootedChest.LoadID;
                                            chestLoot.TextureRecord = OpenFullChestModelID;
                                            chestLoot.Items.TransferAll(LastLootedChest.Items);
                                        }
                                        Destroy(LastLootedChest.gameObject);
                                    }
                                }
                            }
                        }
                    }

                    LastUIWindow = null;
                    LastLootedChest = null;
                }
            }
        }

        private static void DoNothingActivation(RaycastHit hit)
        {
            // This is simply here so the vanilla Daggerfall chest models don't get combined with everything else when the interior/dungeon scene is created.
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
                case PlayerActivateModes.Info: // Inspect Chest
                case PlayerActivateModes.Talk:
                    if (ChestObjRef != null)
                    {
                        LLCObject chestData = ChestObjRef.GetComponent<LLCObject>();
                        if (chestData.HasBeenInspected) { } // Do nothing, will likely change this eventually, so reinspection/rerolling for inspection results is possible at some cost or something.
                        else
                        {
                            ApplyInspectionCosts();
                            chestData.RecentInspectValues = GetInspectionValues(chestData);
                            chestData.HasBeenInspected = true;
                        }
                        InspectionInfoWindow inspectionInfoWindow = new InspectionInfoWindow(DaggerfallUI.UIManager, chestData);
                        DaggerfallUI.UIManager.PushWindow(inspectionInfoWindow);
                    }
                    else
                    {
                        DaggerfallUI.AddHUDText("ERROR: Chest Was Found As Null.", 3f);
                    }
                    break;
                case PlayerActivateModes.Steal: // Attempt To Lock-pick Chest
                    if (ChestObjRef != null)
                    {
                        LLCObject closedChestData = ChestObjRef.GetComponent<LLCObject>();
                        DaggerfallAudioSource dfAudioSource = closedChestData.GetComponent<DaggerfallAudioSource>();
                        ItemCollection closedChestLoot = closedChestData.AttachedLoot;
                        Transform closedChestTransform = ChestObjRef.transform;
                        Vector3 pos = ChestObjRef.transform.position;

                        IsThisACrime(ChestInteractionType.Lockpick);

                        if (closedChestData.IsLockJammed)
                        {
                            DaggerfallUI.AddHUDText(GetLockAlreadyJammedText(), 2f);
                            if (dfAudioSource != null && !dfAudioSource.IsPlaying())
                                dfAudioSource.AudioSource.PlayOneShot(GetLockAlreadyJammedClip(), UnityEngine.Random.Range(1.2f, 1.91f) * DaggerfallUnity.Settings.SoundVolume);
                        }
                        else if (LockPickChance(closedChestData))
                        {
                            closedChestData.PicksAttempted++;
                            ApplyLockPickAttemptCosts();

                            DaggerfallLoot openChestLoot = null;
                            if (ChestGraphicType == 0) // Use sprite based graphics for chests
                            {
                                int spriteID = closedChestLoot.Count <= 0 ? OpenEmptyChestSpriteID : OpenFullChestSpriteID;
                                openChestLoot = GameObjectHelper.CreateLootContainer(LootContainerTypes.Nothing, InventoryContainerImages.Chest, pos, closedChestTransform.parent, spriteID, 0, DaggerfallUnity.NextUID, null, false);
                                openChestLoot.gameObject.name = GameObjectHelper.GetGoFlatName(spriteID, 0);
                                openChestLoot.Items.TransferAll(closedChestLoot); // Transfers items from closed chest's items to the new open chest's item collection.
                                Destroy(openChestLoot.GetComponent<SerializableLootContainer>());
                            }
                            else // Use 3D models for chests
                            {
                                GameObject usedModelPrefab = null;
                                int modelID = 0;
                                if (closedChestLoot.Count <= 0) { usedModelPrefab = (ChestGraphicType == 1) ? Instance.LowPolyOpenEmptyChestPrefab : Instance.HighPolyOpenEmptyChestPrefab; modelID = OpenEmptyChestModelID; }
                                else { usedModelPrefab = (ChestGraphicType == 1) ? Instance.LowPolyOpenFullChestPrefab : Instance.HighPolyOpenFullChestPrefab; modelID = OpenFullChestModelID; }
                                GameObject chestGo = GameObjectHelper.InstantiatePrefab(usedModelPrefab, GameObjectHelper.GetGoModelName((uint)modelID), closedChestTransform.parent, pos);
                                chestGo.transform.rotation = closedChestData.gameObject.transform.rotation;
                                Collider col = chestGo.AddComponent<BoxCollider>();
                                openChestLoot = chestGo.AddComponent<DaggerfallLoot>();
                                ToggleChestShadowsOrCollision(chestGo);
                                if (openChestLoot)
                                {
                                    openChestLoot.ContainerType = LootContainerTypes.Nothing;
                                    openChestLoot.ContainerImage = InventoryContainerImages.Chest;
                                    openChestLoot.LoadID = DaggerfallUnity.NextUID;
                                    openChestLoot.TextureRecord = modelID;
                                    openChestLoot.Items.TransferAll(closedChestLoot); // Transfers items from closed chest's items to the new open chest's item collection.
                                }
                            }

                            // Show success and play unlock sound
                            DaggerfallUI.AddHUDText(GetLockPickSuccessText(), 3f);
                            if (dfAudioSource != null)
                                AudioSource.PlayClipAtPoint(GetLockpickSuccessClip(), closedChestData.gameObject.transform.position, UnityEngine.Random.Range(1.7f, 2.61f) * DaggerfallUnity.Settings.SoundVolume);

                            Destroy(ChestObjRef); // Remove closed chest from scene.
                            ChestObjRef = null;
                        }
                        else
                        {
                            closedChestData.PicksAttempted++;
                            ApplyLockPickAttemptCosts();
                            int mechDamDealt = DetermineDamageToLockMechanism(closedChestData);

                            if (DoesLockJam(closedChestData, mechDamDealt))
                            {
                                DaggerfallUI.AddHUDText(GetJammedLockText(), 3f);
                                if (dfAudioSource != null)
                                    AudioSource.PlayClipAtPoint(GetLockpickJammedClip(), closedChestData.gameObject.transform.position, UnityEngine.Random.Range(8.2f, 9.71f) * DaggerfallUnity.Settings.SoundVolume);
                            }
                            else
                            {
                                DaggerfallUI.AddHUDText(GetLockPickAttemptText(), 2f);
                                if (dfAudioSource != null && !dfAudioSource.IsPlaying())
                                    dfAudioSource.AudioSource.PlayOneShot(GetLockpickAttemptClip(), UnityEngine.Random.Range(1.2f, 1.91f) * DaggerfallUnity.Settings.SoundVolume);
                            }
                        }
                    }
                    else
                    {
                        DaggerfallUI.AddHUDText("ERROR: Chest Was Found As Null.", 3f);
                    }
                    break;
                case PlayerActivateModes.Grab:
                    if (ChestObjRef != null)
                    {
                        LLCObject chestData = ChestObjRef.GetComponent<LLCObject>();
                        ChestChoiceWindow chestChoiceWindow = new ChestChoiceWindow(DaggerfallUI.UIManager, chestData);
                        DaggerfallUI.UIManager.PushWindow(chestChoiceWindow);
                    }
                    else
                    {
                        DaggerfallUI.AddHUDText("ERROR: Chest Was Found As Null.", 3f);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
