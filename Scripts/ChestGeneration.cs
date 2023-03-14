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

namespace LockedLootContainers
{
    public partial class LockedLootContainersMain
    {
        #region Fields

        public bool[] PermittedMaterials_None = new bool[] { false, false, false, false, false, false, false, false };
        public bool[] PermittedMaterials_FireProof = new bool[] { false, false, true, true, true, true, true, true };
        public bool[] PermittedMaterials_All = new bool[] { true, true, true, true, true, true, true, true };

        #endregion

        public void AddChests_OnTransitionDungeonInterior(PlayerEnterExit.TransitionEventArgs args)
        {
            if (SaveLoadManager.Instance.LoadInProgress) // Hopefully this will keep this from running when loading a save, but not when normally entering and exiting while playing, etc.
                return;

            DFLocation locationData = GameManager.Instance.PlayerGPS.CurrentLocation;
            DaggerfallLoot[] lootPiles;

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
                        // Start back here next time I suppose. Keep getting the feeling that I'm going to be reworking this entire system as I go along, but if I keep trying to find this unattainable
                        // "Perfect Method" I'm just going to be sitting here spinning my wheels and getting nowhere. So even if this method has to be reworked/redone later, getting to there is more
                        // important than it actually working in the end honestly. Main thing I'm still not sure how I'm going to do is specifying certain items from each item group so they make
                        // more sense depending on the context, etc. But I'll just have to probably fail a few times before I find "better" method to do this, so just keep chugging along even if
                        // it does not seem like it makes sense, otherwise I"ll be getting nowhere no matter how much I contemplate and think about it, just go.
                        // Might want to consider a sort of point based system here for the loot odds or something, so it's easier to try and "balance out" while doing this maybe.
                }

                // Make list of loot-piles currently in the dungeon "scene."
                lootPiles = FindObjectsOfType<DaggerfallLoot>();

