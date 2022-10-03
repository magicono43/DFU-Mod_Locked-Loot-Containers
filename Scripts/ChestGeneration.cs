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

namespace LockedLootContainers
{
    public partial class LockedLootContainersMain
    {
        #region Fields

        public const PermittedMaterials PermittedMaterials_None = PermittedMaterials.None;
        public const PermittedMaterials PermittedMaterials_FireProof = (PermittedMaterials)0b_1111_1100;
        public const PermittedMaterials PermittedMaterials_All = (PermittedMaterials)0b_1111_1111;

        #endregion

        public void AddChests_OnTransitionDungeonInterior(PlayerEnterExit.TransitionEventArgs args)
        {
            DFLocation locationData = GameManager.Instance.PlayerGPS.CurrentLocation;
            DaggerfallLoot[] lootPiles;

            PermittedMaterials allowedMats = PermittedMaterials_All; // Default will be allow all materials for chests and locks to generate, unless specified otherwise later in process.

            if (GameManager.Instance.PlayerEnterExit.IsPlayerInside)
            {
                switch (locationData.MapTableData.DungeonType) // These will give various modifier values which will then be used afterward to do the actual "work" part in the generation process.
                {
                    case DFRegion.DungeonTypes.Crypt: // Maybe the next parameters will be for when I get to defining what traps, amounts, and trap types are allowed/common in some dungeon types.
                        allowedMats = PermittedMaterials_All; break;
                    case DFRegion.DungeonTypes.OrcStronghold:
                        allowedMats = (PermittedMaterials)0b_0001_1110; break;
                    case DFRegion.DungeonTypes.HumanStronghold:
                        allowedMats = PermittedMaterials_All; break;
                    case DFRegion.DungeonTypes.Prison:
                        allowedMats = (PermittedMaterials)0b_0000_0011; break;
                    case DFRegion.DungeonTypes.DesecratedTemple:
                        allowedMats = (PermittedMaterials)0b_0111_1111; break;
                    case DFRegion.DungeonTypes.Mine:
                        allowedMats = (PermittedMaterials)0b_0011_0111; break;
                    case DFRegion.DungeonTypes.NaturalCave:
                        allowedMats = (PermittedMaterials)0b_0111_1111; break;
                    case DFRegion.DungeonTypes.Coven:
                        allowedMats = (PermittedMaterials)0b_1111_0001; break;
                    case DFRegion.DungeonTypes.VampireHaunt:
                        allowedMats = PermittedMaterials_All; break;
                    case DFRegion.DungeonTypes.Laboratory:
                        allowedMats = (PermittedMaterials)0b_1111_0110; break;
                    case DFRegion.DungeonTypes.HarpyNest:
                        allowedMats = (PermittedMaterials)0b_1111_0111; break;
                    case DFRegion.DungeonTypes.RuinedCastle:
                        allowedMats = PermittedMaterials_All; break;
                    case DFRegion.DungeonTypes.SpiderNest:
                        allowedMats = (PermittedMaterials)0b_0111_1111; break;
                    case DFRegion.DungeonTypes.GiantStronghold:
                        allowedMats = (PermittedMaterials)0b_0000_0111; break;
                    case DFRegion.DungeonTypes.DragonsDen:
                        allowedMats = PermittedMaterials_FireProof; break;
                    case DFRegion.DungeonTypes.BarbarianStronghold:
                        allowedMats = (PermittedMaterials)0b_0001_1111; break;
                    case DFRegion.DungeonTypes.VolcanicCaves:
                        allowedMats = PermittedMaterials_FireProof; break;
                    case DFRegion.DungeonTypes.ScorpionNest:
                        allowedMats = (PermittedMaterials)0b_0111_1111; break;
                    case DFRegion.DungeonTypes.Cemetery:
                        allowedMats = (PermittedMaterials)0b_0000_0011; break;
                    default:
                        allowedMats = PermittedMaterials_All; break;
                }

                if (locationData.MapTableData.DungeonType == DFRegion.DungeonTypes.Cemetery)
                {
                    // Make list of loot-piles currently in the dungeon "scene."
                    lootPiles = FindObjectsOfType<DaggerfallLoot>();

                    for (int i = 0; i < lootPiles.Length; i++)
                    {
                        if (lootPiles[i].ContainerType == LootContainerTypes.RandomTreasure)
                        {
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
                                if (Physics.Raycast(rays[h], out hit, 50f, PlayerLayerMask)) // Using raycast instead of sphere, as I want it to be only if you are purposely targeting the chest.
                                {
                                    rayDistances[h] = hit.distance;
                                }
                                else
                                {
                                    rayDistances[h] = 50f;
                                }
                            }

                            boxDimensions[0] = (rayDistances[0] + rayDistances[1]) / 2; // y-axis? up and down
                            boxDimensions[1] = (rayDistances[2] + rayDistances[3]) / 2; // x-axis? right and left
                            boxDimensions[2] = (rayDistances[4] + rayDistances[5]) / 2; // z-axis? front and back
                            Vector3 boxDimVector = new Vector3(boxDimensions[1], boxDimensions[0], boxDimensions[2]);

                            // Collect GameObjects that overlap the box and ignore duplicates, use these later to estimate a "room value" that a chest is being generated in.
                            List<GameObject> roomObjects = new List<GameObject>();
                            Collider[] overlaps = Physics.OverlapBox(pos, boxDimVector, lootPiles[i].transform.rotation, PlayerLayerMask);
                            for (int r = 0; r < overlaps.Length; r++)
                            {
                                GameObject go = overlaps[r].gameObject;

                                if (go && go == lootPiles[i].gameObject) // Ignore the gameObject (lootpile) this box is originating from.
                                    continue;

                                if (go && !roomObjects.Contains(go))
                                {
                                    roomObjects.Add(go);
                                    //Debug.LogFormat("Box overlapped GameObject, {0}", go.name);
                                }

                                // Continue work from here tomorrow. Try and do filtering out of non-important room objects for our "room value" context related stuff,
                                // which will then be used to somehow modify/determine the odds of certain chest types and rarities being generated in certain locations.
                                // If I need examples for this look at "DaggerfallMissile.cs" under the "DoAreaOfEffect" method for something fairly similar.

                                DaggerfallEntityBehaviour aoeEntity = overlaps[r].GetComponent<DaggerfallEntityBehaviour>(); // Use this as an example for getting components I care about?
                            }
                        }
                    }
                }

                if (locationData.MapTableData.DungeonType == DFRegion.DungeonTypes.Cemetery)
                {
                    // Make list of loot-piles currently in the dungeon "scene."
                    lootPiles = FindObjectsOfType<DaggerfallLoot>();

                    for (int i = 0; i < lootPiles.Length; i++)
                    {
                        if (lootPiles[i].ContainerType == LootContainerTypes.RandomTreasure)
                        {
                            ItemCollection oldPileLoot = lootPiles[i].Items;
                            Transform oldLootPileTransform = lootPiles[i].transform;
                            Vector3 pos = lootPiles[i].transform.position;
                            ulong oldLoadID = lootPiles[i].LoadID;

                            GameObject chestParentObj = GameObjectHelper.CreateDaggerfallBillboardGameObject(810, 0, oldLootPileTransform.parent.parent);

                            LLCObject llcObj = chestParentObj.AddComponent<LLCObject>();
                            llcObj.Oldloot = oldPileLoot;
                            llcObj.AttachedLoot = llcObj.Oldloot; // Will change this later, but placeholder for testing.
                            llcObj.LoadID = oldLoadID;

                            // Set position
                            Billboard dfBillboard = chestParentObj.GetComponent<Billboard>();
                            chestParentObj.transform.position = pos;
                            chestParentObj.transform.position += new Vector3(0, dfBillboard.Summary.Size.y / 2, 0);
                            GameObjectHelper.AlignBillboardToGround(chestParentObj, dfBillboard.Summary.Size);

                            Destroy(lootPiles[i].gameObject); // Removed old loot-pile from scene, but saved its characteristics we care about.
                        }
                    }
                }
            }
        }

