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
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using System.Collections.Generic;
using System;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.Serialization;
using DaggerfallConnect.Arena2;

namespace LockedLootContainers
{
    public partial class LockedLootContainersMain
    {
        #region Fields

        public bool[] PermittedMaterials_None = new bool[] { false, false, false, false, false, false, false, false };
        public bool[] PermittedMaterials_FireProof = new bool[] { false, false, true, true, true, true, true, true };
        public bool[] PermittedMaterials_All = new bool[] { true, true, true, true, true, true, true, true };

        #endregion

        public void AddChests_OnTransitionInterior(PlayerEnterExit.TransitionEventArgs args)
        {
            if (SaveLoadManager.Instance.LoadInProgress) // Hopefully this will keep this from running when loading a save, but not when normally entering and exiting while playing, etc.
                return;

            DFLocation.BuildingTypes buildingType = GameManager.Instance.PlayerEnterExit.BuildingDiscoveryData.buildingType;
            PlayerGPS.DiscoveredBuilding buildingData = GameManager.Instance.PlayerEnterExit.BuildingDiscoveryData;
            DaggerfallLoot[] lootPiles;
            MeshFilter[] containerModels;

            if (DaggerfallWorkshop.Game.Banking.DaggerfallBankManager.IsHouseOwned(buildingData.buildingKey)) // Don't check for adding chests to player owned houses.
                return;

            if (buildingType == DFLocation.BuildingTypes.Ship && DaggerfallWorkshop.Game.Banking.DaggerfallBankManager.OwnsShip) // Don't check for adding chests to player owned ships.
                return;

            bool[] allowedMats = { true, true, true, true, true, true, true, true }; // Wood, Iron, Steel, Orcish, Mithril, Dwarven, Adamantium, Daedric
            int baseChestOdds = 10; // This value will be changed based on the type of dungeon, which will determine the base odds for a chest to be generated in place of a loot-pile in the end.
            // Misc Item Group Odds: Gold, LoC, G-Min, G-Max, LoC-Min, LoC-Max, Potions, Maps, Potion Recipes, Paintings, Soul-gems, Magic Items, Max Condition %, Min Condition %
            int[] miscGroupOdds = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            // Allowed Item Groups:    Drug, Armor, Weap, Cloth, Book, Jewel, Supply, Relic, Ingred
            // Item Group Odds %: 
            int[] itemGroupOdds = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            // When I work on this next. Add these new "allowedItemGroups" and "itemGroupOdds" arrays to each dungeon type with their respective values to modify the allowed item groups and odds later on.

            if (GameManager.Instance.PlayerEnterExit.IsPlayerInside)
            {
                if (IsValidShop(buildingType))
                {
                    switch (buildingType) // I'll deal with filling out the "loot-tables" for each later when I confirm that interior chest spawning actually works at all.
                    {
                        case DFLocation.BuildingTypes.Alchemist:
                            allowedMats = new bool[] { true, true, true, false, true, true, false, false };
                            baseChestOdds = 15;
                            miscGroupOdds = new int[] { 15, 1, 20, 150, 750, 1500, 25, 0, 35, 0, 0, 0, 80, 25 };
                            itemGroupOdds = new int[] { 15, 0, 0, 0, 0, 0, 0, 0, 25, 25, 10, 25, 5, 0, 0, 5, 0, 5, 60, 45, 45, 25, 15, 7, 40, 35 };
                            break;
                        case DFLocation.BuildingTypes.Armorer:
                            allowedMats = new bool[] { false, true, true, true, true, false, false, false };
                            baseChestOdds = 25;
                            miscGroupOdds = new int[] { 20, 1, 60, 400, 1500, 3500, 0, 0, 0, 0, 0, 0, 90, 40 };
                            itemGroupOdds = new int[] { 1, 30, 30, 30, 30, 10, 10, 10, 25, 25, 0, 10, 5, 20, 5, 15, 0, 5, 0, 0, 0, 0, 0, 0, 0, 25 };
                            break;
                        case DFLocation.BuildingTypes.WeaponSmith:
                            allowedMats = new bool[] { false, true, true, true, true, false, false, false };
                            baseChestOdds = 25;
                            miscGroupOdds = new int[] { 20, 1, 60, 400, 1500, 3500, 0, 0, 0, 0, 0, 0, 90, 40 };
                            itemGroupOdds = new int[] { 1, 10, 10, 10, 20, 35, 35, 35, 25, 25, 0, 10, 5, 15, 5, 15, 0, 5, 0, 0, 0, 0, 0, 0, 0, 20 };
                            break;
                        case DFLocation.BuildingTypes.GeneralStore:
                            allowedMats = new bool[] { true, true, true, false, true, false, false, false };
                            baseChestOdds = 25;
                            miscGroupOdds = new int[] { 25, 1, 40, 600, 1250, 2750, 0, 0, 0, 3, 0, 1, 80, 30 };
                            itemGroupOdds = new int[] { 2, 0, 0, 0, 0, 15, 15, 15, 35, 35, 20, 5, 5, 25, 35, 35, 0, 10, 0, 0, 0, 0, 0, 0, 0, 0 };
                            break;
                        case DFLocation.BuildingTypes.PawnShop:
                            allowedMats = new bool[] { true, true, true, true, true, true, true, true };
                            baseChestOdds = 35;
                            miscGroupOdds = new int[] { 20, 1, 30, 500, 1000, 2250, 0, 6, 0, 20, 0, 4, 70, 25 };
                            itemGroupOdds = new int[] { 2, 5, 5, 5, 5, 5, 5, 5, 25, 25, 10, 15, 15, 0, 5, 5, 10, 20, 0, 0, 0, 0, 0, 0, 0, 15 };
                            break;
                        case DFLocation.BuildingTypes.GemStore:
                            allowedMats = new bool[] { true, true, true, false, true, false, false, false };
                            baseChestOdds = 30;
                            miscGroupOdds = new int[] { 20, 1, 30, 650, 1500, 3000, 0, 0, 0, 0, 0, 0, 80, 35 };
                            itemGroupOdds = new int[] { 1, 0, 0, 0, 0, 0, 0, 0, 25, 25, 0, 50, 50, 0, 0, 5, 0, 10, 0, 0, 0, 0, 0, 0, 0, 35 };
                            break;
                        case DFLocation.BuildingTypes.ClothingStore:
                            allowedMats = new bool[] { true, true, true, false, true, false, false, false };
                            baseChestOdds = 15;
                            miscGroupOdds = new int[] { 20, 1, 35, 550, 850, 1850, 0, 0, 0, 0, 0, 0, 90, 55 };
                            itemGroupOdds = new int[] { 1, 0, 0, 0, 0, 0, 0, 0, 125, 125, 0, 5, 15, 0, 0, 5, 0, 15, 0, 0, 0, 0, 0, 0, 0, 0 };
                            break;
                        case DFLocation.BuildingTypes.Bookseller:
                        case DFLocation.BuildingTypes.Library:
                            allowedMats = new bool[] { true, true, true, false, true, false, false, false };
                            baseChestOdds = 15;
                            miscGroupOdds = new int[] { 10, 1, 15, 250, 500, 1000, 0, 20, 20, 1, 0, 0, 70, 30 };
                            itemGroupOdds = new int[] { 2, 0, 0, 0, 0, 0, 0, 0, 30, 30, 75, 5, 5, 0, 10, 40, 0, 5, 0, 0, 0, 0, 0, 0, 0, 0 };
                            break;
                        case DFLocation.BuildingTypes.Bank:
                            allowedMats = new bool[] { false, false, true, true, false, true, true, true };
                            baseChestOdds = 40;
                            miscGroupOdds = new int[] { 50, 10, 200, 950, 3000, 7000, 0, 0, 0, 1, 0, 0, 70, 30 };
                            itemGroupOdds = new int[] { 1, 0, 0, 0, 0, 0, 0, 0, 25, 25, 5, 5, 5, 0, 10, 20, 0, 5, 0, 0, 0, 0, 0, 0, 0, 25 };
                            break;
                        default:
                            allowedMats = new bool[] { true, true, true, false, true, false, false, false };
                            baseChestOdds = 25;
                            miscGroupOdds = new int[] { 25, 1, 40, 600, 1250, 2750, 0, 0, 0, 3, 0, 1, 80, 30 };
                            itemGroupOdds = new int[] { 2, 0, 0, 0, 0, 15, 15, 15, 35, 35, 20, 5, 5, 25, 35, 35, 0, 10, 0, 0, 0, 0, 0, 0, 0, 0 };
                            break;
                    }
                }
                else if (buildingType == DFLocation.BuildingTypes.Tavern)
                {
                    allowedMats = new bool[] { true, true, true, false, true, false, false, false };
                    baseChestOdds = 35;
                    miscGroupOdds = new int[] { 30, 1, 20, 400, 600, 2000, 5, 3, 2, 0, 0, 1, 65, 20 };
                    itemGroupOdds = new int[] { 3, 10, 10, 10, 10, 10, 10, 10, 40, 40, 5, 5, 5, 5, 5, 5, 0, 5, 10, 10, 10, 0, 0, 0, 0, 5 };
                }
                else if (buildingType == DFLocation.BuildingTypes.Palace)
                {
                    allowedMats = new bool[] { false, true, true, true, true, true, true, true };
                    baseChestOdds = 50;
                    miscGroupOdds = new int[] { 50, 3, 200, 800, 2000, 5500, 10, 2, 0, 8, 1, 2, 75, 35 };
                    itemGroupOdds = new int[] { 5, 15, 15, 15, 15, 15, 15, 15, 45, 45, 10, 10, 10, 5, 5, 5, 3, 5, 0, 0, 0, 0, 0, 0, 0, 20 };
                }
                else if (IsValidTownHouse(buildingType))
                {
                    allowedMats = new bool[] { true, true, true, false, true, false, false, false };
                    baseChestOdds = 30;
                    miscGroupOdds = new int[] { 30, 1, 20, 400, 600, 2000, 5, 2, 1, 1, 1, 1, 70, 25 };
                    itemGroupOdds = new int[] { 3, 10, 10, 10, 10, 10, 10, 10, 35, 35, 5, 5, 5, 5, 5, 5, 0, 5, 10, 10, 10, 10, 5, 2, 5, 5 };
                }
                else if (buildingType == DFLocation.BuildingTypes.Temple)
                {
                    allowedMats = new bool[] { true, true, true, true, true, true, true, true };
                    baseChestOdds = 20;
                    miscGroupOdds = new int[] { 40, 1, 100, 900, 550, 1300, 15, 0, 5, 1, 1, 2, 75, 30 };
                    itemGroupOdds = new int[] { 3, 5, 5, 5, 5, 5, 5, 5, 25, 25, 10, 10, 10, 0, 5, 5, 10, 35, 15, 15, 15, 15, 8, 3, 10, 10 };
                }
                else if (buildingType == DFLocation.BuildingTypes.GuildHall)
                {
                    switch (buildingData.factionID)
                    {
                        case (int)FactionFile.FactionIDs.The_Mages_Guild:
                            allowedMats = new bool[] { false, false, false, false, true, true, true, true };
                            baseChestOdds = 20;
                            miscGroupOdds = new int[] { 15, 1, 15, 300, 750, 1650, 10, 1, 3, 0, 3, 15, 65, 25 };
                            itemGroupOdds = new int[] { 3, 0, 0, 0, 0, 10, 0, 0, 25, 25, 25, 0, 5, 0, 10, 10, 0, 0, 10, 10, 10, 10, 8, 3, 10, 10 };
                            break;
                        case (int)FactionFile.FactionIDs.The_Fighters_Guild:
                            allowedMats = new bool[] { false, true, true, true, false, false, false, false };
                            baseChestOdds = 30;
                            miscGroupOdds = new int[] { 30, 1, 40, 500, 800, 1750, 5, 5, 0, 0, 0, 1, 85, 35 };
                            itemGroupOdds = new int[] { 3, 15, 15, 15, 25, 30, 30, 30, 20, 20, 0, 0, 5, 10, 10, 25, 0, 5, 0, 0, 0, 10, 0, 0, 0, 10 };
                            break;
                        case (int)FactionFile.FactionIDs.Generic_Knightly_Order: // Will have to test if this one actually works for all the knightly orders or not, will see.
                            allowedMats = new bool[] { false, true, true, true, true, false, false, false };
                            baseChestOdds = 30;
                            miscGroupOdds = new int[] { 20, 1, 30, 350, 700, 1500, 5, 5, 0, 0, 0, 1, 90, 45 };
                            itemGroupOdds = new int[] { 1, 30, 30, 30, 35, 20, 20, 20, 25, 25, 5, 5, 5, 5, 5, 15, 3, 10, 0, 0, 0, 5, 0, 0, 0, 5 };
                            break;
                        case (int)FactionFile.FactionIDs.The_Thieves_Guild:
                            allowedMats = new bool[] { true, true, false, false, false, true, false, false };
                            baseChestOdds = 20;
                            miscGroupOdds = new int[] { 25, 0, 10, 200, 0, 0, 0, 0, 0, 3, 0, 2, 60, 20 };
                            itemGroupOdds = new int[] { 15, 35, 35, 0, 10, 40, 15, 0, 20, 20, 0, 15, 15, 0, 0, 5, 0, 5, 0, 0, 0, 0, 0, 0, 0, 15 };
                            break;
                        case (int)FactionFile.FactionIDs.The_Dark_Brotherhood:
                            allowedMats = new bool[] { true, true, true, true, true, true, true, true };
                            baseChestOdds = 20;
                            miscGroupOdds = new int[] { 435, 1, 75, 750, 1000, 3000, 25, 3, 10, 0, 4, 1, 70, 25 };
                            itemGroupOdds = new int[] { 5, 25, 25, 5, 5, 50, 25, 5, 20, 20, 5, 5, 10, 0, 0, 10, 0, 7, 7, 7, 7, 20, 15, 5, 15, 10 };
                            break;
                        default:
                            allowedMats = new bool[] { true, true, true, true, true, true, true, true };
                            baseChestOdds = 20;
                            miscGroupOdds = new int[] { 30, 1, 20, 400, 600, 2000, 5, 3, 2, 0, 0, 1, 65, 20 };
                            itemGroupOdds = new int[] { 3, 10, 10, 10, 10, 10, 10, 10, 40, 40, 5, 5, 5, 5, 5, 5, 0, 5, 10, 10, 10, 0, 0, 0, 0, 5 };
                            break;
                    }
                }
                else
                {
                    return;
                }

                // Make list of loot-piles currently in the interior "scene."
                List<GameObject> goList = new List<GameObject>();
                lootPiles = FindObjectsOfType<DaggerfallLoot>();
                for (int i = 0; i < lootPiles.Length; i++)
                {
                    if (lootPiles[i].ContainerType == LootContainerTypes.RandomTreasure && lootPiles[i].ContainerImage == InventoryContainerImages.Chest)
                        goList.Add(lootPiles[i].gameObject);
                }

                // Make list of "valid" container type models in the current interior "scene."
                containerModels = FindObjectsOfType<MeshFilter>();
                for (int i = 0; i < containerModels.Length; i++)
                {
                    int modelID = -1;
                    bool validID = false;
                    string meshName = containerModels[i].mesh.name;

                    if (meshName.Length > 0)
                    {
                        string properName = meshName.Substring(0, meshName.Length - 9);
                        validID = int.TryParse(properName, out modelID);
                    }

                    if (validID)
                    {
                        if (modelID >= 41811 && modelID <= 41813) // Vanilla DF Chest models
                            goList.Add(containerModels[i].gameObject);

                        // Would like to eventually have other "container" type models have a chance to be replaced with a custom chest, but for now I'll just leave it as it is, maybe in a future version.

                        Debug.LogFormat("Overlap found on gameobject: {0} ||||| With MeshFilter Name: {1} ||||| And Mesh Name: {2}", modelID, containerModels[i].name, meshName);
                    }
                    else
                    {
                        Debug.LogFormat("Overlap found on model name: {0}", containerModels[i].mesh.name);
                    }
                }
                GameObject[] validGos = goList.ToArray();

                for (int i = 0; i < validGos.Length; i++)
                {
                    MeshFilter meshFilter = validGos[i].GetComponent<MeshFilter>();
                    DaggerfallLoot lootPile = validGos[i].GetComponent<DaggerfallLoot>();

                    int buildingQualityRoomMod = 0;
                    if (buildingData.quality <= 7)
                        buildingQualityRoomMod = (buildingData.quality * 10) - 80;
                    else
                        buildingQualityRoomMod = (buildingData.quality * 5) - 40;

                    ReplaceWithCustomChest(meshFilter, lootPile, allowedMats, baseChestOdds, miscGroupOdds, itemGroupOdds, buildingQualityRoomMod);
                }
            }
        }

