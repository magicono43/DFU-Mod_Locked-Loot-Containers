// Project:         LockedLootContainers mod for Daggerfall Unity (http://www.dfworkshop.net)
// Copyright:       Copyright (C) 2022 Kirk.O
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Author:          Kirk.O
// Created On: 	    9/8/2022, 11:00 PM
// Last Edit:		3/20/2023, 12:50 AM
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
using DaggerfallWorkshop.Game.Serialization;

namespace LockedLootContainers
{
    public partial class LockedLootContainersMain : MonoBehaviour
    {
        public static LockedLootContainersMain Instance;
        public static LLCSaveData ModSaveData = new LLCSaveData();

        static Mod mod;

        // Global Variables
        public static GameObject ChestObjRef { get; set; }
        public static GameObject MainCamera { get; set; }
        public static int PlayerLayerMask { get; set; }
        public static AudioClip LastSoundPlayed { get { return lastSoundPlayed; } set { lastSoundPlayed = value; } }
        public static PlayerEntity Player { get { return GameManager.Instance.PlayerEntity; } }
        public static PlayerActivateModes CurrentMode { get { return GameManager.Instance.PlayerActivate.CurrentMode; } }
        public static WeaponManager WepManager { get { return GameManager.Instance.WeaponManager; } }

        bool isBashReady = true;

        // Mod Textures
        public Texture2D ChestChoiceMenuTexture;
        public Texture2D InspectionInfoGUITexture;

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

            mod.IsReady = true;
        }

        private void Start()
        {
            Debug.Log("Begin mod init: Locked Loot Containers");

            Instance = this;

            mod.SaveDataInterface = ModSaveData;

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

            // Definitely consider making a custom activation for those silly 3D chest models that are in-game but never used for anything and add replace those with my custom chest objects, etc.
            PlayerActivate.RegisterCustomActivation(mod, 4733, 0, ChestActivation);

            PlayerEnterExit.OnTransitionDungeonInterior += AddChests_OnTransitionDungeonInterior;

            // Load Resources
            LoadTextures();
            LoadAudio();

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

            // So the fact I got basically everything to work so far with the projectile/collision detection stuff is pretty crazy to me.
            // However, looking at the DaggerfallMissile.cs code under the "DoAreaOfEffect" method, I have a feeling without a PR, I won't be able to do a clean/easy
            // method to have the aoe of spell effects be detected for the chests. But I will have to see, either way not a big deal all other successes considered.
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

        // For Testing Custom Windows

        public static void RegisterLLCCommands()
        {
            Debug.Log("[LockedLootContainers] Trying to register console commands.");
            try
            {
                ConsoleCommandsDatabase.RegisterCommand(ChangeButtonRect.command, ChangeButtonRect.description, ChangeButtonRect.usage, ChangeButtonRect.Execute);
                ConsoleCommandsDatabase.RegisterCommand(TeleportToRandomChest.command, TeleportToRandomChest.description, TeleportToRandomChest.usage, TeleportToRandomChest.Execute);
                ConsoleCommandsDatabase.RegisterCommand(MakeJunkItems.command, MakeJunkItems.description, MakeJunkItems.usage, MakeJunkItems.Execute);
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("Error Registering LockLootContainers Console commands: {0}", e.Message));
            }
        }

        private static class MakeJunkItems
        {
            public static readonly string command = "addjunk";
            public static readonly string description = "Spawns LLC Junk Items.";
            public static readonly string usage = "addjunk";

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
                    DaggerfallUnityItem item = ItemBuilder.CreateItem(ItemGroups.UselessItems1, 4722 + i);
                    Player.Items.AddItem(item);
                }

                return "Gave you ALL the junk items.";
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
                            ApplyInspectionCosts();
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
                    if (ChestObjRef != null)
                    {
                        LLCObject closedChestData = ChestObjRef.GetComponent<LLCObject>();
                        DaggerfallAudioSource dfAudioSource = closedChestData.GetComponent<DaggerfallAudioSource>();
                        ItemCollection closedChestLoot = closedChestData.AttachedLoot;
                        Transform closedChestTransform = ChestObjRef.transform;
                        Vector3 pos = ChestObjRef.transform.position;

                        if (closedChestData.IsLockJammed)
                        {
                            DaggerfallUI.AddHUDText("The lock is jammed and inoperable...", 4f);
                            if (dfAudioSource != null && !dfAudioSource.IsPlaying())
                                dfAudioSource.AudioSource.PlayOneShot(GetLockAlreadyJammedClip(), UnityEngine.Random.Range(1.2f, 1.91f) * DaggerfallUnity.Settings.SoundVolume);
                        }
                        else if (LockPickChance(closedChestData))
                        {
                            closedChestData.PicksAttempted++;
                            ApplyLockPickAttemptCosts();
                            DaggerfallLoot openChestLoot = GameObjectHelper.CreateLootContainer(LootContainerTypes.Nothing, InventoryContainerImages.Chest, pos, closedChestTransform.parent, 4734, 0, DaggerfallUnity.NextUID, null, false);
                            openChestLoot.gameObject.name = GameObjectHelper.GetGoFlatName(4734, 0);
                            openChestLoot.Items.TransferAll(closedChestLoot); // Transfers items from closed chest's items to the new open chest's item collection.
                            Destroy(openChestLoot.GetComponent<SerializableLootContainer>());

                            // Show success and play unlock sound
                            DaggerfallUI.AddHUDText("The lock clicks open...", 4f);
                            if (dfAudioSource != null)
                                AudioSource.PlayClipAtPoint(GetLockpickSuccessClip(), closedChestData.gameObject.transform.position, UnityEngine.Random.Range(1.7f, 2.61f) * DaggerfallUnity.Settings.SoundVolume);

                            Destroy(ChestObjRef); // Removed closed chest from scene, but saved its characteristics we care about for opened chest loot-pile.
                            ChestObjRef = null;
                        }
                        else
                        {
                            closedChestData.PicksAttempted++;
                            ApplyLockPickAttemptCosts();
                            int mechDamDealt = DetermineDamageToLockMechanism(closedChestData);

                            if (DoesLockJam(closedChestData, mechDamDealt))
                            {
                                DaggerfallUI.AddHUDText("You jammed the lock, now brute force is the only option.", 4f);
                                if (dfAudioSource != null)
                                    AudioSource.PlayClipAtPoint(GetLockpickJammedClip(), closedChestData.gameObject.transform.position, UnityEngine.Random.Range(8.2f, 9.71f) * DaggerfallUnity.Settings.SoundVolume);
                            }
                            else
                            {
                                DaggerfallUI.AddHUDText("You fail to pick the lock...", 4f);
                                if (dfAudioSource != null && !dfAudioSource.IsPlaying())
                                    dfAudioSource.AudioSource.PlayOneShot(GetLockpickAttemptClip(), UnityEngine.Random.Range(1.2f, 1.91f) * DaggerfallUnity.Settings.SoundVolume);
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