                for (int i = 0; i < lootPiles.Length; i++)
                {
                    if (lootPiles[i].ContainerType == LootContainerTypes.RandomTreasure && lootPiles[i].ContainerImage == InventoryContainerImages.Chest)
                    {
                        int totalRoomValueMod = 0;

                        Transform oldLootPileTransform = lootPiles[i].transform;
                        Vector3 pos = lootPiles[i].transform.position;

                        Ray[] rays = { new Ray(pos, oldLootPileTransform.up), new Ray(pos, -oldLootPileTransform.up),
                        new Ray(pos, oldLootPileTransform.right), new Ray(pos, -oldLootPileTransform.right),
                        new Ray(pos, oldLootPileTransform.forward), new Ray(pos, -oldLootPileTransform.forward)}; // up, down, right, left, front, back.

                        float[] rayDistances = { 0, 0, 0, 0, 0, 0 }; // Distances that up, down, right, left, front, and back rays traveled before hitting a collider.
                        float[] boxDimensions = { 0.2f, 0.2f, 0.2f }; // Default dimensions of the overlap box.

                        RaycastHit hit;
                        for (int h = 0; h < rays.Length; h++)
                        {
                            if (Physics.Raycast(rays[h], out hit, 12f, PlayerLayerMask)) // Using raycast instead of sphere, as I want it to be only if you are purposely targeting the chest.
                            {
                                rayDistances[h] = hit.distance;
                            }
                            else
                            {
                                rayDistances[h] = 12f;
                            }
                        }

                        boxDimensions[0] = (rayDistances[0] + rayDistances[1]); // y-axis? up and down
                        boxDimensions[1] = (rayDistances[2] + rayDistances[3]); // x-axis? right and left
                        boxDimensions[2] = (rayDistances[4] + rayDistances[5]); // z-axis? front and back
                        Vector3 boxDimVector = new Vector3(boxDimensions[1], boxDimensions[0], boxDimensions[2]) * 2f;

                        // Testing Box Sizes Here
                        /*GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.position = pos;
                        cube.transform.rotation = lootPiles[i].transform.rotation;
                        cube.transform.localScale = boxDimVector;*/

                        // Collect GameObjects that overlap the box and ignore duplicates, use these later to estimate a "room value" that a chest is being generated in.
                        // Test this part more properly tomorrow, now after knowing some of the limitations now due to the "combined models" thing with dungeons and such.
                        // Primarily be checking for flats/billboards, other loot-piles, enemies, area of overlap box, action objects, and especially doors.
                        List<GameObject> roomObjects = new List<GameObject>();
                        Collider[] overlaps = Physics.OverlapBox(pos, boxDimVector, lootPiles[i].transform.rotation, PlayerLayerMask);
                        for (int r = 0; r < overlaps.Length; r++)
                        {
                            GameObject go = overlaps[r].gameObject;

                            if (go && go == lootPiles[i].gameObject) // Ignore the gameObject (lootpile) this box is originating from.
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

                        // This section below is once again primarily for testing atm. This is for all checked component types.
                        Debug.LogFormat("Loot-pile being checked is named: {0}. With the Load ID: {1}. With Transform: x = {2}, y = {3}, z = {4}", lootPiles[i].name, lootPiles[i].LoadID, lootPiles[i].gameObject.transform.parent.localPosition.x, lootPiles[i].gameObject.transform.parent.localPosition.y, lootPiles[i].gameObject.transform.parent.localPosition.z);
                        Debug.LogFormat("||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||| {0}", i);
                        for (int g = 0; g < roomObjects.Count; g++)
                        {
                            if (roomObjects[g])
                            {
                                DaggerfallEntityBehaviour entityBehaviour = roomObjects[g].GetComponent<DaggerfallEntityBehaviour>();
                                DaggerfallBillboard billBoard = roomObjects[g].GetComponent<DaggerfallBillboard>(); // Will have to test and eventually account for mods like "Handpainted Models" etc.
                                MeshFilter meshFilter = roomObjects[g].GetComponent<MeshFilter>();

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
                                else if (meshFilter)
                                {
                                    int modelID = -1;
                                    bool validID = false;
                                    string meshName = meshFilter.mesh.name;

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
                                    Debug.LogFormat("Overlap found on gameobject: {0}     By loot-pile with Load ID: {1}", roomObjects[g].name, lootPiles[i].LoadID);
                                }
                            }
                        }
                        Debug.LogFormat("-------------------------------------------------------------------------------------------------------------- {0}-", i);
                        Debug.LogFormat("-------------------------------------- {0}-", i);

                        RollIfChestShouldBeCreated(lootPiles[i], allowedMats, baseChestOdds, totalRoomValueMod, miscGroupOdds, itemGroupOdds);
                    }
                }
            }
        }