        public void AddChests_OnTransitionDungeonInterior(PlayerEnterExit.TransitionEventArgs args)
        {
            if (SaveLoadManager.Instance.LoadInProgress) // Hopefully this will keep this from running when loading a save, but not when normally entering and exiting while playing, etc.
                return;

            DFLocation locationData = GameManager.Instance.PlayerGPS.CurrentLocation;
            DaggerfallLoot[] lootPiles;
            MeshFilter[] containerModels;

            bool[] allowedMats = { true, true, true, true, true, true, true, true }; // Wood, Iron, Steel, Orcish, Mithril, Dwarven, Adamantium, Daedric
            int baseChestOdds = 10; // This value will be changed based on the type of dungeon, which will determine the base odds for a chest to be generated in place of a loot-pile in the end.
            // Misc Item Group Odds: Gold, LoC, G-Min, G-Max, LoC-Min, LoC-Max, Potions, Maps, Potion Recipes, Paintings, Soul-gems, Magic Items, Max Condition %, Min Condition %
            int[] miscGroupOdds = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            // Allowed Item Groups:    Drug, Armor, Weap, Cloth, Book, Jewel, Supply, Relic, Ingred
            // Item Group Odds %: 
            int[] itemGroupOdds = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            // When I work on this next. Add these new "allowedItemGroups" and "itemGroupOdds" arrays to each dungeon type with their respective values to modify the allowed item groups and odds later on.

            if (GameManager.Instance.PlayerEnterExit.IsPlayerInside)
            {
                switch (locationData.MapTableData.DungeonType) // These will give various modifier values which will then be used afterward to do the actual "work" part in the generation process.
                {
                    case DFRegion.DungeonTypes.Crypt: // Maybe the next parameters will be for when I get to defining what traps, amounts, and trap types are allowed/common in some dungeon types.
                        allowedMats = PermittedMaterials_All;
                        baseChestOdds = 25;
                        miscGroupOdds = new int[] { 35, 1, 30, 350, 1000, 5000, 0, 10, 3, 15, 3, 8, 70, 25 };
                        itemGroupOdds = new int[] { 0, 25, 25, 25, 25, 25, 25, 25, 50, 50, 15, 20, 40, 5, 10, 40, 0, 50, 0, 0, 0, 15, 10, 0, 0, 20 };
                        break;
                    case DFRegion.DungeonTypes.OrcStronghold:
                        allowedMats = new bool[] { false, true, true, true, true, false, false, false };
                        baseChestOdds = 25;
                        miscGroupOdds = new int[] { 75, 2, 75, 455, 600, 2500, 15, 6, 8, 1, 0, 3, 95, 40 };
                        itemGroupOdds = new int[] { 15, 10, 25, 75, 20, 20, 60, 60, 20, 20, 0, 10, 25, 30, 20, 60, 0, 20, 30, 30, 30, 20, 10, 5, 15, 30 };
                        break;
                    case DFRegion.DungeonTypes.HumanStronghold:
                        allowedMats = PermittedMaterials_All;
                        baseChestOdds = 30;
                        miscGroupOdds = new int[] { 85, 6, 50, 500, 750, 4000, 10, 5, 5, 8, 3, 5, 85, 35 };
                        itemGroupOdds = new int[] { 10, 40, 40, 40, 40, 40, 40, 40, 75, 75, 35, 20, 30, 15, 30, 60, 0, 30, 10, 10, 10, 10, 5, 2, 5, 5 };
                        break;
                    case DFRegion.DungeonTypes.Prison:
                        allowedMats = new bool[] { true, true, false, false, false, false, false, false };
                        baseChestOdds = 10;
                        miscGroupOdds = new int[] { 40, 1, 15, 200, 400, 1500, 0, 3, 2, 2, 0, 1, 60, 30 };
                        itemGroupOdds = new int[] { 75, 40, 0, 0, 0, 200, 0, 0, 40, 40, 0, 5, 10, 10, 15, 40, 0, 15, 0, 0, 0, 0, 0, 0, 15, 3 };
                        break;
                    case DFRegion.DungeonTypes.DesecratedTemple: // Could even make these specific to some worshipped god and such, but that's just idea guy talking.
                        allowedMats = new bool[] { true, true, true, true, true, true, true, false };
                        baseChestOdds = 25;
                        miscGroupOdds = new int[] { 90, 7, 30, 525, 700, 4000, 25, 0, 15, 7, 3, 6, 75, 25 };
                        itemGroupOdds = new int[] { 5, 20, 20, 0, 0, 10, 30, 20, 75, 75, 40, 15, 40, 5, 30, 70, 60, 200, 30, 30, 30, 30, 20, 10, 30, 10 };
                        break;
                    case DFRegion.DungeonTypes.Mine:
                        allowedMats = new bool[] { true, true, true, false, true, true, false, false };
                        baseChestOdds = 15;
                        miscGroupOdds = new int[] { 40, 1, 10, 250, 500, 2250, 0, 15, 0, 0, 0, 3, 75, 40 };
                        itemGroupOdds = new int[] { 5, 30, 10, 0, 0, 10, 10, 25, 15, 15, 0, 65, 0, 300, 200, 80, 0, 0, 0, 0, 0, 15, 0, 0, 10, 500 };
                        break;
                    case DFRegion.DungeonTypes.NaturalCave:
                        allowedMats = new bool[] { true, true, true, true, true, true, true, false };
                        baseChestOdds = 10;
                        miscGroupOdds = new int[] { 35, 1, 20, 300, 400, 2000, 5, 8, 4, 1, 1, 2, 55, 15 };
                        itemGroupOdds = new int[] { 15, 30, 10, 0, 10, 20, 30, 15, 0, 0, 0, 40, 15, 10, 25, 20, 0, 15, 15, 15, 15, 45, 15, 5, 20, 200 };
                        break;
                    case DFRegion.DungeonTypes.Coven:
                        allowedMats = new bool[] { true, false, false, false, true, true, true, true };
                        baseChestOdds = 20;
                        miscGroupOdds = new int[] { 30, 1, 5, 120, 400, 1000, 45, 3, 30, 1, 10, 9, 60, 35 };
                        itemGroupOdds = new int[] { 10, 15, 0, 0, 0, 85, 0, 0, 35, 35, 70, 25, 30, 0, 0, 30, 0, 70, 200, 200, 200, 300, 150, 55, 200, 40 };
                        break;
                    case DFRegion.DungeonTypes.VampireHaunt:
                        allowedMats = PermittedMaterials_All;
                        baseChestOdds = 20;
                        miscGroupOdds = new int[] { 55, 2, 40, 450, 800, 3750, 7, 4, 6, 9, 7, 5, 75, 35 };
                        itemGroupOdds = new int[] { 0, 35, 20, 10, 10, 75, 40, 15, 60, 60, 40, 30, 45, 0, 0, 20, 0, 0, 0, 0, 0, 35, 25, 10, 5, 0 };
                        break;
                    case DFRegion.DungeonTypes.Laboratory:
                        allowedMats = new bool[] { false, true, true, false, true, true, true, true };
                        baseChestOdds = 20;
                        miscGroupOdds = new int[] { 35, 1, 10, 170, 500, 1500, 35, 2, 35, 0, 15, 8, 75, 40 };
                        itemGroupOdds = new int[] { 10, 15, 0, 0, 0, 60, 0, 0, 15, 15, 300, 20, 10, 10, 60, 85, 0, 0, 100, 100, 100, 300, 200, 80, 300, 125 };
                        break;
                    case DFRegion.DungeonTypes.HarpyNest:
                        allowedMats = new bool[] { true, true, true, false, true, true, true, true };
                        baseChestOdds = 10;
                        miscGroupOdds = new int[] { 35, 1, 20, 300, 400, 2000, 5, 8, 4, 1, 1, 2, 55, 15 };
                        itemGroupOdds = new int[] { 10, 30, 10, 0, 10, 20, 30, 15, 0, 0, 0, 40, 15, 10, 25, 20, 0, 0, 25, 25, 25, 250, 25, 10, 20, 30 };
                        break;
                    case DFRegion.DungeonTypes.RuinedCastle:
                        allowedMats = PermittedMaterials_All;
                        baseChestOdds = 40;
                        miscGroupOdds = new int[] { 50, 8, 70, 600, 1250, 6000, 2, 3, 0, 12, 0, 2, 60, 30 };
                        itemGroupOdds = new int[] { 5, 20, 40, 80, 20, 60, 60, 60, 20, 20, 30, 40, 50, 0, 15, 25, 0, 40, 0, 0, 0, 0, 0, 0, 0, 20 };
                        break;
                    case DFRegion.DungeonTypes.SpiderNest:
                        allowedMats = new bool[] { true, true, true, true, true, true, true, false };
                        baseChestOdds = 10;
                        miscGroupOdds = new int[] { 35, 1, 20, 300, 400, 2000, 5, 8, 4, 1, 1, 2, 55, 15 };
                        itemGroupOdds = new int[] { 10, 30, 10, 0, 10, 20, 30, 15, 0, 0, 0, 40, 15, 10, 25, 20, 0, 0, 25, 25, 25, 250, 25, 10, 20, 30 };
                        break;
                    case DFRegion.DungeonTypes.GiantStronghold:
                        allowedMats = new bool[] { true, true, true, false, false, false, false, false };
                        baseChestOdds = 15;
                        miscGroupOdds = new int[] { 65, 1, 60, 650, 500, 1500, 3, 6, 2, 1, 0, 1, 65, 25 };
                        itemGroupOdds = new int[] { 20, 15, 15, 15, 40, 10, 20, 50, 10, 10, 0, 10, 20, 0, 20, 20, 0, 0, 10, 10, 10, 150, 10, 0, 0, 20 };
                        break;
                    case DFRegion.DungeonTypes.DragonsDen:
                        allowedMats = PermittedMaterials_FireProof;
                        baseChestOdds = 45;
                        miscGroupOdds = new int[] { 90, 0, 150, 2500, 0, 0, 5, 7, 3, 10, 6, 12, 70, 25 };
                        itemGroupOdds = new int[] { 0, 10, 45, 90, 60, 30, 70, 70, 0, 0, 10, 100, 100, 0, 30, 30, 10, 40, 0, 0, 0, 40, 20, 70, 20, 80 };
                        break;
                    case DFRegion.DungeonTypes.BarbarianStronghold:
                        allowedMats = new bool[] { true, true, true, true, true, false, false, false };
                        baseChestOdds = 20;
                        miscGroupOdds = new int[] { 70, 1, 90, 700, 500, 1750, 4, 6, 3, 1, 0, 2, 75, 35 };
                        itemGroupOdds = new int[] { 20, 60, 30, 0, 20, 20, 40, 80, 20, 20, 0, 10, 25, 10, 50, 50, 0, 0, 20, 20, 20, 100, 10, 5, 10, 30 };
                        break;
                    case DFRegion.DungeonTypes.VolcanicCaves:
                        allowedMats = PermittedMaterials_FireProof;
                        baseChestOdds = 15;
                        miscGroupOdds = new int[] { 70, 1, 120, 1200, 300, 1250, 8, 2, 2, 0, 15, 7, 55, 25 };
                        itemGroupOdds = new int[] { 0, 0, 20, 50, 40, 30, 30, 30, 0, 0, 0, 60, 40, 0, 50, 0, 0, 20, 0, 0, 0, 50, 15, 10, 0, 200 };
                        break;
                    case DFRegion.DungeonTypes.ScorpionNest:
                        allowedMats = new bool[] { true, true, true, true, true, true, true, false };
                        baseChestOdds = 10;
                        miscGroupOdds = new int[] { 35, 1, 20, 300, 400, 2000, 5, 8, 4, 1, 1, 2, 55, 15 };
                        itemGroupOdds = new int[] { 10, 30, 10, 0, 10, 20, 30, 15, 0, 0, 0, 40, 15, 10, 25, 20, 0, 0, 25, 25, 25, 250, 25, 10, 20, 30 };
                        break;
                    case DFRegion.DungeonTypes.Cemetery:
                        allowedMats = new bool[] { true, true, false, false, false, false, false, false };
                        baseChestOdds = 20;
                        miscGroupOdds = new int[] { 25, 1, 20, 400, 500, 1750, 0, 6, 2, 4, 1, 3, 65, 25 };
                        itemGroupOdds = new int[] { 0, 25, 25, 25, 25, 25, 25, 25, 50, 50, 15, 20, 40, 5, 10, 40, 0, 50, 0, 0, 0, 15, 10, 0, 0, 20 };
                        break;
                    default:
                        allowedMats = PermittedMaterials_All;
                        baseChestOdds = 10;
                        miscGroupOdds = new int[] { 50, 5, 50, 500, 500, 5000, 5, 5, 5, 5, 5, 5, 55, 5 };
                        itemGroupOdds = new int[] { 10, 40, 40, 40, 40, 40, 40, 40, 75, 75, 35, 20, 30, 15, 30, 60, 0, 30, 10, 10, 10, 10, 5, 2, 5, 5 };
                        break;
                }

                // Make list of loot-piles currently in the dungeon "scene."
                List<GameObject> goList = new List<GameObject>();
                lootPiles = FindObjectsOfType<DaggerfallLoot>();
                for (int i = 0; i < lootPiles.Length; i++)
                {
                    if (lootPiles[i].ContainerType == LootContainerTypes.RandomTreasure && lootPiles[i].ContainerImage == InventoryContainerImages.Chest)
                        goList.Add(lootPiles[i].gameObject);
                }

                // Make list of "valid" container type models in the current dungeon "scene."
                containerModels = FindObjectsOfType<MeshFilter>();
                for (int i = 0; i < containerModels.Length; i++)
                {
                    int modelID = -1;
                    bool validID = false;
                    string meshName = containerModels[i].mesh.name;

                    if (meshName.Length > 0)
                    {
                        string properName = meshName.Substring(0, meshName.Length - 9);
                        validID = int.TryParse(properName, out modelID);
                    }

                    if (validID)
                    {
                        if (modelID >= 41811 && modelID <= 41813) // Vanilla DF Chest models
                            goList.Add(containerModels[i].gameObject);

                        // Would like to eventually have other "container" type models have a chance to be replaced with a custom chest, but for now I'll just leave it as it is, maybe in a future version.

                        Debug.LogFormat("Overlap found on gameobject: {0} ||||| With MeshFilter Name: {1} ||||| And Mesh Name: {2}", modelID, containerModels[i].name, meshName);
                    }
                    else
                    {
                        Debug.LogFormat("Overlap found on model name: {0}", containerModels[i].mesh.name);
                    }
                }
                GameObject[] validGos = goList.ToArray();

                for (int i = 0; i < validGos.Length; i++)
                {
                    MeshFilter meshFilter = null;
                    DaggerfallLoot lootPile = validGos[i].GetComponent<DaggerfallLoot>();

                    if (lootPile == null) // So lootPiles also have a MeshFilter component, so the way before was causing chests to always replace loot-piles, will want to fix this better eventually.
                        meshFilter = validGos[i].GetComponent<MeshFilter>();

                    ReplaceWithCustomChest(meshFilter, lootPile, allowedMats, baseChestOdds, miscGroupOdds, itemGroupOdds);
                }
            }
        }

