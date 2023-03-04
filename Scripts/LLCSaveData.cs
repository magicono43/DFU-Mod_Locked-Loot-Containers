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
    public class ChestsSaveData_v1
    {
        public Dictionary<ulong, ChestData> Chests;
    }

    public class ChestData
    {
        public ulong loadID;
        public Vector3 currentPosition;
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;
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
        public ItemData_v1[] attachedLoot;
    }

    public class LLCSaveData : IHasModSaveData
    {
        public Type SaveDataType
        {
            get { return typeof(ChestsSaveData_v1); }
        }

        public object NewSaveData()
        {
            ChestsSaveData_v1 emptyData = new ChestsSaveData_v1();
            emptyData.Chests = new Dictionary<ulong, ChestData>();
            return emptyData;
        }

        public object GetSaveData()
        {
            Dictionary<ulong, ChestData> chestEntries = new Dictionary<ulong, ChestData>();
            LLCObject[] chests = GameObject.FindObjectsOfType<LLCObject>();

            if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon) // Will do other stuff later, but for testing right now just deal with dungeons first, to confirm this works at all.
            {
                for (int i = 0; i < chests.Length; i++)
                {
                    if (chests[i].LoadID <= 0)
                        continue;

                    ChestData chest = new ChestData
                    {
                        loadID = chests[i].LoadID,
                        currentPosition = chests[i].transform.position,
                        localPosition = chests[i].transform.localPosition,
                        localRotation = chests[i].transform.localRotation,
                        localScale = chests[i].transform.localScale,
                        recentInspectValues = chests[i].RecentInspectValues,
                        isLockJammed = chests[i].IsLockJammed,
                        hasBeenBashed = chests[i].HasBeenBashed,
                        hasBeenInspected = chests[i].HasBeenInspected,
                        chestMaterial = chests[i].ChestMaterial,
                        chestSturdiness = chests[i].ChestSturdiness,
                        chestMagicResist = chests[i].ChestMagicResist,
                        lockMaterial = chests[i].LockMaterial,
                        lockSturdiness = chests[i].LockSturdiness,
                        lockMagicResist = chests[i].LockMagicResist,
                        lockComplexity = chests[i].LockComplexity,
                        jamResist = chests[i].JamResist,
                        picksAttempted = chests[i].PicksAttempted,
                        lockBashedLightTimes = chests[i].LockBashedLightTimes,
                        lockBashedHardTimes = chests[i].LockBashedHardTimes,
                        chestBashedLightTimes = chests[i].ChestBashedLightTimes,
                        chestBashedHardTimes = chests[i].ChestBashedHardTimes,
                        attachedLoot = chests[i].AttachedLoot.SerializeItems()
                    };

                    if (chest != null)
                        chestEntries.Add(chest.loadID, chest);
                }
            }

            ChestsSaveData_v1 data = new ChestsSaveData_v1();
            data.Chests = chestEntries;
            return data;
        }

        public void RestoreSaveData(object dataIn)
        {
            ChestsSaveData_v1 data = (ChestsSaveData_v1)dataIn;
            Dictionary<ulong, ChestData> chests = data.Chests;

            if (GameManager.Instance.PlayerEnterExit.IsPlayerInsideDungeon) // Will do other stuff later, but for testing right now just deal with dungeons first, to confirm this works at all.
            {
                foreach (KeyValuePair<ulong, ChestData> chest in chests)
                {
                    if (chest.Value.loadID <= 0)
                        continue;

                    // Continue working on this next time. It feels like I made good progress on this, better than expected atleast. Attempt to recreate chests when save is loaded here, then test, etc.
                }
            }
        }
    }
}