        // This is really rough, mainly just for testing, because having some trouble thinking of a good formula for it all atm.
        // Definitely going to have to tweak and probably reconsider how the room context stuff modifies the chest odds, might be too drastic in some contexts and dungeon types, will have to see.
        public static void RollIfChestShouldBeCreated(DaggerfallLoot lootPile, bool[] allowedMats, int baseChestOdds, int totalRoomValueMod, int[] miscGroupOdds, int[] itemGroupOdds)
        {
            int chestOdds = baseChestOdds + totalRoomValueMod;
            int permitMatsCount = 0; // Total count of materials that are "true" from "allowedMats" bool array, if there is none then return and don't generate chest.

            if (chestOdds <= 0)
                return;

            for (int i = 0; i < allowedMats.Length; i++)
            {
                if (allowedMats[i] == true)
                    permitMatsCount++;
            }

            if (permitMatsCount <= 0)
                return;

            if (Dice100.SuccessRoll(chestOdds)) // If roll is successful, start process of replacing loot-pile with chest object as well as what properties the chest should have.
            {
                ItemCollection oldPileLoot = new ItemCollection();
                oldPileLoot.TransferAll(lootPile.Items);
                Transform oldLootPileTransform = lootPile.transform;
                Vector3 pos = lootPile.transform.position;
                ulong newLoadID = DaggerfallUnity.NextUID;

                GameObject chestParentObj = GameObjectHelper.CreateDaggerfallBillboardGameObject(4733, 0, oldLootPileTransform.parent.parent);

                LLCObject llcObj = chestParentObj.AddComponent<LLCObject>();
                llcObj.Oldloot = oldPileLoot;
                llcObj.AttachedLoot = llcObj.Oldloot; // Will change this later, but placeholder for testing.
                llcObj.LoadID = newLoadID;

                // Creates random seed for determining chest material value
                string combinedText = llcObj.LoadID.ToString().Substring(4) + UnityEngine.Random.Range(333, 99999).ToString();
                string truncText = (combinedText.Length > 9) ? combinedText.Substring(0, 9) : combinedText; // Might get index out of range, but will see.
                Debug.LogFormat("Attempting to parse value for chest seed: {0}", truncText);
                int seed = int.Parse(truncText);
                UnityEngine.Random.InitState(seed); // This is to attempt to combat patterns in generation due to this all happening in a small period of time with a similar system-time seed by default.

                // Adding new properties to chest based on rolls for chest materials, rarity, possibly traps later, etc.
                llcObj.ChestMaterial = RollChestMaterial(allowedMats, permitMatsCount, totalRoomValueMod); // Alright for now just do something simple without a limited ticket sort of system, will try that more later on.

                // Creates random seed for determining lock material value and other values
                string jumbledText = truncText.Substring(5) + UnityEngine.Random.Range(333, 99999).ToString();
                truncText = (jumbledText.Length > 9) ? jumbledText.Substring(0, 9) : jumbledText; // Might get index out of range, but will see.
                Debug.LogFormat("Attempting to parse value for lock & others seed: {0}", truncText);
                seed = int.Parse(truncText);
                UnityEngine.Random.InitState(seed); // This is to attempt to combat patterns in generation due to this all happening in a small period of time with a similar system-time seed by default.

                llcObj.LockMaterial = RollLockMaterial(allowedMats, permitMatsCount, totalRoomValueMod, llcObj.ChestMaterial);
                /*
                So I'm thinking I'll try and make a sort of "Hat of tickets" function that will populate a "hat" of a limited number of possible "tickets" to pick from.
                This will work kind of similar to my random generation stuff in stuff like "Jewelry Additions", but possibly better, but also maybe more complex?
                But the tickets will have a value given to them based on what values from the possible chest and lock materials are permitted in this case. Somehow the rarity
                of the material in question will determine what proportion of tickets from the limited pool that material is given. Maybe have the flags checked from least rare to most rare,
                and give the least rare some large amount of tickets from the max at the start, then slowly go through the rest of the permitted ones from what amount of tickets are remaining
                and give them tickets based on what are left each time another is given more or something. Somehow also try to incorperate the "totalRoomValueMod" to either increase the
                amount of tickets rarer materials get, if it's positive above some value, or more to less rare materials if it's below some value or something. Don't know how I'll do this
                all exactly, but atleast I have some idea how I might try it. Worst case I could try something similar to how vanilla DFU determines the plate armors in FormulaHelper.cs?
                Don't know, will have to see tomorrow how it pans out, but might be a bit confusing at first, maybe make the tickets from the pool like 300, 500, or 1000 or something, will see.
                */

                llcObj.ChestSturdiness = RollChestSturdiness(llcObj.ChestMaterial, lootPile.transform.parent.parent.name, totalRoomValueMod);
                llcObj.LockSturdiness = RollLockSturdiness(llcObj.LockMaterial, lootPile.transform.parent.parent.name, totalRoomValueMod);

                llcObj.ChestHitPoints = CalculateChestHitPoints(llcObj.ChestSturdiness);
                llcObj.LockHitPoints = CalculateLockHitPoints(llcObj.LockSturdiness);

                llcObj.ChestMagicResist = RollChestMagicResist(llcObj.ChestMaterial, llcObj.ChestSturdiness, totalRoomValueMod);
                llcObj.LockMagicResist = RollLockMagicResist(llcObj.LockMaterial, llcObj.LockSturdiness, totalRoomValueMod);

                llcObj.LockComplexity = RollLockComplexity(llcObj.LockMaterial, totalRoomValueMod);
                llcObj.JamResist = RollJamResist(llcObj.LockMaterial, totalRoomValueMod);
                llcObj.LockMechHitPoints = CalculateLockMechanismHitPoints(llcObj.JamResist, llcObj.LockSturdiness);

                UnityEngine.Random.InitState((int)DateTime.Now.Ticks); // Here to try and reset the random generation seed value back to the "default" for what Unity normally uses in most operations.

                // ALSO NOTE: Add console debug helper comments/lines to make viewing generation results quicker, DO THIS BEFORE THE BELOW STUFF TOMORROW!
                // Possibly also look at giving more random seeds somehow during chest generation to reduce potential overservable patterns, using stuff like hashcodes or something.
                // Now that I have the primary immediately important attributes set and defined for the chest and lock, I will next do some actual testing of all of this in the Unity Editor, to confirm
                // that it works, as well as make any fixes/modifications of these features for now, so then afterward I can work on the next aspects. So basically, test what I have done the past few
                // days in Unity Editor, then afterward if I am satisfied with the results generally, I will likely start on the next mod feature aspect, that being dealing with the loot generation
                // such as how old loot items are handled and imported in or not, what the overall value of items and what ones to generate for a specific chest based on many variables, and possibly some
                // other things as well. The loot part might time some time to get right, but then after all that, probably try to work on traps, then after those probably more testing and polishing, etc.

                // Set position
                Billboard dfBillboard = chestParentObj.GetComponent<Billboard>();
                chestParentObj.transform.position = pos;
                chestParentObj.transform.position += new Vector3(0, dfBillboard.Summary.Size.y / 2, 0);
                GameObjectHelper.AlignBillboardToGround(chestParentObj, dfBillboard.Summary.Size);

                Debug.LogFormat("Chest Generated With Transform: x = {0}, y = {1}, z = {2}. Chest Material = {3}, Sturdiness = {4}, Magic Resist = {5}. With A Lock Made From = {6}, Sturdiness = {7}, Magic Resist = {8}, Lock Complexity = {9}, Jam Resistance = {10}.", chestParentObj.transform.localPosition.x, chestParentObj.transform.localPosition.y, chestParentObj.transform.localPosition.z, llcObj.ChestMaterial.ToString(), llcObj.ChestSturdiness, llcObj.ChestMagicResist, llcObj.LockMaterial.ToString(), llcObj.LockSturdiness, llcObj.LockMagicResist, llcObj.LockComplexity, llcObj.JamResist); // Might have to mess with the position values a bit, might need the "parent" or something instead.

                lootPile.Items.Clear(); // Likely not necessary, but doing it just in case.
                Destroy(lootPile.gameObject);

                // Maybe later on add some Event stuff here so other mods can know when this made added a chest or when loot generation happens for the chests or something? Will see.

                // This is where chest loot generation starts, for now atleast.
                // Will have to change some stuff around in the actual chest "replacement" part to instead use the "new" loot collection rather than just the old one like for testing so far.
                PopulateChestLoot(llcObj, totalRoomValueMod, miscGroupOdds, itemGroupOdds);
            }
        }