        public static void ReplaceWithCustomChest(MeshFilter meshFilter, DaggerfallLoot lootPile, bool[] allowedMats, int baseChestOdds, int[] miscGroupOdds, int[] itemGroupOdds, int buildQualMod = 0)
        {
            int totalRoomValueMod = 0 + buildQualMod;
            Transform goTransform = null;
            Vector3 pos = new Vector3(0, 0, 0);

            if (meshFilter == null && lootPile == null)
                return;

            if (meshFilter)
            {
                goTransform = meshFilter.transform;
                pos = meshFilter.transform.position;
            }

            if (lootPile)
            {
                goTransform = lootPile.transform;
                pos = lootPile.transform.position;
            }

            Ray[] rays = { new Ray(pos, goTransform.up), new Ray(pos, -goTransform.up),
                new Ray(pos, goTransform.right), new Ray(pos, -goTransform.right),
                new Ray(pos, goTransform.forward), new Ray(pos, -goTransform.forward)}; // up, down, right, left, front, back.

            float[] rayDistances = { 0, 0, 0, 0, 0, 0 }; // Distances that up, down, right, left, front, and back rays traveled before hitting a collider.
            float[] boxDimensions = { 0.2f, 0.2f, 0.2f }; // Default dimensions of the overlap box.

            RaycastHit hit;
            for (int h = 0; h < rays.Length; h++)
            {
                if (Physics.Raycast(rays[h], out hit, 5f, PlayerLayerMask)) // Using raycast instead of sphere, as I want it to be only if you are purposely targeting the chest.
                {
                    rayDistances[h] = hit.distance;
                }
                else
                {
                    rayDistances[h] = 5f;
                }
            }

            boxDimensions[0] = (rayDistances[0] + rayDistances[1]); // y-axis? up and down
            boxDimensions[1] = (rayDistances[2] + rayDistances[3]); // x-axis? right and left
            boxDimensions[2] = (rayDistances[4] + rayDistances[5]); // z-axis? front and back
            Vector3 boxDimVector = new Vector3(boxDimensions[1], boxDimensions[0], boxDimensions[2]) * 2f;

            /*// Testing Box Sizes Here
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = pos;
            cube.transform.rotation = lootPiles[i].transform.rotation;
            cube.transform.localScale = boxDimVector;*/

            // Collect GameObjects that overlap the box and ignore duplicates, use these later to estimate a "room value" that a chest is being generated in.
            // Test this part more properly tomorrow, now after knowing some of the limitations now due to the "combined models" thing with dungeons and such.
            // Primarily be checking for flats/billboards, other loot-piles, enemies, area of overlap box, action objects, and especially doors.
            List<GameObject> roomObjects = new List<GameObject>();
            Collider[] overlaps = Physics.OverlapBox(pos, boxDimVector, goTransform.rotation, PlayerLayerMask);
            for (int r = 0; r < overlaps.Length; r++)
            {
                GameObject go = overlaps[r].gameObject;

                if (go && go == goTransform.gameObject) // Ignore the gameObject (lootpile) this box is originating from.
                    continue;

                if (go && go.name == "CombinedModels") // Ignore the entire combinedmodels gameobject
                    continue;

                if (go && !roomObjects.Contains(go))
                {
                    roomObjects.Add(go);
                    //Debug.LogFormat("Box overlapped GameObject, {0}", go.name);
                }

                // If I need examples for this look at "DaggerfallMissile.cs" under the "DoAreaOfEffect" method for something fairly similar.
                // Turns out, depending on the size of certain overlap boxes, you can sort of get some idea of what the object is currently in, like inside a coffin and such.
                // Later on consider adding toggle setting for this whole "room context" thing here, for possibly faster loading times or something without it potentially.

                DaggerfallEntityBehaviour aoeEntity = overlaps[r].GetComponent<DaggerfallEntityBehaviour>(); // Use this as an example for getting components I care about?
            }

            for (int g = 0; g < roomObjects.Count; g++)
            {
                if (roomObjects[g])
                {
                    DaggerfallEntityBehaviour entityBehaviour = roomObjects[g].GetComponent<DaggerfallEntityBehaviour>();
                    DaggerfallBillboard billBoard = roomObjects[g].GetComponent<DaggerfallBillboard>(); // Will have to test and eventually account for mods like "Handpainted Models" etc.
                    MeshFilter meshFilt = roomObjects[g].GetComponent<MeshFilter>();

                    if (entityBehaviour)
                    {
                        EnemyEntity enemyEntity = entityBehaviour.Entity as EnemyEntity;
                        if (enemyEntity != null)
                        {
                            MobileEnemy enemy = enemyEntity.MobileEnemy;
                            int mobileID = enemy.ID;
                            MobileAffinity affinity = enemy.Affinity;
                            MobileTeams team = enemy.Team;

                            if (team != MobileTeams.PlayerAlly && team != MobileTeams.CityWatch) // Ignore mobile entities that are currently on the player's team or are city guards.
                            {
                                totalRoomValueMod += EnemyRoomValueMods(enemyEntity);

                                Debug.LogFormat("Overlap found on gameobject: {0} ||||| With mobileID: {1} ||||| And Affinity: {2} ||||| And On Team: {3}", roomObjects[g].name, mobileID, affinity.ToString(), team.ToString());
                            }
                        }
                    }
                    else if (billBoard)
                    {
                        BillboardSummary summary = billBoard.Summary;
                        FlatTypes flatType = summary.FlatType;
                        int archive = summary.Archive;
                        int record = summary.Record;

                        if (flatType != FlatTypes.Editor && flatType != FlatTypes.Nature) // Ignore editor flats and nature flats, I assume nature is just the trees and plants in exteriors?
                        {
                            totalRoomValueMod += BillboardRoomValueMods(flatType, archive, record);

                            Debug.LogFormat("Overlap found on gameobject: {0} ||||| With BillBoard Archive: {1} ||||| And Record: {2} ||||| Flat Type: {3}", roomObjects[g].name, archive, record, flatType.ToString());
                        }
                    }
                    else if (meshFilt)
                    {
                        int modelID = -1;
                        bool validID = false;
                        string meshName = meshFilt.mesh.name;

                        if (meshName.Length > 0)
                        {
                            string properName = meshName.Substring(0, meshName.Length - 9);
                            validID = int.TryParse(properName, out modelID);
                        }

                        if (validID)
                        {
                            totalRoomValueMod += ModelRoomValueMods(modelID);

                            Debug.LogFormat("Overlap found on gameobject: {0} ||||| With MeshFilter Name: {1} ||||| And Mesh Name: {2}", modelID, roomObjects[g].name, meshName);
                        }
                        else
                        {
                            Debug.LogFormat("Overlap found on gameobject: {0}", roomObjects[g].name);
                        }
                    }
                    else
                    {
                        Debug.LogFormat("Overlap found on gameobject: {0}", roomObjects[g].name);
                    }
                }
            }

            RollIfChestShouldBeCreated(meshFilter, lootPile, allowedMats, baseChestOdds, totalRoomValueMod, miscGroupOdds, itemGroupOdds);
        }

