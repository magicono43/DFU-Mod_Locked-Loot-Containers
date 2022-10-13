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
            int baseChestOdds = 20; // This value will be changed based on the type of dungeon, which will determine the base odds for a chest to be generated in place of a loot-pile in the end.

            if (GameManager.Instance.PlayerEnterExit.IsPlayerInside)
            {
                switch (locationData.MapTableData.DungeonType) // These will give various modifier values which will then be used afterward to do the actual "work" part in the generation process.
                {
                    case DFRegion.DungeonTypes.Crypt: // Maybe the next parameters will be for when I get to defining what traps, amounts, and trap types are allowed/common in some dungeon types.
                        allowedMats = PermittedMaterials_All; baseChestOdds = 35; break;
                    case DFRegion.DungeonTypes.OrcStronghold:
                        allowedMats = (PermittedMaterials)0b_0001_1110; baseChestOdds = 35; break;
                    case DFRegion.DungeonTypes.HumanStronghold:
                        allowedMats = PermittedMaterials_All; baseChestOdds = 40; break;
                    case DFRegion.DungeonTypes.Prison:
                        allowedMats = (PermittedMaterials)0b_0000_0011; baseChestOdds = 20; break;
                    case DFRegion.DungeonTypes.DesecratedTemple:
                        allowedMats = (PermittedMaterials)0b_0111_1111; baseChestOdds = 35; break;
                    case DFRegion.DungeonTypes.Mine:
                        allowedMats = (PermittedMaterials)0b_0011_0111; baseChestOdds = 25; break;
                    case DFRegion.DungeonTypes.NaturalCave:
                        allowedMats = (PermittedMaterials)0b_0111_1111; baseChestOdds = 20; break;
                    case DFRegion.DungeonTypes.Coven:
                        allowedMats = (PermittedMaterials)0b_1111_0001; baseChestOdds = 30; break;
                    case DFRegion.DungeonTypes.VampireHaunt:
                        allowedMats = PermittedMaterials_All; baseChestOdds = 30; break;
                    case DFRegion.DungeonTypes.Laboratory:
                        allowedMats = (PermittedMaterials)0b_1111_0110; baseChestOdds = 30; break;
                    case DFRegion.DungeonTypes.HarpyNest:
                        allowedMats = (PermittedMaterials)0b_1111_0111; baseChestOdds = 20; break;
                    case DFRegion.DungeonTypes.RuinedCastle:
                        allowedMats = PermittedMaterials_All; baseChestOdds = 50; break;
                    case DFRegion.DungeonTypes.SpiderNest:
                        allowedMats = (PermittedMaterials)0b_0111_1111; baseChestOdds = 20; break;
                    case DFRegion.DungeonTypes.GiantStronghold:
                        allowedMats = (PermittedMaterials)0b_0000_0111; baseChestOdds = 25; break;
                    case DFRegion.DungeonTypes.DragonsDen:
                        allowedMats = PermittedMaterials_FireProof; baseChestOdds = 55; break;
                    case DFRegion.DungeonTypes.BarbarianStronghold:
                        allowedMats = (PermittedMaterials)0b_0001_1111; baseChestOdds = 30; break;
                    case DFRegion.DungeonTypes.VolcanicCaves:
                        allowedMats = PermittedMaterials_FireProof; baseChestOdds = 25; break;
                    case DFRegion.DungeonTypes.ScorpionNest:
                        allowedMats = (PermittedMaterials)0b_0111_1111; baseChestOdds = 20; break;
                    case DFRegion.DungeonTypes.Cemetery:
                        allowedMats = (PermittedMaterials)0b_0000_0011; baseChestOdds = 30; break;
                    default:
                        allowedMats = PermittedMaterials_All; baseChestOdds = 20; break;
                }

                // Make list of loot-piles currently in the dungeon "scene."
                lootPiles = FindObjectsOfType<DaggerfallLoot>();

                for (int i = 0; i < lootPiles.Length; i++)
                {
                    if (lootPiles[i].ContainerType == LootContainerTypes.RandomTreasure)
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
                            if (Physics.Raycast(rays[h], out hit, 30f, PlayerLayerMask)) // Using raycast instead of sphere, as I want it to be only if you are purposely targeting the chest.
                            {
                                rayDistances[h] = hit.distance;
                            }
                            else
                            {
                                rayDistances[h] = 30f;
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

                        RollIfChestShouldBeCreated(lootPiles[i], allowedMats, baseChestOdds, totalRoomValueMod);
                    }
                }
            }
        }

        // This is really rough, mainly just for testing, because having some trouble thinking of a good formula for it all atm.
        // Definitely going to have to tweak and probably reconsider how the room context stuff modifies the chest odds, might be too drastic in some contexts and dungeon types, will have to see.
        public static void RollIfChestShouldBeCreated(DaggerfallLoot lootPile, PermittedMaterials allowedMats, int baseChestOdds, int totalRoomValueMod)
        {
            int chestOdds = baseChestOdds + totalRoomValueMod;

            if (chestOdds <= 0)
                return;

            if (allowedMats.HasFlag(PermittedMaterials.None)) // If no materials are allowed, return and don't generate chest.
                return;

            if (Dice100.SuccessRoll(chestOdds)) // If roll is successful, start process of replacing loot-pile with chest object as well as what properties the chest should have.
            {
                ItemCollection oldPileLoot = lootPile.Items;
                Transform oldLootPileTransform = lootPile.transform;
                Vector3 pos = lootPile.transform.position;
                ulong oldLoadID = lootPile.LoadID;

                GameObject chestParentObj = GameObjectHelper.CreateDaggerfallBillboardGameObject(810, 0, oldLootPileTransform.parent.parent);

                LLCObject llcObj = chestParentObj.AddComponent<LLCObject>();
                llcObj.Oldloot = oldPileLoot;
                llcObj.AttachedLoot = llcObj.Oldloot; // Will change this later, but placeholder for testing.
                llcObj.LoadID = oldLoadID;

                // Adding new properties to chest based on rolls for chest materials, rarity, possibly traps later, etc.
                llcObj.ChestMaterial = RollChestMaterial(allowedMats, totalRoomValueMod); // Alright for now just do something simple without a limited ticket sort of system, will try that more later on.
                llcObj.LockMaterial = RollLockMaterial(allowedMats, totalRoomValueMod);
                /*
                So I'm thinking tomorrow I'll try and make a sort of "Hat of tickets" function that will populate a "hat" of a limited number of possible "tickets" to pick from.
                This will work kind of similar to my random generation stuff in stuff like "Jewelry Additions", but possibly better, but also maybe more complex?
                But the tickets will have a value given to them based on what values from the possible chest and lock materials are permitted in this case. Somehow the rarity
                of the material in question will determine what proportion of tickets from the limited pool that material is given. Maybe have the flags checked from least rare to most rare,
                and give the least rare some large amount of tickets from the max at the start, then slowly go through the rest of the permitted ones from what amount of tickets are remaining
                and give them tickets based on what are left each time another is given more or something. Somehow also try to incorperate the "totalRoomValueMod" to either increase the
                amount of tickets rarer materials get, if it's positive above some value, or more to less rare materials if it's below some value or something. Don't know how I'll do this
                all exactly, but atleast I have some idea how I might try it. Worst case I could try something similar to how vanilla DFU determines the plate armors in FormulaHelper.cs?
                Don't know, will have to see tomorrow how it pans out, but might be a bit confusing at first, maybe make the tickets from the pool like 300, 500, or 1000 or something, will see.
                */

                // Set position
                Billboard dfBillboard = chestParentObj.GetComponent<Billboard>();
                chestParentObj.transform.position = pos;
                chestParentObj.transform.position += new Vector3(0, dfBillboard.Summary.Size.y / 2, 0);
                GameObjectHelper.AlignBillboardToGround(chestParentObj, dfBillboard.Summary.Size);

                Destroy(lootPile.gameObject); // Removed old loot-pile from scene, but saved its characteristics we care about.
            }
        }

        public static ChestMaterials RollChestMaterial(PermittedMaterials allowedMats, int roomValueMod)
        {


            return ChestMaterials.Wood;
        }

        public static LockMaterials RollLockMaterial(PermittedMaterials allowedMats, int roomValueMod)
        {


            return LockMaterials.Wood;
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