        public static ChestMaterials RollChestMaterial(bool[] allowedMats, int permitMatsCount, int roomValueMod)
        {
            int[] itemRolls = new int[] { };
            List<int> itemRollsList = new List<int>();

            for (int i = 0; i < allowedMats.Length; i++)
            {
                int arrayStart = itemRollsList.Count;
                int fillElements = 0;
                switch (i)
                {
                    case (int)PermittedMaterials.Wood:
                        fillElements = (allowedMats[i]) ? (int)Mathf.Clamp(95 - (roomValueMod * 4), 1, 300) : 0; break;
                    case (int)PermittedMaterials.Iron:
                        fillElements = (allowedMats[i]) ? (int)Mathf.Clamp(75 - (roomValueMod * 3), 1, 300) : 0; break;
                    case (int)PermittedMaterials.Steel:
                        fillElements = (allowedMats[i]) ? (int)Mathf.Clamp(45 + (roomValueMod * 2), 1, 300) : 0; break;
                    case (int)PermittedMaterials.Orcish:
                        fillElements = (allowedMats[i]) ? (int)Mathf.Clamp(25 + (roomValueMod * 1), 1, 300) : 0; break;
                    case (int)PermittedMaterials.Mithril:
                        fillElements = (allowedMats[i]) ? (int)Mathf.Clamp(35 + (roomValueMod * 2), 1, 300) : 0; break;
                    case (int)PermittedMaterials.Dwarven:
                        fillElements = (allowedMats[i]) ? (int)Mathf.Clamp(20 + (roomValueMod * 1), 1, 300) : 0; break;
                    case (int)PermittedMaterials.Adamantium:
                        fillElements = (allowedMats[i]) ? (int)Mathf.Clamp(15 + (roomValueMod * 1), 1, 300) : 0; break;
                    case (int)PermittedMaterials.Daedric:
                        fillElements = (allowedMats[i]) ? (int)Mathf.Clamp(5 + (roomValueMod * 1), 1, 300) : 0; break;
                    default:
                        fillElements = 0; break;
                }

                if (fillElements <= 0)
                    continue;

                itemRolls = FillArray(itemRollsList, arrayStart, fillElements, i);
            }

            int chosenMaterial = -1;

            if (itemRolls.Length > 0)
                chosenMaterial = PickOneOf(itemRolls);

            if (chosenMaterial == -1)
            {
                Debug.Log("For some reason this chest material did not get chosen properly, defaulting to wood...");
                return ChestMaterials.Wood;
            }
            else
            {
                chosenMaterial += 1; // Add 1 to this to factor out the "none" value from both chest and lock material enums.
                return (ChestMaterials)chosenMaterial; // Will have to see if this works to return the proper enum value, I think it should but will see with live debugging.
            }
        }

