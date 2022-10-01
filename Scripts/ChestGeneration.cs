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

namespace LockedLootContainers
{
    public partial class LockedLootContainersMain
    {
        public void AddLootChests_OnTransitionDungeonInterior(PlayerEnterExit.TransitionEventArgs args)
        {
            DFLocation locationData = GameManager.Instance.PlayerGPS.CurrentLocation;
            DaggerfallLoot[] lootPiles;

            if (GameManager.Instance.PlayerEnterExit.IsPlayerInside)
            {
                switch (locationData.MapTableData.DungeonType) // These will give various modifier values which will then be used afterward to do the actual "work" part in the generation process.
                {
                    default:
                    case DFRegion.DungeonTypes.Crypt: // Guess first thing to do is define the new enum flag attributes for these "PermittedMaterials" somehow.
                    case DFRegion.DungeonTypes.OrcStronghold:
                    case DFRegion.DungeonTypes.HumanStronghold:
                    case DFRegion.DungeonTypes.Prison:
                    case DFRegion.DungeonTypes.DesecratedTemple:
                    case DFRegion.DungeonTypes.Mine:
                    case DFRegion.DungeonTypes.NaturalCave:
                    case DFRegion.DungeonTypes.Coven:
                    case DFRegion.DungeonTypes.VampireHaunt:
                    case DFRegion.DungeonTypes.Laboratory:
                    case DFRegion.DungeonTypes.HarpyNest:
                    case DFRegion.DungeonTypes.RuinedCastle:
                    case DFRegion.DungeonTypes.SpiderNest:
                    case DFRegion.DungeonTypes.GiantStronghold:
                    case DFRegion.DungeonTypes.DragonsDen:
                    case DFRegion.DungeonTypes.BarbarianStronghold:
                    case DFRegion.DungeonTypes.VolcanicCaves:
                    case DFRegion.DungeonTypes.ScorpionNest:
                    case DFRegion.DungeonTypes.Cemetery:
                        break;
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