        public static void RollIfChestShouldBeCreated(MeshFilter meshFilter, DaggerfallLoot lootPile, bool[] allowedMats, int baseChestOdds, int totalRoomValueMod, int[] miscGroupOdds, int[] itemGroupOdds)
        {
            float roomMod = 1f;
            int permitMatsCount = 0; // Total count of materials that are "true" from "allowedMats" bool array, if there is none then return and don't generate chest.

            if (meshFilter == null && lootPile == null)
                return;

            if (totalRoomValueMod < 0)
            {
                if (Dice100.SuccessRoll(20))
                    roomMod = (Mathf.Abs(totalRoomValueMod) / 50f) + 1f;
                else
                    roomMod = (totalRoomValueMod / 50f) + 1f;
            }
            else
            {
                if (totalRoomValueMod < 70 && Dice100.SuccessRoll(10))
                    roomMod = ((totalRoomValueMod * -1) / 100f) + 1f;
                else
                    roomMod = Mathf.Abs((totalRoomValueMod / 25f) + 1f);
            }

            int chestOdds = Mathf.RoundToInt(baseChestOdds * roomMod);

            if (chestOdds <= 0)
                return;

            for (int i = 0; i < allowedMats.Length; i++)
            {
                if (allowedMats[i] == true)
                    permitMatsCount++;
            }

            if (permitMatsCount <= 0)
                return;

            ulong newLoadID = DaggerfallUnity.NextUID;

            if (meshFilter) // For right now, just assuming the model is detecting one of the "useless" vanilla chest models and nothing else that might have a loot-pile component attached.
            {
                Transform oldModelTransform = meshFilter.transform;
                Vector3 pos = meshFilter.transform.position;

                //GameObject chestParentObj = GameObjectHelper.CreateDaggerfallBillboardGameObject(4733, 0, oldModelTransform.parent.parent);

                GameObject chestGo = GameObjectHelper.InstantiatePrefab(LockedLootContainersMain.Instance.TestClosed3DChestPrefab, GameObjectHelper.GetGoModelName(47330), oldModelTransform.parent.parent, pos);
                Collider col = chestGo.AddComponent<BoxCollider>();
                chestGo.transform.Rotate(0f, 0f, UnityEngine.Random.Range(0, 9) * 45f);

                LLCObject llcObj = chestGo.AddComponent<LLCObject>();
                llcObj.AttachedLoot = new ItemCollection();
                llcObj.LoadID = newLoadID;

                // Creates random seed for determining chest material value
                string combinedText = llcObj.LoadID.ToString().Substring(4) + UnityEngine.Random.Range(333, 99999).ToString();
                string truncText = (combinedText.Length > 9) ? combinedText.Substring(0, 9) : combinedText; // Might get index out of range, but will see.
                Debug.LogFormat("Attempting to parse value for chest seed: {0}", truncText);
                int seed = int.Parse(truncText);
                UnityEngine.Random.InitState(seed); // This is to attempt to combat patterns in generation due to this all happening in a small period of time with a similar system-time seed by default.

                llcObj.ChestMaterial = RollChestMaterial(allowedMats, permitMatsCount, totalRoomValueMod);

                // Creates random seed for determining lock material value and other values
                string jumbledText = truncText.Substring(5) + UnityEngine.Random.Range(333, 99999).ToString();
                truncText = (jumbledText.Length > 9) ? jumbledText.Substring(0, 9) : jumbledText;
                Debug.LogFormat("Attempting to parse value for lock & others seed: {0}", truncText);
                seed = int.Parse(truncText);
                UnityEngine.Random.InitState(seed); // This is to attempt to combat patterns in generation due to this all happening in a small period of time with a similar system-time seed by default.

                llcObj.LockMaterial = RollLockMaterial(allowedMats, permitMatsCount, totalRoomValueMod, llcObj.ChestMaterial);

                llcObj.ChestSturdiness = RollChestSturdiness(llcObj.ChestMaterial, meshFilter.transform.parent.parent.name, totalRoomValueMod);
                llcObj.LockSturdiness = RollLockSturdiness(llcObj.LockMaterial, meshFilter.transform.parent.parent.name, totalRoomValueMod);

                SetChestHitPoints(llcObj);
                SetLockHitPoints(llcObj);

                llcObj.ChestMagicResist = RollChestMagicResist(llcObj.ChestMaterial, llcObj.ChestSturdiness, totalRoomValueMod);
                llcObj.LockMagicResist = RollLockMagicResist(llcObj.LockMaterial, llcObj.LockSturdiness, totalRoomValueMod);

                llcObj.LockComplexity = RollLockComplexity(llcObj.LockMaterial, totalRoomValueMod);
                llcObj.JamResist = RollJamResist(llcObj.LockMaterial, totalRoomValueMod);
                SetLockMechanismHitPoints(llcObj);

                UnityEngine.Random.InitState((int)DateTime.Now.Ticks); // Here to try and reset the random generation seed value back to the "default" for what Unity normally uses in most operations.

                Destroy(meshFilter.gameObject);

                // Set position
                /*
                Billboard dfBillboard = chestParentObj.GetComponent<Billboard>();
                chestParentObj.transform.position = pos;

                // Cast ray down to find ground below
                RaycastHit hit;
                Ray ray = new Ray(chestParentObj.transform.position + new Vector3(0, 0.2f, 0), Vector3.down);
                if (!Physics.Raycast(ray, out hit, 2))
                    return;

                // Position bottom just above ground by adjusting parent gameobject
                chestParentObj.transform.position = new Vector3(hit.point.x, hit.point.y - dfBillboard.Summary.Size.y * 0.08f, hit.point.z); // Not perfect, but seems to work alright in most cases.
                */

                //Debug.LogFormat("Chest Generated With Transform: x = {0}, y = {1}, z = {2}. Chest Material = {3}, Sturdiness = {4}, Magic Resist = {5}. With A Lock Made From = {6}, Sturdiness = {7}, Magic Resist = {8}, Lock Complexity = {9}, Jam Resistance = {10}.", chestParentObj.transform.localPosition.x, chestParentObj.transform.localPosition.y, chestParentObj.transform.localPosition.z, llcObj.ChestMaterial.ToString(), llcObj.ChestSturdiness, llcObj.ChestMagicResist, llcObj.LockMaterial.ToString(), llcObj.LockSturdiness, llcObj.LockMagicResist, llcObj.LockComplexity, llcObj.JamResist); // Might have to mess with the position values a bit, might need the "parent" or something instead.

                // Maybe later on add some Event stuff here so other mods can know when this made added a chest or when loot generation happens for the chests or something? Will see.

                PopulateChestLoot(llcObj, totalRoomValueMod, miscGroupOdds, itemGroupOdds);
                return;
            }

            if (lootPile)
            {
                if (Dice100.SuccessRoll(chestOdds)) // If roll is successful, start process of replacing loot-pile with chest object as well as what properties the chest should have.
                {
                    ItemCollection oldPileLoot = new ItemCollection();
                    oldPileLoot.TransferAll(lootPile.Items);
                    Transform oldLootPileTransform = lootPile.transform;
                    Vector3 pos = lootPile.transform.position;

                    //GameObject chestParentObj = GameObjectHelper.CreateDaggerfallBillboardGameObject(4733, 0, oldLootPileTransform.parent.parent);

                    GameObject chestGo = GameObjectHelper.InstantiatePrefab(LockedLootContainersMain.Instance.TestClosed3DChestPrefab, GameObjectHelper.GetGoModelName(47330), oldLootPileTransform.parent.parent, pos);
                    Collider col = chestGo.AddComponent<BoxCollider>();
                    chestGo.transform.Rotate(0f, 0f, UnityEngine.Random.Range(0, 9) * 45f);

                    LLCObject llcObj = chestGo.AddComponent<LLCObject>();
                    llcObj.Oldloot = oldPileLoot;
                    llcObj.AttachedLoot = llcObj.Oldloot; // Will change this later, but placeholder for testing.
                    llcObj.LoadID = newLoadID;

                    // Creates random seed for determining chest material value
                    string combinedText = llcObj.LoadID.ToString().Substring(4) + UnityEngine.Random.Range(333, 99999).ToString();
                    string truncText = (combinedText.Length > 9) ? combinedText.Substring(0, 9) : combinedText; // Might get index out of range, but will see.
                    Debug.LogFormat("Attempting to parse value for chest seed: {0}", truncText);
                    int seed = int.Parse(truncText);
                    UnityEngine.Random.InitState(seed); // This is to attempt to combat patterns in generation due to this all happening in a small period of time with a similar system-time seed by default.

                    llcObj.ChestMaterial = RollChestMaterial(allowedMats, permitMatsCount, totalRoomValueMod);

                    // Creates random seed for determining lock material value and other values
                    string jumbledText = truncText.Substring(5) + UnityEngine.Random.Range(333, 99999).ToString();
                    truncText = (jumbledText.Length > 9) ? jumbledText.Substring(0, 9) : jumbledText;
                    Debug.LogFormat("Attempting to parse value for lock & others seed: {0}", truncText);
                    seed = int.Parse(truncText);
                    UnityEngine.Random.InitState(seed); // This is to attempt to combat patterns in generation due to this all happening in a small period of time with a similar system-time seed by default.

                    llcObj.LockMaterial = RollLockMaterial(allowedMats, permitMatsCount, totalRoomValueMod, llcObj.ChestMaterial);

                    llcObj.ChestSturdiness = RollChestSturdiness(llcObj.ChestMaterial, lootPile.transform.parent.parent.name, totalRoomValueMod);
                    llcObj.LockSturdiness = RollLockSturdiness(llcObj.LockMaterial, lootPile.transform.parent.parent.name, totalRoomValueMod);

                    SetChestHitPoints(llcObj);
                    SetLockHitPoints(llcObj);

                    llcObj.ChestMagicResist = RollChestMagicResist(llcObj.ChestMaterial, llcObj.ChestSturdiness, totalRoomValueMod);
                    llcObj.LockMagicResist = RollLockMagicResist(llcObj.LockMaterial, llcObj.LockSturdiness, totalRoomValueMod);

                    llcObj.LockComplexity = RollLockComplexity(llcObj.LockMaterial, totalRoomValueMod);
                    llcObj.JamResist = RollJamResist(llcObj.LockMaterial, totalRoomValueMod);
                    SetLockMechanismHitPoints(llcObj);

                    UnityEngine.Random.InitState((int)DateTime.Now.Ticks); // Here to try and reset the random generation seed value back to the "default" for what Unity normally uses in most operations.

                    // Set position
                    /*
                    Billboard dfBillboard = chestParentObj.GetComponent<Billboard>();
                    chestParentObj.transform.position = pos;
                    chestParentObj.transform.position += new Vector3(0, dfBillboard.Summary.Size.y / 2, 0);
                    GameObjectHelper.AlignBillboardToGround(chestParentObj, dfBillboard.Summary.Size);
                    */

                    //Debug.LogFormat("Chest Generated With Transform: x = {0}, y = {1}, z = {2}. Chest Material = {3}, Sturdiness = {4}, Magic Resist = {5}. With A Lock Made From = {6}, Sturdiness = {7}, Magic Resist = {8}, Lock Complexity = {9}, Jam Resistance = {10}.", chestParentObj.transform.localPosition.x, chestParentObj.transform.localPosition.y, chestParentObj.transform.localPosition.z, llcObj.ChestMaterial.ToString(), llcObj.ChestSturdiness, llcObj.ChestMagicResist, llcObj.LockMaterial.ToString(), llcObj.LockSturdiness, llcObj.LockMagicResist, llcObj.LockComplexity, llcObj.JamResist); // Might have to mess with the position values a bit, might need the "parent" or something instead.

                    lootPile.Items.Clear(); // Likely not necessary, but doing it just in case.
                    Destroy(lootPile.gameObject);

                    // Maybe later on add some Event stuff here so other mods can know when this made added a chest or when loot generation happens for the chests or something? Will see.

                    PopulateChestLoot(llcObj, totalRoomValueMod, miscGroupOdds, itemGroupOdds);
                }
            }
        }

