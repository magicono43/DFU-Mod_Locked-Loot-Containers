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

namespace LockedLootContainers
{
    [FullSerializer.fsObject("v1")]
    public class LLCSaveData : IHasModSaveData
    {
        public Dictionary<string, List<LandmarkLocation>> Towns; // Continue work on renovating this Penwick Papers save-data tomorrow for my chest objects in a dictionary or something.
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