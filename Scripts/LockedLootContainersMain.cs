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

        [Invoke(StateManager.StateTypes.Start, 0)]
        public static void Init(InitParams initParams)
        {
            mod = initParams.Mod;
            instance = new GameObject("LockedLootContainers").AddComponent<LockedLootContainersMain>(); // Add script to the scene.

            mod.IsReady = true;
        }

        private void Start()
        {
            Debug.Log("Begin mod init: Locked Loot Containers");

            PlayerEnterExit.OnTransitionDungeonInterior += AddLootChests_OnTransitionDungeonInterior;

            Debug.Log("Finished mod init: Locked Loot Containers");
        }

        public void AddLootChests_OnTransitionDungeonInterior(PlayerEnterExit.TransitionEventArgs args)
        {
            DFLocation locationData = GameManager.Instance.PlayerGPS.CurrentLocation;

            if (GameManager.Instance.PlayerEnterExit.IsPlayerInside)
            {
                if (locationData.MapTableData.DungeonType == DFRegion.DungeonTypes.Cemetery)
                {
                    // Make list of loot-piles currently in the dungeon "scene."
                }
            }
        }
    }
}