        public static ChestMaterials RollChestMaterial(bool[] allowedMats, int permitMatsCount, int roomValueMod)
        {
            // Wood, Iron, Steel, Orcish, Mithril, Dwarven, Adamantium, Daedric
            // int[] rarity = { 180, 162, 126, 84, 108, 66, 48, 24 };
            float[] odds = new float[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] matRolls = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            float mod = roomValueMod;

            odds = new float[] { 180-(mod*1.75f), 162-(mod*0.75f), 126-(mod*0.25f), 84+(mod*0.5f), 108-(mod*0.25f), 66+(mod*0.25f), 48+(mod*0.5f), 24+(mod*0.75f) };

            for (int i = 0; i < odds.Length; i++)
            {
                if (!allowedMats[i])
                    continue;

                int randomModifier = UnityEngine.Random.Range(0, Mathf.RoundToInt(odds[i]) + 1);
                matRolls[i] = randomModifier;
            }

            int max = matRolls[0];
            int index = 0;

            for (int i = 0; i < matRolls.Length; i++)
            {
                if (index == i)
                    continue;

                if (max == matRolls[i])
                {
                    if (CoinFlip())
                        continue;
                    else
                    {
                        max = matRolls[i];
                        index = i;
                        continue;
                    }
                }

                if (max < matRolls[i])
                {
                    max = matRolls[i];
                    index = i;
                }
            }

            index += 1;
            return (ChestMaterials)index;
        }