        public static string BuildName()
        {
            return GameManager.Instance.PlayerEnterExit.BuildingDiscoveryData.displayName;
        }

        public static string CityName()
        {   // %cn
            PlayerGPS gps = GameManager.Instance.PlayerGPS;
            if (gps.HasCurrentLocation)
                return gps.CurrentLocation.Name;
            else
                return gps.CurrentRegion.Name;
        }

        private static string CurrentRegion()
        {   // %crn going to use for %reg as well here
            return GameManager.Instance.PlayerGPS.CurrentRegion.Name;
        }

        public static string OneWordQuality(int quality)
        {
            int variant = UnityEngine.Random.Range(0, 11);
            string word = "Bugged";

            if (quality <= 3) // 01 - 03
            {
                if (variant == 0) { word = "Dreadful"; }
                else if (variant == 1) { word = "Horrendous"; }
                else if (variant == 2) { word = "Horrible"; }
                else if (variant == 3) { word = "Atrocious"; }
                else if (variant == 4) { word = "Horrid"; }
                else if (variant == 5) { word = "Awful"; }
                else { word = "Terrible"; }
            }
            else if (quality <= 7) // 04 - 07
            {
                if (variant == 0) { word = "Bad"; }
                else if (variant == 1) { word = "Reduced"; }
                else if (variant == 2) { word = "Low"; }
                else if (variant == 3) { word = "Inferior"; }
                else if (variant == 4) { word = "Meager"; }
                else if (variant == 5) { word = "Scant"; }
                else { word = "Poor"; }
            }
            else if (quality <= 13) // 08 - 13
            {
                if (variant == 0) { word = "Modest"; }
                else if (variant == 1) { word = "Medium"; }
                else if (variant == 2) { word = "Moderate"; }
                else if (variant == 3) { word = "Passable"; }
                else if (variant == 4) { word = "Adequate"; }
                else if (variant == 5) { word = "Typical"; }
                else { word = "Average"; }
            }
            else if (quality <= 17) // 14 - 17
            {
                if (variant == 0) { word = "Reasonable"; }
                else if (variant == 1) { word = "Solid"; }
                else if (variant == 2) { word = "Honorable"; }
                else if (variant == 3) { word = "Nice"; }
                else if (variant == 4) { word = "True"; }
                else if (variant == 5) { word = "Worthy"; }
                else { word = "Good"; }
            }
            else // 18 - 20
            {
                if (variant == 0) { word = "Exemplary"; }
                else if (variant == 1) { word = "Esteemed"; }
                else if (variant == 2) { word = "Astonishing"; }
                else if (variant == 3) { word = "Marvelous"; }
                else if (variant == 4) { word = "Wonderful"; }
                else if (variant == 5) { word = "Spectacular"; }
                else if (variant == 6) { word = "Incredible"; }
                else { word = "Exceptional"; }
            }

            return word;
        }
    }
}