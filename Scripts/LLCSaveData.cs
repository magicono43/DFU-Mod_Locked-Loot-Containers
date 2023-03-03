using UnityEngine;
using DaggerfallWorkshop.Game;
using DaggerfallConnect;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using System;
using System.Collections.Generic;
using DaggerfallWorkshop.Game.Serialization;

namespace LockedLootContainers
{
    [FullSerializer.fsObject("v1")]
    public class ChestData_v1
    {
        public ulong loadID;
        public int[] recentInspectValues;
        public bool isLockJammed;
        public bool hasBeenBashed;
        public bool hasBeenInspected;
        public ChestMaterials chestMaterial;
        public int chestSturdiness;
        public int chestMagicResist;
        public LockMaterials lockMaterial;
        public int lockSturdiness;
        public int lockMagicResist;
        public int lockComplexity;
        public int jamResist;
        public int picksAttempted;
        public int lockBashedLightTimes;
        public int lockBashedHardTimes;
        public int chestBashedLightTimes;
        public int chestBashedHardTimes;
        public Vector3 currentPosition;
        public Vector3 localPosition;
        public ItemData_v1[] attachedLoot;
    }

    [FullSerializer.fsObject("v1")]
    public struct ChestsSaveData_v1
    {
        public ChestData_v1[] chests;
    }

    public class LLCSaveData : IHasModSaveData
    {
        public Dictionary<string, List<LandmarkLocation>> Towns; // Continue work on this tomorrow, this time likely using mostly example from Kab's "Location Loader" mod for WoD and such.
        public List<LandmarkLocation> DungeonLocations;

        public LLCSaveData()
        {
            Towns = new Dictionary<string, List<LandmarkLocation>>();
            DungeonLocations = new List<LandmarkLocation>();
        }


        public Type SaveDataType
        {
            get { return typeof(LLCSaveData); }
        }

        public object NewSaveData()
        {
            return new LLCSaveData();
        }

        public object GetSaveData()
        {
            return this;
        }

        public void RestoreSaveData(object obj)
        {
            LLCSaveData other = (LLCSaveData)obj;

            Towns = other.Towns;
            DungeonLocations = other.DungeonLocations;
        }


        public class LandmarkLocation : IComparable<LandmarkLocation>
        {
            public string Name;
            public Vector3 Position;

            public LandmarkLocation(string name, Vector3 position)
            {
                Name = name;
                Position = position;
            }

            public int CompareTo(LandmarkLocation other)
            {
                if (other == null)
                    return 1;
                else
                    return Name.CompareTo(other.Name);
            }

        }
    }
}