        public static LockMaterials RollLockMaterial(bool[] allowedMats, int permitMatsCount, int roomValueMod, ChestMaterials chestMats)
        {
            // Wood, Iron, Steel, Orcish, Mithril, Dwarven, Adamantium, Daedric
            // int[] rarity = { 180, 162, 126, 84, 108, 66, 48, 24 };
            float[] odds = new float[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] matRolls = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 };
            float mod = roomValueMod;

            odds = new float[] { 180-(mod*1.75f), 162-(mod*0.75f), 126-(mod*0.25f), 84+(mod*0.5f), 108-(mod*0.25f), 66+(mod*0.25f), 48+(mod*0.5f), 24+(mod*0.75f) };

            for (int i = 0; i < odds.Length; i++)
            {
                if (!allowedMats[i])
                    continue;

                if (i < (int)chestMats - 3 || i >= (int)chestMats + 3)
                    continue;

                if (i == (int)chestMats - 1)
                    odds[i] = (int)Mathf.Floor(odds[i] / 3); // This is here to attempt to make having the same material chest and lock less likely, giving the dupe material a lower value.

                int randomModifier = UnityEngine.Random.Range(0, Mathf.RoundToInt(odds[i]) + 1);
                matRolls[i] = randomModifier;
            }

            int max = matRolls[0];
            int index = 0;