        // I eventually will want the lock material to be somewhat restricted depending on what material the chest it is attached to is made of. So like a daedric chest won't have a wooden lock
        // and stuff like that. Or so that it's unlikely for a wooden chest to have a daedric lock, that sort of thing. I don't know how I will do it right this moment, but I'll try and get that
        // resolved later on for polishing and such. For now, just leave it the same process as for determing chest material basically, just for testing and proof of working and such, etc.
        public static LockMaterials RollLockMaterial(bool[] allowedMats, int permitMatsCount, int roomValueMod, ChestMaterials chestMats)
        {
            int[] itemRolls = new int[] { };
            List<int> itemRollsList = new List<int>();

            for (int i = 0; i < allowedMats.Length; i++)
            {
                if (i < (int)chestMats - 3 || i >= (int)chestMats + 3) // Maybe do this based on a "rarity" value or something, or a general "protection level" or something instead of this current method.
                    continue; // Still need to test this, possibly with live-debugging, because for some reason the concept in numbers is confusing me a bit, will test and see the results.

                int arrayStart = itemRollsList.Count;
                int fillElements = 0;
                switch (i)
                {
                    case (int)PermittedMaterials.Wood:
                        fillElements = (allowedMats[i]) ? (int)Mathf.Clamp(95 - (roomValueMod * 4), 1, 300) : 0; break;
                    case (int)PermittedMaterials.Iron:
                        fillElements = (allowedMats[i]) ? (int)Mathf.Clamp(75 - (roomValueMod * 3), 1, 300) : 0; break;
                    case (int)PermittedMaterials.Steel:
                        fillElements = (allowedMats[i]) ? (int)Mathf.Clamp(45 + (roomValueMod * 2), 1, 300) : 0; break;
                    case (int)PermittedMaterials.Orcish:
                        fillElements = (allowedMats[i]) ? (int)Mathf.Clamp(25 + (roomValueMod * 1), 1, 300) : 0; break;
                    case (int)PermittedMaterials.Mithril:
                        fillElements = (allowedMats[i]) ? (int)Mathf.Clamp(35 + (roomValueMod * 2), 1, 300) : 0; break;
                    case (int)PermittedMaterials.Dwarven:
                        fillElements = (allowedMats[i]) ? (int)Mathf.Clamp(20 + (roomValueMod * 1), 1, 300) : 0; break;
                    case (int)PermittedMaterials.Adamantium:
                        fillElements = (allowedMats[i]) ? (int)Mathf.Clamp(15 + (roomValueMod * 1), 1, 300) : 0; break;
                    case (int)PermittedMaterials.Daedric:
                        fillElements = (allowedMats[i]) ? (int)Mathf.Clamp(5 + (roomValueMod * 1), 1, 300) : 0; break;
                    default:
                        fillElements = 0; break;
                }

                if (i == (int)chestMats)
                    fillElements = (int)Mathf.Floor(fillElements / 4); // This is here to attempt to make having the same material chest and lock less likely, giving the dupe material less tickets.

                if (fillElements <= 0)
                    continue;

                itemRolls = FillArray(itemRollsList, arrayStart, fillElements, i);
            }

            int chosenMaterial = -1;

            if (itemRolls.Length > 0)
                chosenMaterial = PickOneOf(itemRolls);

            if (chosenMaterial == -1)
            {
                Debug.Log("For some reason this chest material did not get chosen properly, defaulting to wood...");
                return LockMaterials.Wood;
            }
            else
            {
                chosenMaterial += 1; // Add 1 to this to factor out the "none" value from both chest and lock material enums.
                return (LockMaterials)chosenMaterial; // Will have to see if this works to return the proper enum value, I think it should but will see with live debugging.
            }
        }