            for (int i = 0; i < matRolls.Length; i++)
            {
                if (index == i)
                    continue;

                if (max == matRolls[i])
                {
                    if (CoinFlip())
                        continue;
                    else
                    {
                        max = matRolls[i];
                        index = i;
                        continue;
                    }
                }

                if (max < matRolls[i])
                {
                    max = matRolls[i];
                    index = i;
                }
            }

            index += 1;
            return (LockMaterials)index;
        }

        public static int RollChestSturdiness(ChestMaterials chestMat, string dungBlocName, int roomValueMod)
        {
            int chestSturdiness = 1;
            int randomMod = UnityEngine.Random.Range(-20, 21);
            int roomMod = (int)Mathf.Clamp(Mathf.Round(roomValueMod / 5f), -10, 11); // May add some random factor to this later, but for now just static based on room value mod stuff.
            bool insideWaterBlock = false; // Will determine this properly later when I get into the Unity editor again and see how getting the block-name works and such.

            switch (chestMat)
            {
                default:
                case ChestMaterials.Wood:
                    chestSturdiness = (insideWaterBlock) ? UnityEngine.Random.Range(-15, 1) + 10 : 10; break;
                case ChestMaterials.Iron:
                    chestSturdiness = (insideWaterBlock) ? UnityEngine.Random.Range(-15, 1) + 40 : 40; break;
                case ChestMaterials.Steel:
                    chestSturdiness = (insideWaterBlock) ? UnityEngine.Random.Range(-10, 1) + 60 : 60; break;
                case ChestMaterials.Orcish:
                    chestSturdiness = (insideWaterBlock) ? UnityEngine.Random.Range(-10, 1) + 80 : 80; break;
                case ChestMaterials.Mithril:
                    chestSturdiness = (insideWaterBlock) ? UnityEngine.Random.Range(-5, 1) + 70 : 70; break;
                case ChestMaterials.Dwarven:
                    chestSturdiness = (insideWaterBlock) ? UnityEngine.Random.Range(-5, 1) + 50 : 50; break;
                case ChestMaterials.Adamantium:
                    chestSturdiness = (insideWaterBlock) ? UnityEngine.Random.Range(-5, 1) + 30 : 30; break;
                case ChestMaterials.Daedric:
                    chestSturdiness = (insideWaterBlock) ? UnityEngine.Random.Range(-10, 1) + 75 : 75; break;
            }

            return (int)Mathf.Clamp(chestSturdiness + randomMod + roomMod, 1, 100);
        }

        public static void SetChestHitPoints(LLCObject chest)
        {
            float randomMod = Mathf.Abs((UnityEngine.Random.Range(-30, 31) / 100f) - 1f);
            int finalHP = (int)Mathf.Max(1, chest.ChestSturdiness * randomMod * 5f);
            chest.ChestStartHP = finalHP;
            chest.ChestCurrentHP = finalHP;
        }

        public static int RollLockSturdiness(LockMaterials lockMat, string dungBlocName, int roomValueMod)
        {
            int lockSturdiness = 1;
            int randomMod = UnityEngine.Random.Range(-20, 21);
            int roomMod = (int)Mathf.Clamp(Mathf.Round(roomValueMod / 5f), -10, 11); // May add some random factor to this later, but for now just static based on room value mod stuff.
            bool insideWaterBlock = false; // Will determine this properly later when I get into the Unity editor again and see how getting the block-name works and such.

            switch (lockMat)
            {
                default:
                case LockMaterials.Wood:
                    lockSturdiness = (insideWaterBlock) ? UnityEngine.Random.Range(-15, 1) + 10 : 10; break;
                case LockMaterials.Iron:
                    lockSturdiness = (insideWaterBlock) ? UnityEngine.Random.Range(-15, 1) + 40 : 40; break;
                case LockMaterials.Steel:
                    lockSturdiness = (insideWaterBlock) ? UnityEngine.Random.Range(-10, 1) + 60 : 60; break;
                case LockMaterials.Orcish:
                    lockSturdiness = (insideWaterBlock) ? UnityEngine.Random.Range(-10, 1) + 80 : 80; break;
                case LockMaterials.Mithril:
                    lockSturdiness = (insideWaterBlock) ? UnityEngine.Random.Range(-5, 1) + 70 : 70; break;
                case LockMaterials.Dwarven:
                    lockSturdiness = (insideWaterBlock) ? UnityEngine.Random.Range(-5, 1) + 50 : 50; break;
                case LockMaterials.Adamantium:
                    lockSturdiness = (insideWaterBlock) ? UnityEngine.Random.Range(-5, 1) + 30 : 30; break;
                case LockMaterials.Daedric:
                    lockSturdiness = (insideWaterBlock) ? UnityEngine.Random.Range(-10, 1) + 75 : 75; break;
            }

            return (int)Mathf.Clamp(lockSturdiness + randomMod + roomMod, 1, 100);
        }

        public static void SetLockHitPoints(LLCObject chest)
        {
            float randomMod = Mathf.Abs((UnityEngine.Random.Range(-30, 31) / 100f) - 1f);
            int finalHP = (int)Mathf.Max(1, chest.LockSturdiness * randomMod * 2f);
            chest.LockStartHP = finalHP;
            chest.LockCurrentHP = finalHP;
        }

        public static int RollChestMagicResist(ChestMaterials chestMat, int chestSturdy, int roomValueMod)
        {
            int chestMagicRes = 1;
            int randomMod = UnityEngine.Random.Range(-10, 11);
            int roomMod = (int)Mathf.Clamp(Mathf.Round(roomValueMod / 7f), -5, 6); // May add some random factor to this later, but for now just static based on room value mod stuff.

            switch (chestMat)
            {
                default:
                case ChestMaterials.Wood:
                    chestMagicRes = 10; break;
                case ChestMaterials.Iron:
                    chestMagicRes = 10; break;
                case ChestMaterials.Steel:
                    chestMagicRes = 20; break;
                case ChestMaterials.Orcish:
                    chestMagicRes = 30; break;
                case ChestMaterials.Mithril:
                    chestMagicRes = 45; break;
                case ChestMaterials.Dwarven:
                    chestMagicRes = 60; break;
                case ChestMaterials.Adamantium:
                    chestMagicRes = 90; break;
                case ChestMaterials.Daedric:
                    chestMagicRes = 80; break;
            }

            return (int)Mathf.Clamp(chestMagicRes + randomMod + roomMod, 1, 100);
        }

        public static int RollLockMagicResist(LockMaterials lockMat, int lockSturdy, int roomValueMod)
        {
            int lockMagicRes = 1;
            int randomMod = UnityEngine.Random.Range(-10, 11);
            int roomMod = (int)Mathf.Clamp(Mathf.Round(roomValueMod / 7f), -5, 6); // May add some random factor to this later, but for now just static based on room value mod stuff.

            switch (lockMat)
            {
                default:
                case LockMaterials.Wood:
                    lockMagicRes = 10; break;
                case LockMaterials.Iron:
                    lockMagicRes = 10; break;
                case LockMaterials.Steel:
                    lockMagicRes = 20; break;
                case LockMaterials.Orcish:
                    lockMagicRes = 30; break;
                case LockMaterials.Mithril:
                    lockMagicRes = 45; break;
                case LockMaterials.Dwarven:
                    lockMagicRes = 60; break;
                case LockMaterials.Adamantium:
                    lockMagicRes = 90; break;
                case LockMaterials.Daedric:
                    lockMagicRes = 80; break;
            }

            return (int)Mathf.Clamp(lockMagicRes + randomMod + roomMod, 1, 100);
        }