        public static int RollChestSturdiness(ChestMaterials chestMat, string dungBlocName, int roomValueMod)
        {
            int chestSturdiness = 1;
            int randomMod = UnityEngine.Random.Range(-20, 21);
            int roomMod = (int)Mathf.Clamp(Mathf.Round(roomValueMod / 3f), -10, 11); // May add some random factor to this later, but for now just static based on room value mod stuff.
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

        public static int CalculateChestHitPoints(int chestStab)
        {
            float randomMod = Mathf.Abs((UnityEngine.Random.Range(-30, 31) / 100f) - 1f);
            return (int)Mathf.Max(1, chestStab * randomMod * 10f);
        }

        public static int RollLockSturdiness(LockMaterials lockMat, string dungBlocName, int roomValueMod)
        {
            int lockSturdiness = 1;
            int randomMod = UnityEngine.Random.Range(-20, 21);
            int roomMod = (int)Mathf.Clamp(Mathf.Round(roomValueMod / 3f), -10, 11); // May add some random factor to this later, but for now just static based on room value mod stuff.
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

        public static int CalculateLockHitPoints(int lockStab)
        {
            float randomMod = Mathf.Abs((UnityEngine.Random.Range(-30, 31) / 100f) - 1f);
            return (int)Mathf.Max(1, lockStab * randomMod * 3f);
        }

        public static int RollChestMagicResist(ChestMaterials chestMat, int chestSturdy, int roomValueMod)
        {
            int chestMagicRes = 1;
            int randomMod = UnityEngine.Random.Range(-10, 11);
            int roomMod = (int)Mathf.Clamp(Mathf.Round(roomValueMod / 4f), -5, 6); // May add some random factor to this later, but for now just static based on room value mod stuff.

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
            int roomMod = (int)Mathf.Clamp(Mathf.Round(roomValueMod / 4f), -5, 6); // May add some random factor to this later, but for now just static based on room value mod stuff.

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
            int roomMod = (int)Mathf.Clamp(Mathf.Round(roomValueMod / 3f), -5, 6); // May add some random factor to this later, but for now just static based on room value mod stuff.

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
            int roomMod = (int)Mathf.Clamp(Mathf.Round(roomValueMod / 3f), -10, 11); // May add some random factor to this later, but for now just static based on room value mod stuff.

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

        public static int CalculateLockMechanismHitPoints(int jamRes, int lockStab)
        {
            float randomMod = Mathf.Abs((UnityEngine.Random.Range(-30, 31) / 100f) - 1f);
            float stabMod = Mathf.Abs((lockStab * 0.20f / 100f) + 1f);
            return (int)Mathf.Max(1, jamRes * stabMod * randomMod * 2f);
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
                        return 4;
                    case (int)ClassCareers.Mage:
                    case (int)ClassCareers.Spellsword:
                    case (int)ClassCareers.Sorcerer:
                    case (int)ClassCareers.Healer:
                    case (int)ClassCareers.Ranger:
                        return 2;
                    case (int)ClassCareers.Rogue:
                    case (int)ClassCareers.Acrobat:
                    case (int)ClassCareers.Assassin:
                    case (int)ClassCareers.Barbarian:
                        return -2;
                    case (int)ClassCareers.Nightblade:
                    case (int)ClassCareers.Bard:
                    case (int)ClassCareers.Burglar:
                    case (int)ClassCareers.Thief:
                        return -4;
                    default:
                        return 0;
                }
            }
            else
            {
                switch (enemyEntity.CareerIndex) // Assumed context that room/area would be in that would warrant based on monsters/beasts near it, some remove, many add to value based on this.
                {
                    case (int)MonsterCareers.Rat:
                        return -5;
                    case (int)MonsterCareers.GiantBat:
                    case (int)MonsterCareers.Spider:
                    case (int)MonsterCareers.Slaughterfish:
                    case (int)MonsterCareers.Giant:
                    case (int)MonsterCareers.GiantScorpion:
                        return -3;
                    case (int)MonsterCareers.GrizzlyBear:
                    case (int)MonsterCareers.SabertoothTiger:
                    case (int)MonsterCareers.Harpy:
                    case (int)MonsterCareers.Dreugh:
                        return -2;
                    case (int)MonsterCareers.Imp:
                    case (int)MonsterCareers.OrcSergeant:
                    case (int)MonsterCareers.Ghost:
                    case (int)MonsterCareers.Dragonling:
                        return 1;
                    case (int)MonsterCareers.SkeletalWarrior:
                    case (int)MonsterCareers.OrcShaman:
                    case (int)MonsterCareers.Wraith:
                    case (int)MonsterCareers.Daedroth:
                    case (int)MonsterCareers.Vampire:
                    case (int)MonsterCareers.FireAtronach:
                    case (int)MonsterCareers.FleshAtronach:
                    case (int)MonsterCareers.IceAtronach:
                        return 2;
                    case (int)MonsterCareers.Mummy:
                    case (int)MonsterCareers.Gargoyle:
                    case (int)MonsterCareers.FrostDaedra:
                    case (int)MonsterCareers.FireDaedra:
                    case (int)MonsterCareers.IronAtronach:
                    case (int)MonsterCareers.Lamia:
                        return 3;
                    case (int)MonsterCareers.OrcWarlord:
                    case (int)MonsterCareers.DaedraSeducer:
                    case (int)MonsterCareers.VampireAncient:
                    case (int)MonsterCareers.DaedraLord:
                    case (int)MonsterCareers.Lich:
                    case (int)MonsterCareers.AncientLich:
                    case (int)MonsterCareers.Dragonling_Alternate:
                        return 4;
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
                    return 5;
                case 208: // Lab Equipment
                case 216: // Treasure
                    return 3;
                case 205: // Containers
                case 207: // Equipment
                case 209: // Library Stuff
                    return 2;
                case 200: // Misc Furniture
                case 204: // Various Clothing
                case 210: // Lights
                case 218: // Pots and Pans
                    return 1;
                case 214: // Work Tools
                    return -1;
                case 201: // Animals
                    return -2;
                case 206: // Death
                    return -3;
                default:
                    return 0;
            }
        }

        public static int ModelRoomValueMods(int modelID) // Will have to check and take into consideration mods like Hand-painted models and see what that might do to these in that circumstance.
        {
            if ((modelID >= 55000 && modelID <= 55005) || (modelID >= 55013 && modelID <= 55015)) // Presumably Non-secret doors
                return 4;
            if ((modelID >= 55006 && modelID <= 55012) || (modelID >= 55017 && modelID <= 55033)) // Presumably Secret doors
                return 12;
            if (modelID >= 41000 && modelID <= 41002) // Bed models
                return 6;
            if (modelID >= 41815 && modelID <= 41834) // Crate models
                return 2;
            if (modelID >= 41811 && modelID <= 41813) // Chest models
                return 14;

            return 0;
        }
    }
}