        public static int RollLockComplexity(LockMaterials lockMat, int roomValueMod)
        {
            int lockComplexity = 1;
            int roomMod = (int)Mathf.Clamp(Mathf.Round(roomValueMod / 5f), -5, 6); // May add some random factor to this later, but for now just static based on room value mod stuff.

            switch (lockMat)
            {
                default:
                case LockMaterials.Wood:
                    lockComplexity = UnityEngine.Random.Range(1, 31); break;
                case LockMaterials.Iron:
                    lockComplexity = UnityEngine.Random.Range(10, 51); break;
                case LockMaterials.Steel:
                    lockComplexity = UnityEngine.Random.Range(25, 86); break;
                case LockMaterials.Orcish:
                    lockComplexity = UnityEngine.Random.Range(35, 101); break;
                case LockMaterials.Mithril:
                    lockComplexity = UnityEngine.Random.Range(20, 71); break;
                case LockMaterials.Dwarven:
                    lockComplexity = UnityEngine.Random.Range(65, 101); break;
                case LockMaterials.Adamantium:
                    lockComplexity = UnityEngine.Random.Range(30, 86); break;
                case LockMaterials.Daedric:
                    lockComplexity = UnityEngine.Random.Range(50, 101); break;
            }

            return (int)Mathf.Clamp(lockComplexity + roomMod, 1, 100);
        }

        public static int RollJamResist(LockMaterials lockMat, int roomValueMod)
        {
            int jamResist = 1;
            int randomMod = UnityEngine.Random.Range(-20, 21);
            int roomMod = (int)Mathf.Clamp(Mathf.Round(roomValueMod / 5f), -10, 11); // May add some random factor to this later, but for now just static based on room value mod stuff.

            switch (lockMat)
            {
                default:
                case LockMaterials.Wood:
                    jamResist = 30; break;
                case LockMaterials.Iron:
                    jamResist = 20; break;
                case LockMaterials.Steel:
                    jamResist = 40; break;
                case LockMaterials.Orcish:
                    jamResist = 50; break;
                case LockMaterials.Mithril:
                    jamResist = 30; break;
                case LockMaterials.Dwarven:
                    jamResist = 60; break;
                case LockMaterials.Adamantium:
                    jamResist = 70; break;
                case LockMaterials.Daedric:
                    jamResist = 80; break;
            }

            return (int)Mathf.Clamp(jamResist + randomMod + roomMod, 1, 100);
        }

        public static void SetLockMechanismHitPoints(LLCObject chest)
        {
            float randomMod = Mathf.Abs((UnityEngine.Random.Range(-30, 31) / 100f) - 1f);
            float stabMod = Mathf.Abs((chest.LockSturdiness * 0.20f / 100f) + 1f);
            int finalHP = (int)Mathf.Max(1, chest.JamResist * stabMod * randomMod * 2f);
            chest.LockMechStartHP = finalHP;
            chest.LockMechCurrentHP = finalHP;
        }

        public static int EnemyRoomValueMods(EnemyEntity enemyEntity) // Will want to take into account modded enemies and such later on when getting to the polishing parts.
        {
            if (enemyEntity.EntityType == EntityTypes.EnemyClass) // Thief-like classes remove room value, non-thief add room value. Logic being one is protecting something, other is trying to take, etc.
            {
                switch (enemyEntity.CareerIndex)
                {
                    case (int)ClassCareers.Battlemage:
                    case (int)ClassCareers.Monk:
                    case (int)ClassCareers.Archer:
                    case (int)ClassCareers.Warrior:
                    case (int)ClassCareers.Knight:
                        return 10;
                    case (int)ClassCareers.Mage:
                    case (int)ClassCareers.Spellsword:
                    case (int)ClassCareers.Sorcerer:
                    case (int)ClassCareers.Healer:
                    case (int)ClassCareers.Ranger:
                        return 5;
                    case (int)ClassCareers.Rogue:
                    case (int)ClassCareers.Acrobat:
                    case (int)ClassCareers.Assassin:
                    case (int)ClassCareers.Barbarian:
                        return -5;
                    case (int)ClassCareers.Nightblade:
                    case (int)ClassCareers.Bard:
                    case (int)ClassCareers.Burglar:
                    case (int)ClassCareers.Thief:
                        return -10;
                    default:
                        return 0;
                }
            }
            else
            {
                switch (enemyEntity.CareerIndex) // Assumed context that room/area would be in that would warrant based on monsters/beasts near it, some remove, many add to value based on this.
                {
                    case (int)MonsterCareers.Rat:
                        return -10;
                    case (int)MonsterCareers.GiantBat:
                    case (int)MonsterCareers.Spider:
                    case (int)MonsterCareers.Slaughterfish:
                    case (int)MonsterCareers.Giant:
                    case (int)MonsterCareers.GiantScorpion:
                        return -6;
                    case (int)MonsterCareers.GrizzlyBear:
                    case (int)MonsterCareers.SabertoothTiger:
                    case (int)MonsterCareers.Harpy:
                    case (int)MonsterCareers.Dreugh:
                        return -3;
                    case (int)MonsterCareers.Imp:
                    case (int)MonsterCareers.OrcSergeant:
                    case (int)MonsterCareers.Ghost:
                    case (int)MonsterCareers.Dragonling:
                        return 2;
                    case (int)MonsterCareers.SkeletalWarrior:
                    case (int)MonsterCareers.OrcShaman:
                    case (int)MonsterCareers.Wraith:
                    case (int)MonsterCareers.Daedroth:
                    case (int)MonsterCareers.Vampire:
                    case (int)MonsterCareers.FireAtronach:
                    case (int)MonsterCareers.FleshAtronach:
                    case (int)MonsterCareers.IceAtronach:
                        return 3;
                    case (int)MonsterCareers.Mummy:
                    case (int)MonsterCareers.Gargoyle:
                    case (int)MonsterCareers.FrostDaedra:
                    case (int)MonsterCareers.FireDaedra:
                    case (int)MonsterCareers.IronAtronach:
                    case (int)MonsterCareers.Lamia:
                        return 6;
                    case (int)MonsterCareers.OrcWarlord:
                    case (int)MonsterCareers.DaedraSeducer:
                    case (int)MonsterCareers.VampireAncient:
                    case (int)MonsterCareers.DaedraLord:
                    case (int)MonsterCareers.Lich:
                    case (int)MonsterCareers.AncientLich:
                    case (int)MonsterCareers.Dragonling_Alternate:
                        return 10;
                    default:
                        return 0;
                }
            }
        }

        public static int BillboardRoomValueMods(FlatTypes type, int archive, int record)
        {
            switch (archive)
            {
                case 97: // Divine Statues
                case 98: // Monster Statues
                case 202: // Demonic Statues
                    return 11;
                case 208: // Lab Equipment
                case 216: // Treasure
                    return 7;
                case 205: // Containers
                case 207: // Equipment
                case 209: // Library Stuff
                    return 4;
                case 200: // Misc Furniture
                case 204: // Various Clothing
                case 210: // Lights
                case 218: // Pots and Pans
                    return 2;
                case 214: // Work Tools
                    return -2;
                case 201: // Animals
                    return -5;
                case 206: // Death
                    return -11;
                default:
                    return 0;
            }
        }

        public static int ModelRoomValueMods(int modelID) // Will have to check and take into consideration mods like Hand-painted models and see what that might do to these in that circumstance.
        {
            if ((modelID >= 55000 && modelID <= 55005) || (modelID >= 55013 && modelID <= 55015)) // Presumably Non-secret doors
                return 7;
            if ((modelID >= 55006 && modelID <= 55012) || (modelID >= 55017 && modelID <= 55033)) // Presumably Secret doors
                return 20;
            if (modelID >= 41000 && modelID <= 41002) // Bed models
                return 10;
            if (modelID >= 41815 && modelID <= 41834) // Crate models
                return 3;
            if (modelID >= 41811 && modelID <= 41813) // Chest models
                return 15;

            return 0;
        }

        public static bool IsValidShop(DFLocation.BuildingTypes buildingType) // Check if building shop type is valid to have chests be spawned in it.
        {
            switch (buildingType)
            {
                case DFLocation.BuildingTypes.Alchemist:
                case DFLocation.BuildingTypes.Armorer:
                case DFLocation.BuildingTypes.WeaponSmith:
                case DFLocation.BuildingTypes.GeneralStore:
                case DFLocation.BuildingTypes.PawnShop:
                case DFLocation.BuildingTypes.FurnitureStore:
                case DFLocation.BuildingTypes.GemStore:
                case DFLocation.BuildingTypes.ClothingStore:
                case DFLocation.BuildingTypes.Bookseller:
                case DFLocation.BuildingTypes.Library:
                case DFLocation.BuildingTypes.Bank:
                    return true;
                default:
                    return false;
            }
        }
    }
}